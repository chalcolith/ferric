using System;
using System.IO;
using System.Linq;
using Ferric.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ferric.Text.WordNet.Tests
{
    [TestClass]
    public class PipelineTests
    {
        [TestMethod]
        public void Ferric_Pipeline_WordNetDTMPipeline()
        {
            var configPath = Path.GetFullPath(@"..\..\..\..\..\..\data\english\samples\wndtm.config");
            var textFiles = Enumerable.Range(0, 3).Select(i => Path.GetFullPath(string.Format(@"..\..\..\..\..\..\data\english\samples\sample{0}.txt", i + 1)));

            const string expected = @"documents: 3
lemmas:    12	Ferric.Text.Common.Lexicon.FileLexicon
	name	ozyma..	king	second	random	sentenc	four	score	seven	year	ago
0	1	1	2								
1				1	1	1					
2							1	1	1	1	1
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
