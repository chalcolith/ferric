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
        ulong classFilters = 0;
        ISet<string> stopWords;

        public StopwordFilter(string file, string filter)
            : base()
        {
            LoadClassesToFilter(filter);
            LoadStopwords(file);
        }

        public StopwordFilter(string file)
            : base()
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
                    if ((classFilters & (1ul << (int)token.TokenClass)) == 0)
                        continue;

                    if (stopWords.Contains(token.Lemma))
                        continue;
                }
                yield return input;
            }
        }

        void LoadClassesToFilter(string classesToFilter)
        {
            classFilters = 0;
            if (string.IsNullOrWhiteSpace(classesToFilter))
                return;

            foreach (var classStr in classesToFilter.Split(','))
            {
                TokenClass tc;
                if (Enum.TryParse<TokenClass>(classStr.Trim(), out tc))
                {
                    classFilters |= (1ul << (int)tc);
                }
            }
        }

        void LoadStopwords(string stopWordsFilePath)
        {
            stopWordsFilePath = Path.GetFullPath(stopWordsFilePath);

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
