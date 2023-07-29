using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Org.BouncyCastle.Crypto.Macs;
using Services.Payment;
using Services.Repos;
using Stripe;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Charge( PaymentDto paymentDto)
        {
            // Get the ID of the current logged in user
            int userId = Convert.ToInt32(User.Identity.GetUserId());

            // All items in the basket owned by the current user are searched
            var cartItems = await _unitOfWork.Carts.FindAllAsync(
                c => c.UserId == userId, new[] { "Course" });

            // If the basket is empty, an invalid order is returned
            if (cartItems == null)
            {
                return BadRequest("The cart is empty.");
            }

            //TODO
            #region
           

            // All items in the cart are deleted after payment
            await _unitOfWork.Carts.DeleteRangeAsync(cartItems);

            // Initialize Stripe API key
            StripeConfiguration.SetApiKey("sk_test_51N5J4IG9hGD06mXXwMdVxMtH3TdbbzlAclC1jcppTKoUw0T8SCWRzL8gDGeS26rF5XW1qrBWfwyGgy69OebqPgfS00LSUj85Zl");

            // The customer object is created using Stripe Customer Service
            var customers = new CustomerService();
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = paymentDto.Email, 
                Source = paymentDto.Token // Stripe's response token is used to generate the payment request
            });

            var charges = new ChargeService();
            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (int)cartItems.Sum(x => x.Course.Price)*100, // The total payment amount is calculated using the price of the courses in the cart
                Description = "", // An additional description of the batch can be specified here
                Currency = "usd", // US dollars are used as the default currency for the payment
                Customer = customer.Id // The generated customer ID is set
            });

            // If the batch fails, a 500 Internal Server Error is returned with the error message
            if (charge.Status != "succeeded")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Payment failed: " + charge.FailureMessage);
            }
            #endregion
            #region

            await _unitOfWork.UsersCourses.AddRangeAsync(cartItems.Select(c => new UserCourses
            {
                CourseId = c.CourseId,
                UserId = c.UserId,
                
            })); ;

            foreach (var item in cartItems)
            {
                var accounts = await _unitOfWork.InstructorsAccounts.FindAsync(i => i.UserId == item.Course.UserId);
                if (accounts == null)
                {
                    var instructorAccount = new InstructorAccount() {
                        UserId = item.Course.UserId,
                        Account = (item.Course.Price * 0.9)
                    };        
                    await _unitOfWork.InstructorsAccounts.AddAsync(instructorAccount);
                }
                else
                {
                    accounts.Account += (item.Course.Price * 0.9);
                    await _unitOfWork.InstructorsAccounts.UpdateAsync(accounts);
                }
            }
            #endregion

            return Ok();
        }
    }
}