using System;
using System.Collections.Generic;

namespace OrlovMikhail.LJ.Grabber
{
    public interface IEntryPageHelper
    {
        /// <summary>Adds everything possible to target from another source.
        /// This may include new comments, full comments, larger text etc.</summary>
        /// <param name="target">Object to add data to.</param>
        /// <param name="otherSource">Object to use data from.</param>
        /// <returns>Added something or not.</returns>
        bool AddData( EntryPage target, EntryPage otherSource);

    }
}