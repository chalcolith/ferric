using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                // load all dlls in the exe directory
                var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var entry in Directory.EnumerateFiles(dllDir, "*.dll"))
                {
                    try
                    {
                        Assembly.LoadFile(entry);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error loading {0}: {1}.", entry, e.Message);
                    }
                }

                // load transducer
                var transducer = Pipeline.Load(options.ConfigFilePath);
                if (!transducer.InputType.IsAssignableFrom(typeof(string)))
                    throw new Exception("The top-level transducer must take strings for its input.");

                // run
                var results = transducer.Process(options.Arguments);
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
