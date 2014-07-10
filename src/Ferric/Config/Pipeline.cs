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
        public const string ContextParameterName = "context";

        public static ITransducer Load(string path, string configDir = null)
        {
            using (var tr = new StreamReader(path))
                return Load(tr, Path.GetDirectoryName(path));
        }

        public static ITransducer Load(TextReader tr, string configDir = null)
        {
            using (var xr = XmlReader.Create(tr))
                return Load(xr, configDir);
        }

        public static ITransducer Load(XmlReader xr, string configDir = null)
        {
            var elem = XElement.Load(xr);
            return Load(elem, configDir);
        }

        public static ITransducer Load(XElement elem, string configDir = null)
        {
            var context = new CreateContext
            {
                ConfigDir = string.IsNullOrWhiteSpace(configDir) ? Directory.GetCurrentDirectory() : configDir
            };

            return CreateFromXml(elem, null, context);
        }

        static ITransducer CreateFromXml(XElement elem, ITransducer parent, CreateContext context)
        {
            // find type
            string typeName;
            Type type;
            FindType(elem, context, out typeName, out type);

            // construct instance
            var transducer = CreateTransducer(elem, context, typeName, type);

            // now get sub transducers
            transducer.SubTransducers = elem.Elements()
                .Select(subElem => CreateFromXml(subElem, transducer, context))
                .Where(subTransducer => subTransducer != null)
                .ToList();

            return transducer;
        }

        static ITransducer CreateTransducer(XElement elem, CreateContext context, string typeName, Type type)
        {
            var atts = elem.Attributes()
                .Where(att => att.Name != "typeargs")
                .ToDictionary(att => att.Name.LocalName, att => att.Value);
            if (!atts.ContainsKey(ContextParameterName))
                atts.Add(ContextParameterName, "");

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
                        if (formalParm.Name == ContextParameterName)
                            return context;

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
            return transducer;
        }

        static readonly string[] TypeNamePrefixes = new[]
        {
            "",
            "Ferric.Text.Common",
            "Ferric.Text",
            "Ferric"
        };

        static void FindType(XElement elem, CreateContext context, out string typeName, out Type type)
        {
            var elemName = elem.Name.LocalName;
            var typeArgs = elem.Attributes("typeArgs").FirstOrDefault();

            typeName = null;
            type = null;
            foreach (var prefix in TypeNamePrefixes)
            {
                var nameToTry = elemName;
                if (!string.IsNullOrWhiteSpace(prefix))
                    nameToTry = prefix + "." + nameToTry;

                if (typeArgs != null)
                {
                    var args = typeArgs.Value.Split(',')
                        .Select(name => GetLoadedType(name, context, expectTransducer: false))
                        .Where(t => t != null)
                        .ToArray();
                    if (args.Any())
                    {
                        var gen = GetLoadedType(string.Format("{0}`{1}", nameToTry, args.Length), 
                            context, expectTransducer: false);
                        if (gen != null)
                            type = gen.MakeGenericType(args);
                    }
                }

                if (type == null)
                    type = GetLoadedType(nameToTry, context);

                if (type != null)
                {
                    typeName = nameToTry;
                    return;
                }
            }

            if (type == null)
                throw new Exception(string.Format("Unable to find a transducer of type {0}.", elemName));
        }

        static IList<Assembly> allAssemblies;
        static object assemblyLock = new object();

        static Type GetLoadedType(string name, CreateContext context, bool expectTransducer = true)
        {
            // try to find dlls
            lock (assemblyLock)
            {
                if (allAssemblies == null)
                {
                    allAssemblies = new List<Assembly>();

                    var executableDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    foreach (var dllFile in Directory.EnumerateFiles(executableDir, "*.dll"))
                    {
                        try
                        {
                            allAssemblies.Add(Assembly.LoadFile(dllFile));
                        }
                        catch
                        {
                        }
                    }

                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        allAssemblies.Add(assembly);
                    }
                }

                Type loadedType;
                if (context.TypeCache.TryGetValue(name, out loadedType))
                {
                    if (expectTransducer && !typeof(ITransducer).IsAssignableFrom(loadedType))
                        return null;
                    else
                        return loadedType;
                }

                foreach (var assembly in allAssemblies)
                {
                    var type = assembly.GetType(name);
                    if (type != null)
                    {
                        context.TypeCache[name] = type;

                        if (!expectTransducer || typeof(ITransducer).IsAssignableFrom(type))
                        {
                            loadedType = type;
                            break;
                        }
                    }
                }

                return loadedType;
            }
        }
    }
}
