using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ferric.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ferric.Tests
{
    [TestClass]
    public class PipelineTests
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var entry in Directory.EnumerateFiles(dllDir, "*.dll"))
                Assembly.LoadFile(entry);
        }

        [TestMethod]
        public void Ferric_Pipeline_TokenizerPipeline()
        {
            var configPath = Path.GetFullPath(@"..\..\..\..\..\data\english\samples\tokenize.config");
            var textPath = Path.GetFullPath(@"..\..\..\..\..\data\english\samples\sample1.txt");

            var transducer = Pipeline.Load(configPath);
            var results = transducer.Process(new[] { textPath });

            foreach (string result in results)
            {
                Assert.AreEqual(@"{ FileSystemDocument:0 3-37 " + textPath + @" }
  { TokenSpan:2 3-7 Word ""name"":name }
  { TokenSpan:6 11-21 Word ""Ozymandias"":ozymandia }
  { TokenSpan:7 21-22 Punct "","":, }
  { TokenSpan:9 23-27 Word ""king"":king }
  { TokenSpan:13 31-36 Word ""kings"":king }
  { TokenSpan:14 36-37 Punct ""\."":\. }
", result);
            }
        }

        [TestMethod]
        public void Ferric_Pipeline_DTMPipeline()
        {
            var configPath = Path.GetFullPath(@"..\..\..\..\..\data\english\samples\dtm.config");
            var textFiles = Enumerable.Range(0, 3).Select(i => Path.GetFullPath(string.Format(@"..\..\..\..\..\data\english\samples\sample{0}.txt", i+1)));

            const string expected = @"documents: 3
lemmas:    12	Ferric.Text.Common.Lexicon.AdHocLexicon

	?unkn..	name	ozyma..	king	second	random	sentenc	four	score	seven	year	ago
0		1	1	2								
1					1	1	1					
2								1	1	1	1	1
";

            var transducer = Pipeline.Load(configPath);
            var results = transducer.Process(textFiles);

            foreach (string result in results)
            {
                Assert.AreEqual(expected, result);
            }
        }
    }
}
