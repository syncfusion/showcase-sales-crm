import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AppIconComponent } from './shared/app-icon.component';
import { ThemeService } from './layout/theme.service';


@Component({
  selector: 'app-root',
  imports: [AppIconComponent, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly themeService = inject(ThemeService);
  private readonly router = inject(Router);
  protected readonly sidebarOpen = signal(false);
  protected readonly navItems = [
    { label: 'Dashboard', to: '/', icon: 'dashboard' },
    { label: 'Accounts', to: '/accounts', icon: 'accounts' },
    { label: 'Contacts', to: '/contacts', icon: 'contacts' },
    { label: 'Opportunities', to: '/opportunities', icon: 'opportunities' },
    { label: 'Pipeline Kanban', to: '/pipeline', icon: 'pipeline' },
    { label: 'Forecast', to: '/forecast', icon: 'forecast' },
    { label: 'Activities Calendar', to: '/activities', icon: 'activities' },
    { label: 'Territory Map', to: '/territories', icon: 'territories' },
    { label: 'Documents', to: '/documents', icon: 'documents' },
    { label: 'AI Sales Assistant', to: '/assistant', icon: 'assistant' },
    { label: 'Admin Seed Data', to: '/admin', icon: 'admin' },
  ];

  protected toggleSidebar() {
    this.sidebarOpen.update((open) => !open);
  }
  protected closeSidebar() {
    this.sidebarOpen.set(false);
  }

  constructor() {
    this.router.events.subscribe(() => this.closeSidebar());
  }
}
