using System.Linq;
using AutoMapper;
using bfsv.Dtos;
using bfsv.DataAccess;
using bfsv.DataAccess.Repositories;
using bfsv.Models;
using bfsv.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using System.Xml.Linq;

namespace Backend.Challenge.Controllers
{
    /// <summary>
    /// Main controller (Comment controller).
    /// </summary>
    [ApiController]
    [Route("main/[action]/")]
    [Produces("application/json")]
    public class MainController : Controller
    {
        private readonly IMapper myMapper;
        private readonly IRavenDBRepository<Comment> myCommentRepository;

        public MainController(IRavenDBRepository<Comment> commentrRepository, IMapper mapper)
        {
            myCommentRepository = commentrRepository;
            myMapper = mapper;
        }

        // This action responds to the url /main/users/42 and /main/users?id=4&id=10
        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(GetUserResponse), 200)]
        //public async Task<ActionResult<GetUserResponse>> Users([FromQuery(Name = "id")] int[] id)
        //{
        //    return new GetUserResponse
        //    {
        //        Users = id.ToDictionary(i => i, i => new UserResponse
        //        {
        //            Id = i,
        //            Username = $"User {i}",
        //            Email = $"user-{i}@example.com"
        //        })
        //    };
        //}

        /// <summary>
        /// Get a comment.
        /// </summary>
        /// <param name="id">Comment id.</param>
        /// <returns>A comment.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CommentResponse), 200)]
        public async Task<ActionResult<CommentResponse>> GetComment(string id)
        {
            return myMapper.Map<CommentResponse>(await myCommentRepository.Get(id));
        }

        /// <summary>
        /// Get the comments by entity with pagination.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>A CommentWithTotalResponse. Total comments and List of comments.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CommentWithTotalResponse), 200)]
        public async Task<ActionResult<CommentWithTotalResponse>> GetCommentsByEntity(int pageNumber, int pageSize, string entityId)
        {
            var getComments = await myCommentRepository.GetCommentsByEntity(pageNumber, pageSize, entityId);

            return new CommentWithTotalResponse
            {
                Comments = myMapper.Map<List<CommentResponse>>(getComments.Item2),
                TotalResults = getComments.Item1,
            };
        }

        /// <summary>
        /// Get the unseen comments by an entity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>A CommentWithTotalResponse. Total comments unseen and List of comments unseen.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CommentWithTotalResponse), 200)]
        public async Task<ActionResult<CommentWithTotalResponse>> GetUnseenComments(string userId, string entityId)
        {

            var getComments = await myCommentRepository.GetUnseenComments(userId, entityId);

            return new CommentWithTotalResponse
            {
                Comments = myMapper.Map<List<CommentResponse>>(getComments.Item2),
                TotalResults = getComments.Item1,
            };
        }

        /// <summary>
        /// Create or update comment.
        /// </summary>
        /// <param name="commentDTO">Comment dto (input).</param>
        /// <param name="Id">The comment Id.</param>
        /// <returns>Comment inserted.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<CommentResponse>> CreateOrUpdateComment(CommentDto commentDTO, string? Id)
        {
            var comment = myMapper.Map<Comment>(commentDTO);

            if (!string.IsNullOrEmpty(Id))
            {
                var getComment = await myCommentRepository.Get(Id);

                if (getComment == null)
                {
                    return NotFound();
                }

                comment.InsertDate = getComment != null ? getComment.InsertDate : comment.InsertDate;
                comment.Id = getComment != null ? getComment.Id : comment.Id;
            }

            myCommentRepository.SetCreatedAndModified(!string.IsNullOrEmpty(Id), comment);

            await myCommentRepository.InsertOrUpdate(comment);

            return CreatedAtAction(nameof(GetComment), myMapper.Map<CommentResponse>(comment));
        }

        /// <summary>
        /// Update unseen comments by an entity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commentId">The comment id.</param>
        /// <returns>A CommentWithTotalResponse. Total comments unseen and List of comments unseen.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CommentResponse), 200)]
        public async Task<ActionResult<CommentResponse>> UpdateUnseenComment(string userId, string commentId)
        {
            var comment = myMapper.Map<CommentResponse>(await myCommentRepository.UpdateUnseenComment(userId, commentId));

            if(comment == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetComment), comment);
        }
    }
}