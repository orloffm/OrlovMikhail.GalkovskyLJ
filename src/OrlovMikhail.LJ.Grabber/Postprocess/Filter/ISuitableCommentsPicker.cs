using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Returns comments as arrays of branches
    /// containing journal owner.</summary>
   public interface ISuitableCommentsPicker
   {
       /// <summary>Do the conversion. Replace WWW links
       /// with local links and leave only meaningful comments.</summary>
       List<Comment[]> Pick(EntryPage ep);
   }
}
