using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Galkovsky
{
    public interface IGalkovskyFolderNamingStrategy : IFolderNamingStrategy
    {
        /// <summary>Returns the entry key used by the author.</summary>
        string GetGalkovskyEntryKey(string subject);
    }
}