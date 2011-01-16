using System.Collections.Generic;
using Spark.Compiler;
using Spark.Compiler.NodeVisitors;
using Spark.Parser;
using Spark.Parser.Markup;
using Spark.Parser.Syntax;

namespace SparkSense.Parser
{
    public class CompletionSyntaxProvider : DefaultSyntaxProvider
    {
        private readonly CompletionGrammar _grammar;

        public CompletionSyntaxProvider() : base(new ParserSettings())
        {
            _grammar = new CompletionGrammar();
        }

        public override IList<Chunk> GetChunks(VisitorContext context, string path)
        {
            context.SyntaxProvider = this;
            context.ViewPath = path;

            SourceContext sourceContext = CreateSourceContext(context.ViewPath, context.ViewFolder);
            var position = new Position(sourceContext);

            ParseResult<IList<Node>> result = _grammar.Nodes(position);
            if (result.Rest.PotentialLength() != 0)
            {
                ThrowParseException(context.ViewPath, position, result.Rest);
            }

            context.Paint = result.Rest.GetPaint();

            IList<Node> nodes = result.Value;
            foreach (INodeVisitor visitor in BuildNodeVisitors(context))
            {
                visitor.Accept(nodes);
                nodes = visitor.Nodes;
            }

            var chunkBuilder = new ChunkBuilderVisitor(context);
            chunkBuilder.Accept(nodes);
            return chunkBuilder.Chunks;
        }

        private IList<INodeVisitor> BuildNodeVisitors(VisitorContext context)
        {
            return new INodeVisitor[]
                       {
                           new NamespaceVisitor(context),
                           new IncludeVisitor(context),
                           new PrefixExpandingVisitor(context),
                           new SpecialNodeVisitor(context),
                           new CacheAttributeVisitor(context),
                           new WhitespaceCleanerVisitor(context),
                           new ForEachAttributeVisitor(context),
                           new ConditionalAttributeVisitor(context),
                           new TestElseElementVisitor(context),
                           new OnceAttributeVisitor(context),
                           new UrlAttributeVisitor(context),
                           new BindingExpansionVisitor(context)
                       };
        }
    }
}