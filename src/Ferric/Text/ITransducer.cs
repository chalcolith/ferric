using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text
{
    public interface ITransducer
    {
        Type InputType { get; }
        Type OutputType { get; }

        IEnumerable<ITransducer> SubTransducers { get; set; }
        IEnumerable Process(IEnumerable inputs);
    }

    public interface ITransducer<TIn, TOut> : ITransducer
    {
        IEnumerable<TOut> Process(IEnumerable<TIn> inputs);
    }
}
