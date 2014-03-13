using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text
{
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

        public IEnumerable Process(IEnumerable inputs)
        {
            return Process((IEnumerable<TIn>)inputs).ToList();
        }

        public abstract IEnumerable<TOut> Process(IEnumerable<TIn> inputs);

        protected IEnumerable SubProcess<TSubIn>(IEnumerable<TSubIn> inputs)
        {
            Type lastOutputType = typeof(TSubIn);

            IEnumerable lastEnumerable = inputs;
            foreach (var sub in SubTransducers)
            {
                if (!sub.InputType.IsAssignableFrom(lastOutputType))
                    throw new Exception(string.Format("Type {0} expects inputs of {1}, not {2}.", sub.GetType().Name, sub.InputType.Name, lastOutputType.Name));

                MethodInfo process = null;
                var methods = sub.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    if (method.Name != "Process")
                        continue;

                    var parms = method.GetParameters();
                    if (parms.Length == 1
                        && typeof(IEnumerable).IsAssignableFrom(parms[0].ParameterType)
                        && parms[0].ParameterType.IsGenericType
                        && parms[0].ParameterType.GetGenericArguments()[0].IsAssignableFrom(lastOutputType))
                    {
                        process = method;
                        break;
                    }
                }

                if (process == null)
                    throw new Exception(string.Format("Could not find a Process method for {0}.", sub.GetType().Name));
                lastOutputType = sub.OutputType;
                lastEnumerable = (IEnumerable) process.Invoke(sub, new object[] { lastEnumerable });
            }

            return lastEnumerable;
        }
    }

    public class DebugPrintTransducer : BaseTransducer<string, string>
    {
        public override IEnumerable<string> Process(IEnumerable<string> inputs)
        {
            var outputs = SubProcess(inputs);
            foreach (object output in outputs)
            {
                var span = output as ISpan;
                var str = span != null ? span.ToString("") : output.ToString();
                Console.WriteLine(str);
                yield return str;
            }
        }
    }
}
