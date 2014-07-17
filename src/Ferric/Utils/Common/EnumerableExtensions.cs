using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Utils.Common
{
    public static class EnumerableExtensions
    {
        public static int CompareTo<T>(this IEnumerable<T> a, IEnumerable<T> b)
            where T : IComparable<T>
        {
            int result = 0;
            Func<T, T, bool> onMatch = (T itemA, T itemB) =>
            {
                var cmp = itemA.CompareTo(itemB);
                if (cmp != 0)
                {
                    result = cmp;
                    return false;
                }
                return true;
            };
            Action<bool, bool> postMatch = (bool hasA, bool hasB) =>
            {
                if (hasA)
                    result = 1;
                else if (hasB)
                    result = -1;
                else
                    result = 0;
            };

            Match(a, b, onMatch, postMatch);
            return result;
        }

        public static void Match<T>(this IEnumerable<T> a, IEnumerable<T> b, Func<T, T, bool> onMatch, Action<bool, bool> postMatch)
        {
            bool hasA = false;
            bool hasB = false;
            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();

            while ((hasA = ea.MoveNext()) && (hasB = eb.MoveNext()))
            {
                if (onMatch != null && !onMatch(ea.Current, eb.Current))
                    return;
            }

            if (postMatch != null)
                postMatch(hasA, hasB);
        }
    }
}
