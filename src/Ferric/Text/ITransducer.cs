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

        IEnumerable<ITransducer> SubTransducers { get; }
    }

    public interface ITransducer<TIn, TOut> : ITransducer
    {
        IEnumerable<TOut> Process(IEnumerable<TIn> inputs);
    }

    public abstract class BaseTransducer<TIn, TOut> : ITransducer<TIn, TOut>
    {
        IEnumerable<ITransducer> subTransducers;

        public Type InputType { get { return typeof(TIn); } }
        public Type OutputType { get { return typeof(TOut); } }

        public IEnumerable<ITransducer> SubTransducers
        {
            get { return subTransducers ?? Enumerable.Empty<ITransducer>(); }
            set { subTransducers = value; }
        }

        public abstract IEnumerable<TOut> Process(IEnumerable<TIn> inputs);

        protected IEnumerable<TSubOut> SubProcess<TSubIn, TSubOut>(IEnumerable<TSubIn> inputs)
        {
            object lastEnumerable = inputs;
            foreach (var sub in SubTransducers)
            {
                var process = sub.GetType().GetMethod("Process", BindingFlags.Public | BindingFlags.Instance);
                if (process == null)
                    throw new Exception("Could not find a Process method for type " + sub.GetType().FullName);
                lastEnumerable = process.Invoke(sub, new object[] { lastEnumerable });
            }
            return (IEnumerable<TSubOut>)lastEnumerable;
        }

        protected bool SubPathIsValid(Type subInputType, Type subOutputType)
        {
            var curType = subInputType;
            foreach (var sub in SubTransducers)
            {
                if (!sub.InputType.IsAssignableFrom(curType))
                    return false;
                curType = sub.OutputType;
            }
            return subOutputType.IsAssignableFrom(curType);
        }
    }
}
