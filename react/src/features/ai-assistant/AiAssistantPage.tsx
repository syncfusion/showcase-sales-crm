import { useRef } from 'react';
import { AIAssistViewComponent } from '@syncfusion/ej2-react-interactive-chat';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { salesChat, LocalSalesChatClient } from '../../shared/services/chatClient';
import { AppIcon } from '../../shared/components/AppIcon';
import { formatUsd } from '../../shared/utils/formatters';

export function AiAssistantPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getAiRecommendations(), []);
  const assistRef = useRef<any>(null);
  // Taller assist view on QHD / 4K so more conversation/recommendations fit on screen.
  const assistHeight = useResponsiveHeight('650px', '760px', '840px');

  // Surface which chat client is active so end users know exactly where to swap for a real AI key.
  // (See src/shared/services/chatClient.ts for the one-line switch + commented OpenAI example.)
  const usingLocal = salesChat instanceof LocalSalesChatClient;

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No assistant recommendations yet." />;

  const topRisks = data.filter((item) => item.priority === 'High').slice(0, 6);
  const top = topRisks[0] ?? data[0];

  // Pre-seed prompts when running the local responder (deterministic from CRM data). When a real
  // AI provider is configured, leave the seeded responses blank so the user experiences the live
  // provider by pressing Enter — see src/shared/services/chatClient.ts.
  const seedResponse = (prompt: string, subset = data): string =>
    usingLocal ? buildLocalResponse(subset, prompt) : '';

  const initialPrompts = [
    { prompt: 'Summarize risks for commit deals', response: seedResponse('Summarize risks for commit deals') },
    { prompt: `Create a follow-up plan for ${top.relatedRecord}`, response: seedResponse(`Create a follow-up plan for ${top.relatedRecord}`, [top]) }
  ];

  const suggestions = [
    'Summarize risks for commit deals',
    'Which deals may slip this quarter?',
    'Create a follow-up email for the champion',
    'Generate a mutual action plan',
    'Explain why this opportunity is high risk'
  ];

  const handlePromptRequest = async (args: { prompt?: string }) => {
    const prompt = args.prompt ?? '';
    try {
      const response = await salesChat.generateResponse(prompt);
      setTimeout(() => assistRef.current?.addPromptResponse(response), 100);
    } catch (ex) {
      setTimeout(
        () => assistRef.current?.addPromptResponse(`Unable to generate a response: ${(ex as Error).message}`),
        100
      );
    }
  };

  return (
    <section>
      <PageHeader title="AI Sales Assistant" subtitle="Guided recommendations and action suggestions." icon={<AppIcon name="assistant" />} />

      <div className="content-card" style={{ marginBottom: 16, padding: '12px 16px', fontSize: 13, color: 'var(--text-muted)' }}>
        <strong>Responder:</strong> {usingLocal ? 'Local rule-based (no API key)' : 'Live AI'}
        {' — '}
        Swap to a real LLM in <code>src/shared/services/chatClient.ts</code> (commented OpenAI example included).
      </div>

      <div className="ai-assistant-layout">
        <aside className="content-card ai-risk-list">
          <h2>Risk Queue</h2>
          {topRisks.map((item) => (
            <button key={item.id} type="button" className="risk-item" onClick={() => assistRef.current?.executePrompt?.(`Explain why ${item.relatedRecord} is high risk`)}>
              <strong>{item.relatedRecord}</strong>
              <span>{item.title}</span>
              <small>{item.priority} · {formatUsd(item.amount)} · closes {item.closeDate}</small>
            </button>
          ))}
        </aside>
        <div className="content-card ai-assist-panel">
          <div className="w-full overflow-hidden" style={{ height: assistHeight }}>
            <AIAssistViewComponent
              ref={assistRef}
              height={assistHeight}
              promptSuggestions={suggestions}
              promptSuggestionsHeader="Suggested sales prompts"
              promptPlaceholder="Ask about deal risk, forecast movement, or next best action"
              prompts={initialPrompts}
              promptRequest={handlePromptRequest}
              showClearButton={true}
            />
          </div>
        </div>
      </div>
    </section>
  );
}

// Local copy of the rule-based builder so the AI Assistant page stays self-contained when the
// chatClient.ts module is configured to use a real AI provider (in which case its own
// buildDealRiskResponse is private). Kept identical to the LocalSalesChatClient implementation.
function buildLocalResponse(
  items: Array<{
    title: string;
    reason: string;
    supportingEvidence: string;
    relatedRecord: string;
    amount: number;
    closeDate: string;
    priority: string;
  }>,
  prompt: string
) {
  const lower = prompt.toLowerCase();
  const take = lower.includes('which') ? 5 : 3;
  const candidates = items.slice(0, take);
  if (!candidates.length) return 'No AI recommendations are currently available.';
  const total = candidates.reduce((s, i) => s + i.amount, 0);
  const lines = candidates.map((item, index) =>
    `${index + 1}. ${item.relatedRecord} (${item.priority}, ${formatUsd(item.amount)}, closes ${item.closeDate})\n   - Signal: ${item.reason}\n   - Evidence: ${item.supportingEvidence}\n   - Next best action: ${item.title}`
  );
  if (lower.includes('email')) {
    const item = candidates[0];
    return `Subject: Aligning next steps for ${item.relatedRecord}\n\nHi team,\n\nI wanted to follow up on ${item.relatedRecord}. Based on the current close date of ${item.closeDate}, the highest-value next step is to ${item.title.toLowerCase()}.\n\nThe main signal I am watching is: ${item.reason}\n\nCould we confirm the owner for the next decision milestone and align on the mutual action plan this week?\n\nBest,\nSales Team`;
  }
  if (lower.includes('mutual action')) {
    const item = candidates[0];
    return `Mutual Action Plan for ${item.relatedRecord}\n\n1. Confirm business case and success metrics.\n2. Assign security, legal, and procurement owners.\n3. Review implementation milestones and target go-live.\n4. Finalize commercial terms before ${item.closeDate}.\n5. Executive approval and signature.\n\nRecommended action: ${item.title}.`;
  }
  return `Pipeline risk summary for ${candidates.length} deal${candidates.length === 1 ? '' : 's'} totaling ${formatUsd(total)}:\n\n${lines.join('\n\n')}\n\nRecommendation: prioritize the highest-value High priority deal first, then update the forecast category after the next confirmed buyer action.`;
}
