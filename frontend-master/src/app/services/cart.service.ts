import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface CartItem {
  id?: string;
  name: string;
  price: number;
  quantity: number;
  image?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems: CartItem[] = [];
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  private cartCountSubject = new BehaviorSubject<number>(0);

  cartCount$ = this.cartCountSubject.asObservable();

  constructor() {
    this.loadCartFromStorage();
  }

  // Required by the cart feature: return current cart items.
  getItems(): CartItem[] {
    return this.cartItems.map(item => ({ ...item }));
  }

  // Observable stream used by components that need live item updates.
  getCartItems(): Observable<CartItem[]> {
    return this.cartItemsSubject.asObservable();
  }

  // Backward-compatible alias if older components still call this.
  getCartCount(): Observable<number> {
    return this.cartCount$;
  }

  addToCart(item: CartItem): void {
    const existingItem = this.cartItems.find(existing => this.isSameItem(existing, item));

    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      this.cartItems.push({
        ...item,
        quantity: item.quantity > 0 ? item.quantity : 1
      });
    }

    this.emitCartChanges();
  }

  // Backward-compatible alias while migrating older calls.
  addItem(item: CartItem): void {
    this.addToCart(item);
  }

  removeItem(item: CartItem): void {
    this.cartItems = this.cartItems.filter(existing => !this.isSameItem(existing, item));
    this.emitCartChanges();
  }

  increaseQuantity(item: CartItem): void {
    const existingItem = this.cartItems.find(existing => this.isSameItem(existing, item));

    if (!existingItem) {
      return;
    }

    existingItem.quantity += 1;
    this.emitCartChanges();
  }

  decreaseQuantity(item: CartItem): void {
    const existingItem = this.cartItems.find(existing => this.isSameItem(existing, item));

    if (!existingItem) {
      return;
    }

    existingItem.quantity -= 1;

    if (existingItem.quantity <= 0) {
      this.removeItem(existingItem);
      return;
    }

    this.emitCartChanges();
  }

  updateCartCount(): void {
    const count = this.cartItems.reduce((total, item) => total + item.quantity, 0);
    this.cartCountSubject.next(count);
  }

  clearCart(): void {
    this.cartItems = [];
    this.emitCartChanges();
  }

  private emitCartChanges(): void {
    this.cartItemsSubject.next(this.getItems());
    this.updateCartCount();
    this.saveCartToStorage();
  }

  private isSameItem(a: CartItem, b: CartItem): boolean {
    if (a.id && b.id) {
      return a.id === b.id;
    }

    return a.name === b.name;
  }

  private saveCartToStorage(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartItems));
  }

  private loadCartFromStorage(): void {
    const savedCart = localStorage.getItem('cart');

    if (savedCart) {
      try {
        this.cartItems = JSON.parse(savedCart) as CartItem[];
      } catch (e) {
        console.error('Failed to load cart from storage', e);
        this.cartItems = [];
      }
    }

    this.emitCartChanges();
  }
}
