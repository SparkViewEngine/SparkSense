using System;
using System.Runtime.InteropServices;
using EnvDTE;
using NUnit.Framework;
using Spark.FileSystem;
using Spark.Parser;
using Spark.Parser.Syntax;
using SparkSense.Parsing;
using Rhino.Mocks;

namespace SparkSense.Tests.Parsing
{
    [TestFixture]
    public class ProjectExplorerTests
    {
        private const string ROOT_VIEW_PATH = "SparkSense.Tests.Views";
        private DefaultSyntaxProvider _syntaxProvider;
        private ViewLoader _viewLoader;

        [SetUp]
        public void Setup()
        {
            _syntaxProvider = new DefaultSyntaxProvider(new ParserSettings());
            _viewLoader = new ViewLoader { ViewFolder = new FileSystemViewFolder(ROOT_VIEW_PATH), SyntaxProvider = _syntaxProvider };
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfTheProjectEnvironmentIsNull()
        {
            new ProjectExplorer(null);
        }

        [Test]
        public void ShouldProvideATypeDiscoveryService()
        {
            

        }
    }
}
