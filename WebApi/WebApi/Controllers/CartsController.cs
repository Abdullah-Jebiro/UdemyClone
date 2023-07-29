using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Models.Exceptions;
using Services.Repos;
using System.Net;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the cart items for the authenticated user.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetCartItems()
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var cartItems = await _unitOfWork.Carts.FindAllAsync(
                c => c.UserId == userId,
                new[] { "Course" },
                c => c.Course
            );
            return Ok(_mapper.Map<IReadOnlyList<CartItemDto>>(cartItems));
        }

        /// <summary>
        /// Gets a cart item for the authenticated user by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItemDto>> GetCartItem(int id)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var cartItem = await _unitOfWork.Carts.FindAsync(
                c => c.UserId == userId && c.CartItemId == id,
                new[] { "Course" }
            );

            if (cartItem == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<CartItemDto>(cartItem);
            return Ok(result);
        }

        /// <summary>
        /// Adds an item to the cart for the authenticated user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddToCart(int courseId)
        {
           
            // Check if the course exists
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new ApiException(HttpStatusCode.NotFound);
            }

            // Check if the user already has the course in their cart
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var existingCartItem = await _unitOfWork.Carts.ExistsAsync(
                c => c.CourseId == courseId && c.UserId == userId
            );
            if (existingCartItem)
            {
                // return response (409)
                throw new ApiException(HttpStatusCode.Conflict, "Course already exists in cart.");
            }

            var cartItem = new CartItem() { CourseId = courseId, UserId = userId, };
            await _unitOfWork.Carts.AddAsync(cartItem);
            var result = _mapper.Map<CartItemDto>(cartItem);
            return CreatedAtAction(nameof(GetCartItem), new { id = result.CartItemId }, result);
        }

        /// <summary>
        /// Removes an item from the cart for the authenticated user.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveFromCart(int id)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var cartItem = await _unitOfWork.Carts.FindAsync(
                c => c.UserId == userId && c.CartItemId == id
            );
            if (cartItem == null)
            {
                throw new ApiException(HttpStatusCode.NotFound);

            }
            await _unitOfWork.Carts.DeleteAsync(cartItem);
            return NoContent();
        }

        /// <summary>
        /// Deletes all cart items for the authenticated user.
        /// </summary>
        /// <returns>A response with a 204 No Content status code.</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCartItemsForUser()
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            await _unitOfWork.Carts.DeleteRangeAsync(c => c.UserId == userId);
            return NoContent();
        }

        /// <summary>
        /// Gets the count of items in the user's cart.
        /// </summary>
        /// <returns>An OkObjectResult containing the count of items in the user's cart.</returns>
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetCountItem()
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var count = await _unitOfWork.Carts.CountAsync(c => c.UserId == userId);

            return Ok(count);
        }
    }
}
