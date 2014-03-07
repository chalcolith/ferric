using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text;
using Ferric.Text.Documents;
using Ferric.Text.Tokenizer;

namespace Ferric
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenizer = new UnicodeRegexpTokenizer();
            var reader = new FileSystemReader
            {
                SubTransducers = new [] { tokenizer }
            };

            foreach (var document in reader.Process(args))
            {
                Console.WriteLine(document.ToString(""));
            }
        }
    }
}
