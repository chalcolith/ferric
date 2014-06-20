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
