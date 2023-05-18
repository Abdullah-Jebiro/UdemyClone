using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Payment
{
    public class PaymentIntentCreateOptions
    {
        public long Amount { get; set; }

        public string Currency { get; set; }

        public List<string> PaymentMethodTypes { get; set; }

        public PaymentIntentTransferDataOptions TransferData { get; set; }

        public string StatementDescriptor { get; set; }
    }
}
