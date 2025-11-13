import { Component } from '@angular/core';
import { LayoutComponent } from './components/layout/layout.component';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [LayoutComponent],
  template: `
    <app-layout type="full">
      <div class="not-found-content">
        <h1>404 - Page Not Found</h1>
        <p>The page you are looking for does not exist.</p>
      </div>
    </app-layout>
  `,
  styles: [`
    .not-found-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      text-align: center;
    }
  `]
})
export class NotFoundComponent {}
