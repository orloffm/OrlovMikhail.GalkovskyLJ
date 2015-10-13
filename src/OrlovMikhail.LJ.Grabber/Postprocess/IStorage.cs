using System;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Storage of data by URL.</summary>
    public interface IStorage : IDisposable
    {
        /// <summary>Stores the userpic content.</summary>
        /// <param name="username">Username of the owner.</param>
        /// <param name="url">Original url.</param>
        /// <param name="data">File content.</param>
        FileInfoBase EnsureStored(Uri url, byte[] data);

        /// <summary>Gets stored userpic's absolute location.</summary>
        /// <param name="url">Original userpic URL.</param>
        /// <returns>Location or null if file didn't exist.</returns>
        FileInfoBase TryGet(Uri url);

        /// <summary>Gets stored userpic's absolute location.</summary>
        /// <param name="url">Original userpic URL.</param>
        /// <returns>Location or null if file didn't exist.</returns>
        FileInfoBase TryGet(string url);
    }
}