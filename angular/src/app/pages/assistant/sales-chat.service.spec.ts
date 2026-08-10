import { AiRecommendation } from '../../shared/models';
import { buildSalesResponse } from './sales-chat.service';

const recommendations: AiRecommendation[] = [
  {
    id: '1',
    title: 'Schedule an executive review',
    reason: 'No executive meeting is booked',
    priority: 'High',
    supportingEvidence: 'Close date is within 14 days',
    relatedRecord: 'Contoso Expansion',
    amount: 250000,
    closeDate: '2026-07-31',
  },
];

describe('buildSalesResponse', () => {
  it('builds CRM-grounded risk summaries', () => {
    const response = buildSalesResponse(recommendations, 'Summarize high-priority commit risks');
    expect(response).toContain('Contoso Expansion');
    expect(response).toContain('Schedule an executive review');
  });

  it('routes follow-up prompts to an email response', () => {
    const response = buildSalesResponse(recommendations, 'Draft a follow-up email');
    expect(response).toContain('Follow-up email');
    expect(response).toContain('Subject: Aligning next steps');
  });

  it('routes planning prompts to a mutual action plan', () => {
    expect(buildSalesResponse(recommendations, 'Create a mutual action plan')).toContain(
      'Mutual action plan',
    );
  });
});
