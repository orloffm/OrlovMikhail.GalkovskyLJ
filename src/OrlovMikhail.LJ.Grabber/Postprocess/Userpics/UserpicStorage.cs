using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class UserpicStorage : StorageBase, IUserpicStorage
    {
        private readonly IFileSystem _fs;
        private readonly string _baseDir;

        private const string pattern = @"http://l-userpic\.livejournal\.com/(?<p1>[\d]*)/(?<p2>[\d]*)";
        private readonly Regex _regex;

        public UserpicStorage(IFileSystem fs, string baseDir)
        {
            _fs = fs;
            _baseDir = baseDir;

            _regex = new Regex(pattern);
        }

        public override FileInfoBase EnsureStored(Uri url, byte[] data)
        {
            // Convert it to PNG.
            data = ImageHelper.ConvertToPNG(data);

            // TryGet path.
            string getPath = MakeActualPNGPath(url);

            // Write, first create directory.
            FileInfoBase fi = _fs.FileInfo.FromFileName(getPath);
            _fs.Directory.CreateDirectory(fi.DirectoryName);
            _fs.File.WriteAllBytes(getPath, data);
            return _fs.FileInfo.FromFileName(getPath);
        }

        /// <summary>Creates full path for the userpic saved from a given location.</summary>
        private string MakeActualPNGPath(Uri url)
        {
            // http://l-userpic.livejournal.com/112166236/40184012
            string p1, p2;
            Match m = _regex.Match(url.AbsoluteUri);
            p1 = m.Groups["p1"].Value;
            p2 = m.Groups["p2"].Value;

            StringBuilder sb = new StringBuilder(30);
            sb.Append(p1[0]);
            sb.Append(p1[1]);
            sb.Append(_fs.Path.DirectorySeparatorChar);
            sb.Append(p1[2]);
            sb.Append(p1[3]);
            sb.Append(_fs.Path.DirectorySeparatorChar);
            sb.Append(p1.Substring(4));
            sb.Append("_");
            sb.Append(p2);
            sb.Append(".png");

            string ret = _fs.Path.Combine(_baseDir, sb.ToString());
            return ret;
        }

        public override FileInfoBase TryGet(Uri url)
        {
            string getPath = MakeActualPNGPath(url);
            if(!_fs.File.Exists(getPath))
                return null;

            return _fs.FileInfo.FromFileName(getPath);
        }
    }
}
