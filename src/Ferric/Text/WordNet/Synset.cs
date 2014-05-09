using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet
{
    public class Synset
    {
        public virtual int Id { get; set; }

        public virtual int WordNetId { get; set; }
        public virtual SynsetType SynsetType { get; set; }
        public virtual string Gloss { get; set; }
        
        public virtual IEnumerable<WordSense> Senses { get; set; }
        
        public virtual IEnumerable<Synset> Hypernyms { get; set; }
        public virtual IEnumerable<Synset> Hyponyms { get; set; }

        public virtual IEnumerable<Synset> Prototypes { get; set; }
        public virtual IEnumerable<Synset> Instances { get; set; }

        public virtual IEnumerable<Synset> Entailments { get; set; }

        public virtual IEnumerable<Synset> Satellites { get; set; }

        public virtual IEnumerable<Synset> MemberMeronyms { get; set; }
        public virtual IEnumerable<Synset> MemberHolonyms { get; set; }

        public virtual IEnumerable<Synset> SubstanceMeronyms { get; set; }
        public virtual IEnumerable<Synset> SubstanceHolonyms { get; set; }

        public virtual IEnumerable<Synset> PartMeronyms { get; set; }
        public virtual IEnumerable<Synset> PartHolonyms { get; set; }

        public virtual IEnumerable<Synset> Derivations { get; set; }

        public virtual IEnumerable<Synset> Causes { get; set; }

        public virtual IEnumerable<Synset> Attributes { get; set; }
    }

    public enum SynsetType
    {
        Noun,
        Verb,
        Adjective,
        AdjectiveSatellite,
        Adverb
    }
}
