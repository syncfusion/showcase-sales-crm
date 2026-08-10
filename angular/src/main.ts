import { bootstrapApplication } from '@angular/platform-browser';
import { registerLicense } from '@syncfusion/ej2-base';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { environment } from './environments/environment';

// Register the Syncfusion license key before Angular bootstraps. The key lives in
// the environment files (src/environments/*.ts) and is swapped per build
// config via `fileReplacements` in angular.json.
registerLicense(environment.syncfusionLicenseKey);

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
