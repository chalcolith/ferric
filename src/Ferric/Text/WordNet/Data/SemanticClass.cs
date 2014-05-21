using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Data
{
    public class SemanticClass
    {
        [Key]
        public virtual int SemanticClassId { get; set; }

        public SemanticClassType ClassType { get; set; }

        public virtual ICollection<WordSense> Heads { get; set; }
        public virtual ICollection<WordSense> Members { get; set; }
    }
}
