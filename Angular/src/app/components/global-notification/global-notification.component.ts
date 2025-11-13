import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
}

@Component({
  selector: 'app-global-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-container">
      <div 
        *ngFor="let notification of notifications" 
        class="notification"
        [ngClass]="'notification-' + notification.type"
        [@slideIn]
      >
        <div class="notification-content">
          <div class="notification-icon">
            <span *ngIf="notification.type === 'success'">✓</span>
            <span *ngIf="notification.type === 'error'">✗</span>
            <span *ngIf="notification.type === 'warning'">⚠</span>
            <span *ngIf="notification.type === 'info'">ℹ</span>
          </div>
          <div class="notification-text">
            <div class="notification-title">{{ notification.title }}</div>
            <div class="notification-message">{{ notification.message }}</div>
          </div>
          <button 
            class="notification-close" 
            (click)="removeNotification(notification.id)"
            aria-label="Close notification"
          >
            ×
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notification-container {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 9999;
      max-width: 400px;
    }

    .notification {
      margin-bottom: 10px;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      overflow: hidden;
      animation: slideIn 0.3s ease-out;
    }

    .notification-success {
      background-color: #d4edda;
      border-left: 4px solid #28a745;
    }

    .notification-error {
      background-color: #f8d7da;
      border-left: 4px solid #dc3545;
    }

    .notification-warning {
      background-color: #fff3cd;
      border-left: 4px solid #ffc107;
    }

    .notification-info {
      background-color: #d1ecf1;
      border-left: 4px solid #17a2b8;
    }

    .notification-content {
      display: flex;
      align-items: flex-start;
      padding: 16px;
    }

    .notification-icon {
      font-size: 20px;
      margin-right: 12px;
      flex-shrink: 0;
    }

    .notification-text {
      flex: 1;
    }

    .notification-title {
      font-weight: 600;
      margin-bottom: 4px;
      color: #333;
    }

    .notification-message {
      color: #666;
      font-size: 14px;
    }

    .notification-close {
      background: none;
      border: none;
      font-size: 20px;
      cursor: pointer;
      color: #999;
      margin-left: 12px;
      flex-shrink: 0;
    }

    .notification-close:hover {
      color: #333;
    }

    @keyframes slideIn {
      from {
        transform: translateX(100%);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }
  `]
})
export class GlobalNotificationComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  private destroy$ = new Subject<void>();

  ngOnInit() {
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  addNotification(notification: Omit<Notification, 'id'>) {
    const id = Math.random().toString(36).substr(2, 9);
    const newNotification: Notification = {
      ...notification,
      id,
      duration: notification.duration || 5000
    };

    this.notifications.push(newNotification);

    if (newNotification.duration && newNotification.duration > 0) {
      setTimeout(() => {
        this.removeNotification(id);
      }, newNotification.duration);
    }
  }

  removeNotification(id: string) {
    this.notifications = this.notifications.filter(n => n.id !== id);
  }
}
