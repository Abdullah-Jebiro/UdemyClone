using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Models.Exceptions;
using Org.BouncyCastle.Crypto.Macs;
using Services.Payment;
using Services.Repos;
using Stripe;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Charge(PaymentDto paymentDto)
        {

            int userId = Convert.ToInt32(User.Identity.GetUserId());

            await _paymentService.ProcessPayment(paymentDto, userId);
            return Ok();
        }
    }
}