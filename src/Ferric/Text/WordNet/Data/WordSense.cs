using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Ferric.Text.WordNet.Data
{
    public partial class WordSense
    {
        [Key]
        public int WordSenseId { get; set; }
        public int WordNum { get; set; }

        [Required]
        [MaxLength(200)]
        public string Lemma { get; set; }

        public int SenseNumber { get; set; }
        public int? TagCount { get; set; }

        [MaxLength(200)]
        public string SenseKey { get; set; }

        public AdjectiveSyntax? Syntax { get; set; }
        public int? Frame { get; set; }

        public int? SynsetId { get; set; }
        public virtual Synset Synset { get; set; }

        public virtual ICollection<WordSense> Groups { get; set; }
        public virtual ICollection<WordSense> Antonyms { get; set; }
        public virtual ICollection<WordSense> SeeAlsos { get; set; }
        public virtual ICollection<WordSense> Participles { get; set; }
        public virtual ICollection<WordSense> PertainsTo { get; set; }
    }
}
