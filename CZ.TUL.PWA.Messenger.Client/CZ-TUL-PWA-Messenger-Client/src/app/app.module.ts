import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app.component';
import { routing } from './app.routing';

import { JwtInterceptor, ErrorInterceptor } from './_helpers';
import { HomeComponent } from './home';
import { LoginComponent } from './login';

import { RouterModule, Routes } from '@angular/router';
import { RegistrationComponent } from './registration/registration.component';
import { AddAddresseComponent } from './add-addresse/add-addresse.component';

import { ChecklistModule } from 'angular-checklist';

@NgModule({
    imports: [
        routing,
        BrowserModule,
        HttpClientModule,
        ReactiveFormsModule,
        FormsModule,
        ChecklistModule
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        LoginComponent,
        RegistrationComponent,
        AddAddresseComponent
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ],
    bootstrap: [AppComponent]
})

export class AppModule { }
