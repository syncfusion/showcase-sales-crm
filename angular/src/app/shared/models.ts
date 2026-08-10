export type DashboardSummary = {
  revenue: number;
  pipelineValue: number;
  winRate: number;
  weightedForecast: number;
  recentOpportunities: Array<{
    id: string;
    name: string;
    account: string;
    stage: string;
    amount: number;
    expectedCloseDate: string;
  }>;
  stageBreakdown: Array<{ stage: string; value: number; count: number }>;
};

export type Account = {
  id: string;
  accountName: string;
  industry: string;
  region: string;
  status: string;
  owner: string;
  revenue: number;
  healthScore: number;
};

export type Contact = {
  id: string;
  name: string;
  account: string;
  role: string;
  email: string;
  phone?: string;
};

export type Opportunity = {
  id: string;
  opportunityName: string;
  account: string;
  stage: string;
  amount: number;
  probability: number;
  expectedCloseDate: string;
  owner: string;
};

export type PipelineCard = {
  id: string;
  title: string;
  stage: string;
  account: string;
  amount: number;
  forecastCategory: string;
  owner: string;
  expectedCloseDate: string;
  probability: number;
  priority: string;
  stageTotal: number;
  stageCount: number;
  wonDate?: string | null;
  lostDate?: string | null;
  lossReason?: string | null;
  salesCycleDays?: number | null;
  content?: string;
};

export type ForecastSummary = {
  commit: number;
  bestCase: number;
  pipeline: number;
  weighted: number;
  trend: Array<{
    period: string;
    commit: number;
    bestCase: number;
    pipeline: number;
    weighted: number;
  }>;
};

export type Activity = {
  id: string;
  subject: string;
  account: string;
  start: string;
  end: string;
  status: string;
  owner: string;
};

export type TerritorySummary = {
  id: string;
  name: string;
  region: string;
  revenue: number;
  accounts: number;
  winRate: number;
  latitude: number;
  longitude: number;
};

export type DocumentTemplate = {
  id: string;
  title: string;
  type: string;
  status: string;
  owner: string;
  account: string;
  opportunity: string;
  templateKey: string;
};

export type AiRecommendation = {
  id: string;
  title: string;
  reason: string;
  priority: string;
  supportingEvidence: string;
  relatedRecord: string;
  amount: number;
  closeDate: string;
};

export type SeedRefreshResponse = {
  status: string;
};

export type UpdateOpportunityStageRequest = {
  stage: string;
};
