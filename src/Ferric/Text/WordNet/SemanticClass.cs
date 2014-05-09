using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet
{
    public class SemanticClass
    {
        public virtual int Id { get; set; }

        public virtual ClassType ClassType { get; set; }
        public virtual IEnumerable<WordSense> Heads { get; set; }
        public virtual IEnumerable<WordSense> Members { get; set; }
    }

    public enum ClassType
    {
        Topical,
        Usage,
        Regional
    }
}
