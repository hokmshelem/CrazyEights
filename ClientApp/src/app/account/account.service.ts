import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, of, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApplicationUser } from '../shared/models/applicationUser';
import { Login } from '../shared/models/login';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private applicationUserSource = new ReplaySubject<ApplicationUser | null>(1);
  applicationUser$ = this.applicationUserSource.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  refreshApplicationUser(jwt: string | null) {
    if (jwt === null) {
      this.applicationUserSource.next(null);
      return of(undefined);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);

    return this.http.get<ApplicationUser>(`${environment.appUrl}/api/account/refresh-application-user`, {headers}).pipe(
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

  setApplicationUser(applicationUser: ApplicationUser) {
    localStorage.setItem(environment.applicationUserKey, JSON.stringify(applicationUser));
    this.applicationUserSource.next(applicationUser);
  }

  logout() {
    localStorage.removeItem(environment.applicationUserKey);
    this.applicationUserSource.next(null);
    this.router.navigateByUrl('/');
  }

  getJwt() {
    const key = localStorage.getItem(environment.applicationUserKey);
    if (key) {
      const applicationUser: ApplicationUser = JSON.parse(key);
      return applicationUser.jwt;
    }

    return null;
  }
}
