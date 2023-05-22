using System.Linq;
using AutoMapper;
using bfsv.DataAccess;
using bfsv.DataAccess.Repositories;
using bfsv.Dtos;
using bfsv.Models;
using bfsv.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace Backend.Challenge.Controllers
{
    [ApiController]
    [Route("entity/[action]/")]
    [Produces("application/json")]
    public class EntityController : Controller
    {
        private readonly IMapper myMapper;
        private readonly IRavenDBRepository<Entity> myEntityRepository;

        public EntityController(IRavenDBRepository<Entity> EntityRepository, IMapper mapper)
        {
            myEntityRepository = EntityRepository;
            myMapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<EntityResponse>> CreateEntity(EntityDto entityDTO)
        {
            var entity = myMapper.Map<Entity>(entityDTO);

            var getEntity = await myEntityRepository.Get(entity.Id.ToString());

            entity.InsertDate = getEntity != null ? getEntity.InsertDate : entity.InsertDate;

            myEntityRepository.SetCreatedAndModified(getEntity != null, entity);

            await myEntityRepository.InsertOrUpdate(entity);

            return CreatedAtAction(nameof(GetEntity), myMapper.Map<EntityResponse>(entity));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EntityResponse), 200)]
        public async Task<ActionResult<EntityResponse>> GetEntity(int id)
        {
            return myMapper.Map<EntityResponse>(await myEntityRepository.Get(id.ToString()));
        }
    }
}