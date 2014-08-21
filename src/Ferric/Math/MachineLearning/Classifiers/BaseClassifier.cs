using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.MachineLearning.Classifiers
{
    public abstract class BaseClassifier<TInput, TOutput> : IClassifier<TInput, TOutput>
        where TOutput : IComparable<TOutput>
    {
        #region IClassifier<TInput, TOutput> Members

        public abstract IEnumerable<IOutputClass<TOutput>> Classify(IEnumerable<TInput> input);
        public abstract void TrainModel(IEnumerable<IEnumerable<TInput>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs);

        public virtual double TestModel(IEnumerable<IEnumerable<TInput>> testingInputs, 
            IEnumerable<IEnumerable<TOutput>> testingOutputs, 
            Func<TOutput, TOutput, double> calcMatch = null)
        {
            if (calcMatch == null)
                calcMatch = (a, b) => a.Equals(b) ? 1.0 : 0.0;

            var results = testingInputs.Zip(testingOutputs, (input, expected) =>
            {
                var actual = Classify(input).First().Output;
                return expected.MatchPercent(actual, calcMatch);
            });

            double total = 0, correct = 0;
            foreach (var result in results)
            {
                total += 1;
                if (result >= 0.5)
                    correct += 1;
            }

            return total > 0
                ? correct / total
                : double.NaN;
        }

        #endregion

        #region ISerializable Members

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        #endregion
    }
}
