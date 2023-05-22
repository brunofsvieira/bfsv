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

            var getUser = await myUserRepository.Get(user.Id);

            user.InsertDate = getUser != null ? getUser.InsertDate : user.InsertDate;

            myUserRepository.SetCreatedAndModified(getUser != null, user);

            await myUserRepository.Insert(user);

            return CreatedAtAction(nameof(GetUser), myMapper.Map<UserResponse>(user));
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<ActionResult<UserResponse>> GetUser(string id)
        {
            return myMapper.Map<UserResponse>(await myUserRepository.Get(id));
        }

        // This action responds to the url /user/users/42 and /user/users?id=4&id=10
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