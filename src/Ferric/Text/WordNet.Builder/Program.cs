using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ferric.Text.WordNet.Builder
{
    class Program
    {
        const string BasePath = @"..\..\..\..\..\..\data\english\wordnet";

        static void Main(string[] args)
        {
            var program = new Program();
            program.BuildWordNet();
        }

        void BuildWordNet()
        {
            try
            {
                // read data
                Console.WriteLine("reading...");
                var info = new BuilderInfo();

                Load<LoaderS>("wn_s.pl", info);
                Load<LoaderSk>("wn_sk.pl", info);
                Load<LoaderG>("wn_g.pl", info);

                // write lemmas
                WriteLemmas(info);

                // add to db
                var csBuilder = new SqlConnectionStringBuilder();
                csBuilder.DataSource = @"(LocalDB)\v11.0";
                csBuilder.AttachDBFilename = Path.GetFullPath(Path.Combine(BasePath, @"WordNet.mdf"));
                csBuilder.IntegratedSecurity = true;

                Console.WriteLine("connecting to " + csBuilder.ConnectionString);
                using (var context = new Data.WordNet(csBuilder.ConnectionString))
                {
                    // clear the db
                    Console.WriteLine("clearing...");
                    context.Database.Delete();
                    context.Database.Create();

                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        // add to context
                        Console.WriteLine("assimilating...");
                        AddToContext(context, info);
                    }
                }
            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                PrintException(sb, e);
                Console.Write(sb.ToString());
                Console.WriteLine();
            }
        }

        static Type[] CtorTypes = new[] { typeof(TextReader), typeof(BuilderInfo) };

        void Load<T>(string fname, BuilderInfo info)
            where T : Loader
        {
            var type = typeof(T);
            var ctor = type.GetConstructor(CtorTypes);
            if (ctor == null)
                throw new Exception("Unable to get constructor for " + type.FullName);

            using (var tr = new StreamReader(Path.GetFullPath(Path.Combine(BasePath, fname))))
            {
                var loader = ctor.Invoke(new object[] { tr, info }) as Loader;
                loader.Load();
            }
        }

        void WriteLemmas(BuilderInfo builderInfo)
        {
            Console.WriteLine("writing lemmas...");
            using (var tr = new StreamWriter(Path.GetFullPath(Path.Combine(BasePath, "lemmas.txt"))))
            {
                var lemmas = new HashSet<string>();
                foreach (var synset in builderInfo.SynsetsByWordNetId.Values)
                {
                    foreach (var wordsense in synset.Senses)
                        lemmas.Add(wordsense.Lemma);
                }

                foreach (var lemma in lemmas)
                    tr.WriteLine(lemma);
            }
        }

        void AddToContext(Data.WordNet context, BuilderInfo builderInfo)
        {
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;

            int num = 0;
            foreach (var synset in builderInfo.SynsetsByWordNetId.Values)
            {
                if ((num++ % 1000) == 0)
                    context.SaveChanges();

                foreach (var wordsense in synset.Senses)
                    context.WordSenses.Add(wordsense);
                context.Synsets.Add(synset);
            }
            context.SaveChanges();
        }

        void PrintException(StringBuilder sb, Exception e, string indent = null)
        {
            if (indent == null)
                indent = "";

            sb.AppendFormat("{0}{1}", indent, e.Message);
            sb.AppendLine();

            var aggregate = e as AggregateException;
            if (aggregate != null)
            {
                foreach (var inner in aggregate.InnerExceptions)
                    PrintException(sb, inner, indent + "  ");
            }

            if (e.InnerException != null)
                PrintException(sb, e.InnerException, indent + "  ");
        }
    }
}
