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
            var configPath = Path.GetFullPath(@"..\..\..\..\data\english\samples\tokenize.config");
            var textPath = Path.GetFullPath(@"..\..\..\..\data\english\samples\sample1.txt");

            var transducer = Pipeline.Load(configPath);
            var results = transducer.Process(new[] { textPath });

            foreach (string result in results)
            {
                Assert.AreEqual(@"{ FileSystemDocument:0 3-37 " + textPath + @" }
  { TokenSpan:2 3-7 Word ""name"":{name/1} }
  { TokenSpan:6 11-21 Word ""Ozymandias"":{ozymandia/1} }
  { TokenSpan:7 21-22 Punct "","":{,/1} }
  { TokenSpan:9 23-27 Word ""king"":{king/1} }
  { TokenSpan:13 31-36 Word ""kings"":{king/1} }
  { TokenSpan:14 36-37 Punct ""\."":{\./1} }
", result);
            }
        }

        [TestMethod]
        public void Ferric_Pipeline_DTMPipeline()
        {
            var configPath = Path.GetFullPath(@"..\..\..\..\data\english\samples\dtm.config");
            var textFiles = Enumerable.Range(0, 3).Select(i => Path.GetFullPath(string.Format(@"..\..\..\..\data\english\samples\sample{0}.txt", i+1)));

            const string expected = @"documents: 3
lexemes:   12	Ferric.Text.Common.Lexicon.FlatFileLexicon
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

        [TestMethod]
        public void Ferric_Pipeline_WordNetDTMPipeline()
        {
            var configPath = Path.GetFullPath(@"..\..\..\..\..\data\english\samples\wndtm.config");
            var textFiles = Enumerable.Range(0, 3).Select(i => Path.GetFullPath(string.Format(@"..\..\..\..\..\data\english\samples\sample{0}.txt", i + 1)));

            const string expected = @"documents: 3
lexemes:   206979	Ferric.Text.WordNet.Lexicon.SynsetLexicon
	name	name	name,..	name,..	name,..	name	diagn..	ident..	list,..	name	menti..	name,..	name,..	appoi..	name,..	?unkn..	king	king	king	baron..	king,..	king,..	king,..	king,..	king,..	king	secon..	irreg..	secon..	secon..	second	secon..	second	secon..	momen..	momen..	second	secon..	second	secon..	secon..	random	convi..	sente..	priso..	sente..	four-..	four,..	four,..	sexua..	score	mark,..	score	score..	grudg..	score	score..	score	score..	score	grade..	score..	score..	score	score..	seduc..	score	seven..	seven..	seven..	days,..	old a..	long ..	class..	year	year,..	year	ago,a..	ago
0	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	1	0.2	0.2	0.2	0.2	0.2	0.2	0.2	0.2	0.2	0.2																																																					
1																											0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	0.0666666666666667	1	0.25	0.25	0.25	0.25																																	
2																																															0.333333333333333	0.333333333333333	0.333333333333333	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.0555555555555556	0.333333333333333	0.333333333333333	0.333333333333333	0.142857142857143	0.142857142857143	0.142857142857143	0.142857142857143	0.142857142857143	0.142857142857143	0.142857142857143	0.5	0.5
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
