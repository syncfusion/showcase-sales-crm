import { Injectable, InjectionToken, inject } from '@angular/core';
import { AiRecommendation } from '../../shared/models';
import { formatUsd } from '../../shared/formatters';

export interface SalesChatClient {
  readonly name: string;
  generateResponse(
    prompt: string,
    recommendations: AiRecommendation[],
    signal?: AbortSignal,
  ): Promise<string>;
}

@Injectable({ providedIn: 'root' })
export class LocalSalesChatClient implements SalesChatClient {
  readonly name = 'Local CRM intelligence';

  async generateResponse(
    prompt: string,
    recommendations: AiRecommendation[],
    signal?: AbortSignal,
  ) {
    await wait(250, signal);
    return buildSalesResponse(recommendations, prompt);
  }
}

export const SALES_CHAT_CLIENT = new InjectionToken<SalesChatClient>('SALES_CHAT_CLIENT', {
  providedIn: 'root',
  factory: () => inject(LocalSalesChatClient),
});

@Injectable({ providedIn: 'root' })
export class SalesAssistantService {
  private readonly client = inject(SALES_CHAT_CLIENT);
  readonly providerName = this.client.name;

  respond(prompt: string, recommendations: AiRecommendation[], signal?: AbortSignal) {
    const safePrompt = prompt
      .trim()
      .replace(/<[^>]*>/g, '')
      .slice(0, 2000);
    if (!safePrompt)
      return Promise.resolve('Please enter a question about pipeline risk or next actions.');
    return this.client.generateResponse(safePrompt, recommendations, signal);
  }
}

export function buildSalesResponse(items: AiRecommendation[], prompt: string) {
  const lower = prompt.toLowerCase();
  const highPriority = items.filter((item) => item.priority.toLowerCase() === 'high');
  const source = lower.includes('high') || lower.includes('commit') ? highPriority : items;
  const candidates = source.slice(0, lower.includes('which') ? 5 : 3);

  if (!candidates.length) return 'No matching CRM recommendations are currently available.';

  const item = candidates[0];
  if (lower.includes('email') || lower.includes('follow-up')) {
    return `## Follow-up email\n\n**Subject: Aligning next steps for ${item.relatedRecord}**\n\nHi team,\n\nI wanted to follow up on **${item.relatedRecord}**. With the current close date of ${item.closeDate}, the highest-value next step is to ${item.title.toLowerCase()}.\n\nThe main signal I am watching is: ${item.reason}\n\nCould we confirm the owner for the next decision milestone and align on the mutual action plan this week?\n\nBest,  \nSales Team`;
  }

  if (lower.includes('mutual action') || lower.includes('plan')) {
    return `## Mutual action plan — ${item.relatedRecord}\n\n1. Confirm the business case and success metrics.\n2. Assign security, legal, and procurement owners.\n3. Review implementation milestones and target go-live.\n4. Finalize commercial terms before ${item.closeDate}.\n5. Obtain executive approval and signature.\n\n**Recommended action:** ${item.title}.`;
  }

  const total = candidates.reduce((sum, candidate) => sum + candidate.amount, 0);
  const lines = candidates.map(
    (candidate, index) =>
      `${index + 1}. **${candidate.relatedRecord}** — ${candidate.priority}, ${formatUsd(candidate.amount)}, closes ${candidate.closeDate}\n   - Signal: ${candidate.reason}\n   - Evidence: ${candidate.supportingEvidence}\n   - Next action: ${candidate.title}`,
  );
  return `## Pipeline risk summary\n\n${candidates.length} deal${candidates.length === 1 ? '' : 's'} totaling **${formatUsd(total)}**:\n\n${lines.join('\n\n')}\n\nPrioritize the highest-value high-priority deal, then update its forecast category after the next confirmed buyer action.`;
}

function wait(milliseconds: number, signal?: AbortSignal) {
  return new Promise<void>((resolve, reject) => {
    if (signal?.aborted) return reject(new DOMException('Aborted', 'AbortError'));
    const timeout = window.setTimeout(resolve, milliseconds);
    signal?.addEventListener(
      'abort',
      () => {
        window.clearTimeout(timeout);
        reject(new DOMException('Aborted', 'AbortError'));
      },
      { once: true },
    );
  });
}
