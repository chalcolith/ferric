using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common
{
    public interface ITransducer
    {
        Type InputType { get; }
        Type OutputType { get; }

        IEnumerable<ITransducer> SubTransducers { get; set; }
        
        IEnumerable Process(IEnumerable inputs);
    }

    public interface ITransducer<in TIn, out TOut> : ITransducer
    {
        IEnumerable<TOut> Process(IEnumerable<TIn> inputs);
    }

    public interface ICreateContext
    {
        string ConfigDir { get; }
        string GetFullPath(string path);
    }
}
