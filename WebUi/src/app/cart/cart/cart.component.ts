import { Component, OnDestroy, OnInit } from '@angular/core';
import { CartService } from 'src/app/services/cart.service';
import { CartItemDto } from '../models/CartItemDto';
import { environment } from 'src/environments/environment';
import { SubSink } from 'subsink';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})

export class CartComponent implements OnInit, OnDestroy {
  public cartItems: CartItemDto[] = [];
  public cartItemsFilter: CartItemDto[] = [];
  public countItems = 0;
  public totalPrice = 0;
  public imageBaseUrl = environment.imageEndpoint;

  private subs = new SubSink();

  constructor(
    private cartService: CartService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    this.loadCartItems();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  calculateTotalPrice(): void {
    this.totalPrice = this.cartItems.reduce((acc, item) => acc + item.price, 0);
  }

  onSearch(searchInput: string): void {
    this.cartItemsFilter = this.cartItems.filter(item => {
      return item.name.toLocaleLowerCase().includes(searchInput.toLocaleLowerCase());
    });
  }

  loadCartItems(): void {
    this.subs.sink = this.cartService.getItems.subscribe({
      next: (items) => {
        this.cartItems = items;
        this.cartItemsFilter = items;
        this.countItems = items.length;
        this.calculateTotalPrice();
      },
      error: () => {
        this.toastrService.error('Please try again.', 'Error', { timeOut: 5000 });
      }
    });
  }

  removeItem(itemId: number): void {
    const itemIndex = this.cartItems.findIndex(item => item.cartItemId === itemId);
    if (itemIndex !== -1) {
      this.subs.sink = this.cartService.RemoveItem(itemId).subscribe({
        next: () => {
          this.cartItems.splice(itemIndex, 1);
          this.cartItemsFilter = this.cartItems;
          this.countItems--;
          this.calculateTotalPrice();
        },
        error: () => {
          this.toastrService.error('Please try again.', 'Error', { timeOut: 5000 });
        }
      });
    }
  }

}
