import { Component, OnInit, Output, EventEmitter, OnDestroy, HostListener, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subject, takeUntil, filter } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay } from 'rxjs/operators';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { MatSidenav } from '@angular/material/sidenav';
import { AuthService } from '../auth/auth.service';
import { User } from "../auth/user.model";

export interface MenuItem {
  label: string;
  icon: string;
  route?: string;
  children?: MenuItem[];
  expanded?: boolean;
}

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  animations: [
    trigger('expandCollapse', [
      state('expanded', style({ height: '*', opacity: 1 })),
      state('collapsed', style({ height: '0', opacity: 0 })),
      transition('expanded <=> collapsed', [
        animate('300ms cubic-bezier(0.4, 0.0, 0.2, 1)')
      ])
    ]),
    trigger('rotate', [
      state('expanded', style({ transform: 'rotate(180deg)' })),
      state('collapsed', style({ transform: 'rotate(0deg)' })),
      transition('expanded <=> collapsed', [
        animate('300ms cubic-bezier(0.4, 0.0, 0.2, 1)')
      ])
    ])
  ]
})
export class NavMenuComponent implements OnInit, OnDestroy {
  @ViewChild('sidenav') sidenav!: MatSidenav;
  @Output() sidenavToggled = new EventEmitter<boolean>();

  private destroySubject = new Subject<void>();

  isLoggedIn = false;
  user: User | null = null;
  activeRoute = '';
  appTitle = 'GPA';

  // Responsive breakpoint detection
  isHandset$ = this.breakpointObserver.observe([Breakpoints.Handset, Breakpoints.TabletPortrait])
    .pipe(
      map(result => result.matches),
      shareReplay(1)
    );

  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'dashboard',
      route: '/dashboard'
    },
    {
      label: 'Kita',
      icon: 'inventory_2',
      expanded: false,
      children: [
        { label: 'All Products', icon: 'list', route: '/products' },
        { label: 'Categories', icon: 'category', route: '/categories' },
        { label: 'Inventory Logs', icon: 'receipt_long', route: '/inventory-logs' },
        { label: 'Sales Transactions', icon: 'receipt_long', route: '/sales-transactions' },
        { label: 'Expenses', icon: 'wallet', route: '/expenses' }
      ]
    },
    {
      label: 'Settings',
      icon: 'settings',
      expanded: false,
      children: [
        { label: 'Profile', icon: 'person', route: '/settings/profile' },
        { label: 'Preferences', icon: 'tune', route: '/settings/preferences' },
        { label: 'Security', icon: 'security', route: '/settings/security' },
        { label: 'Data Maintainance', icon: 'data_array', route: '/data-maintainance'}
      ]
    }
  ];

  constructor(
    private authService: AuthService,
    private router: Router,
    private breakpointObserver: BreakpointObserver
  ) {
    // Listen to authentication status changes
    this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
        this.user = result ? this.authService.getCurrentUser() : null;
      });

    // Track active route for highlighting
    this.router.events
      .pipe(
        filter((event): event is NavigationEnd => event instanceof NavigationEnd),
        takeUntil(this.destroySubject)
      )
      .subscribe((event: NavigationEnd) => {
        this.activeRoute = event.urlAfterRedirects;
        this.updateActiveStates();
      });
  }

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
    this.user = this.isLoggedIn ? this.authService.getCurrentUser() : null;
    this.activeRoute = this.router.url;
    this.updateActiveStates();
  }

  ngOnDestroy(): void {
    this.destroySubject.next();
    this.destroySubject.complete();
  }

  toggleSubmenu(item: MenuItem): void {
    if (!item.children) return;

    item.expanded = !item.expanded;

    // Close other submenus (optional - remove if you want multiple submenus open)
    this.menuItems.forEach(menuItem => {
      if (menuItem !== item && menuItem.children) {
        menuItem.expanded = false;
      }
    });
  }

  navigate(route: string): void {
    this.router.navigate([route]);

    // Close sidenav on mobile after navigation
    this.isHandset$.pipe(takeUntil(this.destroySubject)).subscribe(isHandset => {
      if (isHandset && this.sidenav) {
        this.sidenav.close();
      }
    });
  }

  toggleSidenav(): void {
    if (this.sidenav) {
      this.sidenav.toggle();
      this.sidenavToggled.emit(this.sidenav.opened);
    }
  }

  isActive(route: string): boolean {
    return this.activeRoute === route || this.activeRoute.startsWith(route + '/');
  }

  isParentActive(item: MenuItem): boolean {
    if (!item.children) return false;
    return item.children.some(child => child.route && this.isActive(child.route));
  }

  private updateActiveStates(): void {
    // Expand parent menu if child is active
    this.menuItems.forEach(item => {
      if (item.children && this.isParentActive(item)) {
        item.expanded = true;
      }
    });
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  onLogin(): void {
    this.router.navigate(['/login']);
  }

  // Handle responsive behavior
  @HostListener('window:resize', ['$event'])
  onResize(): void {
  }
}
