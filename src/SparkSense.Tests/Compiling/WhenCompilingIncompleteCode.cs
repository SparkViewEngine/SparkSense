using NUnit.Framework;
using Spark.Compiler;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.Compiling
{
    public class WhenCompilingIncompleteCode : SyntaxCompilationScenario
    {
        public WhenCompilingIncompleteCode()
        {
            GivenSomeContentToCompile(@"<div>${user.</div>");
            WhenCompilingIntoCodeSnippitChunks();
        }

        [Test]
        public void ShouldContainAnExpressionChunkBetweenTwoLiteralChunks()
        {
            TheParsedChunks[0]
                .ShouldBeOfType<SendLiteralChunk>()
                .Text
                .ShouldBe("<div>");

            TheParsedChunks[1]
                .ShouldBeOfType<SendExpressionChunk>()
                .Code.ToString()
                .ShouldBe("user.");

            TheParsedChunks[2]
                .ShouldBeOfType<SendLiteralChunk>()
                .Text
                .ShouldBe("</div>");
        }

        [Test]
        public void ShouldReturnANumberOfCompiledChunks()
        {
            TheParsedChunks
                .ShouldNotBeNull()
                .ShouldHaveCount(3);
        }
    }
}