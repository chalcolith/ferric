using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Linear;

namespace Ferric.Math.Stats.Classifiers.Linear
{
    /// <summary>
    /// The implementation of this classifier assumes that the basis is built into 
    /// the inputs; that is, that input vectors will have 1 prepended to them.
    /// </summary>
    /// <typeparam name="T">The type of the features of this model.</typeparam>
    [Serializable]
    public class LeastSquares<T> : Classifier<T>
    {
        Matrix<T> beta;

        public int InputDimensions { get { return beta.Rows; } }
        public int OutputDimensions { get { return beta.Cols; } }

        public LeastSquares(int inputDimensions, int outputDimensions)
        {
            this.beta = new DenseMatrix<T>(inputDimensions, outputDimensions);
        }

        #region Classifier<T> Members

        public void TrainModel(IEnumerable<Tuple<Math.Linear.Vector<T>, Math.Linear.Vector<T>>> trainingData)
        {
            throw new NotImplementedException();
        }

        public void TestModel(IEnumerable<Tuple<Math.Linear.Vector<T>, Math.Linear.Vector<T>>> testingData)
        {
            throw new NotImplementedException();
        }

        public Math.Linear.Vector<T> Classify(Math.Linear.Vector<T> input)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISerializable Members

        public LeastSquares(SerializationInfo info, StreamingContext context)
        {
            beta = (Matrix<T>)info.GetValue("beta", typeof(Matrix<T>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("beta", this.beta);
        }

        #endregion
    }
}
