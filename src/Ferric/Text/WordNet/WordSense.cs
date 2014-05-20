using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet
{
    public class WordSense
    {
        public int WordSenseId { get; set; }

        public virtual Synset Synset { get; set; }

        public int WordNum { get; set; }
        public string Lemma { get; set; }
        public int SenseNumber { get; set; }
        public int TagCount { get; set; }
        public string SenseKey { get; set; }
        public AdjectiveSyntax Syntax { get; set; }
        public int Frame { get; set; }

        public virtual ICollection<WordSense> GroupedWith { get; set; }
        public virtual ICollection<WordSense> Antonyms { get; set; }
        public virtual ICollection<WordSense> SeeAlso { get; set; }
        public virtual ICollection<WordSense> Participles { get; set; }
        public virtual ICollection<WordSense> Pertains { get; set; }
    }

    public enum AdjectiveSyntax
    {
        Predicate,
        Prenominal,
        Postnominal
    }
}
