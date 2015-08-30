using System;

namespace OrlovMikhail.LJ.Grabber
{
    public interface IEntryBase
    {
        long Id { get; set; }
        string Url { get; set; }
        UserLite Poster { get; set; }
        Userpic PosterUserpic { get; set; }
        DateTime? Date { get; set; }
        string Text { get; set; }
        string Subject { get; set; }
    }
}