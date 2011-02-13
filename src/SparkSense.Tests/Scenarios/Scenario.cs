using NUnit.Framework;

namespace SparkSense.Tests.Scenarios
{
    [TestFixture]
    public class Scenario
    {
        #region Setup/Teardown

        [SetUp]
        protected virtual void SetUp()
        {
        }

        [TearDown]
        protected virtual void TearDown()
        {
        }

        #endregion

        [Test]
        public void ShouldNotBeIgnoredByTestRunner() { }
    }
}