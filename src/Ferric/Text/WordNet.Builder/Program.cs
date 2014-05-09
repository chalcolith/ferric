using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class Program
    {
        const string BasePath = @"..\..\..\..\..\..\data\english\wordnet";

        static void Main(string[] args)
        {
            try
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<WordNetContext, Migrations.Configuration>());
                var builderInfo = new BuilderInfo();
                using (var context = new WordNetContext())
                {
                    // clear the db
                    context.Database.Delete();
                    context.Database.Create();

                    // read data
                    using (var tr = new StreamReader(Path.Combine(BasePath, "wn_s.pl")))
                        LoaderS.Load(tr, builderInfo);

                    foreach (var synset in builderInfo.SynsetsByWordNetId.Values)
                    {
                        context.Synsets.Add(synset);
                    }

                    // save
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
