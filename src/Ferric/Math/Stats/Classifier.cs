using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Linear;

namespace Ferric.Math.Stats
{
    public interface Classifier<T> : ISerializable
    {
        void TrainModel(IEnumerable<Tuple<Vector<T>, Vector<T>>> trainingData);
        void TestModel(IEnumerable<Tuple<Vector<T>, Vector<T>>> testingData);

        Vector<T> Classify(Vector<T> input);
    }
}
