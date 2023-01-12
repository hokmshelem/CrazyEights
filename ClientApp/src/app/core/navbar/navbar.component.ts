import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/account/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  collapsed = true;

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  }

  toggleCollapsed() {
    this.collapsed = !this.collapsed;
  }

  logout() {
    this.accountService.logout();
  }
}
