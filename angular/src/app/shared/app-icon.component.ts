import { Component, Input } from '@angular/core';
import { LucideDynamicIcon } from '@lucide/angular';

const icons: Record<string, string> = {
  dashboard: 'layout-dashboard',
  accounts: 'building-2',
  contacts: 'contact',
  opportunities: 'badge-dollar-sign',
  pipeline: 'columns-3',
  forecast: 'trending-up',
  activities: 'calendar-days',
  territories: 'map-pinned',
  documents: 'file-text',
  assistant: 'bot',
  admin: 'database',
  revenue: 'dollar-sign',
  work: 'briefcase-business',
  sun: 'sun',
  moon: 'moon'
};

@Component({
  selector: 'app-icon',
  imports: [LucideDynamicIcon],
  template: `
    <span class="app-icon" [attr.aria-label]="name">
      <svg [lucideIcon]="icons[name] || 'layout-dashboard'" [size]="size" />
    </span>
  `
})
export class AppIconComponent {
  @Input() name = 'dashboard';
  @Input() size = 18;
  protected readonly icons = icons;
}
