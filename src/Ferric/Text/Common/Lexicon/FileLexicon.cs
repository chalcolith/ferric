using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Lexicon
{
    public class FileLexicon : BaseLexicon
    {
        private int nextIndex = 1;

        public string FullPath { get; private set; }

        public FileLexicon(string fname, bool createIfNotFound = false)
        {
            this.FullPath = Path.GetFullPath(fname);

            if (!File.Exists(this.FullPath))
            {
                if (createIfNotFound)
                    File.Create(FullPath);
                else
                    throw new ArgumentException("file does not exist: " + fname, "fname");
            }

            Load();
        }

        static readonly Regex LineRegex = new Regex(@"(\d+) (.*)", RegexOptions.Compiled);

        private void Load()
        {
            IndicesByLemma.Clear();
            LemmasByIndex.Clear();

            IndicesByLemma[Constants.UnknownToken] = 0;
            LemmasByIndex[0] = Constants.UnknownToken;

            using (var sr = new StreamReader(FullPath))
            {
                int num = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    num++;

                    var match = LineRegex.Match(line);
                    if (!match.Success)
                        throw new Exception("Error in line: " + num + ": " + line);

                    int index = int.Parse(match.Groups[1].Value);
                    var lemma = match.Groups[2].Value;

                    if (index >= nextIndex)
                        nextIndex = index + 1;

                    IndicesByLemma[lemma] = index;
                    LemmasByIndex[index] = lemma;
                }
            }
        }

        public void Save()
        {
            using (var sw = new StreamWriter(FullPath))
            {
                foreach (var kv in LemmasByIndex)
                {
                    sw.WriteLine("{0} {1}", kv.Key, kv.Value);
                }
            }
        }

        public override int AddLemma(string lemma)
        {
            int index;
            if (IndicesByLemma.TryGetValue(lemma, out index))
                return index;

            index = nextIndex++;
            IndicesByLemma[lemma] = index;
            LemmasByIndex[index] = lemma;

            return index;
        }
    }
}
