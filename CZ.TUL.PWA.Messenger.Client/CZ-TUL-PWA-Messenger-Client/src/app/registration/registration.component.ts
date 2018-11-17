import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../_services/user.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
  registrationForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';


  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService) { }

  ngOnInit() {
    this.registrationForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      name: ['', Validators.required]
    });
  }

  onSubmit() {
    this.submitted = true;

    if (this.registrationForm.invalid) {
        return;
    }

    this.loading = true;

    this.userService.registerUser(this.registrationForm.controls.username.value,
                                  this.registrationForm.controls.password.value,
                                  this.registrationForm.controls.name.value)
        .subscribe(
          data => this.successRegistration(data),
          error => this.failedRegistration(error),
          () => this.completeRegistration()
        );
  }

  get f() { return this.registrationForm.controls; }

  completeRegistration(): void {
  }

  failedRegistration(error: any): void {
    console.error(error);
    this.loading = false;
    // TODO
  }

  successRegistration(data: any): void {
    this.loading = false;
    this.router.navigate(['login']);
  }

}
