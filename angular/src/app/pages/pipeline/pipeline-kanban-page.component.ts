import { AsyncPipe } from '@angular/common';
import { Component, ViewChild, inject, signal } from '@angular/core';
import { KanbanComponent, KanbanModule } from '@syncfusion/ej2-angular-kanban';
import type { DragEventArgs } from '@syncfusion/ej2-kanban';
import { firstValueFrom } from 'rxjs';
import { formatDateUs, formatPercent, formatUsd } from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { PipelineCard } from '../../shared/models';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-pipeline-kanban-page',
  imports: [
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    KanbanModule,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  template: `
    <section class="pipeline-page">
      <app-page-header
        title="Pipeline Kanban"
        subtitle="Stage-based deal movement and prioritization."
        icon="pipeline"
      />
      @if (mutationMessage()) {
        <div class="mutation-status" role="status">{{ mutationMessage() }}</div>
      }
      @if (mutationError()) {
        <app-error-state [message]="mutationError()" />
      }
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.length) {
          <div class="content-card kanban-card">
            <div class="kanban-scroll">
              <ejs-kanban
                #kanban
                keyField="stage"
                height="720px"
                [allowDragAndDrop]="true"
                [dataSource]="state.data"
                [cardSettings]="cardSettings"
                (dragStart)="onDragStart($event)"
                (dragStop)="onDragStop($event)"
              >
                <e-columns>
                  @for (column of columns; track column.keyField) {
                    <e-column [headerText]="column.headerText" [keyField]="column.keyField" />
                  }
                </e-columns>
                <ng-template #cardSettingsTemplate let-data>
                  <div class="kanban-card-shell">
                    <div class="kanban-card-topline">
                      <strong>{{ data.title }}</strong>
                      @if (showForecastBadge(data)) {
                        <span class="forecast-chip">{{ data.forecastCategory }}</span>
                      }
                    </div>
                    <span class="kanban-account">{{ data.account }}</span>
                    <b class="kanban-amount">{{ usd(data.amount) }}</b>
                    @if (isTerminalStage(data)) {
                      <dl class="kanban-meta terminal-meta">
                        <div>
                          <dt>Owner</dt>
                          <dd>{{ data.owner }}</dd>
                        </div>
                        <div>
                          <dt>{{ terminalDateLabel(data) }}</dt>
                          <dd>{{ terminalDateValue(data) }}</dd>
                        </div>
                        @if (data.stage === 'ClosedWon' && data.salesCycleDays !== null) {
                          <div>
                            <dt>Cycle</dt>
                            <dd>{{ data.salesCycleDays }} days</dd>
                          </div>
                        }
                        @if (data.stage === 'ClosedLost' && data.lossReason) {
                          <div class="kanban-meta-wide">
                            <dt>Loss Reason</dt>
                            <dd>{{ data.lossReason }}</dd>
                          </div>
                        }
                      </dl>
                    } @else {
                      <dl class="kanban-meta">
                        <div>
                          <dt>Owner</dt>
                          <dd>{{ data.owner }}</dd>
                        </div>
                        <div>
                          <dt>Close</dt>
                          <dd>{{ dateUs(data.expectedCloseDate) }}</dd>
                        </div>
                        <div>
                          <dt>Prob.</dt>
                          <dd>{{ percent(data.probability) }}</dd>
                        </div>
                        <div>
                          <dt>Priority</dt>
                          <dd>
                            <span [class]="priorityClass(data.priority)">{{ data.priority }}</span>
                          </dd>
                        </div>
                      </dl>
                    }
                  </div>
                </ng-template>
              </ejs-kanban>
            </div>
          </div>
        } @else {
          <app-empty-state message="No pipeline cards available." />
        }
      }
    </section>
  `,
})
export class PipelineKanbanPageComponent {
  private readonly api = inject(SalesCrmApiService);
  @ViewChild('kanban') private kanban?: KanbanComponent;
  readonly state$ = toLoadState(this.api.getPipeline());
  protected readonly mutationMessage = signal('');
  protected readonly mutationError = signal('');
  private readonly dragStartStages = new Map<string, string>();
  private readonly activeForecastStages = new Set([
    'Qualification',
    'Discovery',
    'Proposal',
    'Negotiation',
  ]);
  private readonly allowedForecastBadges = new Set(['Pipeline', 'Best Case', 'Commit', 'Omitted']);
  protected readonly columns = [
    'Qualification',
    'Discovery',
    'Proposal',
    'Negotiation',
    'ClosedWon',
    'ClosedLost',
  ].map((stage) => ({
    headerText: stage.replace(/([a-z])([A-Z])/g, '$1 $2'),
    keyField: stage,
  }));
  protected readonly cardSettings = { headerField: 'id', contentField: 'content' };
  protected readonly usd = formatUsd;
  protected readonly dateUs = formatDateUs;
  protected readonly percent = formatPercent;

  protected onDragStart(args: DragEventArgs) {
    this.dragStartStages.clear();
    for (const item of args.data) {
      const card = item as PipelineCard;
      this.dragStartStages.set(card.id, card.stage);
    }
    this.mutationError.set('');
  }

  protected async onDragStop(args: DragEventArgs) {
    for (const item of args.data) {
      const card = item as PipelineCard;
      const previousStage = this.dragStartStages.get(card.id);
      if (!previousStage || previousStage === card.stage) continue;

      const nextStage = card.stage;
      this.mutationMessage.set(`Saving ${card.title}…`);
      try {
        await firstValueFrom(this.api.updateOpportunityStage(card.id, nextStage));
        this.kanban?.updateCard(card);
        this.mutationMessage.set(`${card.title} moved to ${this.stageLabel(nextStage)}.`);
      } catch {
        card.stage = previousStage;
        this.kanban?.updateCard(card);
        this.mutationMessage.set('');
        this.mutationError.set(
          `Could not move ${card.title}. The card was restored to ${this.stageLabel(previousStage)}.`,
        );
      }
    }
    this.dragStartStages.clear();
    if (this.mutationMessage()) window.setTimeout(() => this.mutationMessage.set(''), 3000);
  }

  protected priorityClass(priority: string) {
    return `priority-chip priority-${(priority || 'medium').toLowerCase()}`;
  }
  protected showForecastBadge(card: PipelineCard) {
    return (
      this.activeForecastStages.has(card.stage) &&
      this.allowedForecastBadges.has(card.forecastCategory)
    );
  }
  protected isTerminalStage(card: PipelineCard) {
    return card.stage === 'ClosedWon' || card.stage === 'ClosedLost';
  }
  protected terminalDateLabel(card: PipelineCard) {
    return card.stage === 'ClosedWon' ? 'Won Date' : 'Lost Date';
  }
  protected terminalDateValue(card: PipelineCard) {
    return this.dateUs(
      card.stage === 'ClosedWon'
        ? card.wonDate || card.expectedCloseDate
        : card.lostDate || card.expectedCloseDate,
    );
  }
  private stageLabel(stage: string) {
    return stage.replace(/([a-z])([A-Z])/g, '$1 $2');
  }
}
