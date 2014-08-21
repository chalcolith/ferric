using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;

namespace Ferric.Math.MachineLearning
{
    public static class Extensions
    {
        public static double MatchPercent<T>(this IEnumerable<T> a, IEnumerable<T> b, Func<T, T, double> matches)
        {
            var elements = a.Zip(b, matches);
            double total = 0;
            double sum = 0;
            foreach (var element in elements)
            {
                total += 1;
                sum += element;                   
            }

            if (total > Constants.Epsilon)
                return sum / total;
            else
                return double.NaN;
        }
    }
}
