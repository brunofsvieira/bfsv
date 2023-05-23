using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using bfsv.DataAccess;
using bfsv.DataAccess.Repositories;
using bfsv.Dtos;
using bfsv.Models;
using bfsv.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Session;
using Sparrow.Binary;

namespace Backend.Challenge.Controllers
{
    /// <summary>
    /// User controller.
    /// </summary>
    [ApiController]
    [Route("user/[action]/")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IMapper myMapper;
        private readonly IRavenDBRepository<User> myUserRepository;

        public UserController(IRavenDBRepository<User> userRepository, IMapper mapper)
        
        {
            myUserRepository = userRepository;
            myMapper = mapper;
        }

        /// <summary>
        /// Create or update user.
        /// </summary>
        /// <param name="commentDTO">User dto (input).</param>
        /// <param name="Id">The user Id.</param>
        /// <returns>User inserted.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<UserResponse>> CreateOrUpdateUser(UserDto userDTO, string? Id)
        {
            var user = myMapper.Map<User>(userDTO);

            var getUser = !string.IsNullOrEmpty(Id) ? await myUserRepository.Get(Id) : null;

            user.InsertDate = getUser != null ? getUser.InsertDate : user.InsertDate;
            user.Id = getUser != null ? getUser.Id : user.Id;

            myUserRepository.SetCreatedAndModified(getUser != null, user);

            await myUserRepository.InsertOrUpdate(user);

            return CreatedAtAction(nameof(GetUser), myMapper.Map<UserResponse>(user));
        }

        /// <summary>
        /// Get a user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>A user.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<ActionResult<UserResponse>> GetUser(string id)
        {
            return myMapper.Map<UserResponse>(await myUserRepository.Get(id));
        }

        /// <summary>
        /// Get users.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <returns>A list of users.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<ActionResult<List<UserResponse>>> GetUsers(int pageSize, int pageNumber)
        {
            return myMapper.Map<List<UserResponse>>(await myUserRepository.GetAll(pageSize, pageNumber));
        }

        /// <summary>
        /// Get multiple users.
        /// </summary>
        /// <param name="id">Multiple users.</param>
        /// <returns>A list of users.</returns>
        [HttpGet("{id}")]
        [HttpGet("")]
        [ProducesResponseType(typeof(GetUserResponse), 200)]
        public async Task<ActionResult<GetUserResponse>> Users([FromQuery(Name = "id")] string[] id)
        {
            var users = await myUserRepository.GetMultipleIds(id);

            var response = new GetUserResponse();

            response.Users = users.ToDictionary(x =>  x.Key, x => myMapper.Map<UserResponse>(x.Value));

            return response;
        }
    }
}