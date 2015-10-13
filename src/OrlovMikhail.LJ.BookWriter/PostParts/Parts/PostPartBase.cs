using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public abstract class PostPartBase : IPostPart
    {
        public virtual IPostPart FullClone()
        {
            return (IPostPart)this.MemberwiseClone();
        }
    }
}
