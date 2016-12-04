using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class FragmentHelper : IFragmentHelper
    {
        private readonly IGalkovskyFolderNamingStrategy _ns;

        public const string FRAGMENT_FILE_NAME = "fragment.asc";

        public FragmentHelper(IGalkovskyFolderNamingStrategy ns)
        {
            _ns = ns;
        }

        /// <summary>Returns a dictionary for all dump files, of their entry's ids and the relative paths.</summary>
        public Dictionary<long, FragmentInformation> GetAllFragmentPaths(IFileSystem fs, string root)
        {
            Dictionary<long, FragmentInformation> ret = new Dictionary<long, FragmentInformation>();

            DirectoryInfoBase rootInfo = fs.DirectoryInfo.FromDirectoryName(root);
            FileInfoBase[] dumps = rootInfo.EnumerateFiles(Worker.DumpFileName, SearchOption.AllDirectories).ToArray();

            foreach (FileInfoBase dumpFile in dumps)
            {
                FileInfoBase fragmentFile = dumpFile.Directory.GetFiles(FRAGMENT_FILE_NAME).FirstOrDefault();

                string content = fs.File.ReadAllText(dumpFile.FullName);

                ILayerParser layerParser = new LayerParser();
                EntryPage ep = layerParser.ParseAsAnEntryPage(content);

                FragmentInformation fi = new FragmentInformation();
                fi.GalkovskyEntryKey = _ns.GetGalkovskyEntryKey(ep.Entry.Subject);

                if (fragmentFile != null)
                    fi.RelativeFragmentPath = IOTools.MakeRelativePath(rootInfo, fragmentFile);

                long entryId = ep.Entry.Id;
                ret.Add(entryId, fi);
            }

            return ret;
        }

        public IEnumerable<FragmentInformation> SelectValuesFor(Split current, Dictionary<long, FragmentInformation> fragsById)
        {
            return fragsById
                .Where(z => z.Key >= current.FromId && (current.Next == null || z.Key < current.Next.FromId))
                .OrderBy(z => z.Key)
                .Select(z => z.Value);
        }
    }
}
