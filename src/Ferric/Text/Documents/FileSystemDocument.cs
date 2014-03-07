using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Documents
{
    public class FileSystemDocument : BaseSpan, IDocument
    {
        public const string FileNameKey = "filename";

        public string FileName
        {
            get { return Metadata[FileNameKey]; }
            set { Metadata[FileNameKey] = value; }
        }

        #region IDocument Members

        public string Identifier { get { return FileName; } }

        #endregion

        public override string ToString()
        {
            return string.Format("{{ {0}:{1} {2}-{3} {4} }}", this.GetType().Name, Ordinal, CharPos, CharNext, FileName);
        }
    }
}
