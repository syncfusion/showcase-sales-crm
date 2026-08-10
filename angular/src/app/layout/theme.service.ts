import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly theme = signal<'light' | 'dark'>(
    (localStorage.getItem('sales-crm-angular-theme') as 'light' | 'dark') || 'light',
  );

  constructor() {
    this.apply(this.theme());
  }

  toggle() {
    this.apply(this.theme() === 'dark' ? 'light' : 'dark');
  }

  private apply(theme: 'light' | 'dark') {
    this.theme.set(theme);
    document.documentElement.dataset['theme'] = theme;
    document.body.classList.toggle('e-dark-mode', theme === 'dark');
    localStorage.setItem('sales-crm-angular-theme', theme);
  }
}
