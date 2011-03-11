using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using NUnit.Framework;
using Rhino.Mocks;
using SparkSense.Parsing;
using SparkSense.StatementCompletion.CompletionSets;

namespace SparkSense.Tests.StatementCompletion
{
    [TestFixture]
    public class ExpressionCompletionSetTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _mockViewExplorer = MockRepository.GenerateMock<IViewExplorer>();
            _stubSnapshot = MockRepository.GenerateStub<ITextSnapshot>();
            _stubViewExplorer = MockRepository.GenerateStub<IViewExplorer>();
            _stubTrackingSpan = MockRepository.GenerateStub<ITrackingSpan>();
        }

        [TearDown]
        public void Cleanup()
        {
            _mockViewExplorer.VerifyAllExpectations();
            _mockViewExplorer = null;
        }

        #endregion

        private IViewExplorer _mockViewExplorer;
        private ITextSnapshot _stubSnapshot;
        private ITrackingSpan _stubTrackingSpan;
        private IViewExplorer _stubViewExplorer;

        private SnapshotPoint GetSnapShotPoint(string content, int position)
        {
            _stubSnapshot.Stub(x => x.Length).Return(content.Length);
            _stubSnapshot.Stub(x => x.GetText()).Return(content);
            return new SnapshotPoint(_stubSnapshot, position);
        }

        [Test, Ignore("Not ready for this yet")]
        public void ShouldHaveAListOfTypes()
        {
            SnapshotPoint point = GetSnapShotPoint("${", 2);
            CompletionSet completionSet = CompletionSetFactory.GetCompletionSetFor(point, _stubTrackingSpan,
                                                                                   _stubViewExplorer);
            List<Completion> completions = completionSet.Completions.ToList();

            Assert.That(completionSet, Is.InstanceOf(typeof (ExpressionCompletionSet)));
            Assert.That(completions.Count, Is.EqualTo(7));
            Assert.That(completions.Exists(a => a.DisplayText == "System"));
            Assert.That(completions.Exists(a => a.DisplayText == "Spark"));
        }

        [Test, Ignore("Not ready for this yet")]
        public void ShouldHaveAListOfVariables()
        {
            SnapshotPoint point = GetSnapShotPoint("${", 2);
            CompletionSet completionSet = CompletionSetFactory.GetCompletionSetFor(point, _stubTrackingSpan,
                                                                                   _stubViewExplorer);
            List<Completion> completions = completionSet.Completions.ToList();

            Assert.That(completionSet, Is.InstanceOf(typeof (ExpressionCompletionSet)));
            Assert.That(completions.Count, Is.EqualTo(7));
            Assert.That(completions.Exists(a => a.DisplayText == "System"));
            Assert.That(completions.Exists(a => a.DisplayText == "Spark"));
        }
    }
}