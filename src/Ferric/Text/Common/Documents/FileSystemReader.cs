﻿using System;
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
        public const string PathNameKey = "pathname";

        Encoding encoding = Encoding.UTF8;

        public override IEnumerable<IDocument> Process(IEnumerable<string> paths)
        {
            ulong ordinal = 0;
            foreach (var path in paths)
            {
                var absPath = Path.GetFullPath(path);
                using (var sr = new StreamReader(absPath, encoding))
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
}