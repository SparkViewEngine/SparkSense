using Spark.Parser;
using Spark.Parser.Markup;
using SparkSense.Parser;

namespace SparkSense.Tests.Scenarios.Parsing
{
    public class SyntaxParsingScenario : Scenario
    {
        private CompletionGrammar _grammar;
        private Position _sourceContent;
        protected ExpressionNode TheParsedExpressionNode { get; private set; }

        protected Position Source(string content)
        {
            return new Position(new SourceContext(content));
        }

        protected void GivenSomeContentToParse(string content)
        {
            _grammar = new CompletionGrammar();
            _sourceContent = Source(content);
        }

        protected void WhenParsingForAnyNode()
        {
            TheParsedExpressionNode = _grammar.AnyNode(_sourceContent).Value as ExpressionNode;
        }

        protected void WhenParsingForIncompleteCode()
        {
            TheParsedExpressionNode = null;
            ParseResult<ExpressionNode> result = _grammar.IncompleteCode(_sourceContent);
            TheParsedExpressionNode = result == null ? null : result.Value;
        }
    }
}