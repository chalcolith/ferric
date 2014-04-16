using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common;

namespace Ferric.Config
{
    public class CreateContext : ICreateContext
    {
        public string ConfigDir { get; set; }
        public IDictionary<string, Type> TypeCache { get; private set; }

        public CreateContext()
        {
            ConfigDir = Directory.GetCurrentDirectory();
            TypeCache = new Dictionary<string, Type>();
        }
    }
}
