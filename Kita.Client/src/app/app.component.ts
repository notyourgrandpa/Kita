import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, NavigationStart, NavigationError } from '@angular/router';
import { AuthService } from './auth/auth.service';
import { filter } from 'rxjs/operators';
import { ConnectionService, ConnectionServiceOptions, ConnectionState } from 'ng-connection-service';
import { Observable, map } from 'rxjs';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  public isOffline: Observable<boolean>;
  isLoginRoute = false;
  public isExpanded = false;

  constructor(
    public router: Router,
    private authService: AuthService,
    private connectionService: ConnectionService
  ) {
    const options: ConnectionServiceOptions = {
      enableHeartbeat: false,
      heartbeatUrl: environment.baseUrl + 'api/heartbeat',
      heartbeatInterval: 100000
    };

    this.isOffline = this.connectionService.monitor(options)
      .pipe(map(state => !state.hasNetworkConnection || !state.hasInternetAccess));

    // Debug router events
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        console.log('Navigation started to:', event.url);
      } else if (event instanceof NavigationEnd) {
        console.log('Navigation ended at:', event.url);
      } else if (event instanceof NavigationError) {
        console.error('Navigation error:', event.error);
      }
    });
  }

  ngOnInit(): void {
    console.log('App component initialized');
    console.log('Current auth status:', this.authService.isAuthenticated());
    console.log('Current URL:', this.router.url);

    this.authService.init();

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event) => {
        const navEndEvent = event as NavigationEnd;
        this.isLoginRoute = navEndEvent.urlAfterRedirects.startsWith('/login');
      });
  }

  public toggleMenu(): void {
    this.isExpanded = !this.isExpanded;
  }

  public getCurrentUrl(): string {
    return this.router.url;
  }

  public onSidenavToggled(opened: boolean): void {
    console.log('Sidenav toggled:', opened);
  }

  // Public getter for template access (better practice)
  get isAuthenticated$(): Observable<boolean> {
    return this.authService.authStatus;
  }
}
