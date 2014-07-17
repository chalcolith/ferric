using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ferric.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ferric.Text.Classifiers.Tests.NaiveBayes
{
    [TestClass]
    public class MultinomialTests
    {
        [TestMethod]
        public void Text_Classifiers_NaiveBayes_Multinomial()
        {
            var dataDir = @"..\..\..\..\..\..\data\english\samples";
            var trainConfig = Path.GetFullPath(dataDir + @"\naivebayes_multinomial_train.config");

            // train
            var trainer = Pipeline.Load(trainConfig);
            var fnames = trainer.Process(Enumerable.Empty<string>()).OfType<string>().ToList();

            Assert.IsNotNull(fnames);
            Assert.AreEqual(1, fnames.Count);
            Assert.IsTrue(fnames.First().EndsWith("fortune.model"));

            // get testing tags
            var testTagPath = Path.GetFullPath(dataDir + @"\naivebayes\testlabels.txt");
            var expected = Classifier.GetTags(testTagPath);

            // classify
            var testConfig = Path.GetFullPath(dataDir + @"\naivebayes_multinomial_test.config");
            var tester = Pipeline.Load(testConfig);
            var actual = tester.Process(Enumerable.Empty<string>()).OfType<IEnumerable<double>>();

            // test
            double total = 0;
            double correct = 0;

            bool hasExp = false;
            IEnumerator<IEnumerable<double>> exp = expected.GetEnumerator();

            bool hasAct = false;
            IEnumerator<IEnumerable<double>> act = actual.GetEnumerator();

            while ((hasExp = exp.MoveNext()) == true || (hasAct = act.MoveNext()) == true)
            {
                total++;

                if (hasExp != hasAct)
                    continue;

                if (VectorsMatch(exp.Current, act.Current, 0.1))
                    correct++;

                hasExp = hasAct = false;
            }

            Assert.IsTrue(total > 0);

            var ratio = correct / total;
            Assert.IsTrue(ratio > 0.8);
        }

        public bool VectorsMatch(IEnumerable<double> a, IEnumerable<double> b, double epsilon)
        {
            var hasA = false;
            var ae = a.GetEnumerator();

            var hasB = false;
            var be = b.GetEnumerator();

            while ((hasA = ae.MoveNext()) == true || (hasB = be.MoveNext()) == true)
            {
                if (hasA != hasB)
                    return false;

                var delta = System.Math.Abs(ae.Current - be.Current);
                if (delta > epsilon)
                    return false;

                hasA = hasB = false;
            }

            return true;
        }
    }
}
