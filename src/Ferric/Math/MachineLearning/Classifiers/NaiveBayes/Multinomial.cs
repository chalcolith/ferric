using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Utils.Common;

namespace Ferric.Math.MachineLearning.Classifiers.NaiveBayes
{
    [Serializable]
    public class Multinomial<TOutput> : IClassifier<double, TOutput>
        where TOutput : struct, IComparable<TOutput>
    {
        IDictionary<IOutputClass<TOutput>, double> priorByOutputClass = 
            new Dictionary<IOutputClass<TOutput>, double>();

        IDictionary<IOutputClass<TOutput>, IDictionary<int, double>> condProbByOutputClass =
            new Dictionary<IOutputClass<TOutput>, IDictionary<int, double>>();

        public Multinomial()
        {
        }

        #region IClassifier<TInput,TOutput> Members

        public IEnumerable<IOutputClass<TOutput>> Classify(IEnumerable<double> input)
        {
            var array = input.ToArray();

            // for all possible classes
            var outputClasses = condProbByOutputClass.Select(kv =>
            {
                var oc = kv.Key;
                var prob = kv.Value;

                double score = priorByOutputClass[oc];

                var V = array.Length;
                for (int t = 0; t < V; t++)
                {
                    if (array[t] > 0)
                        score += System.Math.Log(prob[t]);
                }

                return new OutputClass<TOutput>(score, oc.Output);
            })
            .OrderByDescending(oc => oc.Weight)
            .ToList();

            // normalize weights
            double total = 0;
            foreach (var oc in outputClasses)
                total += oc.Weight;

            if (total > 0)
            {
                foreach (var oc in outputClasses)
                    oc.Weight = oc.Weight / total;
            }

            return outputClasses;
        }

        public void TrainModel(IEnumerable<IEnumerable<double>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs)
        {
            int numInputs = 0; // number of inputs
            var inputsByOutputClass = new Dictionary<IOutputClass<TOutput>, IList<IEnumerable<double>>>();

            // map inputs by class
            var inputs = trainingInputs.GetEnumerator();
            var outputClasses = trainingOutputs.GetEnumerator();
            while (inputs.MoveNext() && outputClasses.MoveNext())
            {
                var oc = new OutputClass<TOutput>(1.0, outputClasses.Current);

                numInputs++;
                IList<IEnumerable<double>> inputsInOc;
                if (!inputsByOutputClass.TryGetValue(oc, out inputsInOc))
                {
                    inputsInOc = new List<IEnumerable<double>>();
                    inputsByOutputClass.Add(oc, inputsInOc);
                }
                inputsInOc.Add(inputs.Current);
            }

            // now get prior prob and collect counts
            foreach (var kv in inputsByOutputClass)
            {
                var oc = kv.Key;
                var inputsInOc = kv.Value;

                if (inputsInOc.Count == 0)
                    continue;

                priorByOutputClass[oc] = (double)inputsInOc.Count / (double)numInputs;

                double[] counts = null;
                foreach (var inOc in inputsInOc)
                {
                    if (counts == null)
                    {
                        counts = inOc.ToArray();
                    }
                    else
                    {
                        int i = 0;
                        foreach (var n in inOc)
                            counts[i++] += n;
                    }
                }

                // normalize & smooth
                double norm = 0;
                for (int t = 0; t < counts.Length; t++)
                    norm += counts[t] + 1;

                // calculate conditional probability
                IDictionary<int, double> probByFeature;
                if (!condProbByOutputClass.TryGetValue(oc, out probByFeature))
                {
                    probByFeature = new Dictionary<int, double>();
                    condProbByOutputClass.Add(oc, probByFeature);
                }

                for (int t = 0; t < counts.Length; t++)
                {
                    probByFeature.Add(t, (counts[t] + 1) / norm);
                }
            }
        }

        public double TestModel(IEnumerable<IEnumerable<double>> testingInputs, 
            IEnumerable<IEnumerable<TOutput>> testingOutputs,
            Func<TOutput, TOutput, bool> matches)
        {
            double total = 0, correct = 0;
            bool hasInput = false, hasOutput = false;

            var ei = testingInputs.GetEnumerator();
            var eo = testingOutputs.GetEnumerator();

            while ((hasInput = ei.MoveNext()) || (hasOutput = eo.MoveNext()))
            {
                total = total + 1.0;
                if (hasInput && hasOutput)
                {
                    var expected = eo.Current;
                    var actual = Classify(ei.Current).First().Output;

                    bool correctResult = true;
                    actual.Match(expected, (a, e) =>
                    {
                        if (!matches(a, e))
                        {
                            correctResult = false;
                            return false;
                        }
                        return true;
                    },
                    null);

                    if (correctResult)
                        correct += 1.0;
                }
            }

            return total > 0
                ? correct / total
                : double.NaN;
        }

        #endregion

        #region ISerializable Members

        public Multinomial(SerializationInfo info, StreamingContext context)
        {
            this.priorByOutputClass = (IDictionary<IOutputClass<TOutput>, double>)
                info.GetValue("prior", typeof(IDictionary<IOutputClass<TOutput>, double>));
            this.condProbByOutputClass = (IDictionary<IOutputClass<TOutput>, IDictionary<int, double>>)
                info.GetValue("condProb", typeof(IDictionary<IOutputClass<TOutput>, IDictionary<int, double>>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("prior", this.priorByOutputClass);
            info.AddValue("condProb", this.condProbByOutputClass);
        }

        #endregion
    }
}
