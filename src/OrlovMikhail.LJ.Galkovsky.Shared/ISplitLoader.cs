using System.IO.Abstractions;

namespace OrlovMikhail.LJ.Galkovsky
{
    public interface ISplitLoader
    {
        Split[] LoadSplits(IFileSystem fs);
    }
}