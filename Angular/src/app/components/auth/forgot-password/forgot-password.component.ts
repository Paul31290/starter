import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgotPasswordRequest } from '../../../models/auth.model';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
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
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm: FormGroup;
  loading = false;
  errorMessage = false;
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
        this.errorMessage = false;
  
        const forgotPasswordRequest: ForgotPasswordRequest = {
          email: this.forgotPasswordForm.value.email,
        };
  
        this.authService.forgotPassword(forgotPasswordRequest).subscribe({
          next: (response) => {
            this.loading = false;
            if(response === false){
              this.errorMessage = true;
            } else {
              this.showSuccess = true;
            }
          },
          error: (error) => {
            console.error('Error sending email:', error);
            this.loading = false;
            this.errorMessage = true;
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
  
  isFieldRequired(fieldName: string): boolean {
      const control = this.forgotPasswordForm.get(fieldName);
      if (control?.errors && control.touched) {
        if (control.errors['required']) {
          return !!(control?.invalid && control.touched);
        }
      }
    return false;
  }

  isFieldEmail(fieldName: string): boolean {
    const control = this.forgotPasswordForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['email']) {
        return !!(control?.invalid && control.touched);
      }
    }
    return false;
  }
}