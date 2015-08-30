using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.Grabber.Tests.Entities
{
    [TestFixture]
    public sealed class EntryPage_Testing
    {
        [Test]
        public void CommentsAreSerializedOnlyWhenAreNotEmpty()
        {
            // Does the function exist with the proper name?
            Type t = typeof(EntryPage);

            string propertyName = t.GetProperties().Single(z => z.PropertyType == typeof(CommentPages)).Name;
            bool methodExists = t.GetMethods().Any(z => z.Name == "ShouldSerialize" + propertyName);
            if(!methodExists)
                throw new Exception("No serialization method.");

            // Non-empty
            EntryPage ep = new EntryPage();
            ep.CommentPages.Total = 10;
            Assert.IsTrue(ep.ShouldSerializeCommentPages());

            ep = new EntryPage();
            Assert.IsFalse(ep.ShouldSerializeCommentPages());
        }
    }
}
