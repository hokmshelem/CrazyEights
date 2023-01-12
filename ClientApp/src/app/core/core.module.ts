import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorsComponent } from './errors/errors.component';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    ErrorsComponent,
    NavbarComponent,
    FooterComponent
  ],
  imports: [
    CommonModule,
    SharedModule
  ],
  exports: [
    NavbarComponent,
    FooterComponent
  ]
})
export class CoreModule { }
