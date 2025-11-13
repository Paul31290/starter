import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="confirm-overlay" *ngIf="visible">
      <div class="confirm-dialog">
        <div class="confirm-header">
          <h3 class="confirm-title">{{ title || 'Confirm' }}</h3>
        </div>
        <div class="confirm-body">
          <p class="confirm-message">{{ message }}</p>
        </div>
        <div class="confirm-footer">
          <button 
            type="button" 
            class="btn btn-secondary" 
            (click)="onCancel()"
          >
            Cancel
          </button>
          <button 
            type="button" 
            class="btn btn-primary" 
            (click)="onConfirm()"
          >
            Confirm
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .confirm-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 10000;
    }

    .confirm-dialog {
      background: white;
      border-radius: 8px;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
      max-width: 400px;
      width: 90%;
      max-height: 90vh;
      overflow: hidden;
    }

    .confirm-header {
      padding: 20px 20px 0 20px;
      border-bottom: 1px solid #e9ecef;
    }

    .confirm-title {
      margin: 0;
      font-size: 1.25rem;
      font-weight: 600;
      color: #333;
    }

    .confirm-body {
      padding: 20px;
    }

    .confirm-message {
      margin: 0;
      color: #666;
      line-height: 1.5;
    }

    .confirm-footer {
      padding: 20px;
      display: flex;
      justify-content: flex-end;
      gap: 10px;
      border-top: 1px solid #e9ecef;
    }

    .btn {
      padding: 8px 16px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-secondary {
      background-color: #6c757d;
      color: white;
    }

    .btn-secondary:hover {
      background-color: #5a6268;
    }

    .btn-primary {
      background-color: #007bff;
      color: white;
    }

    .btn-primary:hover {
      background-color: #0056b3;
    }
  `]
})
export class ConfirmComponent {
  @Input() visible: boolean = true;
  @Input() title: string = '';
  @Input() message: string = '';
  @Output() confirmed = new EventEmitter<boolean>();

  resolve: ((value: boolean) => void) | null = null;

  onConfirm() {
    this.confirmed.emit(true);
    this.visible = false;
    if (this.resolve) {
      this.resolve(true);
    }
  }

  onCancel() {
    this.confirmed.emit(false);
    this.visible = false;
    if (this.resolve) {
      this.resolve(false);
    }
  }
}
