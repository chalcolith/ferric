using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Text.Common;
using Ferric.Text.Common.Documents;
using Ferric.Utils.Common;

namespace Ferric.Text.Classifiers.NaiveBayes.Multinomial
{
    public class Trainer<TLexiconEntry> : BaseTransducer<IDocumentCollection<TLexiconEntry>, string>
    {
        string tagsPath;
        string modelPath;

        public Trainer(ICreateContext context, string tags, string model)
            : base(context)
        {
            this.tagsPath = tags;
            this.modelPath = model;
        }

        public override IEnumerable<string> Process(IEnumerable<IDocumentCollection<TLexiconEntry>> inputs)
        {
            // get tags
            var tags = Classifier.GetTags(CreateContext.GetFullPath(tagsPath));

            // now train
            IEnumerable<IEnumerable<double>> counts = inputs.SelectMany(dtm => dtm.DocumentTermMatrix);
            var classifier = new Math.MachineLearning.Classifiers.NaiveBayes.Multinomial<double>();

            classifier.TrainModel(counts, new CircularEnumerable<IEnumerable<double>>(tags));

            // save
            var fullPath = CreateContext.GetFullPath(modelPath);
            using (var s = File.Open(fullPath, FileMode.Create, FileAccess.Write))            
            {
                var f = new BinaryFormatter();
                f.Serialize(s, classifier);
            }

            return new[] { fullPath };
        }
    }
}
