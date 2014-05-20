using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet
{
    public class Synset
    {
        public int SynsetId { get; set; }
        public int WordNetId { get; set; }

        public SynsetType SynsetType { get; set; }
        public string Gloss { get; set; }
        
        public virtual ICollection<WordSense> Senses { get; set; }

        public virtual ICollection<Synset> Hypernyms { get; set; }
        public virtual ICollection<Synset> Hyponyms { get; set; }

        public virtual ICollection<Synset> Prototypes { get; set; }
        public virtual ICollection<Synset> Instances { get; set; }

        public virtual ICollection<Synset> Entailments { get; set; }

        public virtual ICollection<Synset> Satellites { get; set; }

        public virtual ICollection<Synset> MemberMeronyms { get; set; }
        public virtual ICollection<Synset> MemberHolonyms { get; set; }

        public virtual ICollection<Synset> SubstanceMeronyms { get; set; }
        public virtual ICollection<Synset> SubstanceHolonyms { get; set; }

        public virtual ICollection<Synset> PartMeronyms { get; set; }
        public virtual ICollection<Synset> PartHolonyms { get; set; }

        public virtual ICollection<Synset> Derivations { get; set; }

        public virtual ICollection<Synset> Causes { get; set; }

        public virtual ICollection<Synset> Attributes { get; set; }
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
