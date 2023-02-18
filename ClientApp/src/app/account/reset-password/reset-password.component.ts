import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { ResetPassword } from 'src/app/shared/models/account/resetPassword';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  requestCompleted = false;
  resetPasswordForm: FormGroup = new FormGroup({});
  token: string = '';
  submitted = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (applicationUser: ApplicationUser | null) => {
        if (applicationUser) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (parmas: any) => {
              this.token = parmas.get('token');
              this.initializeForm(parmas.get('username'), parmas.get('email'));
              this.requestCompleted = true;
            }
          })
        }
      }
    })
  }

  initializeForm(username: string, email: string) {
    this.resetPasswordForm = this.formBuilder.group({
      username: [{value: username, disabled: true}],
      email: [{value: email, disabled: true}],
      newPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]],
    })
  }

  resetPassword() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.resetPasswordForm.valid) {
      const model: ResetPassword = {
        token: this.token,
        email: this.resetPasswordForm.get('email')?.value,
        newPassword: this.resetPasswordForm.get('newPassword')?.value
      };

      this.accountService.resetPassword(model).subscribe({
        next: response => {
          this.accountService.showNotification('Password reset', response);
          this.router.navigateByUrl('/account/login');
        },
        error: error => {
          if (typeof(error.error) !== 'object') {
            this.errorMessages.push(error.error);
          }
        }
      })
    }
  }

  cancel() {
    this.router.navigateByUrl('/account/login');
  }
}
