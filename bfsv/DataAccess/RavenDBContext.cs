using bfsv.DataAccess.Indexs;
using bfsv.Models;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System.Security.Cryptography.X509Certificates;

namespace bfsv.DataAccess
{
    /// <summary>
    /// The context to initialize the Database.
    /// </summary>
    public class RavenDBContext : IRavenDBContext
    {
        private readonly IDocumentStore myStore;
        private readonly ReavenDBSettings mySettings;

        public RavenDBContext(IOptionsMonitor<ReavenDBSettings> settings)
        {
            mySettings = settings.CurrentValue;

            myStore = new DocumentStore
            {
                Urls = mySettings.Urls,
                Database = mySettings.DatabaseName,
                Certificate = new X509Certificate2(mySettings.CertPath, mySettings.CertPass)
            };

            myStore.Initialize();

            InsureDatabaseIsCreated();

            IndexCreation.CreateIndexes(typeof(CommentsByEntityNotViewedIndex).Assembly, myStore);

        }

        public IDocumentStore Store => myStore;

        public void InsureDatabaseIsCreated()
        {
            try
            {
                var myDb = myStore.Maintenance.Server.Send(new GetDatabaseRecordOperation(mySettings.DatabaseName));

                if(myDb == null)
                {
                    myStore.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(mySettings.DatabaseName)));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
