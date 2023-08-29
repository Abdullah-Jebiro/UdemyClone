using System.Net;
using Models.DbEntities;
using Models.Dtos;
using Models.Exceptions;
using Services.Repos;
using Stripe;

namespace Services.Payment
{
    public class PaymentService:IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessPayment(PaymentDto paymentDto, int userId)
        {
            var cartItems = await _unitOfWork.Carts.FindAllAsync(
                c => c.UserId == userId,
                new[] { "Course" });

            if (cartItems == null || cartItems?.Count() == 0)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The cart is empty.");
            }

            await DeleteCartItems(cartItems);
            var customer = await CreateStripeCustomer(paymentDto);
            await ChargeCustomer(cartItems, customer);
            await UpdateUserCoursesAndInstructorAccounts(cartItems);
        }

        private async Task DeleteCartItems(IEnumerable<CartItem> cartItems)
        {
            await _unitOfWork.Carts.DeleteRangeAsync(cartItems);
        }

        private async Task<Customer> CreateStripeCustomer(PaymentDto paymentDto)
        {
            StripeConfiguration.SetApiKey("YOUR_STRIPE_SECRET_KEY");

            var customers = new CustomerService();
            var customer = await customers.CreateAsync(
                new CustomerCreateOptions { Email = paymentDto.Email, Source = paymentDto.Token }
            );

            return customer;
        }

        private async Task ChargeCustomer(IEnumerable<CartItem> cartItems, Customer customer)
        {
            var charges = new ChargeService();
            var charge = await charges.CreateAsync(
                new ChargeCreateOptions
                {
                    Amount = (int)cartItems.Sum(x => x.Course.Price) * 100,
                    Description = "",
                    Currency = "usd",
                    Customer = customer.Id
                }
            );

            if (charge.Status != "succeeded")
            {
                throw new ApiException(
                    HttpStatusCode.InternalServerError,
                    "Payment failed: " + charge.FailureMessage
                );
            }
        }

        private async Task UpdateUserCoursesAndInstructorAccounts(IEnumerable<CartItem> cartItems)
        {
            foreach (var item in cartItems)
            {
                await _unitOfWork.UsersCourses.AddAsync(
                    new UserCourses { CourseId = item.CourseId, UserId = item.UserId }
                );

                var accounts = await _unitOfWork.InstructorsAccounts.FindAsync(
                    i => i.UserId == item.Course.UserId
                );
                if (accounts == null)
                {
                    var instructorAccount = new InstructorAccount
                    {
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
        }
    }
}
