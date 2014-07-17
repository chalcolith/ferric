﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.MachineLearning;
using Ferric.Text.Common;
using Ferric.Text.Common.Documents;

namespace Ferric.Text.Classifiers
{
    public class Classifier<TLexiconEntry, TOutput> : BaseTransducer<IDocumentCollection<TLexiconEntry>, IEnumerable<TOutput>>
    {
        string modelPath;
        IClassifier<double, TOutput> classifier;

        public Classifier(ICreateContext context, string model)
            : base(context)
        {
            this.modelPath = model;
        }

        public override IEnumerable<IEnumerable<TOutput>> Process(IEnumerable<IDocumentCollection<TLexiconEntry>> inputs)
        {
            // load model
            using (var s = File.Open(CreateContext.GetFullPath(modelPath), FileMode.Open, FileAccess.Read))
            {
                var f = new BinaryFormatter();
                classifier = f.Deserialize(s) as IClassifier<double, TOutput>;
            }

            // classify
            foreach (var collection in inputs)
            {
                foreach (var row in collection.DocumentTermMatrix)
                {
                    var result = classifier.Classify(row);
                    yield return result;
                }
            }
        }
    }

    public static class Classifier
    {
        public static IList<IEnumerable<double>> GetTags(string path)
        {
            IList<IEnumerable<double>> tags = new List<IEnumerable<double>>();
            using (var sr = new StreamReader(path))
            {
                int num = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ++num;

                    double tag;
                    if (!double.TryParse(line, out tag))
                        throw new Exception(string.Format("Invalid tag '{0}' at {1} line {2}", line, path, num));

                    tags.Add(new[] { tag });
                }
            }

            return tags;
        }
    }
}
