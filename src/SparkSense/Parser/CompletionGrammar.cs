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

            var LessThanTextNode = Ch('<').Build(hit => (Node) new TextNode("<"));

            var EntityRefOrAmpersand = AsNode(EntityRef).Or(Ch('&').Build(hit => (Node)new TextNode("&")));

            AnyNode = 
                
                AsNode(Element).Paint()
                .Or(AsNode(EndElement).Paint())
                .Or(EntityRefOrAmpersand.Paint())
                .Or(AsNode(Statement))
                .Or(AsNode(Code).Paint())
                .Or(AsNode(IncompleteCode).Paint())
                .Or(AsNode(Text).Paint())
                .Or(AsNode(DoctypeDecl).Paint())
                .Or(AsNode(Comment).Paint())
                .Or(AsNode(XMLDecl).Paint())
                .Or(AsNode(ProcessingInstruction).Paint())
                .Or(AsNode(LessThanTextNode).Paint());
            
            Nodes = Rep(AnyNode);
        }
    }
}