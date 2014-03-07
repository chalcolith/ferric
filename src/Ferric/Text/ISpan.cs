using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text
{
    public interface ISpan
    {
        ulong CharPos { get; }
        ulong CharNext { get; }
        ulong Ordinal { get; }

        IDictionary<string, string> Metadata { get; }
        IEnumerable<ISpan> Children { get; }

        IEnumerable<T> ChildrenOfType<T>() where T : class, ISpan;

        string ToString(string indent);
    }

    public abstract class BaseSpan : ISpan
    {
        ulong? charPos, charNext, ordinal;
        IDictionary<string, string> metadata;
        IList<ISpan> children;

        #region ISpan Members

        public ulong CharPos
        {
            get 
            {
                if (charPos == null)
                {
                    if (children != null && children.Count > 0)
                        charPos = children[0].CharPos;
                    else
                        charPos = 0;
                }
                return charPos.Value;
            }
            set
            {
                charPos = value;
            }
        }

        public ulong CharNext
        {
            get
            {
                if (charNext == null)
                {
                    if (children != null && children.Count > 0)
                        charNext = children[children.Count - 1].CharNext;
                    else
                        charNext = 0;
                }
                return charNext.Value;
            }
            set
            {
                charNext = value;
            }
        }

        public ulong Ordinal
        {
            get
            {
                return ordinal ?? 0;
            }
            set
            {
                ordinal = value;
            }
        }

        public IDictionary<string, string> Metadata 
        { 
            get { return metadata ?? (metadata = new Dictionary<string, string>()); } 
        }

        public IEnumerable<ISpan> Children
        {
            get { return children ?? Enumerable.Empty<ISpan>(); }
            set { children = value.ToList(); }
        }

        /// <summary>
        /// Returns an enumerable of spans of a given type in depth-first order, including the initial span.
        /// </summary>
        /// <typeparam name="T">The type of span to check for.</typeparam>
        public IEnumerable<T> ChildrenOfType<T>()
            where T: class, ISpan
        {
            foreach (var child in Children)
            {
                foreach (var cc in child.ChildrenOfType<T>())
                    yield return cc;
            }

            var t = this as T;
            if (t != null)
                yield return t;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{{ {0}:{1} {2}-{3} }}", this.GetType().Name, Ordinal, CharPos, CharNext);
        }

        public virtual string ToString(string indent)
        {
            var sb = new StringBuilder();
            sb.Append(indent);
            sb.AppendLine(this.ToString());
            indent = indent + "  ";
            foreach (var child in Children)
            {
                sb.Append(child.ToString(indent));
            }
            return sb.ToString();
        }
    }
}
