
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Services.Repos;

namespace WebApi.Controllers
{
    /// <summary>
    /// API controller for managing languages.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class LanguagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LanguagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all languages.
        /// </summary>
        /// <returns>A list of language DTOs.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLanguages()
        {
            var languages = await _unitOfWork.Languages.GetAllAsync();
            if (languages == null)
            {
                return NotFound();
            }
            var languageDtos = _mapper.Map<IReadOnlyList<LanguageDto>>(languages);
            return Ok(languageDtos);
        }

        /// <summary>
        /// Retrieves a language by ID.
        /// </summary>
        /// <param name="languageId">The ID of the language to retrieve.</param>
        /// <returns>The language DTO.</returns>
        [AllowAnonymous]
        [HttpGet("{languageId}")]
        public async Task<ActionResult> GetLanguage(int languageId)
        {
            var language = await _unitOfWork.Languages.GetByIdAsync(languageId);
            if (language == null)
            {
                return NotFound();
            }
            var languageDto = _mapper.Map<LanguageDto>(language);
            return Ok(languageDto);
        }

        /// <summary>
        /// Creates a new language.
        /// </summary>
        /// <param name="dto">The language DTO.</param>
        /// <returns>The created language DTO.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(LanguageForEditDto dto)
        {
            string toUpper = dto.Name.ToUpper();
            var existingLanguage = await _unitOfWork.Languages.ExistsAsync(l => l.Name.ToUpper() == toUpper);
            if (existingLanguage)
            {
                return BadRequest("A language with this name already exists.");
            }

            var language = _mapper.Map<Language>(dto);
            language = await _unitOfWork.Languages.AddAsync(language);
            return CreatedAtAction(
                nameof(GetLanguage),
                new { languageId = language.LanguageId },
                _mapper.Map<LanguageDto>(language));
        }

        /// <summary>
        /// Updates a language by ID.
        /// </summary>
        /// <param name="dto">The language DTO.</param>
        /// <param name="languageId">The ID of the language to update.</param>
        /// <returns>No content.</returns>
        [HttpPut("{languageId}")]
        public async Task<ActionResult> Update(LanguageForEditDto dto, int languageId)
        {
            var language = await _unitOfWork.Languages.GetByIdAsync(languageId);
            if (language == null)
            {
                return NotFound($"The language with ID {languageId} was not found.");
            }
            _mapper.Map(dto, language);
            await _unitOfWork.Languages.UpdateAsync(language);
            return NoContent();
        }

        /// <summary>
        /// Deletes a language by ID.
        /// </summary>
        /// <param name="languageId">The ID of the language to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{languageId}")]
        public async Task<ActionResult> Delete(int languageId)
        {
            var language = await _unitOfWork.Languages.GetByIdAsync(languageId);
            if (language == null)
            {
                return NotFound();
            }
            await _unitOfWork.Languages.DeleteAsync(language);
            return NoContent();
        }
    }
}