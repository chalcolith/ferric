using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;

namespace Ferric.Math.MachineLearning
{
    public interface Classifier<TInput, TOutput> : ISerializable
    {
        void TrainModel(IEnumerable<IEnumerable<TInput>> trainingInputs, IEnumerable<IEnumerable<TOutput>> trainingOutputs);
        double TestModel(IEnumerable<IEnumerable<TInput>> testingInputs, IEnumerable<IEnumerable<TOutput>> testingOutputs, Func<TOutput, TOutput, bool> matches = null);

        IEnumerable<TOutput> Classify(IEnumerable<TInput> input);
    }
}
