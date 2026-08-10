import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import {
  LucideBadgeDollarSign,
  LucideBot,
  LucideBriefcaseBusiness,
  LucideBuilding2,
  LucideCalendarDays,
  LucideColumns3,
  LucideContact,
  LucideDatabase,
  LucideDollarSign,
  LucideFileText,
  LucideHandshake,
  LucideLayoutDashboard,
  LucideMapPinned,
  LucideMoon,
  LucideSun,
  LucideTrendingUp,
  provideLucideIcons
} from '@lucide/angular';

import { routes } from './app.routes';
import { SALES_CRM_API_BASE_URL } from './shared/sales-crm-api.service';
import { environment } from '../environments/environment';

// Backend Web API base URL. Comes from the environment files (src/environments/*.ts),
// which angular.json swaps per build configuration via `fileReplacements`, so the
// dev build uses https://localhost:59527 and production uses the deployed API URL —
// matching the React app's VITE_API_BASE_URL .env setup.
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(),
    provideRouter(routes),
    { provide: SALES_CRM_API_BASE_URL, useValue: environment.webApiBaseUrl },
    provideLucideIcons(
      LucideBadgeDollarSign,
      LucideBot,
      LucideBriefcaseBusiness,
      LucideBuilding2,
      LucideCalendarDays,
      LucideColumns3,
      LucideContact,
      LucideDatabase,
      LucideDollarSign,
      LucideFileText,
      LucideHandshake,
      LucideLayoutDashboard,
      LucideMapPinned,
      LucideMoon,
      LucideSun,
      LucideTrendingUp
    )
  ]
};
