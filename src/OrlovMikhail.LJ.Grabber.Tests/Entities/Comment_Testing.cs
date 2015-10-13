using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.Grabber.Tests.Entities
{
    [TestFixture]
   public class Comment_Testing
    {
        [Test]
        public void SaysItHasAParentIfAndOnlyIfItHasAParentUrl()
        {
            Comment c = new Comment();
            c.ParentUrl = String.Empty;
            Assert.IsFalse(c.ParentUrlSpecified);

            c.ParentUrl = null;
            Assert.IsFalse(c.ParentUrlSpecified);

            c.ParentUrl = "http://www.url";
            Assert.IsTrue(c.ParentUrlSpecified);
        }
    }
}
