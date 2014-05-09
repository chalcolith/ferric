using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet
{
    public class WordSense
    {
        public virtual int Id { get; set; }

        public virtual Synset Synset { get; set; }
        public virtual int WordNum { get; set; }
        public virtual string Lemma { get; set; }
        public virtual int SenseNumber { get; set; }
        public virtual int TagCount { get; set; }
        public virtual int SenseKey { get; set; }
        public virtual AdjectiveSyntax Syntax { get; set; }
        public virtual int Frame { get; set; }

        public virtual IEnumerable<WordSense> GroupedWith { get; set; }
        public virtual IEnumerable<WordSense> Antonyms { get; set; }
        public virtual IEnumerable<WordSense> SeeAlso { get; set; }
        public virtual IEnumerable<WordSense> Participles { get; set; }
        public virtual IEnumerable<WordSense> Pertains { get; set; }
    }

    public enum AdjectiveSyntax
    {
        Predicate,
        Prenominal,
        Postnominal
    }
}
