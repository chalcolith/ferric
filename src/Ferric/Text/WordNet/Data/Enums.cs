using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Data
{
    public enum SemanticClassType
    {
        Topical,
        Usage,
        Regional
    }

    public enum AdjectiveSyntax
    {
        Predicate,
        Prenominal,
        Postnominal
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
