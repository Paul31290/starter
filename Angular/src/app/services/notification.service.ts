import { Injectable, ApplicationRef, Injector, EnvironmentInjector, inject as ngInject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { createComponent } from '@angular/core';
import { ConfirmComponent } from '../components/confirm/confirm.component';

export interface Notification {
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationSubject = new BehaviorSubject<Notification | null>(null);
  public notification$: Observable<Notification | null> = this.notificationSubject.asObservable();

  private autoDismissTimeout: any;
  private envInjector = ngInject(EnvironmentInjector);

  show(notification: Notification): void {
    if (this.autoDismissTimeout) {
      clearTimeout(this.autoDismissTimeout);
      this.autoDismissTimeout = null;
    }

    this.notificationSubject.next(notification);

    if (notification.duration && notification.duration > 0) {
      this.autoDismissTimeout = setTimeout(() => {
        this.clear();
      }, notification.duration);
    }
  }

  success(message: string, duration?: number): void {
    this.show({ message, type: 'success', duration });
  }

  error(message: string, duration?: number): void {
    this.show({ message, type: 'error', duration });
  }

  warning(message: string, duration?: number): void {
    this.show({ message, type: 'warning', duration });
  }

  info(message: string, duration?: number): void {
    this.show({ message, type: 'info', duration });
  }

  clear(): void {
    if (this.autoDismissTimeout) {
      clearTimeout(this.autoDismissTimeout);
      this.autoDismissTimeout = null;
    }

    this.notificationSubject.next(null);
  }

  confirm(message: string, title?: string): Promise<boolean> {
    try {
      const compRef = createComponent(ConfirmComponent, { environmentInjector: this.envInjector });
      compRef.instance.message = message;
      compRef.instance.title = title || '';
      compRef.instance.visible = true;

      const native = compRef.location.nativeElement;
      document.body.appendChild(native);
      compRef.changeDetectorRef.detectChanges();

      return new Promise<boolean>((resolve) => {
        compRef.instance.resolve = (v: boolean) => {
          try { 
            resolve(Boolean(v)); 
          } catch { 
            resolve(false); 
          }
          try { 
            compRef.destroy(); 
          } catch { }
          try { 
            if (native.parentNode) {
              native.parentNode.removeChild(native);
            }
          } catch { }
        };
      });
    } catch (err) {
      try {
        const text = title ? `${title}\n\n${message}` : message;
        const ok = window.confirm(text);
        return Promise.resolve(Boolean(ok));
      } catch (e) {
        return Promise.resolve(false);
      }
    }
  }
}
