using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Ferric.Text.Common;

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
            return Load(elem);
        }

        public static ITransducer Load(XElement elem)
        {
            return CreateFromXml(elem, null, new CreateContext());
        }

        static ITransducer CreateFromXml(XElement elem, ITransducer parent, CreateContext context)
        {
            // find type
            var typeName = elem.Name.LocalName;
            var type = GetLoadedType(typeName, context);
            if (type == null)
                throw new Exception(string.Format("Unable to find a transducer of type {0}.", typeName));

            if (type.IsGenericType)
                throw new Exception(string.Format("You cannot create an instance of a generic transducer from a config file."));

            // get parameters
            var atts = elem.Attributes()
                .ToDictionary(att => att.Name.LocalName, att => att.Value);

            // find an appropriate constructor
            ITransducer transducer = null;

            var ctors = type.GetConstructors();
            if (ctors.Length == 0)
            {
                if (atts.Count > 0)
                    throw new Exception(string.Format("Unable to find a constructor for type {0} with parameters ({1}).", typeName, string.Join(", ", atts.Keys)));

                transducer = (ITransducer)Activator.CreateInstance(type);
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
                    transducer = (ITransducer)ctor.Invoke(actualParms);
                    break;
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

            // now get sub transducers
            transducer.SubTransducers = elem.Elements()
                .Select(subElem => CreateFromXml(subElem, transducer, context))
                .Where(subTransducer => subTransducer != null)
                .ToList();

            return transducer;
        }

        class CreateContext
        {
            public string ConfigDir { get; set; }
            public IDictionary<string, Type> TypeCache { get; set; }

            public CreateContext()
            {
                ConfigDir = Directory.GetCurrentDirectory();
                TypeCache = new Dictionary<string, Type>();
            }
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
