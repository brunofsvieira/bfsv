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

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<UserResponse>> CreateUser(UserDto userDTO)
        {
            var user = myMapper.Map<User>(userDTO);

            var getUser = await myUserRepository.Get(user.Id.ToString());

            user.InsertDate = getUser != null ? getUser.InsertDate : user.InsertDate;

            myUserRepository.SetCreatedAndModified(getUser != null, user);

            await myUserRepository.InsertOrUpdate(user);

            return CreatedAtAction(nameof(GetUser), myMapper.Map<UserResponse>(user));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            return myMapper.Map<UserResponse>(await myUserRepository.Get(id.ToString()));
        }

        // This action responds to the url /user/users/42 and /user/users?id=4&id=10
        [HttpGet("{id}")]
        [HttpGet("")]
        [ProducesResponseType(typeof(GetUserResponse), 200)]
        public async Task<ActionResult<GetUserResponse>> Users([FromQuery(Name = "id")] int[] id)
        {
            var users = await myUserRepository.GetMultipleIds(Array.ConvertAll(id, x => x.ToString()));

            var response = new GetUserResponse();

            response.Users = users.ToDictionary(p => int.TryParse(p.Key, out var myKey) ? myKey : 0, p => myMapper.Map<UserResponse>(p.Value));

            return response;
        }
    }
}