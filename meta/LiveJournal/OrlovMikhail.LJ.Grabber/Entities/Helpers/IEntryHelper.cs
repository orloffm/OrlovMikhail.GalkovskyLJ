namespace OrlovMikhail.LJ.Grabber
{
    public interface IEntryHelper
    {
        /// <summary>Updates entry with newer information
        /// or just puts data if it was empty.</summary>
        /// <param name="target">Entry to update.</param>
        /// <param name="source">Entry to get data from.</param>
        /// <returns>Updated or not.</returns>
        bool UpdateWith( Entry target, Entry source);
    }
}