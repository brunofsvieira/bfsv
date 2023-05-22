using bfsv.Models;
using Raven.Client.Documents.Indexes;

namespace bfsv.DataAccess.Indexs
{

    public class CommentsByEntityNotViewedIndex : AbstractIndexCreationTask<Comment>
    {
        public class IndexEntry
        {
            public string CommentId { get; set; }
            public string AuthorId { get; set; }
            public string EntityId { get; set; }
            public List<string> UsersViewed { get; set; }
        }

        public CommentsByEntityNotViewedIndex()
        {
            Map = comments => from comment in comments
                              select new
                              {
                                  CommentId = comment.Id,
                                  AuthorId = comment.AuthorId,
                                  EntityId = comment.EntityId,
                                  UsersViewed = comment.UsersViews.Select(userView => userView.UserId)
                              };
        }
    }
}
