using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Config;
using Ferric.Text;
using Ferric.Text.Documents;
using Ferric.Text.Tokenizer;

namespace Ferric
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: ferric configfile [parameters...]");
                return 1;
            }

            var options = Options.Parse(args);

            try
            {
                var transducer = Pipeline.Load<string, object>(options.ConfigFilePath);
                if (!transducer.InputType.IsAssignableFrom(typeof(IEnumerable<string>)))
                    throw new Exception("The top-level transducer must take strings for its input.");

                transducer.Process(options.Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 2;
            }

            return 0;
        }
    }
}
