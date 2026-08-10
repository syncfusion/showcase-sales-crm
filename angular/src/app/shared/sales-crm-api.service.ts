import { HttpClient } from '@angular/common/http';
import { Injectable, InjectionToken, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import {
  Account,
  Activity,
  AiRecommendation,
  Contact,
  DashboardSummary,
  DocumentTemplate,
  ForecastSummary,
  Opportunity,
  PipelineCard,
  SeedRefreshResponse,
  TerritorySummary,
} from './models';

type ArrayOrItemsResponse<T> = T[] | { items?: T[] };
type AnyRecord = Record<string, unknown>;

export const SALES_CRM_API_BASE_URL = new InjectionToken<string>('SALES_CRM_API_BASE_URL', {
  providedIn: 'root',
  factory: () => '',
});
const toNumber = (value: unknown, fallback = 0) => (typeof value === 'number' ? value : fallback);
const toString = (value: unknown, fallback = '') => (typeof value === 'string' ? value : fallback);
function getItems<T>(payload: ArrayOrItemsResponse<T>): T[] {
  return Array.isArray(payload) ? payload : (payload.items ?? []);
}

@Injectable({ providedIn: 'root' })
export class SalesCrmApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(SALES_CRM_API_BASE_URL).replace(/\/$/, '');

  private endpoint(path: string) {
    return `${this.baseUrl}${path}`;
  }

  getDashboard(): Observable<DashboardSummary> {
    return this.http
      .get<{ summary?: Partial<DashboardSummary> }>(this.endpoint('/api/dashboard'))
      .pipe(
        map((payload) => {
          const summary = payload.summary ?? {};
          return {
            revenue: toNumber(summary.revenue),
            pipelineValue: toNumber(summary.pipelineValue),
            winRate: toNumber(summary.winRate),
            weightedForecast: toNumber(summary.weightedForecast),
            recentOpportunities: summary.recentOpportunities ?? [],
            stageBreakdown: summary.stageBreakdown ?? [],
          };
        }),
      );
  }

  getAccounts(): Observable<Account[]> {
    return this.http.get<ArrayOrItemsResponse<AnyRecord>>(this.endpoint('/api/accounts')).pipe(
      map((payload) =>
        getItems(payload).map((item, index) => ({
          id: toString(item['id'], `account-${index + 1}`),
          accountName: toString(item['accountName'] ?? item['name'], 'Unknown'),
          industry: toString(item['industry'], 'Unknown'),
          region: toString(item['region'], 'Unknown'),
          status: toString(item['status'], 'Unknown'),
          owner: toString(item['owner'], 'Unassigned'),
          revenue: toNumber(item['revenue']),
          healthScore: toNumber(item['healthScore']),
        })),
      ),
    );
  }

  getContacts(): Observable<Contact[]> {
    return this.http
      .get<ArrayOrItemsResponse<Contact>>(this.endpoint('/api/contacts'))
      .pipe(map(getItems));
  }

  getOpportunities(): Observable<Opportunity[]> {
    return this.http
      .get<ArrayOrItemsResponse<Opportunity>>(this.endpoint('/api/opportunities'))
      .pipe(map(getItems));
  }

  getPipeline(): Observable<PipelineCard[]> {
    return this.http
      .get<ArrayOrItemsResponse<PipelineCard>>(this.endpoint('/api/pipeline/stages'))
      .pipe(
        map((payload) =>
          getItems(payload).map((item) => ({
            ...item,
            amount: toNumber(item.amount),
            probability: toNumber(item.probability),
            stageTotal: toNumber(item.stageTotal),
            stageCount: toNumber(item.stageCount),
            forecastCategory: toString(item.forecastCategory),
            expectedCloseDate: toString(item.expectedCloseDate),
            priority: toString(item.priority, 'Medium'),
            wonDate: toString(item.wonDate),
            lostDate: toString(item.lostDate),
            lossReason: toString(item.lossReason),
            salesCycleDays:
              item.salesCycleDays === null || item.salesCycleDays === undefined
                ? null
                : toNumber(item.salesCycleDays),
            content: item.account,
          })),
        ),
      );
  }

  getForecast(): Observable<ForecastSummary> {
    return this.http.get<Partial<ForecastSummary>>(this.endpoint('/api/forecast')).pipe(
      map((payload) => ({
        commit: toNumber(payload.commit),
        bestCase: toNumber(payload.bestCase),
        pipeline: toNumber(payload.pipeline),
        weighted: toNumber(payload.weighted),
        trend: payload.trend ?? [],
      })),
    );
  }

  getActivities(): Observable<Activity[]> {
    return this.http.get<ArrayOrItemsResponse<AnyRecord>>(this.endpoint('/api/activities')).pipe(
      map((payload) =>
        getItems(payload).map((item) => ({
          id: toString(item['id']),
          subject: toString(item['subject']),
          account: toString(item['account']),
          start: toString(item['startTime'] ?? item['start']),
          end: toString(item['endTime'] ?? item['end']),
          status: toString(item['status']),
          owner: toString(item['owner']),
        })),
      ),
    );
  }

  getTerritories(): Observable<TerritorySummary[]> {
    return this.http.get<ArrayOrItemsResponse<AnyRecord>>(this.endpoint('/api/territories')).pipe(
      map((payload) =>
        getItems(payload).map((item) => ({
          id: toString(item['id']),
          name: toString(item['name'] ?? item['territory']),
          region: toString(item['region']),
          revenue: toNumber(item['revenue'] ?? item['value']),
          accounts: toNumber(item['accounts']),
          winRate: toNumber(item['winRate'] ?? item['coverage']),
          latitude: toNumber(item['latitude']),
          longitude: toNumber(item['longitude']),
        })),
      ),
    );
  }

  getDocuments(): Observable<DocumentTemplate[]> {
    return this.http.get<ArrayOrItemsResponse<AnyRecord>>(this.endpoint('/api/documents')).pipe(
      map((payload) =>
        getItems(payload).map((item) => ({
          id: toString(item['id']),
          title: toString(item['title']),
          type: toString(item['type']),
          status: toString(item['status']),
          owner: toString(item['owner']),
          account: toString(item['account']),
          opportunity: toString(item['opportunity']),
          templateKey: toString(item['templateKey']),
        })),
      ),
    );
  }

  getAiRecommendations(): Observable<AiRecommendation[]> {
    return this.http
      .get<ArrayOrItemsResponse<AnyRecord>>(this.endpoint('/api/ai/recommendations'))
      .pipe(
        map((payload) =>
          getItems(payload).map((item) => ({
            id: toString(item['id']),
            title: toString(item['title']),
            reason: toString(item['reason']),
            priority: toString(item['priority']),
            supportingEvidence: toString(item['supportingEvidence']),
            relatedRecord: toString(item['relatedRecord']),
            amount: toNumber(item['amount']),
            closeDate: toString(item['closeDate']),
          })),
        ),
      );
  }

  refreshSeedData(): Observable<SeedRefreshResponse> {
    return this.http.post<SeedRefreshResponse>(this.endpoint('/api/admin/seed/refresh'), {});
  }

  updateOpportunityStage(opportunityId: string, stage: string): Observable<void> {
    return this.http.put<void>(
      this.endpoint(`/api/opportunities/${encodeURIComponent(opportunityId)}/stage`),
      { stage },
    );
  }
}
