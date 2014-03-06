using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;

namespace Ferric.Math.Stats.Classifiers.Linear
{
    /// <summary>
    /// The implementation of this classifier assumes that the basis is built into 
    /// the inputs; that is, that input vectors will have 1 prepended to them.
    /// </summary>
    /// <typeparam name="TInput">The type of the input features of this model.</typeparam>
    /// <typeparam name="TOutput">The type of the output features of this model.</typeparam>
    [Serializable]
    public class LeastSquares<TInput, TOutput> : Classifier<TInput, TOutput>
        where TOutput : struct, IConvertible
    {
        Func<TOutput, TInput> toInput;
        Func<TInput, TOutput> toOutput;
        Matrix<TInput> beta;

        public int InputDimensions { get { return beta.Rows; } }
        public int OutputDimensions { get { return beta.Cols; } }

        public LeastSquares(int inputDimensions, int outputDimensions, Func<TOutput, TInput> toInput = null, Func<TInput, TOutput> toOutput = null)
        {
            this.beta = new DenseMatrix<TInput>(inputDimensions, outputDimensions);
            this.toInput = toInput ?? (o => (TInput)Convert.ChangeType(o, typeof(TInput)));
            this.toOutput = toOutput ?? (i => (TOutput)Convert.ChangeType(i, typeof(TOutput)));
        }

        #region Classifier<T> Members

        public void TrainModel(IEnumerable<IEnumerable<TInput>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs)
        {
            var X = new DenseMatrix<TInput>(trainingInputs, copy: false);
            DenseMatrix<TInput> y;

            if (typeof(TInput) == typeof(TOutput))
            {
                y = new DenseMatrix<TInput>((IEnumerable<IEnumerable<TInput>>)trainingOutputs, copy: false);
            }
            else
            {
                var temp = trainingOutputs.Select(row => row.Select(item => toInput(item)));
                y = new DenseMatrix<TInput>(temp, copy: true);
            }

            var Xt = X.Transpose();
            beta = (Xt * X).Inverse() * Xt * y;
        }

        public double TestModel(IEnumerable<IEnumerable<TInput>> testingInputs, 
            IEnumerable<IEnumerable<TOutput>> testingOutputs, 
            Func<TOutput, TOutput, bool> matches = null)
        {
            if (beta == null)
                throw new ArgumentException("Model has not been trained");

            if (matches == null)
                matches = (a, b) => a.Equals(b);

            double sum = 0, total = 0;
            var input = testingInputs.GetEnumerator();
            var expected = testingOutputs.GetEnumerator();
            while (input.MoveNext() && expected.MoveNext())
            {
                var res = Classify(input.Current).GetEnumerator();
                var exp = expected.Current.GetEnumerator();
                bool equal = true;
                while (res.MoveNext() && exp.MoveNext())
                {
                    equal = equal && matches(exp.Current, res.Current);
                }
                total += 1;
                if (equal)
                    sum += 1;
            }

            if (total > 1.0e-20)
                return sum / total;
            else
                return double.NaN;
        }

        public Vector<TOutput> Classify(Vector<TInput> input)
        {
            var result = input * beta;
            var row = Enumerable.Range(0, result.Cols).Select(i => toOutput(result[0, i]));
            return new Vector<TOutput>(row, copy: true);
        }

        public Vector<TOutput> Classify(IEnumerable<TInput> input)
        {
            return Classify(new Vector<TInput>(input));
        }

        #endregion

        #region ISerializable Members

        public LeastSquares(SerializationInfo info, StreamingContext context)
        {
            beta = (Matrix<TInput>)info.GetValue("beta", typeof(Matrix<TInput>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("beta", this.beta);
        }

        #endregion
    }
}
