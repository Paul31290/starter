import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface PasswordStrength {
  score: number;
  label: string;
  color: string;
}

@Component({
  selector: 'app-password-strength-indicator',
  standalone: true,
  imports: [CommonModule],
  template: `
  @if(password){
    <div class="password-strength-indicator">
      <div class="strength-bar">
        <div 
          class="strength-fill" 
          [style.width.%]="strength.score * 20"
          [style.background-color]="strength.color"
        ></div>
      </div>
      <div class="strength-label" [style.color]="strength.color">
        {{ strength.label }}
      </div>
    </div>
  }
  `,
  styles: [`
    .password-strength-indicator {
      margin-top: 8px;
    }

    .strength-bar {
      width: 100%;
      height: 4px;
      background-color: #e9ecef;
      border-radius: 2px;
      overflow: hidden;
    }

    .strength-fill {
      height: 100%;
      transition: width 0.3s ease, background-color 0.3s ease;
    }

    .strength-label {
      font-size: 12px;
      font-weight: 500;
      margin-top: 4px;
    }
  `]
})
export class PasswordStrengthIndicatorComponent implements OnChanges {
  @Input() password: string = '';

  strength: PasswordStrength = {
    score: 0,
    label: '',
    color: '#6c757d'
  };

  ngOnChanges(changes: SimpleChanges) {
    if (changes['password']) {
      this.calculateStrength();
    }
  }

  private calculateStrength() {
    if (!this.password) {
      this.strength = { score: 0, label: '', color: '#6c757d' };
      return;
    }

    let score = 0;
    const password = this.password;

    if (password.length >= 8) score += 1;
    if (password.length >= 12) score += 1;

    if (/[a-z]/.test(password)) score += 1;
    if (/[A-Z]/.test(password)) score += 1;
    if (/[0-9]/.test(password)) score += 1;
    if (/[^A-Za-z0-9]/.test(password)) score += 1;

    // Determine strength level
    if (score <= 2) {
      this.strength = { score, label: 'Weak', color: '#dc3545' };
    } else if (score <= 4) {
      this.strength = { score, label: 'Fair', color: '#ffc107' };
    } else if (score <= 5) {
      this.strength = { score, label: 'Good', color: '#17a2b8' };
    } else {
      this.strength = { score, label: 'Strong', color: '#28a745' };
    }
  }
}
