using bfsv.DataAccess.Indexs;
using bfsv.Models;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using static System.Formats.Asn1.AsnWriter;

namespace bfsv.DataAccess.Repositories
{
    /// <summary>
    /// The Raven DB repository to create the connection to query and update database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RavenDBRepository<T> : IRavenDBRepository<T>
    {
        private readonly IRavenDBContext myContext;

        public RavenDBRepository(IRavenDBContext context) 
        { 
            myContext = context;
        }

        /// <summary>
        /// Get by id of all types of entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A document from a entity.</returns>
        public async Task<T> Get(string id)
        {
            using (IAsyncDocumentSession session = myContext.Store.OpenAsyncSession())
            {
                return await session.LoadAsync<T>(id);
            }
        }

        /// <summary>
        /// Return multiple documents of all types of entities.
        /// </summary>
        /// <param name="ids">A list of ids.</param>
        /// <returns>A list of document from a entity.</returns>
        public async Task<Dictionary<string, T>> GetMultipleIds(string[] ids)
        {
            using (IAsyncDocumentSession session = myContext.Store.OpenAsyncSession())
            {
                return await session.LoadAsync<T>(ids);
            }
        }

        /// <summary>
        /// Get all documents of all types of entities.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <returns>A list of document from a entity with pagination.</returns>
        public async Task<List<T>> GetAll(int pageSize, int pageNumber)
        {
            using (IAsyncDocumentSession session = myContext.Store.OpenAsyncSession())
            {
                return await session.Query<T>()
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Insert a document of type T.
        /// </summary>
        /// <param name="element">The element.</param>
        public async Task InsertOrUpdate(T element)
        {
            using (IAsyncDocumentSession session = myContext.Store.OpenAsyncSession())
            {
                await session.StoreAsync(element);

                await session.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Get comments by entity.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="entityId">Entity id.</param>
        /// <returns>The comments by entity with pagination and the total of comments in this entity.</returns>
        public async Task<(int, List<Comment>)> GetCommentsByEntity(int pageNumber, int pageSize, string entityId)
        {
            using (IAsyncDocumentSession session = myContext.Store.OpenAsyncSession())
            {
                var comments = await session.Query<Comment>()
                                .Statistics(out QueryStatistics stats)
                                .Skip(pageSize * (pageNumber - 1))
                                .Take(pageSize)
                                .Where(x => x.EntityId == entityId)
                                .OrderByDescending(x => x.UpdateDate)
                                .ToListAsync();

                return(stats.TotalResults, comments);
            }
        }

        /// <summary>
        /// Get unseen comments.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="entityId">Entity id.</param>
        /// <returns>The unseen comments.</returns>
        public async Task<(int, List<Comment>)> GetUnseenComments(string userId, string entityId)
        {
            using (var session = myContext.Store.OpenAsyncSession())
            {
                var unseenComments = await session.Query<CommentsByEntityNotViewedIndex.IndexEntry, CommentsByEntityNotViewedIndex>()
                                           .Statistics(out QueryStatistics stats)
                                           .Where(c => c.EntityId == entityId && !c.UsersViewed.Contains(userId) && c.AuthorId != userId)
                                           .OfType<Comment>()
                                           .ToListAsync();

                unseenComments = unseenComments.OrderByDescending(x => x.UpdateDate).ToList();

                //Update all unseen comments when it returns the value in the next execution this list of comments are "seen comments"
                //UpdateUnseenComments(userId, unseenComments);
                //await session.SaveChangesAsync();

                return (stats.TotalResults, unseenComments);
            }
        }

        /// <summary>
        /// Update unseen comment.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <param name="commentId">Unseen comment.</param>
        /// <returns>The updated comment.</returns>
        public async Task<Comment> UpdateUnseenComment(string userId, string commentId)
        {
            using (IAsyncDocumentSession session = myContext.Store.OpenAsyncSession())
            {
                var comment = await session.LoadAsync<Comment>(commentId);

                if (comment != null && comment.AuthorId != userId && (comment.UsersViews != null && !comment.UsersViews.Any(x => x.UserId == userId)) || comment.UsersViews == null)
                {
                    var view = new UserView
                    {
                        UserId = userId,
                        ViewDate = DateTime.UtcNow,
                    };

                    comment.UsersViews = comment.UsersViews != null && comment.UsersViews.Any() ? comment.UsersViews.Append(view).ToList() : new List<UserView> { view };

                    await session.SaveChangesAsync();
                }

                return comment;
            }
        }


        /// <summary>
        /// Set the base entity.
        /// </summary>
        /// <param name="exist">If the date exists in DB.</param>
        /// <param name="baseEntity">A base entity.</param>
        public void SetCreatedAndModified(bool exist, BaseEntity baseEntity)
        {
            if (exist)
            {
                baseEntity.InsertDate = baseEntity.InsertDate;
                baseEntity.UpdateDate = DateTime.UtcNow;
                baseEntity.Id = baseEntity.Id;
            }
            else
            {
                baseEntity.InsertDate = DateTime.UtcNow;
                baseEntity.UpdateDate = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Update unseen comments for seen comments.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <param name="comments">Unseen comments</param>
        private void UpdateUnseenComments(string userId, List<Comment> comments)
        {
            var view = new UserView
            {
                UserId = userId,
                ViewDate = DateTime.UtcNow,
            };

            comments = comments.Select(c =>
            {
                c.UsersViews = c.UsersViews != null && c.UsersViews.Any() ? c.UsersViews.Append(view).ToList() : new List<UserView> { view };
                return c;
            }).ToList();
        }
    }
}
