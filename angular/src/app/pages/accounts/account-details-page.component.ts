import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { distinctUntilChanged, map, switchMap } from 'rxjs';
import { formatUsd, metricCss, statusCss } from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-account-details-page',
  imports: [
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  template: `
    <section>
      <app-page-header
        title="Account Details"
        subtitle="Review account health, contacts, and pipeline context."
        icon="accounts"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data; as account) {
          <div class="account-details-grid">
            <div class="content-card">
              <h2>{{ account.accountName }}</h2>
              <dl class="account-meta">
                <div>
                  <dt>Industry</dt>
                  <dd>{{ account.industry }}</dd>
                </div>
                <div>
                  <dt>Region</dt>
                  <dd>{{ account.region }}</dd>
                </div>
                <div>
                  <dt>Status</dt>
                  <dd>
                    <span [class]="statusClass(account.status)">{{ account.status }}</span>
                  </dd>
                </div>
                <div>
                  <dt>Owner</dt>
                  <dd>{{ account.owner }}</dd>
                </div>
                <div>
                  <dt>Revenue</dt>
                  <dd>{{ usd(account.revenue) }}</dd>
                </div>
                <div>
                  <dt>Health Score</dt>
                  <dd>
                    <span [class]="scoreClass(account.healthScore)">{{ account.healthScore }}</span>
                  </dd>
                </div>
              </dl>
            </div>
            <div class="content-card">
              <h2>Pipeline Context</h2>
              <p class="text-muted">
                Related opportunities and recent activities will appear here.
              </p>
            </div>
          </div>
        } @else {
          <app-empty-state message="Account not found." />
        }
      }
    </section>
  `,
})
export class AccountDetailsPageComponent {
  private readonly api = inject(SalesCrmApiService);
  private readonly route = inject(ActivatedRoute);
  readonly state$ = toLoadState(
    this.route.paramMap.pipe(
      map((params) => params.get('accountId') ?? ''),
      distinctUntilChanged(),
      switchMap((accountId) =>
        this.api
          .getAccounts()
          .pipe(
            map(
              (accounts) =>
                accounts.find((account) => account.id.toLowerCase() === accountId.toLowerCase()) ??
                null,
            ),
          ),
      ),
    ),
  );
  protected readonly usd = formatUsd;
  protected readonly statusClass = statusCss;
  protected readonly scoreClass = metricCss;
}
