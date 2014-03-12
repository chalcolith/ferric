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
        public static ITransducer<TIn, TOut> Load<TIn, TOut>(string path)
        {
            using (var tr = new StreamReader(path))
                return Load<TIn, TOut>(tr);
        }

        public static ITransducer<TIn, TOut> Load<TIn, TOut>(TextReader tr)
        {
            using (var xr = XmlReader.Create(tr))
                return Load<TIn, TOut>(xr);
        }

        public static ITransducer<TIn, TOut> Load<TIn, TOut>(XmlReader xr)
        {
            var elem = XElement.Load(xr);
            return Load<TIn, TOut>(elem);
        }

        public static ITransducer<TIn, TOut> Load<TIn, TOut>(XElement elem)
        {
            return Create<TIn, TOut>(elem, null, new CreateContext());
        }

        class CreateContext
        {
            public IDictionary<string, Type> TypeCache { get; set; }

            public CreateContext()
            {
                TypeCache = new Dictionary<string, Type>();
            }
        }

        static ITransducer<TIn, TOut> Create<TIn, TOut>(XElement elem, ITransducer parent, CreateContext context)
        {
            // find type
            var typeName = elem.Name.LocalName;
            var type = GetLoadedType(typeName, context);
            if (type == null)
                throw new Exception(string.Format("Unable to find a transducer of type {0}.", typeName));

            // get parameters
            var atts = elem.Attributes()
                .ToDictionary(att => att.Name.LocalName, att => att.Value);

            // find an appropriate constructor
            ITransducer<TIn, TOut> transducer = null;

            var ctors = type.GetConstructors(BindingFlags.Public);
            if (ctors.Length == 0)
            {
                if (atts.Count > 0)
                    throw new Exception(string.Format("Unable to find a constructor for type {0} with parameters {1}.", typeName, string.Join(", ", atts.Keys)));

                transducer = (ITransducer<TIn, TOut>)Activator.CreateInstance(type);
            }
            else
            {
                foreach (var ctor in ctors)
                {
                    // check if we have all the right names
                    var formalParms = ctor.GetParameters();
                    if (formalParms.Any(p => !atts.ContainsKey(p.Name)))
                        continue;

                    // assemble the actual parameters
                    var actualParms = formalParms.Select(formalParm =>
                        {
                            var actualStr = atts[formalParm.Name];

                            try
                            {
                                object actualObj = Convert.ChangeType(actualStr, formalParm.ParameterType);
                                if (actualObj == null)
                                    throw new Exception("Value is empty");
                                return actualObj;
                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format("Unable to convert value '{0}' for paramter {2} to {3}: {4}",
                                    actualStr, formalParm.Name, formalParm.ParameterType.FullName, e.Message));
                            }
                        }).ToArray();

                    // call the constructor
                    transducer = (ITransducer<TIn, TOut>)ctor.Invoke(actualParms);
                }
            }

            // handle failure to launch
            if (transducer == null)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Unable to find a constructor for type {0} with parameters {1}.", typeName,
                    string.Join(", ", atts.Keys));
                sb.AppendLine();
                sb.AppendLine("Available constructors are:");
                foreach (var ctor in ctors)
                {
                    var parms = ctor.GetParameters().Select(p => string.Format("{0} {1}", p.ParameterType, p.Name));
                    sb.AppendFormat("  {0}({1})", typeName, string.Join(", ", parms));
                    sb.AppendLine();
                }
                throw new Exception(sb.ToString());
            }

            // now get children
            transducer.SubTransducers = elem.Elements()
                .Select(child => Create<TIn, TOut>(child, transducer, context))
                .Where(sub => sub != null)
                .ToList();

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
                if (foundType != null && typeof(ITransducer).IsAssignableFrom(foundType))
                    return foundType;
            }

            return null;
        }
    }
}
