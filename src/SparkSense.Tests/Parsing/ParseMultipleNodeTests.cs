using NUnit.Framework;
using Spark.Compiler.NodeVisitors;
using Spark.Parser.Markup;
using SparkSense.Parsing;

namespace SparkSense.Tests.Parsing
{
    [TestFixture]
    public class ParseMultipleNodeTests
    {
        [Test]
        public void ShouldParseIntoMultipleNodes()
        {
            var nodes = SparkSyntax.ParseNodes("<div><use content='main'/></div>");
            var visitor = new SpecialNodeVisitor(new VisitorContext());
            visitor.Accept(nodes);

            Assert.That(visitor.Nodes.Count, Is.EqualTo(3));
            Assert.That(visitor.Nodes[0], Is.InstanceOf(typeof(ElementNode)));
            Assert.That(visitor.Nodes[1], Is.InstanceOf(typeof(SpecialNode)));
            Assert.That(visitor.Nodes[2], Is.InstanceOf(typeof(EndElementNode)));
        }
    }
}
