using System.IO.Abstractions;
using System.Linq;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class SplitLoader : ISplitLoader
    {
        private readonly string _folderLocation;
        private readonly string _fileName;

        private const string SplitFileName = "split.txt";

        public SplitLoader(string folderLocation, string fileName = null)
        {
            _folderLocation = folderLocation;
            _fileName = !string.IsNullOrWhiteSpace(fileName) ? fileName : SplitFileName;
        }

        public Split[] LoadSplits(IFileSystem fs)
        {
            string fullPath = fs.Path.Combine(_folderLocation, _fileName);
            string[] lines = fs.File.ReadAllLines(fullPath);

            Split[] splits = lines
                .Select(z => z.Split('\t'))
                .Select(z => new Split()
                {
                    Name = z[0],
                    FromId = long.Parse(z[1]),
                    Description = z[2]
                }).ToArray();

            // Set "To" from the previous ones.
            for (int i = 0; i < splits.Length - 1; i++)
                splits[i].Next = splits[i + 1];
            
            return splits;
        }
    }
}