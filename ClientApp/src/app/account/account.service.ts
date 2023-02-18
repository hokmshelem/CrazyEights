import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { map, of, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { ConfirmEmail } from '../shared/models/account/confirmEmail';
import { Login } from '../shared/models/account/login';
import { Register } from '../shared/models/account/register';
import { ResetPassword } from '../shared/models/account/resetPassword';
import { NotificationModalComponent } from './notification-modal/notification-modal.component';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private applicationUserSource = new ReplaySubject<ApplicationUser | null>(1);
  applicationUser$ = this.applicationUserSource.asObservable();

  constructor(private http: HttpClient,
    private router: Router,
    private bsModalRef: BsModalRef,
    private modalService: BsModalService) { }

  refreshApplicationUser(jwt: string | null) {
    if (jwt === null) {
      this.applicationUserSource.next(null);
      return of(undefined);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);

    return this.http.get<ApplicationUser>(`${environment.appUrl}/api/account/refresh-application-user`, { headers }).pipe(
      map((applicationUser: ApplicationUser) => {
        if (applicationUser) {
          this.setApplicationUser(applicationUser);
        }
      })
    )
  }

  login(model: Login) {
    return this.http.post<ApplicationUser>(`${environment.appUrl}/api/account/login`, model).pipe(
      map((applicationUser: ApplicationUser) => {
        if (applicationUser) {
          this.setApplicationUser(applicationUser);
        }
      })
    );
  }

  register(model: Register) {
    return this.http.post(`${environment.appUrl}/api/account/register`, model, { responseType: 'text' })
  }

  setApplicationUser(applicationUser: ApplicationUser) {
    localStorage.setItem(environment.applicationUserKey, JSON.stringify(applicationUser));
    this.applicationUserSource.next(applicationUser);
  }

  logout() {
    localStorage.removeItem(environment.applicationUserKey);
    this.applicationUserSource.next(null);
    this.router.navigateByUrl('/');
  }

  confirmEmail(model: ConfirmEmail) {
    return this.http.put(`${environment.appUrl}/api/account/confirm-email`, model, { responseType: 'text' });
  }

  resendEmailConfirmationLink(email: string) {
    return this.http.post(`${environment.appUrl}/api/account/resend-email-confirmation-link/${email}`, {}, { responseType: 'text' });
  }

  forgotUsernameOrPassword(email: string) {
    return this.http.post(`${environment.appUrl}/api/account/forgot-username-or-password/${email}`, {}, { responseType: 'text' });
  }

  resetPassword(model: ResetPassword) {
    return this.http.put(`${environment.appUrl}/api/account/reset-password`, model, { responseType: 'text' });
  }

  getJwt() {
    const key = localStorage.getItem(environment.applicationUserKey);
    if (key) {
      const applicationUser: ApplicationUser = JSON.parse(key);
      return applicationUser.jwt;
    }

    return null;
  }

  showNotification(title: string, message: string) {
    const initialState: ModalOptions = {
      initialState: {
        title,
        message
      }
    };

    this.bsModalRef = this.modalService.show(NotificationModalComponent, initialState);
  }
}
