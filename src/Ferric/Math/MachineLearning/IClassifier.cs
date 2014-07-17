using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Utils.Common;

namespace Ferric.Math.MachineLearning
{
    public interface IClassifier<TInput, TOutput> : ISerializable
        where TOutput : IComparable<TOutput>
    {
        IEnumerable<IOutputClass<TOutput>> Classify(IEnumerable<TInput> input);

        void TrainModel(IEnumerable<IEnumerable<TInput>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs);
        double TestModel(IEnumerable<IEnumerable<TInput>> testingInputs, IEnumerable<IEnumerable<TOutput>> testingOutputs, Func<TOutput, TOutput, bool> matches);  
    }

    public interface IOutputClass<T> : IComparable<IOutputClass<T>>
    {
        double Weight { get; set; }
        IEnumerable<T> Output { get; set; }
    }

    [Serializable]
    public class OutputClass<TOutput> : IOutputClass<TOutput>
        where TOutput : IComparable<TOutput>
    {
        public OutputClass(double weight, IEnumerable<TOutput> output)
        {
            this.Weight = weight;
            this.Output = output;
        }

        #region IClass<TOutput> Members

        public double Weight { get; set; }
        public IEnumerable<TOutput> Output { get; set; }

        #endregion

        #region IComparable<IClass<TOutput>> Members

        public int CompareTo(IOutputClass<TOutput> other)
        {
            return this.Output.CompareTo(other.Output);
        }

        #endregion

        #region Object Members

        int? hashCode;

        public override bool Equals(object obj)
        {
            var oc = obj as OutputClass<TOutput>;
            if (oc == null)
                return false;

            return Output.SequenceEqual(oc.Output);
        }

        public override int GetHashCode()
        {
            if (hashCode == null)
            {
                int hash = int.MinValue;
                foreach (var output in Output)
                    hash = hash ^ output.GetHashCode();
                hashCode = hash;
            }
            return hashCode.Value;
        }

        #endregion
    }
}
