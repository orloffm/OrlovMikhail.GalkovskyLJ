using System.IO.Abstractions;
using System.Linq;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class Split
    {
        public string Name { get; set; }
        public int From { get; set; }
        public int? To { get; set; }
        public string Description { get; set; }

        private const string splitFileName = "split.txt";

        public static Split[] LoadSplits(IFileSystem fs, string root, int? maxFound = null)
        {
            string[] lines = fs.File.ReadAllLines(fs.Path.Combine(root, splitFileName));
            Split[] splits = lines.Select(z => z.Split('\t'))
                .Select(z => new Split()
                {
                    Name = z[0],
                    From = int.Parse(z[1]),
                    Description = z[2]
                }).ToArray();
            for (int i = 0; i < splits.Length - 1; i++)
                splits[i].To = splits[i + 1].From - 1;
            // Last one.
            splits[splits.Length - 1].To = maxFound;
            return splits;
        }
    }
}