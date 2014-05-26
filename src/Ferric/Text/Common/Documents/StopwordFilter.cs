using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Documents
{
    public class StopwordFilter : BaseTransducer<ISpan, ISpan>
    {
        ISet<string> stopWords;

        public StopwordFilter(ICreateContext context, string file)
            : base(context)
        {
            LoadStopwords(file);
        }

        public override IEnumerable<ISpan> Process(IEnumerable<ISpan> inputs)
        {
            foreach (var input in inputs)
            {
                var token = input as TokenSpan;
                if (token != null)
                {
                    if (stopWords.Contains(token.Lemma))
                        continue;
                }
                yield return input;
            }
        }

        void LoadStopwords(string stopWordsFilePath)
        {
            stopWordsFilePath = CreateContext.GetFullPath(stopWordsFilePath);

            if (!File.Exists(stopWordsFilePath))
                throw new Exception(string.Format("Unable to find stopwords file {0}", stopWordsFilePath));

            stopWords = new HashSet<string>();

            using (var sr = new StreamReader(stopWordsFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    stopWords.Add(line.Trim().ToLowerInvariant());
                }
            }
        }
    }
}
