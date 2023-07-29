using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Models.Exceptions;
using Services.Repos;
using System.Net;

namespace WebApi.Controllers
{
    /// <summary>
    /// API controller for managing levels.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class LevelsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LevelsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all levels.
        /// </summary>
        /// <returns>A list of level DTOs.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLevels()
        {
            var levels = await _unitOfWork.Levels.GetAllAsync();
            if (levels == null)
            {
                throw new ApiException(HttpStatusCode.NotFound);
            }
            var levelDtos = _mapper.Map<IReadOnlyList<LevelDto>>(levels);
            return Ok(levelDtos);
        }

        /// <summary>
        /// Retrieves a level by ID.
        /// </summary>
        /// <param name="levelId">The ID of the level to retrieve.</param>
        /// <returns>The level DTO.</returns>
        [AllowAnonymous]
        [HttpGet("{levelId}")]
        public async Task<ActionResult> GetLevel(int levelId)
        {
            var level = await _unitOfWork.Levels.GetByIdAsync(levelId);
            if (level == null)
            {
                throw new ApiException(HttpStatusCode.NotFound);
            }
            var levelDto = _mapper.Map<LevelDto>(level);
            return Ok(levelDto);
        }

        /// <summary>
        /// Creates a new level.
        /// </summary>
        /// <param name="dto">The level DTO.</param>
        /// <returns>The created level DTO.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(LevelForEditDto dto)
        {
            string toUpper = dto.Name.ToUpper();
            var existingLevel = await _unitOfWork.Levels.ExistsAsync(l => l.Name.ToUpper() == toUpper);
            if (existingLevel)
            {
                return BadRequest();
                throw new ApiException(HttpStatusCode.BadRequest, "A level with this name already exists.");
            }

            var level = _mapper.Map<Level>(dto);
            level = await _unitOfWork.Levels.AddAsync(level);
            return CreatedAtAction(
                nameof(GetLevel),
                new { levelId = level.LevelId },
                _mapper.Map<LevelDto>(level));
        }

        /// <summary>
        /// Updates a level by ID.
        /// </summary>
        /// <param name="dto">The level DTO.</param>
        /// <param name="levelId">The ID of the level to update.</param>
        /// <returns>An ActionResult.</returns>
        [HttpPut("{levelId}")]
        public async Task<ActionResult> Update(LevelForEditDto dto, int levelId)
        {
            var level = await _unitOfWork.Levels.GetByIdAsync(levelId);
            if (level == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"The level with ID {levelId} was not found.");
            }
            _mapper.Map(dto, level);
            await _unitOfWork.Levels.UpdateAsync(level);
            return Ok();
        }

        /// <summary>
        /// Deletes a level by ID.
        /// </summary>
        /// <param name="levelId">The ID of the level to delete.</param>
        /// <returns>An ActionResult.</returns>
        [HttpDelete("{levelId}")]
        public async Task<ActionResult> Delete(int levelId)
        {
            var level = await _unitOfWork.Levels.GetByIdAsync(levelId);
            if (level == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"The level with ID {levelId} was not found.");
            }
            await _unitOfWork.Levels.DeleteAsync(level);
            return NoContent();
        }
    }
}