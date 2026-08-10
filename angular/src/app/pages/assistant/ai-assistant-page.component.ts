import { AsyncPipe } from '@angular/common';
import { Component, OnDestroy, ViewChild, inject } from '@angular/core';
import {
  AIAssistViewComponent,
  AIAssistViewModule,
} from '@syncfusion/ej2-angular-interactive-chat';
import type {
  PromptRequestEventArgs,
  StopRespondingEventArgs,
} from '@syncfusion/ej2-interactive-chat';
import { formatUsd } from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { AiRecommendation } from '../../shared/models';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';
import { SalesAssistantService, buildSalesResponse } from './sales-chat.service';

@Component({
  selector: 'app-ai-assistant-page',
  imports: [
    AIAssistViewModule,
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  template: `
    <section class="fit-page assistant-page">
      <app-page-header
        title="AI Sales Assistant"
        subtitle="Guided recommendations and action suggestions."
        icon="assistant"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.length) {
          <div class="assistant-provider content-card">
            <span><strong>Responder:</strong> {{ assistant.providerName }}</span>
            <small
              >Responses use current CRM recommendations and can be replaced with a server-side AI
              provider.</small
            >
          </div>
          <div class="ai-assistant-layout">
            <aside class="content-card ai-risk-list">
              <h2>Risk Queue</h2>
              @for (item of topRisks(state.data); track item.id) {
                <button type="button" class="risk-item" (click)="askAbout(item)">
                  <strong>{{ item.relatedRecord }}</strong>
                  <span>{{ item.title }}</span>
                  <small
                    >{{ item.priority }} · {{ usd(item.amount) }} · closes
                    {{ item.closeDate }}</small
                  >
                </button>
              }
            </aside>
            <div
              class="content-card ai-assist-panel"
              role="application"
              aria-label="Sales CRM AI assistant"
            >
              <ejs-aiassistview
                #assist
                id="salesCrmAiAssistView"
                width="100%"
                height="100%"
                cssClass="sales-ai-assist"
                [promptSuggestions]="suggestions"
                promptSuggestionsHeader="Suggested sales prompts"
                promptPlaceholder="Ask about deal risk or next actions"
                [prompts]="initialPrompts(state.data)"
                [showClearButton]="true"
                [enableScrollToBottom]="true"
                [enableStreaming]="true"
                (promptRequest)="onPromptRequest($event, state.data)"
                (stopRespondingClick)="onStopResponding($event)"
              />
            </div>
          </div>
        } @else {
          <app-empty-state message="No assistant recommendations yet." />
        }
      }
    </section>
  `,
})
export class AiAssistantPageComponent implements OnDestroy {
  private readonly api = inject(SalesCrmApiService);
  protected readonly assistant = inject(SalesAssistantService);
  @ViewChild('assist') private assist?: AIAssistViewComponent;
  readonly state$ = toLoadState(this.api.getAiRecommendations());
  private activeRequest?: AbortController;
  protected readonly usd = formatUsd;
  protected readonly suggestions = [
    'Summarize high-priority commit risks',
    'Which deals may slip this quarter?',
    'Create a follow-up email for the champion',
    'Generate a mutual action plan',
    'Explain why this opportunity is high risk',
  ];

  protected topRisks(items: AiRecommendation[]) {
    const risks = items.filter((item) => item.priority.toLowerCase() === 'high').slice(0, 6);
    return risks.length ? risks : items.slice(0, 6);
  }

  protected initialPrompts(items: AiRecommendation[]) {
    const top = this.topRisks(items)[0];
    return top
      ? [
          {
            prompt: 'Summarize high-priority commit risks',
            response: buildSalesResponse(items, 'Summarize high-priority commit risks'),
          },
          {
            prompt: `Create a follow-up plan for ${top.relatedRecord}`,
            response: buildSalesResponse([top], 'Create a follow-up plan'),
          },
        ]
      : [];
  }

  protected askAbout(item: AiRecommendation) {
    this.assist?.executePrompt(
      `Explain why ${item.relatedRecord} is high risk and recommend the next action`,
    );
  }

  protected async onPromptRequest(args: PromptRequestEventArgs, items: AiRecommendation[]) {
    const prompt = args.prompt?.trim() ?? '';
    if (!prompt) {
      args.cancel = true;
      return;
    }

    this.activeRequest?.abort();
    const request = new AbortController();
    this.activeRequest = request;
    try {
      const response = await this.assistant.respond(prompt, items, request.signal);
      if (!request.signal.aborted) this.assist?.addPromptResponse(response, true);
    } catch (error) {
      if (!request.signal.aborted) {
        this.assist?.addPromptResponse('Unable to generate a response. Please try again.', true);
      }
    } finally {
      if (this.activeRequest === request) this.activeRequest = undefined;
    }
  }

  protected onStopResponding(_args: StopRespondingEventArgs) {
    this.activeRequest?.abort();
    this.activeRequest = undefined;
  }

  ngOnDestroy() {
    this.activeRequest?.abort();
  }
}
