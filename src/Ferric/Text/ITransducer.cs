using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text
{
    public interface ITransducer<TIn, TOut>
    {
        IEnumerable<TOut> Process(IEnumerable<TIn> input);
    }

    public interface ISpanTransducer : ITransducer<ISpan, ISpan>
    {
    }
}
