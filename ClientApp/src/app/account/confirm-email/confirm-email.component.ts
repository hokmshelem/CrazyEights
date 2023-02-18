import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { ConfirmEmail } from 'src/app/shared/models/account/confirmEmail';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {
  requestCompleted = false;
  accountActivationSucces?: boolean;
  message: string = '';

  constructor(private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (applicationUser: ApplicationUser | null) => {
        if (applicationUser) {
          this.router.navigateByUrl('/');
        } else {
         this.activatedRoute.queryParamMap.subscribe({
          next: (params: any) => {
            const model: ConfirmEmail = {
              token: params.get('token'),
              email: params.get('email')
            };

            this.accountService.confirmEmail(model).subscribe({
              next: response => {
                this.accountActivationSucces = true;
                this.message = response;
                this.requestCompleted = true;
              },
              error: error => {
                this.accountActivationSucces = false;
                this.message = error.error;
                this.requestCompleted = true;
              }
            })
          }
         }) 
        }
      }
    })
  }

  resendEmailConfirmationLink() {
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link')
  }

}
