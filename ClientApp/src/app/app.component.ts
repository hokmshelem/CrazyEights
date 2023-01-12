import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    this.refreshApplicationUser();
  }

  refreshApplicationUser() {
    const jwt = this.accountService.getJwt();
    if (jwt) {
      this.accountService.refreshApplicationUser(jwt).subscribe({
        next: _ => {},
        error: _ => {
          this.accountService.logout();
        }
      })
    } else {
      this.accountService.refreshApplicationUser(null).subscribe();
    }
  }
}
