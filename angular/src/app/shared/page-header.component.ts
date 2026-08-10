import { Component, Input } from '@angular/core';
import { AppIconComponent } from './app-icon.component';

@Component({
  selector: 'app-page-header',
  imports: [AppIconComponent],
  template: `
    <header class="page-header">
      <span class="page-header-icon"><app-icon [name]="icon" /></span>
      <div>
        <h1>{{ title }}</h1>
        <p>{{ subtitle }}</p>
      </div>
    </header>
  `
})
export class PageHeaderComponent {
  @Input({ required: true }) title = '';
  @Input({ required: true }) subtitle = '';
  @Input({ required: true }) icon = 'dashboard';
}
