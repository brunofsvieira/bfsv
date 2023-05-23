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
    /// <summary>
    /// Entity controler.
    /// </summary>
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

        /// <summary>
        /// Create or update entity.
        /// </summary>
        /// <param name="commentDTO">Entity dto (input).</param>
        /// <param name="Id">The entity Id.</param>
        /// <returns>Entity inserted.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<EntityResponse>> CreateOrUpdateEntity(EntityDto entityDTO, string? Id)
        {
            var entity = myMapper.Map<Entity>(entityDTO);

            var getEntity = !string.IsNullOrEmpty(Id) ? await myEntityRepository.Get(Id) : null;

            entity.InsertDate = getEntity != null ? getEntity.InsertDate : entity.InsertDate;
            entity.Id = getEntity != null ? getEntity.Id : entity.Id;

            myEntityRepository.SetCreatedAndModified(getEntity != null, entity);

            await myEntityRepository.InsertOrUpdate(entity);

            return CreatedAtAction(nameof(GetEntity), myMapper.Map<EntityResponse>(entity));
        }

        /// <summary>
        /// Get a entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>A entity.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(EntityResponse), 200)]
        public async Task<ActionResult<EntityResponse>> GetEntity(string id)
        {
            return myMapper.Map<EntityResponse>(await myEntityRepository.Get(id));
        }

        /// <summary>
        /// Get entities.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <returns>A list of entities..</returns>
        [HttpGet]
        [ProducesResponseType(typeof(EntityResponse), 200)]
        public async Task<ActionResult<List<EntityResponse>>> GetEntities(int pageSize, int pageNumber)
        {
            return myMapper.Map<List<EntityResponse>>(await myEntityRepository.GetAll(pageSize, pageNumber));
        }
    }
}