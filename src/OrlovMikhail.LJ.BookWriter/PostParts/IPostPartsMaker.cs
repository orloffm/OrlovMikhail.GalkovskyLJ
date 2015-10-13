using System.Collections.Generic;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.BookWriter
{
    public interface IPostPartsMaker
    {
        /// <summary>Converts HTML tokens to writeable entities.</summary>
        /// <param name="tokens">HTML parsed.</param>
        /// <param name="fs">File storage to replace URLs with local paths.</param>
        /// <returns></returns>
        IPostPart[] CreateTextParts(HTMLTokenBase[] tokens, IFileStorage fs);
    }
}