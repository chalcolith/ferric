using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ferric.Config;
using Ferric.Math.Common;
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
            var expected = Classifier.GetOutputClasses(testTagPath);

            // classify
            var testConfig = Path.GetFullPath(dataDir + @"\naivebayes_multinomial_test.config");
            var tester = Pipeline.Load(testConfig);
            var actual = tester.Process(Enumerable.Empty<string>()).OfType<IEnumerable<double>>().ToList();

            // test
            double total = 0;
            double correct = 0;

            IEnumerator<IEnumerable<double>> exp = expected.GetEnumerator();
            IEnumerator<IEnumerable<double>> act = actual.GetEnumerator();

            bool hasExp = exp.MoveNext();
            bool hasAct = act.MoveNext();

            while (hasExp || hasAct)
            {
                total++;

                if (hasExp != hasAct)
                    continue;

                if (VectorsMatch(exp.Current, act.Current, Constants.Epsilon))
                    correct++;

                hasExp = exp.MoveNext();
                hasAct = act.MoveNext();
            }

            Assert.IsTrue(total > 0);

            var ratio = correct / total;
            Assert.IsTrue(ratio > 0.8);
        }

        public bool VectorsMatch(IEnumerable<double> a, IEnumerable<double> b, double epsilon)
        {
            var ae = a.GetEnumerator();
            var be = b.GetEnumerator();

            bool hasA = ae.MoveNext();
            bool hasB = be.MoveNext();

            while (hasA || hasB)
            {
                if (hasA != hasB)
                    return false;

                var delta = System.Math.Abs(ae.Current - be.Current);
                if (delta > epsilon)
                    return false;

                hasA = ae.MoveNext();
                hasB = be.MoveNext();
            }

            return true;
        }
    }
}
