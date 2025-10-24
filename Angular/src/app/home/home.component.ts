import { Component, OnInit } from "@angular/core";
import { LayoutComponent } from "../components/layout/layout.component";
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { AuthUser } from '../models/auth.model';

@Component({
  selector: "app-home",
  standalone: true,
  imports: [LayoutComponent, TranslateModule, RouterModule, MatButtonModule, MatCardModule, MatIconModule, CommonModule],
  template: `
    <app-layout type="full">
      <div class="home-content">
        <div class="welcome-section">
          <h1>{{ 'HOME.TITLE' | translate }}</h1>
          <p class="subtitle">Welcome to the Starter Template Application</p>
          <div class="user-info" *ngIf="currentUser">
            <p>Welcome back, <strong>{{ currentUser.firstName || currentUser.userName }}</strong>!</p>
            <div class="user-roles">
              <span class="role-badge" *ngFor="let role of currentUser.roles">{{ role }}</span>
            </div>
          </div>
        </div>
      </div>
    </app-layout>
  `,
  styles: [`
    .home-content {
      padding: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .welcome-section {
      text-align: center;
      margin-bottom: 3rem;
    }

    .welcome-section h1 {
      font-size: 2.5rem;
      margin-bottom: 1rem;
      color: #333;
    }

    .subtitle {
      font-size: 1.2rem;
      color: #666;
      margin: 0;
    }

    .user-info {
      margin-top: 1rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 8px;
      border-left: 4px solid #667eea;
    }

    .user-roles {
      margin-top: 0.5rem;
    }

    .role-badge {
      display: inline-block;
      background: #667eea;
      color: white;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.8rem;
      margin-right: 0.5rem;
      margin-bottom: 0.25rem;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 2rem;
      margin-top: 2rem;
    }

    .feature-card {
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .feature-card mat-card-header {
      margin-bottom: 1rem;
    }

    .feature-card mat-card-title {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 1.2rem;
    }

    .feature-card mat-card-content {
      flex: 1;
    }

    .feature-card mat-card-actions {
      margin-top: auto;
      padding: 1rem;
    }

    .feature-card button {
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
    }

    @media (max-width: 768px) {
      .home-content {
        padding: 1rem;
      }

      .features-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
      }
    }
  `],
})
export class HomeComponent implements OnInit {
  currentUser: AuthUser | null = null;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  hasUserManagementAccess(): boolean {
    return this.authService.hasAnyRole(['Administrator', 'Manager']);
  }

  logout(): void {
    this.authService.logout().subscribe(() => {
    });
  }
}