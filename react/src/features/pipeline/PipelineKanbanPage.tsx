import { KanbanComponent, ColumnsDirective, ColumnDirective } from '@syncfusion/ej2-react-kanban';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';
import { formatMonthDayYear, formatPercent, formatUsd } from '../../shared/utils/formatters';
import type { PipelineCard } from '../../shared/models';

const stageOrder = ['Qualification', 'Discovery', 'Proposal', 'Negotiation', 'ClosedWon', 'ClosedLost'];
const activeForecastStages = new Set(['Qualification', 'Discovery', 'Proposal', 'Negotiation']);
const allowedForecastBadges = new Set(['Pipeline', 'Best Case', 'Commit', 'Omitted']);

function stageLabel(stage: string) {
  return stage.replace(/([a-z])([A-Z])/g, '$1 $2');
}

function isTerminalStage(card: PipelineCard) {
  return card.stage === 'ClosedWon' || card.stage === 'ClosedLost';
}

function showForecastBadge(card: PipelineCard) {
  return activeForecastStages.has(card.stage) && allowedForecastBadges.has(card.forecastCategory);
}

function terminalDateLabel(card: PipelineCard) {
  return card.stage === 'ClosedWon' ? 'Won Date' : 'Lost Date';
}

function terminalDateValue(card: PipelineCard) {
  return formatMonthDayYear(card.stage === 'ClosedWon' ? card.wonDate || card.expectedCloseDate : card.lostDate || card.expectedCloseDate);
}

function priorityClass(priority: PipelineCard['priority']) {
  return `priority-chip priority-${(priority || 'medium').toLowerCase()}`;
}

export function PipelineKanbanPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getPipeline(), []);
  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No pipeline cards available." />;

  const columns = stageOrder.map((stage) => ({ headerText: stageLabel(stage), keyField: stage }));
  const cardTemplate = (card: PipelineCard) => (
    <div className="pipeline-card-template">
      <div className="pipeline-card-topline">
        <strong>{card.title}</strong>
        {showForecastBadge(card) && <span className="forecast-chip">{card.forecastCategory}</span>}
      </div>
      <span className="pipeline-card-account">{card.account}</span>
      <b>{formatUsd(card.amount)}</b>
      {isTerminalStage(card) ? (
        <dl className="pipeline-card-meta terminal-meta">
          <div>
            <dt>Owner</dt>
            <dd>{card.owner}</dd>
          </div>
          <div>
            <dt>{terminalDateLabel(card)}</dt>
            <dd>{terminalDateValue(card)}</dd>
          </div>
          {card.stage === 'ClosedWon' && typeof card.salesCycleDays === 'number' && Number.isFinite(card.salesCycleDays) && (
            <div>
              <dt>Cycle</dt>
              <dd>{card.salesCycleDays} days</dd>
            </div>
          )}
          {card.stage === 'ClosedLost' && card.lossReason && (
            <div className="pipeline-card-meta-wide">
              <dt>Loss Reason</dt>
              <dd>{card.lossReason}</dd>
            </div>
          )}
        </dl>
      ) : (
        <dl className="pipeline-card-meta">
          <div>
            <dt>Owner</dt>
            <dd>{card.owner}</dd>
          </div>
          <div>
            <dt>Close</dt>
            <dd>{formatMonthDayYear(card.expectedCloseDate)}</dd>
          </div>
          <div>
            <dt>Prob.</dt>
            <dd>{formatPercent(card.probability)}</dd>
          </div>
          <div>
            <dt>Priority</dt>
            <dd><span className={priorityClass(card.priority)}>{card.priority}</span></dd>
          </div>
        </dl>
      )}
    </div>
  );

  return (
    <section>
      <PageHeader title="Pipeline Kanban" subtitle="Stage-based deal movement and prioritization." icon={<AppIcon name="pipeline" />} />
      <div className="content-card">
        <div className="w-full overflow-x-auto">
          <KanbanComponent dataSource={data} keyField="stage" cardSettings={{ headerField: 'id', contentField: 'content', template: cardTemplate as any }}>
            <ColumnsDirective>
              {columns.map((column) => (
                <ColumnDirective key={column.keyField} headerText={column.headerText} keyField={column.keyField} />
              ))}
            </ColumnsDirective>
          </KanbanComponent>
        </div>
      </div>
    </section>
  );
}
 