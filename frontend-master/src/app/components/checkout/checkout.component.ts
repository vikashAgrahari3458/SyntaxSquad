import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CartItem, CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { NavbarComponent } from "../navbar/navbar.component";

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  deliveryAddress = '';
  couponCode = '';
  cartItems: CartItem[] = [];

  discount = 0;
  private destroy$ = new Subject<void>();

  couponMessage = '';
  couponMessageType: 'success' | 'error' | '' = '';
  orderPlacedMessage = '';

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartService
      .getCartItems()
      .pipe(takeUntil(this.destroy$))
      .subscribe(items => {
        this.cartItems = items;

        if (!items.length) {
          this.discount = 0;
          this.couponCode = '';
          this.couponMessage = '';
          this.couponMessageType = '';
        } else if (this.couponCode.trim().toUpperCase() === 'SAVE20') {
          this.discount = Math.round(this.subtotal * 0.2);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get subtotal(): number {
    return this.cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
  }

  get hasItems(): boolean {
    return this.cartItems.length > 0;
  }

  applyCoupon(): void {
    const normalizedCode = this.couponCode.trim().toUpperCase();

    this.orderPlacedMessage = '';

    if (!this.hasItems) {
      this.discount = 0;
      this.couponMessage = 'Add items to cart before applying a coupon.';
      this.couponMessageType = 'error';
      return;
    }

    if (!normalizedCode) {
      this.discount = 0;
      this.couponMessage = 'Please enter a coupon code.';
      this.couponMessageType = 'error';
      return;
    }

    if (normalizedCode === 'SAVE20') {
      this.discount = Math.round(this.subtotal * 0.2);
      this.couponMessage = 'Coupon applied. You saved 20%.';
      this.couponMessageType = 'success';
      return;
    }

    this.discount = 0;
    this.couponMessage = 'Invalid coupon code. Try SAVE20.';
    this.couponMessageType = 'error';
  }

  get finalAmount(): number {
    return this.subtotal - this.discount;
  }

  placeOrder(): void {
    if (!this.hasItems) {
      this.orderPlacedMessage = 'Your cart is empty. Add pizzas before placing order.';
      return;
    }

    if (!this.deliveryAddress.trim()) {
      this.orderPlacedMessage = 'Please add a delivery address before placing order.';
      return;
    }

    const orderItems = this.cartItems.map(item => ({
      name: item.name,
      price: item.price,
      quantity: item.quantity
    }));

    this.orderService.placeOrder({
      items: orderItems,
      totalAmount: this.finalAmount,
      deliveryAddress: this.deliveryAddress.trim()
    });

    this.cartService.clearCart();
    this.router.navigate(['/success']);
  }

}
