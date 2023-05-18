using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Services.Repos;




namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")] // Requires admin permission to access this controller
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all categories that have at least the specified number of courses.
        /// </summary>
        /// <param name="minCourseCount">The minimum number of courses that a category must have to be included in the results. Default is 1</param>
        [AllowAnonymous] // Allows access to this action without authentication
        [HttpGet]
        public async Task<ActionResult> GetCategories(int minCourseCount = 1)
        {
            // Filter categories by course count
            var categories = await _unitOfWork.Categories.FindAllAsync(c => c.Courses.Count() >= minCourseCount);
            if (categories == null)
            {
                return NotFound();
            }
            var categoryDtos = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        /// <summary>
        /// Gets a single category using the category ID.
        /// </summary>
        [HttpGet("{categoryId}")]
        public async Task<ActionResult> GetCategory(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDtos = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDtos);
        }

        /// <summary>
        /// Creates a new category using the given CategoryForEditDto.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Create(CategoryForEditDto dto)
        {
            string toUpper = dto.NameCategory.ToUpper();
            var existingCategory = await _unitOfWork.Categories.ExistsAsync(c => c.nameCategory.ToUpper() == toUpper);
            if (existingCategory)
            {
                return BadRequest("A category with this name already exists.");
            }

            var category = _mapper.Map<Category>(dto);
            category = await _unitOfWork.Categories.AddAsync(category);
            return CreatedAtAction(
                nameof(GetCategory),
                new { categoryId = category.CategoryId },
                _mapper.Map<CategoryDto>(category));
        }

        /// <summary>
        /// Updates an existing category using the given CategoryForEditDto and category ID.
        /// </summary>
        [HttpPut("{categoryId}")]
        public async Task<ActionResult> Update(CategoryForEditDto dto, int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound($"The category with ID {categoryId} was not found.");
            }

            // Update existing category properties using the properties in the DTO
            _mapper.Map(dto, category);

            await _unitOfWork.Categories.UpdateAsync(category);
            return Ok();
        }

        /// <summary>
        /// Deletes an existing category using the category ID.
        /// </summary>
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            // Soft delete the category
            await _unitOfWork.Categories.SoftDeleteAsync(category);
            return NoContent();
        }
    }
}
