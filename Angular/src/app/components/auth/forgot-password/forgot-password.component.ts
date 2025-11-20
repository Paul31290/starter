import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgotPasswordRequest } from '../../../models/auth.model';


@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
],
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm: FormGroup;
  loading = false;
  errorMessage = '';
  returnUrl = '';
  showSuccess = false;


  constructor(
      private formBuilder: FormBuilder,
      private authService: AuthService,
      private router: Router,
      private route: ActivatedRoute,) {
        this.forgotPasswordForm = this.formBuilder.group({
        email: ['', [Validators.required, Validators.email]],
      });
  }

  ngOnInit(): void {
      this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

      if (this.authService.isAuthenticated()) {
      this.router.navigate([this.returnUrl]);
      }
  }

  onSubmit(): void {
      if (this.forgotPasswordForm.valid) {
        this.loading = true;
        this.showSuccess = false;
        this.errorMessage = '';
  
        const forgotPasswordRequest: ForgotPasswordRequest = {
          email: this.forgotPasswordForm.value.email,
        };
  
        this.authService.forgotPassword(forgotPasswordRequest).subscribe({
          next: (response) => {
            this.loading = false;
            this.showSuccess = true;
          },
          error: (error) => {
            this.loading = false;
            this.errorMessage = error.error?.message || 'Password reset failed. Please try again.';
          }
        });
      } else {
        this.markFormGroupTouched();
      }
  }

  private markFormGroupTouched(): void {
      Object.keys(this.forgotPasswordForm.controls).forEach(key => {
      const control = this.forgotPasswordForm.get(key);
      control?.markAsTouched();
      });
  }
  
  getFieldError(fieldName: string): string {
      const control = this.forgotPasswordForm.get(fieldName);
      if (control?.errors && control.touched) {
      if (control.errors['required']) {
          return `${this.getFieldLabel(fieldName)} is required.`;
      }
      if (control.errors['email']) {
          return `Please enter a valid email address.`;
      }
  }
  return '';
  }

  isFieldInvalid(fieldName: string): boolean {
      const control = this.forgotPasswordForm.get(fieldName);
      return !!(control?.invalid && control.touched);
  }
  
  private getFieldLabel(fieldName: string): string {
  const labels: { [key: string]: string } = {
    'email': 'Email',
  };
  return labels[fieldName] || fieldName;
  }
}