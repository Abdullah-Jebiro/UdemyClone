using Models.Dtos;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Payment
{
    public interface IPaymentService
    {
        Task ProcessPayment(PaymentDto paymentDto, int userId);
    }
}
