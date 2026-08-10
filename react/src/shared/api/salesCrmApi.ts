import { httpGet, httpPost } from './httpClient';
import type {
  Account,
  Activity,
  AiRecommendation,
  DashboardSummary,
  DocumentTemplate,
  ForecastSummary,
  PipelineCard,
  Contact,
  Opportunity,
  TerritorySummary
} from '../models';

type AccountsResponse = {
  items?: Array<{
    id?: string;
    accountName?: string;
    name?: string;
    industry?: string;
    region?: string;
    status?: string;
    owner?: string;
    revenue?: number;
    healthScore?: number;
  }>;
};

type AccountPayload = {
  id?: string;
  accountName?: string;
  name?: string;
  industry?: string;
  region?: string;
  status?: string;
  owner?: string;
  revenue?: number;
  healthScore?: number;
};

type ListResponse<T> = {
  items?: T[];
};

type ArrayOrItemsResponse<T> = T[] | { items?: T[] };

const toNumber = (value: unknown, fallback = 0) => (typeof value === 'number' ? value : fallback);
const toString = (value: unknown, fallback = '') => (typeof value === 'string' ? value : fallback);

const getItems = <T>(payload: ArrayOrItemsResponse<T>): T[] => {
  const items = Array.isArray(payload) ? payload : payload.items ?? [];
  return items;
};

export const salesCrmApi = {
  async getDashboard(): Promise<DashboardSummary> {
    const payload = await httpGet<{ summary?: Partial<DashboardSummary> }>('/api/dashboard');
    const summary = payload.summary ?? {};
    return {
      revenue: toNumber(summary.revenue),
      pipelineValue: toNumber(summary.pipelineValue),
      winRate: toNumber(summary.winRate),
      weightedForecast: toNumber(summary.weightedForecast),
      recentOpportunities: summary.recentOpportunities ?? [],
      stageBreakdown: summary.stageBreakdown ?? []
    };
  },
  async getAccounts(): Promise<Account[]> {
    const payload = await httpGet<ArrayOrItemsResponse<AccountPayload>>('/api/accounts');
    const items = getItems(payload);
    return items.map((item, index) => ({
      id: item.id ?? `account-${index + 1}`,
      accountName: item.accountName ?? item.name ?? 'Unknown',
      industry: item.industry ?? 'Unknown',
      region: item.region ?? 'Unknown',
      status: item.status ?? 'Unknown',
      owner: item.owner ?? 'Unassigned',
      revenue: toNumber(item.revenue),
      healthScore: toNumber(item.healthScore)
    }));
  },
  async getContacts(): Promise<Contact[]> {
    const payload = await httpGet<ArrayOrItemsResponse<Contact>>('/api/contacts');
    return getItems(payload);
  },
  async getOpportunities(): Promise<Opportunity[]> {
    const payload = await httpGet<ArrayOrItemsResponse<Opportunity>>('/api/opportunities');
    return getItems(payload);
  },
  async getPipeline(): Promise<PipelineCard[]> {
    const payload = await httpGet<ArrayOrItemsResponse<PipelineCard>>('/api/pipeline/stages');
    return getItems(payload).map((item) => ({
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
      salesCycleDays: item.salesCycleDays === null || item.salesCycleDays === undefined ? null : toNumber(item.salesCycleDays),
      content: item.account
    }));
  },
  async getForecast(): Promise<ForecastSummary> {
    const payload = await httpGet<Partial<ForecastSummary>>('/api/forecast');
    return {
      commit: toNumber(payload.commit),
      bestCase: toNumber(payload.bestCase),
      pipeline: toNumber(payload.pipeline),
      weighted: toNumber(payload.weighted),
      trend: payload.trend ?? []
    };
  },
  async getActivities(): Promise<Activity[]> {
    const payload = await httpGet<ArrayOrItemsResponse<any>>('/api/activities');
    return getItems(payload).map((item) => ({
      id: toString(item.id),
      subject: toString(item.subject),
      account: toString(item.account),
      start: toString(item.startTime ?? item.start),
      end: toString(item.endTime ?? item.end),
      status: toString(item.status),
      owner: toString(item.owner)
    }));
  },
  async getTerritories(): Promise<TerritorySummary[]> {
    const payload = await httpGet<ArrayOrItemsResponse<any>>('/api/territories');
    return getItems(payload).map((item) => ({
      id: toString(item.id),
      name: toString(item.name ?? item.territory),
      region: toString(item.region),
      revenue: toNumber(item.revenue ?? item.value),
      accounts: toNumber(item.accounts),
      winRate: toNumber(item.winRate ?? item.coverage),
      latitude: toNumber(item.latitude),
      longitude: toNumber(item.longitude)
    }));
  },
  async getDocuments(): Promise<DocumentTemplate[]> {
    const payload = await httpGet<ArrayOrItemsResponse<any>>('/api/documents');
    return getItems(payload).map((item) => ({
      id: toString(item.id),
      title: toString(item.title),
      type: toString(item.type),
      status: toString(item.status),
      owner: toString(item.owner),
      account: toString(item.account),
      opportunity: toString(item.opportunity),
      templateKey: toString(item.templateKey)
    }));
  },
  async getAiRecommendations(): Promise<AiRecommendation[]> {
    const payload = await httpGet<ArrayOrItemsResponse<any>>('/api/ai/recommendations');
    return getItems(payload).map((item) => ({
      id: toString(item.id),
      title: toString(item.title),
      reason: toString(item.reason),
      priority: toString(item.priority),
      supportingEvidence: toString(item.supportingEvidence),
      relatedRecord: toString(item.relatedRecord),
      amount: toNumber(item.amount),
      closeDate: toString(item.closeDate)
    }));
  },
  async refreshSeedData(): Promise<{ status: string }> {
    return httpPost<{ status: string }>('/api/admin/seed/refresh');
  }
};
