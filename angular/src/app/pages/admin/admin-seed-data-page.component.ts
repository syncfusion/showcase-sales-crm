import { Component, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { getErrorMessage } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import { ErrorStateComponent } from '../../shared/state.component';

@Component({
  selector: 'app-admin-seed-data-page',
  imports: [ErrorStateComponent, PageHeaderComponent],
  template: `
    <section>
      <app-page-header
        title="Admin Seed Data"
        subtitle="Inspect and refresh deterministic demo data."
        icon="admin"
      />
      <div class="content-card admin-seed-card">
        <h2>Deterministic demo data</h2>
        <p>Refresh the CRM dataset only when you want to replace the current demo records.</p>
        <button type="button" class="primary-action" [disabled]="refreshing()" (click)="refresh()">
          {{ refreshing() ? 'Refreshing…' : 'Refresh seed data' }}
        </button>
        @if (status()) {
          <div class="mutation-status" role="status">{{ status() }}</div>
        }
      </div>
      @if (error()) {
        <app-error-state [message]="error()" />
      }
    </section>
  `,
})
export class AdminSeedDataPageComponent {
  private readonly api = inject(SalesCrmApiService);
  protected readonly refreshing = signal(false);
  protected readonly status = signal('');
  protected readonly error = signal('');

  protected async refresh() {
    this.refreshing.set(true);
    this.status.set('');
    this.error.set('');
    try {
      const result = await firstValueFrom(this.api.refreshSeedData());
      this.status.set(result.status || 'Seed data refreshed successfully.');
    } catch (error) {
      this.error.set(getErrorMessage(error, 'Unable to refresh seed data.'));
    } finally {
      this.refreshing.set(false);
    }
  }
}
