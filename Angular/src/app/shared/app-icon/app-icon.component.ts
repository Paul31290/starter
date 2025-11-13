import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-icon',
  standalone: true,
  imports: [CommonModule],
  template: `
    <img
      [src]="iconSrc"
      [attr.alt]="name + ' icon'"
      [style.width.px]="size"
      [style.height.px]="size"
      [class.icon-suffix]="matSuffix"
      class="app-icon"
    />
  `,
  styles: [`
    .app-icon { display: inline-block; vertical-align: middle; }
    .icon-suffix { margin-left: 8px; }
  `]
})
export class AppIconComponent {
  @Input() name = '';
  @Input() size = 20;
  @Input() matSuffix = false; // allow using the matSuffix attribute on the component

  get iconSrc() {
    return `assets/icons/${this.name}.svg`;
  }
}
