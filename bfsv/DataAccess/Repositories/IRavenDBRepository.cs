using bfsv.Models;

namespace bfsv.DataAccess.Repositories
{
    public interface IRavenDBRepository<T>
    {
        Task<T> Get(string id);

        Task<Dictionary<string, T>> GetMultipleIds(string[] ids);

        Task<List<T>> GetAll(int pageSize, int pageNumber);

        Task InsertOrUpdate(T element);
        Task<(int, List<Comment>)> GetCommentsByEntity(int pageNumber, int pageSize, string entityId);

        Task<(int, List<Comment>)> GetUnseenComments(string userId, string entityId);
        Task<Comment> UpdateUnseenComment(string userId, string commentId);
        void SetCreatedAndModified(bool exist, BaseEntity baseEntity);
    }
}
