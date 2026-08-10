import { salesCrmApi } from '../api/salesCrmApi';
import { formatUsd } from '../utils/formatters';

/**
 * Sales CRM AI chat responder abstraction.
 *
 * The application ships with a fully working LOCAL rule-based responder (LocalSalesChatClient)
 * that needs no API key — perfect for a showcase / demo environment. The same `generateResponse`
 * interface is what a real production integration would satisfy, so swapping the implementation
 * is a 1-line change documented at the bottom of this file.
 *
 * To switch to a real LLM (OpenAI, Azure OpenAI, Azure AI Foundry, Ollama, Anthropic, …):
 *
 *   1. Add the relevant SDK to `package.json`, e.g. `npm install openai` (official OpenAI SDK,
 *      also works for Azure OpenAI / Azure AI Foundry via `OpenAIClient` with a custom base URL).
 *   2. Set your API key in a Vite env file (`.env.local`):
 *          VITE_OPENAI_API_KEY=sk-...
 *          VITE_OPENAI_ENDPOINT=https://api.openai.com/v1     (or https://your-azure.openai.azure.com)
 *          VITE_OPENAI_MODEL=gpt-4o-mini
 *   3. Replace the `LocalSalesChatClient` instantiation below with the `OpenAiChatClient`
 *      example class (also included in this file, commented out), or wire the same key on the
 *      server side of your own API proxy.
 *
 * IMPORTANT: For real production apps the recommended pattern is to keep the API key on the
 * server (a .NET / Node proxy that swaps "user key" for "real key") and have the SPA call your
 * own endpoint. The example below shows the direct-from-SLA pattern purely to illustrate the
 * swap — do NOT ship a real key in client-side code in production.
 */

export interface SalesChatClient {
  /** Generate an assistant response for the given prompt, using the CRM recommendation context. */
  generateResponse(prompt: string): Promise<string>;
}

/**
 * The default rule-based responder. Builds a context-aware text response from the live CRM
 * recommendation data via the shared `buildDealRiskResponse` helper. No API key required —
 * ideal for a self-contained showcase.
 */
export class LocalSalesChatClient implements SalesChatClient {
  async generateResponse(prompt: string): Promise<string> {
    const recommendations = await salesCrmApi.getAiRecommendations();
    return buildDealRiskResponse(recommendations, prompt);
  }
}

/** Singleton used by the AI Assistant page. Swap this for `new OpenAiChatClient()` to go live. */
export const salesChat: SalesChatClient = new LocalSalesChatClient();

/* ===========================================================================
 * REAL-AI INTEGRATION EXAMPLE (uncomment to enable)
 *
 * Install:  npm install openai
 *
 * .env.local (Vite exposes a key only to client bundles — visible to the browser):
 *   VITE_OPENAI_API_KEY=sk-...
 *   VITE_OPENAI_ENDPOINT=https://api.openai.com/v1
 *   VITE_OPENAI_MODEL=gpt-4o-mini
 *
 * export class OpenAiChatClient implements SalesChatClient {
 *   private readonly client: OpenAI;
 *   private readonly model: string;
 *
 *   constructor() {
 *     if (!import.meta.env.VITE_OPENAI_API_KEY) {
 *       throw new Error('VITE_OPENAI_API_KEY is not set. Add it to .env.local.');
 *     }
 *     this.client = new OpenAI({
 *       apiKey: import.meta.env.VITE_OPENAI_API_KEY,
 *       baseURL: import.meta.env.VITE_OPENAI_ENDPOINT
 *     });
 *     this.model = import.meta.env.VITE_OPENAI_MODEL ?? 'gpt-4o-mini';
 *   }
 *
 *   async generateResponse(prompt: string): Promise<string> {
 *     // Pull live CRM data so the AI answers based on actual opportunities, not hallucinations.
 *     const recommendations = await salesCrmApi.getAiRecommendations();
 *     const crmContext = recommendations
 *       .map(r => `- ${r.relatedRecord} (${r.priority}, ${formatUsd(r.amount)}, closes ${r.closeDate}). Signal: ${r.reason}.`)
 *       .join('\n');
 *
 *     const completion = await this.client.chat.completions.create({
 *       model: this.model,
 *       messages: [
 *         { role: 'system', content:
 *           'You are the Sales CRM Pipeline Intelligence assistant. Use the live recommendation context to answer concisely. Context:\n' + crmContext
 *         },
 *         { role: 'user', content: prompt }
 *       ]
 *     });
 *     return completion.choices[0]?.message?.content ?? 'No response.';
 *   }
 * }
 *
 * // Swap the export above:
 * //   export const salesChat: SalesChatClient = import.meta.env.VITE_OPENAI_API_KEY
 * //     ? new OpenAiChatClient()
 * //     : new LocalSalesChatClient();
 * ========================================================================= */

function buildDealRiskResponse(
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

  if (candidates.length === 0) {
    return 'No AI recommendations are currently available. Refresh the data via Admin → Seed Data and try again.';
  }

  const total = candidates.reduce((sum, item) => sum + item.amount, 0);

  if (lower.includes('email')) {
    const item = candidates[0];
    return `Subject: Aligning next steps for ${item.relatedRecord}\n\nHi team,\n\nI wanted to follow up on ${item.relatedRecord}. Based on the current close date of ${item.closeDate}, the highest-value next step is to ${item.title.toLowerCase()}.\n\nThe main signal I am watching is: ${item.reason}\n\nCould we confirm the owner for the next decision milestone and align on the mutual action plan this week?\n\nBest,\nSales Team`;
  }

  if (lower.includes('mutual action')) {
    const item = candidates[0];
    return `Mutual Action Plan for ${item.relatedRecord}\n\n1. Confirm business case and success metrics.\n2. Assign security, legal, and procurement owners.\n3. Review implementation milestones and target go-live.\n4. Finalize commercial terms before ${item.closeDate}.\n5. Executive approval and signature.\n\nRecommended action: ${item.title}.`;
  }

  const lines = candidates.map((item, index) => {
    return `${index + 1}. ${item.relatedRecord} (${item.priority}, ${formatUsd(item.amount)}, closes ${item.closeDate})\n   - Signal: ${item.reason}\n   - Evidence: ${item.supportingEvidence}\n   - Next best action: ${item.title}`;
  });

  return `Pipeline risk summary for ${candidates.length} deal${candidates.length === 1 ? '' : 's'} totaling ${formatUsd(total)}:\n\n${lines.join('\n\n')}\n\nRecommendation: prioritize the highest-value High priority deal first, then update the forecast category after the next confirmed buyer action.`;
}
