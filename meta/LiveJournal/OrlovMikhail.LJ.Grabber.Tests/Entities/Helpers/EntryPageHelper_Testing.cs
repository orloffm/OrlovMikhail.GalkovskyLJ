using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InvokeAsExtensionMethod

namespace OrlovMikhail.LJ.Grabber.Tests
{
    [TestFixture]
    public sealed class EntryPageHelper_Testing
    {
        private EntryPageHelper _eph;
        private IEntryHelper _entryHelper;
        private IRepliesHelper _repliesHelper;

        [SetUp]
        public void BeforeTests()
        {
            _entryHelper = MockRepository.GenerateMock<IEntryHelper>();
            _repliesHelper = MockRepository.GenerateMock<IRepliesHelper>();

            // Replace with mocks.
            _eph = new EntryPageHelper(_entryHelper, _repliesHelper);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsIfNoTargetSpecified()
        {
            EntryPage other = TestingShared.GenerateEntryPage();

            _eph.AddData(null, other);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsIfNoSourceSpecified()
        {
            EntryPage other = TestingShared.GenerateEntryPage();

            _eph.AddData(other, null);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnsuresTargetCommentsPagesDataIsNull(bool startFromEmpty)
        {
            EntryPage target = startFromEmpty ? new EntryPage() : TestingShared.GenerateEntryPage();
            EntryPage source = TestingShared.GenerateEntryPage(makeAllFull: true);

            _eph.AddData(target, source);

            Assert.IsTrue(CommentPages.IsEmpty(target.CommentPages));
        }

        [Test]
        public void CallMergeFunctions()
        {
            EntryPage target = new EntryPage();
            EntryPage source = new EntryPage();

            _entryHelper.Expect(z => z.UpdateWith(target.Entry, source.Entry)).Return(true);
            _repliesHelper.Expect(z => z.MergeFrom(target, source)).Return(true);

            _eph.AddData(target, source);

            _entryHelper.VerifyAllExpectations();
            _repliesHelper.VerifyAllExpectations();
        }
    }
}
