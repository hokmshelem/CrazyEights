import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss']
})
export class SendEmailComponent implements OnInit {
  requestCompleted = false;
  emailForm: FormGroup = new FormGroup({});
  submitted = false;
  mode: string = '';
  errorMessages: string[] =[];

  constructor(private formBuilder: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (applicationUser: ApplicationUser | null) => {
        if (applicationUser) {
          this.router.navigateByUrl('/');
        } else {
          const mode = this.activatedRoute.snapshot.paramMap.get('mode');
          if (mode) {
            this.mode = mode;
            this.initializeForm();
            this.requestCompleted = true;
          } else {
            this.router.navigateByUrl('/');
          }
        }
      }
    })
  }

  initializeForm() {
    this.emailForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern('^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$')]],
    })
  }

  sendEmail() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.emailForm.valid) {
      if (this.mode.includes('resend-email-confirmation-link')) {
        this.accountService.resendEmailConfirmationLink(this.emailForm.get('email')?.value).subscribe({
          next: response => {
            this.accountService.showNotification('Email sent', response);
            this.router.navigateByUrl('/account/login');
          },
          error: error => {
            if (typeof(error.error) !== 'object') {
              this.errorMessages.push(error.error);
            }
          }
        })
      } else if (this.mode.includes('forgot-username-or-password')) {
        this.accountService.forgotUsernameOrPassword(this.emailForm.get('email')?.value).subscribe({
          next: response => {
            this.accountService.showNotification('Email sent', response);
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
  }

  cancel() {
    this.router.navigateByUrl('/account/login');
  }
}
