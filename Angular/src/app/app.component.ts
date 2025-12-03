import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { Router, RouterOutlet } from "@angular/router";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Subject, Observable, takeUntil } from "rxjs";

import { LayoutComponent } from "./components/layout/layout.component";
import { GlobalNotificationComponent } from './components/global-notification/global-notification.component';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { MatIconModule } from "@angular/material/icon";
import { AuthService } from "./services/auth.service";
import { CommonModule } from "@angular/common";


@Component({
  selector: "app-root",
  standalone: true,
  imports: [RouterOutlet, LayoutComponent, GlobalNotificationComponent, TranslateModule, MatIconModule, CommonModule],
  template: `
      <app-layout type="full">
      <app-global-notification />
      <div class="main-content">
        @if (currentUser){
          <nav>
            <a routerLink="/settings" class="routerOption settings" (click)=onSettings()><mat-icon>settings</mat-icon></a>
            <a routerLink="/profile" class="routerOption profile" (click)=onProfile()><mat-icon>account_circle</mat-icon></a>
          </nav>
        }        
        <router-outlet></router-outlet>
      </div>
      </app-layout>
  `,
  styles: [
    `
      :host {
        display: flex;
        height: 100vh;
        position: relative;
      }

      .main-content {
        flex: 1;
        padding: 0;
        background-color: #f5f5f5;
        display: flex;
        flex-direction: column;
        overflow: hidden;
        min-height: 0;
        max-height: 100%;
        width: 100%;
      }
      
      .routerOption {
        cursor: pointer;
        padding: 5px;
      }

      @media (max-width: 768px) {
        .main-content {
          padding: 0;
        }
      }
    `,
  ],
})
export class AppComponent implements OnInit, OnDestroy {
  snackBar = inject(MatSnackBar);
  authService = inject(AuthService);
  destroyed$ = new Subject<boolean>();
  router = inject(Router);
  translateService = inject(TranslateService);

  isSidebarCollapsed = false;
  currentUser: any = null;

  onSidebarCollapsed(collapsed: boolean): void {
    this.isSidebarCollapsed = collapsed;
  }

  onSidebarSelection(selection: string): void {
    console.log('Sidebar selection:', selection);
  }

  onProfile(): void {
    this.router.navigate(['/profile']);
  }

  onSettings(): void {
    this.router.navigate(['/settings']);
  }

  ngOnInit(): void {
    const savedLanguage = localStorage.getItem('preferredLanguage') || 'en';
    this.translateService.setDefaultLang('en');
    this.translateService.use(savedLanguage);

    this.translateService.onLangChange.subscribe(() => {
    });

    this.translateService.onTranslationChange.subscribe(() => {
    });

    // Subscribe to current user
    this.authService.currentUser$
      .pipe(takeUntil(this.destroyed$))
      .subscribe(user => {
        this.currentUser = user;
      });
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.unsubscribe();
  }
}
