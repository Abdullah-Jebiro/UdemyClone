import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { PaymentService } from 'src/app/services/payment.service';
import { SubSink } from 'subsink';

declare var StripeCheckout: any;

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit, OnDestroy {
  paymentHandler: any = null;
  private subs = new SubSink();

  constructor(
    private paymentService: PaymentService,
    private toastrService: ToastrService
  ) { }

  ngOnInit() {
    this.loadStripe();

  }

  loadStripe() {
    if (!window.document.getElementById('stripe-script')) {
      const script = window.document.createElement('script');
      script.id = 'stripe-script';
      script.type = 'text/javascript';
      script.src = 'https://checkout.stripe.com/checkout.js';
      window.document.body.appendChild(script);
    }
  }

  openStripeCheckout() {

    this.paymentHandler = StripeCheckout.configure({
      key: 'pk_test_51N5J4IG9hGD06mXXKHrT66GkgyQADVriqgEK1dvuMwVWRJ17pENmpCcLOVH0SHFsAgdE78gUTIvelckHLPXBvsZx00VbhFEnD0',
      locale: 'auto',
      token: (stripeToken: any) => {
        this.subs.sink = this.paymentService.charge({
          email: stripeToken.email,
          token: stripeToken.id
        }).subscribe({
          next: () => {
            this.toastrService.success('Payment successful!', '', {
              timeOut: 3000
            });
          },
          error: (err: string) => {
            this.toastrService.error('Payment failed.', err, {
              timeOut: 3000
            });
          }
        });
      }
    });

    this.paymentHandler.open({
      name: 'Payment',
      description: 'Payment for courses',
    });
  }



  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}