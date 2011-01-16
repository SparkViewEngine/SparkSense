using System.Collections.Generic;
using Spark.Compiler;
using Spark.Compiler.NodeVisitors;
using Spark.FileSystem;
using SparkSense.Parser;

namespace SparkSense.Tests.Scenarios.Compiling
{
    public class SyntaxCompilationScenario : Scenario
    {
        private CompletionSyntaxProvider _syntaxProvider;
        private VisitorContext _context;

        protected IList<Chunk> TheParsedChunks { get; set; }

        protected void GivenSomeContentToCompile(string content)
        {
            _syntaxProvider = new CompletionSyntaxProvider();
            _context = new VisitorContext
                           {ViewFolder = new InMemoryViewFolder {{"Home\\index.spark", @"<div>${user.</div>"}}};
        }

        protected void WhenCompilingIntoCodeSnippitChunks()
        {
            TheParsedChunks = _syntaxProvider.GetChunks(_context, "Home\\index.spark");
        }
    }
}