import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { SharedModule } from '../shared/shared.module';
import { ErrorMessagesComponent } from './errors/error-messages/error-messages.component';



@NgModule({
  declarations: [
    NavbarComponent,
    FooterComponent,
    ErrorMessagesComponent
  ],
  imports: [
    CommonModule,
    SharedModule
  ],
  exports: [
    NavbarComponent,
    FooterComponent,
    ErrorMessagesComponent
  ]
})
export class CoreModule { }
