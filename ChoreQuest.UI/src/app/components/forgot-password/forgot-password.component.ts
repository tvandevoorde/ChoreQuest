import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  email = '';
  message = '';
  errorMessage = '';
  isSubmitting = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.message = '';
    this.errorMessage = '';
    this.isSubmitting = true;

    this.authService.forgotPassword(this.email).subscribe({
      next: (response) => {
        this.message = response.message;
        this.isSubmitting = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'An error occurred. Please try again.';
        this.isSubmitting = false;
      }
    });
  }
}
