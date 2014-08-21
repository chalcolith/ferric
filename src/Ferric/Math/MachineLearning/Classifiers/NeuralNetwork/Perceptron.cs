using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;

namespace Ferric.Math.MachineLearning.Classifiers.NeuralNetwork
{
    public class Perceptron : BaseClassifier<double, double>
    {
        IMatrix<double> weights;

        public Perceptron(int numInputs, int numOutputs)
        {
            if (numInputs < 1)
                throw new ArgumentOutOfRangeException("numInputs", "must be greater than 0");
            if (numOutputs < 1)
                throw new ArgumentOutOfRangeException("numOutputs", "must be greater than 0");

            weights = new SparseMatrix<double>(numInputs + 1, numOutputs);
            for (int i = 0; i < weights.Cols; i++)
                weights[0, i] = -1;
        }

        #region IClassifier<double,double> Members

        public override IEnumerable<IOutputClass<double>> Classify(IEnumerable<double> input)
        {
            var iv = new DenseVector<double>(input);
            var act = iv * weights;
            return new[]
            {
                new OutputClass<double>(1.0, act.SelectMany(row => row.Where(n => n > 0)))
            };
        }

        public override void TrainModel(IEnumerable<IEnumerable<double>> trainingInputs, IEnumerable<IEnumerable<double>> trainingOutputs)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISerializable Members

        public Perceptron(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
