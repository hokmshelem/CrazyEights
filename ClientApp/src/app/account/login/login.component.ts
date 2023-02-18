import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  submitted = false;

  constructor(private formBuilder: FormBuilder, private accountService: AccountService,
    private router: Router) {
    this.accountService.applicationUser$.pipe(take(1)).subscribe(
      (applicationUser: ApplicationUser | null) => {
        if (applicationUser) {
          this.router.navigateByUrl('/');
        }
      }
    )
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    })
  }

  login() {
    this.submitted = true;

    if (this.loginForm.valid) {
      this.accountService.login(this.loginForm.value).subscribe({
        next: _ => this.router.navigateByUrl('/'),
        error: error => console.log(error)
      })
    }
  }
}
