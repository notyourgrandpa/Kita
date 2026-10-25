import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from './../../environments/environment';
import { LoginRequest } from './login-request';
import { LoginResult } from './login-result';
import { User } from './user.model';

@Injectable({
  providedIn: 'root',
})

export class AuthService
{

  private tokenKey: string = "token";

  private _authStatus = new BehaviorSubject<boolean>(false);
  public authStatus = this._authStatus.asObservable();

  constructor(
    protected http: HttpClient,
    private router: Router,
    private snackBar: MatSnackBar) {
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null && !this.isTokenExpired(token);
  }

  /**
   * Redirects to the login page if the user is not authenticated.
   * Returns true if authenticated, false otherwise.
   * Can be used by route guards and by components (e.g. before opening a dialog).
   */
  redirectToLoginIfNotAuthenticated(returnUrl: string): boolean {
    if (this.isAuthenticated()) {
      return true;
    }
    this.snackBar.open('Please log in to continue.', 'Close', {
      duration: 5000
    });
    this.router.navigate(['/login'], {
      queryParams: {
        returnUrl
      }
    });
    return false;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  init(): void {
    if (this.isAuthenticated())
      this.setAuthStatus(true);
  }

  login(item: LoginRequest): Observable<LoginResult> {
    var url = environment.baseUrl + "api/Account/Login";
    return this.http.post<LoginResult>(url, item)
      .pipe(tap(loginResult => {
        if (loginResult.success && loginResult.token) {
          localStorage.setItem(this.tokenKey, loginResult.token);
          // Store user info if available
          if (loginResult.user) {
            this.setCurrentUser(loginResult.user);
          }
          this.setAuthStatus(true);
        }
      }));
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.clearCurrentUser();
    this.setAuthStatus(false);
  }


  private setAuthStatus(isAuthenticated: boolean): void {
    this._authStatus.next(isAuthenticated);
  }

  getCurrentUser(): User | null {
    const userJson = localStorage.getItem('currentUser');
    if (!userJson) {
      return null;
    }
    try {
      return JSON.parse(userJson) as User;
    } catch {
      return null;
    }
  }

  // Call this after successful login
  setCurrentUser(user: User): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  // Call this on logout
  clearCurrentUser(): void {
    localStorage.removeItem('currentUser');
  }

  private isTokenExpired(token: string): boolean {
    try {
      const [, payloadBase64] = token.split('.');
      const payloadJson = atob(payloadBase64);
      const payload = JSON.parse(payloadJson);

      if (!payload.exp) return true;

      const now = Math.floor(Date.now() / 1000); // current time in seconds
      return payload.exp < now;
    } catch (e) {
      return true;
    }
  }

  private getDecodedToken(): any {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch {
      return null;
    }
  }

  private getPermissions(): { [resource: string]: string[] } {
    const decoded = this.getDecodedToken();
    if (!decoded || !decoded.permissions) return {};
    return typeof decoded.permissions === 'string'
      ? JSON.parse(decoded.permissions)
      : decoded.permissions;
  }

  canEdit(resource: string): boolean {
    if (!this.isAuthenticated()) return false;
    const perms = this.getPermissions()[resource];
    return perms ? perms.includes('edit') : false;
  }
}
