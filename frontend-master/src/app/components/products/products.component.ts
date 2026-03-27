import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CartService } from '../../services/cart.service';
import { NavbarComponent } from "../navbar/navbar.component";

type MenuCategory = 'All' | 'Pizza' | 'Drinks';

interface MenuItem {
  id: string;
  name: string;
  price: number;
  category: Exclude<MenuCategory, 'All'>;
  image: string;
  isVeg: boolean;
  isBestseller: boolean;
  rating: number;
  deliveryTime: string;
}

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NavbarComponent],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnInit, OnDestroy {
  selectedCategory: MenuCategory = 'All';
  searchText = '';
  cartCount = 0;

  readonly categories: MenuCategory[] = ['All', 'Pizza', 'Drinks'];

  readonly menuItems: MenuItem[] = [
    {
      id: 'pizza-farmhouse',
      name: 'Farmhouse Pizza',
      price: 299,
      category: 'Pizza',
      image: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: true,
      rating: 4.6,
      deliveryTime: '20-25 mins'
    },
    {
      id: 'pizza-cheese-burst',
      name: 'Cheese Burst',
      price: 329,
      category: 'Pizza',
      image: 'https://images.unsplash.com/photo-1548365328-9f547fb0953b?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: true,
      rating: 4.7,
      deliveryTime: '20-25 mins'
    },
    {
      id: 'pizza-paneer-tikka',
      name: 'Paneer Tikka',
      price: 279,
      category: 'Pizza',
      image: 'https://images.unsplash.com/photo-1594007654729-407eedc4be65?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: false,
      rating: 4.5,
      deliveryTime: '22-27 mins'
    },
    {
      id: 'pizza-veggie-delight',
      name: 'Veggie Delight',
      price: 249,
      category: 'Pizza',
      image: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: false,
      rating: 4.4,
      deliveryTime: '20-25 mins'
    },
    {
      id: 'drink-coca-cola',
      name: 'Coca Cola',
      price: 99,
      category: 'Drinks',
      image: 'https://images.unsplash.com/photo-1629203851122-3726ecdf080e?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: true,
      rating: 4.5,
      deliveryTime: '15-20 mins'
    },
    {
      id: 'drink-pepsi',
      name: 'Pepsi',
      price: 89,
      category: 'Drinks',
      image: 'https://images.unsplash.com/photo-1551024709-8f23befc6cf7?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: false,
      rating: 4.3,
      deliveryTime: '15-20 mins'
    },
    {
      id: 'drink-sprite',
      name: 'Sprite',
      price: 89,
      category: 'Drinks',
      image: 'https://images.unsplash.com/photo-1624517452488-04869289c4ca?auto=format&fit=crop&w=1200&q=80',
      isVeg: true,
      isBestseller: false,
      rating: 4.2,
      deliveryTime: '15-20 mins'
    }
  ];

  private destroy$ = new Subject<void>();

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.cartService.cartCount$
      .pipe(takeUntil(this.destroy$))
      .subscribe(count => {
        this.cartCount = count;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  setCategory(category: MenuCategory): void {
    this.selectedCategory = category;
  }

  addToCart(item: MenuItem): void {
    this.cartService.addToCart({
      id: item.id,
      name: item.name,
      price: item.price,
      quantity: 1,
      image: item.image
    });
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;

    if (img.dataset['fallbackApplied'] === 'true') {
      return;
    }

    img.src = 'https://via.placeholder.com/300x200?text=Food+Image';
    img.dataset['fallbackApplied'] = 'true';
  }

  get filteredItems(): MenuItem[] {
    const query = this.searchText.trim().toLowerCase();

    return this.menuItems.filter(item => {
      const categoryMatch = this.selectedCategory === 'All' || item.category === this.selectedCategory;
      const nameMatch = !query || item.name.toLowerCase().includes(query);

      return categoryMatch && nameMatch;
    });
  }

}
