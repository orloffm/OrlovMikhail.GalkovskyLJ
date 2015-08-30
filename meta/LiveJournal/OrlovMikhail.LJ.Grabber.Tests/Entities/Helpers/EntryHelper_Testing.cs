using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace OrlovMikhail.LJ.Grabber.Tests
{
    [TestFixture]
    public sealed class EntryHelper_Testing
    {
        private EntryHelper _eh;

        [SetUp]
        public void BeforeTests()
        {
            _eh = new EntryHelper(new EntryBaseHelper(new FileUrlExtractor()));
        }

        #region update with
        [Test, ExpectedException]
        public void ThrowsIfArgument1IsNull()
        {
            _eh.UpdateWith(null, new Entry());
        }

        [Test, ExpectedException]
        public void ThrowsIfArgument2IsNull()
        {
            _eh.UpdateWith(new Entry(), null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThrowsIfTargetAndSourceHaveDifferentIds()
        {
            Entry e1 = new Entry();
            Entry e2 = TestingShared.GenerateEntryPage().Entry;
            e1.Id = 1;
            e2.Id = 2;

            _eh.UpdateWith(e1, e2);
        }

        [Test]
        public void DoesntThrowIfOneOfIdsIsZero()
        {
            Entry e1 = new Entry();
            Entry e2 = TestingShared.GenerateEntryPage().Entry;
            e1.Id = 0;
            e2.Id = 2;

            _eh.UpdateWith(e1, e2);
        }

        [Test]
        public void EmptyPageGetsAllDataWhenMergedInto()
        {
            Entry e1 = new Entry();
            EntryPage p2 = TestingShared.GenerateEntryPage();
            e1.Id = 1;
            p2.Entry.Id = 1;

            _eh.UpdateWith(e1, p2.Entry);

            Assert.IsNotNullOrEmpty(e1.Text);
            Assert.IsNotNullOrEmpty(e1.Subject);
            Assert.IsNotNull(e1.Date);
            Assert.AreNotEqual(default(long), e1.Id);
        }
        
        [Test]
        public void EntryTextGetsUpdatedIfItIsLarger()
        {
            Entry p1 = new Entry();
            p1.Text = "ABC";
            Entry p2 = new Entry();
            p2.Text = "ABCD";

            _eh.UpdateWith(p1, p2);
            Assert.AreEqual("ABCD", p1.Text);
        }

        [Test]
        public void EntryTextDoesNotGetUpdatedIfItIsSmaller()
        {
            Entry p1 = new Entry();
            p1.Text = "ABCDEF";
            Entry p2 = new Entry();
            p2.Text = "ABCD";

            _eh.UpdateWith(p1, p2);
            Assert.AreEqual("ABCDEF", p1.Text);
        }
        #endregion
    }
}
