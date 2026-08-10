import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-state',
  template: `<div class="content-card state-card">Loading CRM data...</div>`
})
export class LoadingStateComponent {}

@Component({
  selector: 'app-empty-state',
  template: `<div class="content-card state-card">{{ message }}</div>`
})
export class EmptyStateComponent {
  @Input() message = 'No data available.';
}

@Component({
  selector: 'app-error-state',
  template: `<div class="content-card state-card error-state">{{ message }}</div>`
})
export class ErrorStateComponent {
  @Input() message = 'Unable to load data.';
}
