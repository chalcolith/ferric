using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Ferric.Text;

namespace Ferric.Config
{
    public static class Pipeline
    {
        public static ITransducer Load(string path)
        {
            using (var tr = new StreamReader(path))
                return Load(tr);
        }

        public static ITransducer Load(TextReader tr)
        {
            using (var xr = XmlReader.Create(tr))
                return Load(xr);
        }

        public static ITransducer Load(XmlReader xr)
        {
            var elem = XElement.Load(xr);            
            return Create(elem, new CreateContext());
        }

        class CreateContext
        {
            public IDictionary<string, Type> TypeCache { get; set; }

            public CreateContext()
            {
                TypeCache = new Dictionary<string, Type>();
            }
        }

        static ITransducer Create(XElement elem, CreateContext context)
        {
            // find type
            var typeName = elem.Name.LocalName;
            var type = GetLoadedType(typeName, context);
            if (type == null)
                throw new Exception(string.Format("Unknown transducer type {0}", typeName));

            // get parameters
            var atts = elem.Attributes()
                .ToDictionary(att => att.Name.LocalName, att => att.Value);

            // find an appropriate constructor
            ITransducer transducer = null;

            var ctors = type.GetConstructors(BindingFlags.Public);
            IList<string> errors = new List<string>();
            foreach (var ctor in ctors)
            {
                var formalParms = ctor.GetParameters();
                var actualParms = new List<object>();
                foreach (var formalParm in formalParms)
                {
                }
            }

            // handle failure to launch
            if (transducer == null)
            {
                errors = errors.Distinct().ToList();
                if (errors.Any())
                    throw new Exception(string.Format("Unable to find an appropriate constructor for type {0}:\n{1}", type.FullName, string.Join("\n", errors)));
                else
                    throw new Exception(string.Format("Unable to find an appropriate constructor for type {0}", type.FullName));
            }

            // now get children
            transducer.SubTransducers = elem.Elements()
                .Select(child => Create(child, context))
                .Where(sub => sub != null);

            return transducer;
        }

        static Type GetLoadedType(string name, CreateContext context)
        {
            Type loadedType;
            if (context.TypeCache.TryGetValue(name, out loadedType))
                return loadedType;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var foundType = assembly.GetType(name);
                if (foundType != null && foundType.IsAssignableFrom(typeof(ITransducer)))
                    return foundType;
            }

            return null;
        }
    }
}
