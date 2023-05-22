using Raven.Client.Documents;

namespace bfsv.DataAccess
{
    public interface IRavenDBContext
    {
        public IDocumentStore Store { get; }
    }
}
