using NUnit.Framework;
using Spark.Parser.Markup;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.Parsing
{
    public class WhenParsingIncompleteCode : SyntaxParsingScenario
    {
        [Test]
        public void ShouldCorrectlyParseIncompleteCodeAfterBangSyntax()
        {
            GivenSomeContentToParse("!{user.");
            WhenParsingForIncompleteCode();
            TheParsedExpressionNode
                .ShouldNotBeNull()
                .ShouldBeOfType<ExpressionNode>()
                .Code.ToString()
                .ShouldBe("user.");
        }

        [Test]
        public void ShouldCorrectlyParseIncompleteCodeAfterDollarSyntax()
        {
            GivenSomeContentToParse("${user.");
            WhenParsingForIncompleteCode();
            TheParsedExpressionNode
                .ShouldNotBeNull()
                .ShouldBeOfType<ExpressionNode>()
                .Code.ToString()
                .ShouldBe("user.");
        }

        [Test]
        public void ShouldCorrectlyParseIncompleteCodeWhenFollowedByAnotherNode()
        {
            GivenSomeContentToParse("${user.name</div>");
            WhenParsingForAnyNode();
            TheParsedExpressionNode
                .ShouldNotBeNull()
                .ShouldBeOfType<ExpressionNode>()
                .Code.ToString()
                .ShouldBe("user.name");
        }
    }
}