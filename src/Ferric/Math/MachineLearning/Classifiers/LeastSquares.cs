using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Utils.Common;

namespace Ferric.Math.MachineLearning.Classifiers
{
    /// <summary>
    /// The implementation of this classifier assumes that the basis is built into 
    /// the inputs; that is, that input vectors will have 1 prepended to them.
    /// </summary>
    /// <typeparam name="TInput">The type of the input features of this model.</typeparam>
    /// <typeparam name="TOutput">The type of the output features of this model.</typeparam>
    [Serializable]
    public class LeastSquares<TInput, TOutput> : BaseClassifier<TInput, TOutput>
        where TInput : struct, IComparable<TInput>
        where TOutput : struct, IComparable<TOutput>
    {
        Func<TOutput, TInput> toInput;
        Func<TInput, TOutput> toOutput;
        Matrix<TInput> beta;

        public int InputDimensions { get { return beta.Rows; } }
        public int OutputDimensions { get { return beta.Cols; } }

        public LeastSquares(int inputDimensions, int outputDimensions, Func<TOutput, TInput> toInput = null, Func<TInput, TOutput> toOutput = null)
        {
            this.beta = new SparseMatrix<TInput>(inputDimensions, outputDimensions);
            this.toInput = toInput ?? (o => (TInput)Convert.ChangeType(o, typeof(TInput)));
            this.toOutput = toOutput ?? (i => (TOutput)Convert.ChangeType(i, typeof(TOutput)));
        }

        #region IClassifier<TInput, TOutput> Members

        public override IEnumerable<IOutputClass<TOutput>> Classify(IEnumerable<TInput> input)
        {
            return Classify((Matrix<TInput>)new SparseVector<TInput>(input));
        }

        public IEnumerable<IOutputClass<TOutput>> Classify(Matrix<TInput> input)
        {
            var result = input * beta;
            var row = Enumerable.Range(0, result.Cols).Select(i => toOutput(result[0, i]));
            return new[] { new OutputClass<TOutput>(1.0, new DenseVector<TOutput>(row, copy: true)) };
        }

        public override void TrainModel(IEnumerable<IEnumerable<TInput>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs)
        {
            var X = new SparseMatrix<TInput>(trainingInputs);
            SparseMatrix<TInput> y;

            if (typeof(TInput) == typeof(TOutput))
            {
                y = new SparseMatrix<TInput>((IEnumerable<IEnumerable<TInput>>)trainingOutputs);
            }
            else
            {
                var temp = trainingOutputs.Select(row => row.Select(item => toInput(item)));
                y = new SparseMatrix<TInput>(temp);
            }

            var Xt = X.Transpose();
            beta = (Xt * X).Inverse() * Xt * y;
        }

        public override double TestModel(IEnumerable<IEnumerable<TInput>> testingInputs,
            IEnumerable<IEnumerable<TOutput>> testingOutputs,
            Func<TOutput, TOutput, double> calcMatch = null)
        {
            if (beta == null)
                throw new ArgumentException("Model has not been trained");

            return base.TestModel(testingInputs, testingOutputs, calcMatch);
        }

        #endregion

        #region ISerializable Members

        public LeastSquares(SerializationInfo info, StreamingContext context)
        {
            beta = (Matrix<TInput>)info.GetValue("beta", typeof(Matrix<TInput>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("beta", this.beta);
        }

        #endregion
    }
}
