using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text;
using SharpYaml.Serialization;

namespace Ferric.Config
{
    public static class Pipeline
    {
        public static ITransducer Load(string yamlFileName)
        {
            using (var reader = new StreamReader(yamlFileName))
            {
                return Load(reader);
            }
        }

        public static ITransducer Load(TextReader reader)
        {
            var yaml = new YamlStream();
            yaml.Load(reader);

            var transducers = new List<ITransducer>();
            foreach (var doc in yaml.Documents)
            {

            }

            if (transducers.Count == 1)
                return transducers[0];

            return null;
        }

        ITransducer LoadDoc(YamlDocument doc)
        {

        }
    }
}
