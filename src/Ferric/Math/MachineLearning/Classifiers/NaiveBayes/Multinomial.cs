using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.MachineLearning.Classifiers.NaiveBayes
{
    public class Multinomial<TOutput> : IClassifier<double, TOutput>
        where TOutput : struct, IComparable
    {
        IDictionary<IEnumerable<TOutput>, double> prior = new Dictionary<IEnumerable<TOutput>, double>();
        IDictionary<IEnumerable<TOutput>, IDictionary<int, double>> condProb =
            new Dictionary<IEnumerable<TOutput>, IDictionary<int, double>>();

        public Multinomial()
        {
        }

        #region IClassifier<TInput,TOutput> Members

        public void TrainModel(IEnumerable<IEnumerable<double>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs)
        {
            if (trainingInputs.Count() == 0)
                throw new ArgumentException("cannot train on an empty dataset", "trainingInputs");
            
            if (trainingInputs.Count() != trainingOutputs.Count())
                throw new ArgumentException("there must be the same number of inputs as outputs");

            int N = 0; // number of inputs
            var inputsByClass = new Dictionary<IEnumerable<TOutput>, IList<IEnumerable<double>>>();

            // map inputs by class
            var inputs = trainingInputs.GetEnumerator();
            var classes = trainingOutputs.GetEnumerator();
            while (inputs.MoveNext() && classes.MoveNext())
            {
                N++;
                IList<IEnumerable<double>> inClass;
                if (!inputsByClass.TryGetValue(classes.Current, out inClass))
                {
                    inClass = new List<IEnumerable<double>>();
                    inputsByClass.Add(classes.Current, inClass);
                }
                inClass.Add(inputs.Current);
            }

            // now get prior prob and collect counts
            foreach (var kv in inputsByClass)
            {
                var cls = kv.Key;
                var inClass = kv.Value;

                if (inClass.Count == 0)
                    continue;

                prior[cls] = inClass.Count / N;

                var V = inClass[0].Count();
                var counts = new double[V];
                foreach (var incls in inClass)
                {
                    int i = 0;
                    foreach (var v in incls)
                    {
                        counts[i] = counts[i] + v;
                        i++;
                    }
                }

                // normalize & smooth
                double norm = 0;
                for (int t = 0; t < V; t++)
                    norm += counts[t] + 1;

                // calculate conditional probability
                IDictionary<int, double> probByFeature;
                if (!condProb.TryGetValue(cls, out probByFeature))
                {
                    probByFeature = new Dictionary<int, double>();
                    condProb.Add(cls, probByFeature);
                }

                for (int t = 0; t < V; t++)
                {
                    probByFeature.Add(t, (counts[t] + 1) / norm);
                }
            }
        }

        public double TestModel(IEnumerable<IEnumerable<double>> testingInputs, 
            IEnumerable<IEnumerable<TOutput>> testingOutputs, 
            Func<TOutput, TOutput, bool> matches = null)
        {
            int total = 0, right = 0;

            var inEnum = testingInputs.GetEnumerator();
            var outEnum = testingOutputs.GetEnumerator();
            while (inEnum.MoveNext() && outEnum.MoveNext())
            {
                total++;

                var expected = outEnum.Current.GetEnumerator();

                var cls = Classify(inEnum.Current);
                var actual = cls.GetEnumerator();

                bool eq = true;
                while (expected.MoveNext() && actual.MoveNext())
                {
                    if (!matches(expected.Current, actual.Current))
                    {
                        eq = false;
                        break;
                    }
                }

                if (eq)
                    right++;
            }

            if (total > 0)
                return (double)right / (double)total;
            else
                return double.NaN;
        }

        public IEnumerable<TOutput> Classify(IEnumerable<double> input)
        {
            double maxScore = 0;
            IEnumerable<TOutput> maxCls = null;

            foreach (var kv in condProb)
            {
                var cls = kv.Key;
                var probByFeature = kv.Value;

                double score = prior[cls];

                int V = input.Count();
                for (int t = 0; t < V; t++)
                {
                    score += input.ElementAt(t) * System.Math.Log(probByFeature[t]);
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    maxCls = cls;
                }
            }

            return maxCls;
        }

        #endregion

        #region ISerializable Members

        public Multinomial(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
