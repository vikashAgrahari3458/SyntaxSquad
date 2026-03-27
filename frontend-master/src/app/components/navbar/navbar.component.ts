import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class NavbarComponent implements OnInit, OnDestroy {

  cartCount = 0;
  isLoggedIn = false;
  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    private cartService: CartService
  ) {}

  ngOnInit() {
    // Subscribe to shared cart count updates
    this.cartService.cartCount$
      .pipe(takeUntil(this.destroy$))
      .subscribe(count => {
        this.cartCount = count;
      });

    // Check if user is logged in (placeholder logic)
    this.checkLoginStatus();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  goToCart() {
    this.router.navigate(['/cart']);
  }

  toggleLogin() {
    if (this.isLoggedIn) {
      this.logout();
    } else {
      this.router.navigate(['/login']);
    }
  }

  logout() {
    // Clear auth data
    localStorage.removeItem('authToken');
    this.isLoggedIn = false;
    this.router.navigate(['/login']);
  }

  private checkLoginStatus() {
    // Check if user is authenticated
    this.isLoggedIn = !!localStorage.getItem('authToken');
  }
}