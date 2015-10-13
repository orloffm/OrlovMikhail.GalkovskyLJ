namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Loads all other pages in addition to the source one.</summary>
    public interface IOtherPagesLoader
    {
        EntryPage[] LoadOtherCommentPages(CommentPages commentPages, ILJClientData clientData);
    }
}