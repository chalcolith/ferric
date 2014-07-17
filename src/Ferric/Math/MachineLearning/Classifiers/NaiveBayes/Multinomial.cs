using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.MachineLearning.Classifiers.NaiveBayes
{
    [Serializable]
    public class Multinomial<TOutput> : IClassifier<double, TOutput>
        where TOutput : struct, IComparable
    {
        IDictionary<TOutput[], double> prior = new Dictionary<TOutput[], double>();
        IDictionary<TOutput[], IDictionary<int, double>> condProb =
            new Dictionary<TOutput[], IDictionary<int, double>>();

        public Multinomial()
        {
        }

        #region IClassifier<TInput,TOutput> Members

        public void TrainModel(IEnumerable<IEnumerable<double>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs)
        {
            if (trainingInputs.Count() == 0)
                throw new ArgumentException("cannot train on an empty dataset", "trainingInputs");

            int numInputs = 0; // number of inputs
            var inputsByTag = new Dictionary<TOutput[], IList<IEnumerable<double>>>();

            // map inputs by tag
            var inputs = trainingInputs.GetEnumerator();
            var tags = trainingOutputs.GetEnumerator();
            while (inputs.MoveNext() && tags.MoveNext())
            {
                var tag = tags.Current.ToArray();

                numInputs++;
                IList<IEnumerable<double>> inputsInTag;
                if (!inputsByTag.TryGetValue(tag, out inputsInTag))
                {
                    inputsInTag = new List<IEnumerable<double>>();
                    inputsByTag.Add(tag, inputsInTag);
                }
                inputsInTag.Add(inputs.Current);
            }

            // now get prior prob and collect counts
            foreach (var kv in inputsByTag)
            {
                var tag = kv.Key;
                var inputsInTag = kv.Value;

                if (inputsInTag.Count == 0)
                    continue;

                prior[tag] = inputsInTag.Count / numInputs;

                var V = inputsInTag[0].Count();
                var counts = new double[V];
                foreach (var incls in inputsInTag)
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
                if (!condProb.TryGetValue(tag, out probByFeature))
                {
                    probByFeature = new Dictionary<int, double>();
                    condProb.Add(tag, probByFeature);
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
            input = input.ToArray();
            double maxScore = 0;
            IEnumerable<TOutput> bestTag = condProb.Keys.First();

            foreach (var kv in condProb)
            {
                var tag = kv.Key;
                var probByFeature = kv.Value;

                double score = prior[tag];

                int V = input.Count();


                for (int t = 0; t < V; t++)
                {
                    score += input.ElementAt(t) * System.Math.Log(probByFeature[t]);
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    bestTag = tag;
                }
            }

            return bestTag;
        }

        #endregion

        #region ISerializable Members

        public Multinomial(SerializationInfo info, StreamingContext context)
        {
            this.prior = (IDictionary<TOutput[], double>)
                info.GetValue("prior", typeof(IDictionary<TOutput[], double>));
            this.condProb = (IDictionary<TOutput[], IDictionary<int, double>>)
                info.GetValue("condProb", typeof(IDictionary<TOutput[], IDictionary<int, double>>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("prior", this.prior);
            info.AddValue("condProb", this.condProb);
        }

        #endregion
    }
}
