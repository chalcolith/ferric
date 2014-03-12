using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric
{
    class Options
    {
        public string ConfigFilePath { get; set; }
        public IEnumerable<string> Arguments { get; set; }

        public static Options Parse(IEnumerable<string> args)
        {
            var options = new Options()
            {
                ConfigFilePath = args.First(),
                Arguments = args.Skip(1)
            };
            return options;
        }
    }
}
