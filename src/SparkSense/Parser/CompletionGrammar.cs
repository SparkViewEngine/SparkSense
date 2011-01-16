using Spark.Parser;
using Spark.Parser.Markup;

namespace SparkSense.Parser
{
    public class CompletionGrammar : MarkupGrammar
    {
        public ParseAction<ExpressionNode> IncompleteCode;

        public CompletionGrammar() : base(new ParserSettings())
        {
            IncompleteCode = TkCode(Ch("${").Or(Ch("!{"))).And(Text)
                .Build(hit => new ExpressionNode(hit.Down.Text));

            AnyNode = AsNode(Element).Paint()
                .Or(AsNode(EndElement).Paint())
                .Or(AsNode(Statement))
                .Or(AsNode(Code).Paint())
                .Or(AsNode(IncompleteCode).Paint());
            //.Or(AsNode(Text).Paint())
            //.Or(AsNode(LessThanTextNode).Paint());

            Nodes = Rep(AnyNode);
        }
    }
}