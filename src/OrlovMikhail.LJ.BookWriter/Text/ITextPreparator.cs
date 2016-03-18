namespace OrlovMikhail.LJ.BookWriter
{
    public interface ITextPreparator
    {
        /// <summary>Processes the text before writing it
        /// to the final file.</summary>
        string Prepare(string text);

        /// <summary>Processes the user name.</summary>
        string PrepareUsername(string text);
    }
}