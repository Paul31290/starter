import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { Router, RouterOutlet } from "@angular/router";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Subject, Observable } from "rxjs";

import { LayoutComponent } from "./components/layout/layout.component";
import { GlobalNotificationComponent } from './components/global-notification/global-notification.component';
import { TranslateService, TranslateModule } from '@ngx-translate/core';


@Component({
  selector: "app-root",
  standalone: true,
  imports: [RouterOutlet, LayoutComponent, GlobalNotificationComponent, TranslateModule],
  template: `
      <app-layout type="full">
      <app-global-notification />
      <div class="main-content">        
        <router-outlet />
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
  destroyed$ = new Subject<boolean>();
  router = inject(Router);
  translateService = inject(TranslateService);

  isSidebarCollapsed = false;

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
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.unsubscribe();
  }
}
