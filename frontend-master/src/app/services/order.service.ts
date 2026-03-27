import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Order, PlaceOrderPayload } from '../models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private orders: Order[] = [];
  private ordersSubject = new BehaviorSubject<Order[]>([]);

  constructor() { }

  placeOrder(payload: PlaceOrderPayload): Order {
    const order: Order = {
      id: `ORD-${Date.now()}`,
      items: payload.items,
      totalAmount: payload.totalAmount,
      deliveryAddress: payload.deliveryAddress,
      orderDate: new Date()
    };

    this.orders = [order, ...this.orders];
    this.ordersSubject.next([...this.orders]);

    return order;
  }

  getOrders(): Observable<Order[]> {
    return this.ordersSubject.asObservable();
  }
}
