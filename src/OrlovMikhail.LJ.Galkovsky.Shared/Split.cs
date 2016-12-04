using System.IO.Abstractions;
using System.Linq;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class Split
    {
        public string Name { get; set; }
        public long FromId { get; set; }
        public Split Next { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            string ret = Name + " from " + FromId;

            if (Next != null)
                ret += " up to " + Next.FromId;
            
            return ret;
        }
    }
}