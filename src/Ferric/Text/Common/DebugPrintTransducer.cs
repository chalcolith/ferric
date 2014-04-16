using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common
{
    public class DebugPrintTransducer : BaseTransducer<string, string>
    {
        public DebugPrintTransducer(ICreateContext context)
            : base(context)
        {
        }

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
