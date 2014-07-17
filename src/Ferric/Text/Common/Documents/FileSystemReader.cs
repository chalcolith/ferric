using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Documents
{
    /// <summary>
    /// Takes as input a list of path names and returns a list of documents.
    /// </summary>
    public class FileSystemReader : BaseTransducer<string, IDocument>
    {
        Encoding encoding = Encoding.UTF8;

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public FileSystemReader(ICreateContext context)
            : base(context)
        {
        }

        public override IEnumerable<IDocument> Process(IEnumerable<string> paths)
        {
            ulong ordinal = 0;
            foreach (var path in paths)
            {
                var absPath = Path.GetFullPath(path);
                using (var sr = new StreamReader(absPath, Encoding))
                {
                    var spans = SubProcess(sr.ReadChars()).OfType<ISpan>();
                    var doc = new FileSystemDocument
                    {
                        FileName = absPath,
                        Children = spans,
                        Ordinal = ordinal++
                    };
                    yield return doc;
                }
            }
        }
    }

    public class FileSystemReaderByLine : FileSystemReader
    {
        public FileSystemReaderByLine(ICreateContext context)
            : base(context)
        {
        }

        public override IEnumerable<IDocument> Process(IEnumerable<string> paths)
        {
            ulong ordinal = 0;
            foreach (var path in paths)
            {
                var absPath = Path.GetFullPath(path);
                using (var sr = new StreamReader(absPath, Encoding))
                {
                    int num = 0;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        num++;
                        var spans = SubProcess(line).OfType<ISpan>();
                        var doc = new FileSystemDocument
                        {
                            FileName = string.Format("{0}:{1}", absPath, num),
                            Children = spans,
                            Ordinal = ordinal++
                        };
                        yield return doc;
                    }
                }
            }
        }
    }

    public class LoadFiles : BaseTransducer<string, string>
    {
        IEnumerable<string> paths;

        public LoadFiles(ICreateContext context, string files)
            : base(context)
        {
            paths = files.Split(',');
        }

        public override IEnumerable<string> Process(IEnumerable<string> inputs)
        {
            return paths.Select(path => CreateContext.GetFullPath(path));
        }
    }
}
