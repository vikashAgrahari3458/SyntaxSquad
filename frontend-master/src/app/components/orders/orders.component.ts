import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Order } from '../../models/order';
import { OrderService } from '../../services/order.service';
import { NavbarComponent } from "../navbar/navbar.component";

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit, OnDestroy {
  orders: Order[] = [];
  private destroy$ = new Subject<void>();

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.orderService
      .getOrders()
      .pipe(takeUntil(this.destroy$))
      .subscribe(orders => {
        this.orders = orders;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getItemCount(order: Order): number {
    return order.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  getStatus(order: Order): 'Preparing' | 'Out for Delivery' | 'Delivered' {
    const minutesAgo = (Date.now() - new Date(order.orderDate).getTime()) / 60000;

    if (minutesAgo < 25) {
      return 'Preparing';
    }

    if (minutesAgo < 60) {
      return 'Out for Delivery';
    }

    return 'Delivered';
  }

  getStatusClass(order: Order): string {
    const status = this.getStatus(order);

    if (status === 'Preparing') {
      return 'status-preparing';
    }

    if (status === 'Out for Delivery') {
      return 'status-delivery';
    }

    return 'status-delivered';
  }

  getStatusIcon(order: Order): string {
    const status = this.getStatus(order);

    if (status === 'Preparing') {
      return 'bi-fire';
    }

    if (status === 'Out for Delivery') {
      return 'bi-scooter';
    }

    return 'bi-check-circle-fill';
  }

}
