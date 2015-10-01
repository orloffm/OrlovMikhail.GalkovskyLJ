using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky
{
   public static class FragmentHelper
   {
       public const string FRAGMENT_FILE_NAME = "fragment.asc";

        /// <summary>Fragment numbers and relative paths.</summary>
        public static List<Tuple<int, string>> GetAllFragmentPaths(IFileSystem fs, string root, out int? maxFound)
        {
            DirectoryInfoBase rootInfo = fs.DirectoryInfo.FromDirectoryName(root);
            FileInfoBase[] fragments = rootInfo.EnumerateFiles(FRAGMENT_FILE_NAME, SearchOption.AllDirectories).ToArray();
            List<Tuple<int, string>> relativePaths = new List<Tuple<int, string>>();
            foreach (FileInfoBase fragment in fragments)
            {
                int number = int.Parse(fragment.Directory.Name);
                string relativePath = IOTools.MakeRelativePath(rootInfo, fragment);

                relativePaths.Add(Tuple.Create(number, relativePath));
            }

            if (relativePaths.Any())
                maxFound = relativePaths.Max(z => z.Item1);
            else
                maxFound = null;

            return relativePaths;
        }
    }
}
