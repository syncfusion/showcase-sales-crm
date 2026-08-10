import { ChartComponent, SeriesCollectionDirective, SeriesDirective, Inject, ColumnSeries, Category, DataLabel, Legend, Tooltip } from '@syncfusion/ej2-react-charts';
import { GridComponent, ColumnsDirective, ColumnDirective, Inject as GridInject, Sort, Page } from '@syncfusion/ej2-react-grids';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useTheme } from '../../shared/hooks/useTheme';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon, type AppIconName } from '../../shared/components/AppIcon';
import { StatusBadge } from '../../shared/components/Badges';
import { formatCompactUsd, formatDateNumeric, formatPercent, formatUsd, stageLabel } from '../../shared/utils/formatters';
import { appChartFont, appChartHorizontalGridLines, appChartLabelFont, appChartNoGridLines, appChartTitleFont } from '../../shared/utils/chartStyles';

const terminalStages = new Set(['ClosedWon', 'ClosedLost']);

export function DashboardPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getDashboard(), []);
  const { syncfusionTheme, theme } = useTheme();
  // Grow the chart/grid on QHD / 4K so they look proportional to the wider viewport.
  const chartHeight = useResponsiveHeight('320px', '420px', '480px');
  const gridHeight = useResponsiveHeight('360px', '440px', '520px');

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data || (!data.recentOpportunities.length && !data.stageBreakdown.length)) {
    return <EmptyState message="No dashboard data available." />;
  }

  // Open Opportunities counts only active (non-terminal) stages so closed deals aren't mislabeled.
  const openOpportunities = data.stageBreakdown
    .filter((item: { stage: string }) => !terminalStages.has(item.stage))
    .reduce((sum: number, item: { count: number }) => sum + item.count, 0);

  const kpis: Array<{ label: string; value: string | number; icon: AppIconName }> = [
    { label: 'Revenue', value: formatCompactUsd(data.revenue), icon: 'revenue' },
    { label: 'Pipeline Value', value: formatCompactUsd(data.pipelineValue), icon: 'pipeline' },
    { label: 'Weighted Forecast', value: formatCompactUsd(data.weightedForecast), icon: 'forecast' },
    { label: 'Open Opportunities', value: openOpportunities, icon: 'work' },
    { label: 'Win Rate', value: formatPercent(data.winRate), icon: 'target' }
  ];

  return (
    <section className="dashboard-page">
      <PageHeader title="Dashboard" subtitle="Executive summary for pipeline health and revenue outlook." icon={<AppIcon name="dashboard" />} />

      <div className="dashboard-kpi-card content-card">
        <div className="kpi-grid">
          {kpis.map((kpi) => (
            <div key={kpi.label} className="e-card metric-card">
              <div className="e-card-header">
                <div className="e-card-header-caption">
                  <div className="e-card-title metric-label">{kpi.label}</div>
                </div>
                <span className="metric-card-icon e-card-header-icon">
                  <AppIcon name={kpi.icon} size={18} />
                </span>
              </div>
              <div className="e-card-content">
                <div className="metric-value">{kpi.value}</div>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="content-card dashboard-chart-card">
        <h2>Pipeline Stage Breakdown</h2>
        <div className="w-full overflow-x-auto">
          <ChartComponent
            key={`stage-breakdown-${theme}`}
            width="100%"
            height={chartHeight}
            primaryXAxis={{ valueType: 'Category', labelStyle: appChartLabelFont, majorGridLines: appChartNoGridLines, lineStyle: appChartHorizontalGridLines }}
            primaryYAxis={{ title: 'Pipeline Value', labelStyle: appChartLabelFont, titleStyle: appChartFont, majorGridLines: appChartHorizontalGridLines, minimum: 0, interval: 500000 }}
            title="Stage Breakdown"
            titleStyle={appChartTitleFont}
            legendSettings={{ textStyle: appChartLabelFont }}
            theme={syncfusionTheme}
            background="transparent"
            axisLabelRender={(args: any) => {
              if (args.axis.name === 'primaryYAxis') args.text = formatCompactUsd(args.value);
              if (args.axis.name === 'primaryXAxis') args.text = stageLabel(args.text);
            }}
            tooltipRender={(args: any) => {
              args.text = `${stageLabel(args.point.x)}: ${formatUsd(args.point.y)}`;
            }}
            textRender={(args: any) => {
              if (args.point?.y !== undefined) args.text = formatCompactUsd(args.point.y);
            }}
          >
            <Inject services={[ColumnSeries, Category, DataLabel, Legend, Tooltip]} />
            <SeriesCollectionDirective>
              <SeriesDirective
                dataSource={data.stageBreakdown}
                xName="stage"
                yName="value"
                type="Column"
                name="Pipeline Value"
                columnWidth={0.6}
                marker={{ dataLabel: { visible: true, font: appChartLabelFont } }}
              />
            </SeriesCollectionDirective>
          </ChartComponent>
        </div>
      </div>

      <div className="content-card dashboard-table-card">
        <h2>Recent Opportunities</h2>
        <div className="w-full overflow-x-auto">
          <GridComponent width="100%" dataSource={data.recentOpportunities} allowPaging={true} pageSettings={{ pageSize: 8 }} allowSorting={true} allowTextWrap={true} height={gridHeight}>
            <GridInject services={[Sort, Page]} />
            <ColumnsDirective>
              <ColumnDirective field="name" headerText="Opportunity" width="220" />
              <ColumnDirective field="account" headerText="Account" width="200" />
              <ColumnDirective field="stage" headerText="Stage" width="140" template={(row: any) => <StatusBadge value={row.stage} />} />
              <ColumnDirective field="expectedCloseDate" headerText="Expected Close Date" width="160" template={(row: any) => formatDateNumeric(row.expectedCloseDate)} />
              <ColumnDirective field="amount" headerText="Amount" width="150" textAlign="Right" template={(row: any) => formatUsd(row.amount)} />
            </ColumnsDirective>
          </GridComponent>
        </div>
      </div>
    </section>
  );
}
