import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResetPasswordRequest } from '../../../models/auth.model';
import { TranslateModule } from '@ngx-translate/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    TranslateModule
],
})
export class ResetPasswordComponent implements OnInit {
    resetPasswordForm: FormGroup;
    loading = false;
    showSuccess = false;
    errorMessage = false;
    returnUrl = '';

    private token = '';
    private email = '';


    constructor(
        private formBuilder: FormBuilder,
        private authService: AuthService,
        private router: Router,
        private route: ActivatedRoute,) {
            this.resetPasswordForm = this.formBuilder.group({
            password: ['', [
              Validators.required,
              Validators.minLength(8),
              Validators.maxLength(255),
              Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/)]],
            confirmPassword: ['', [Validators.required]],
        }, { validators: this.passwordMatchValidator });
    }

    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

        if (this.authService.isAuthenticated()) {
        this.router.navigate([this.returnUrl]);
        }

        this.resetPasswordForm.get('confirm')?.setValidators([Validators.required, Validators.minLength(6)]);
        this.token = this.route.snapshot.queryParams['token'];
        this.email = this.route.snapshot.queryParams['email'];
    }


    onSubmit(): void {
        if (this.resetPasswordForm.valid) {
          this.loading = true;
          this.showSuccess = false;
          this.errorMessage = false;
    
          const resetPasswordRequest: ResetPasswordRequest = {
            email: this.resetPasswordForm.value.email,
            password: this.resetPasswordForm.value.password,
            confirmPassword: this.resetPasswordForm.value.confirmPassword,
            token: this.resetPasswordForm.value.token,
          };
    
          this.authService.resetPassword(resetPasswordRequest).subscribe({
            next: (response) => {
              this.loading = false;
              this.showSuccess = true;
              this.router.navigate([this.returnUrl]);
            },
            error: (error) => {
              this.loading = false;
              this.errorMessage = true;
            }
          });
        } else {
          this.markFormGroupTouched();
        }
    }

    private markFormGroupTouched(): void {
        Object.keys(this.resetPasswordForm.controls).forEach(key => {
        const control = this.resetPasswordForm.get(key);
        control?.markAsTouched();
        });
    }

    isFieldRequired(fieldName: string): boolean {
    const control = this.resetPasswordForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) {
        return !!(control?.invalid && control.touched);;
      }
    }
    return false;
  }

  isFieldUnderMinLength(fieldName: string): boolean {
    const control = this.resetPasswordForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['minlength']) {
        return !!(control?.invalid && control.touched);;
      }
    }
    return false;
  }

  isFieldOverMaxLength(fieldName: string): boolean {
    const control = this.resetPasswordForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) {
        return !!(control?.invalid && control.touched);;
      }
    }
    return false;
  }

  isFieldPatternMismatch(fieldName: string): boolean {
    const control = this.resetPasswordForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['pattern']) {
        return !!(control?.invalid && control.touched);;
      }
    }
    return false;
  }
    
    getFieldError(fieldName: string): string {
        const control = this.resetPasswordForm.get(fieldName);
        if (control?.errors && control.touched) {
        if (control.errors['required']) {
          return `${this.getFieldLabel(fieldName)} is required.`;
        }
        if (control.errors['minlength']) {
          return `${this.getFieldLabel(fieldName)} must be at least ${control.errors['minlength'].requiredLength} characters.`;
        }
        if (control.errors['maxlength']) {
          return `${this.getFieldLabel(fieldName)} cannot exceed ${control.errors['maxlength'].requiredLength} characters.`;
        }
        if (control.errors['pattern']) {
          return 'Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.';
        }
        if (control.errors['passwordMismatch']) {
          return 'Passwords do not match.';
        }
      }
    return '';
    }

    isFieldInvalid(fieldName: string): boolean {
        const control = this.resetPasswordForm.get(fieldName);
        return !!(control?.invalid && control.touched);
    }

    passwordMatchValidator(control: AbstractControl): { [key: string]: any } | null {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');

      if (password && confirmPassword && password.value !== confirmPassword.value) {
        return { passwordMismatch: true };
      }
      return null;
    }

    hasPasswordMismatch(): boolean {
      return !!(this.resetPasswordForm.hasError('passwordMismatch') &&
      this.resetPasswordForm.get('confirmPassword')?.touched);
    }
    
  
    private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      'email': 'Email',
    };
    return labels[fieldName] || fieldName;
    }
}