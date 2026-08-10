import { ChartComponent, SeriesCollectionDirective, SeriesDirective, Inject, LineSeries, DateTime, DataLabel, Legend, Tooltip } from '@syncfusion/ej2-react-charts';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useTheme } from '../../shared/hooks/useTheme';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';
import { formatCompactUsd, formatDateReadable, formatUsd, parsePeriodtoDate } from '../../shared/utils/formatters';
import { appChartFont, appChartHorizontalGridLines, appChartLabelFont, appChartNoGridLines } from '../../shared/utils/chartStyles';

type TrendPoint = { period: string; weighted: number };

export function ForecastPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getForecast(), []);
  const { syncfusionTheme, theme } = useTheme();
  // Taller trend chart on QHD / 4K so the wide viewport does not squash the plot.
  const chartHeight = useResponsiveHeight('320px', '420px', '480px');
  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data) return <EmptyState message="No forecast data available." />;

  const summaryData = [
    { label: 'Commit', value: data.commit },
    { label: 'Best Case', value: data.bestCase },
    { label: 'Pipeline', value: data.pipeline },
    { label: 'Weighted', value: data.weighted }
  ];

  // Split the trend into a solid "Actual" line (past months) and a dotted "Forecast" line
  // (current month onward). The last actual point is prepended to the forecast series so the
  // dotted line visually connects to the solid line — a real CRM trend chart convention.
  const currentMonth = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
  const sortedTrend: TrendPoint[] = data.trend.length
    ? [...data.trend].sort((a, b) => {
        const d1 = parsePeriodtoDate(a.period);
        const d2 = parsePeriodtoDate(b.period);
        return (d1?.getTime() ?? 0) - (d2?.getTime() ?? 0);
      })
    : summaryData.map((item) => ({ period: item.label, weighted: item.value }));

  const actualPoints = sortedTrend
    .map((p) => ({ x: parsePeriodtoDate(p.period), y: p.weighted }))
    .filter((p) => p.x !== null && p.x < currentMonth) as Array<{ x: Date; y: number }>;

  const forecastPoints = sortedTrend
    .map((p) => ({ x: parsePeriodtoDate(p.period), y: p.weighted }))
    .filter((p) => p.x !== null && p.x >= currentMonth) as Array<{ x: Date; y: number }>;

  const lastActual = actualPoints.length ? actualPoints[actualPoints.length - 1] : null;
  if (lastActual && forecastPoints.length) {
    forecastPoints.unshift(lastActual);
  }

  return (
    <section>
      <PageHeader title="Forecast" subtitle="Commit, best-case, pipeline, and weighted forecast summary." icon={<AppIcon name="forecast" />} />

      <div className="forecast-summary-strip">
        {summaryData.map((item) => (
          <div key={item.label} className="e-card forecast-summary-item">
            <div className="e-card-header">
              <div className="e-card-header-caption">
                <div className="e-card-title">{item.label}</div>
              </div>
            </div>
            <div className="e-card-content">
              <strong>{formatCompactUsd(item.value)}</strong>
            </div>
          </div>
        ))}
      </div>

      <div className="content-card">
        <h2 style={{ margin: '0 0 16px' }}>Forecast Trend</h2>
        {sortedTrend.length ? (
          <div className="w-full overflow-x-auto">
            <ChartComponent
              key={`forecast-trend-${theme}`}
              width="100%"
              height={chartHeight}
              primaryXAxis={{
                valueType: 'DateTime',
                labelFormat: 'MMM-yyyy',
                intervalType: 'Months',
                interval: 1,
                edgeLabelPlacement: 'Shift',
                labelStyle: appChartLabelFont,
                majorGridLines: appChartNoGridLines
              }}
              primaryYAxis={{
                title: 'Weighted Forecast',
                labelStyle: appChartLabelFont,
                titleStyle: appChartFont,
                majorGridLines: appChartHorizontalGridLines,
                interval: 40000
              }}
              legendSettings={{ textStyle: appChartLabelFont, visible: true }}
              theme={syncfusionTheme}
              background="transparent"
              tooltipRender={(args: any) => {
                args.text = `${formatDateReadable(args.point.x)}: ${formatUsd(args.point.y)}`;
              }}
              textRender={(args: any) => {
                if (args.point?.y !== undefined) args.text = formatCompactUsd(args.point.y);
              }}
              axisLabelRender={(args: any) => {
                if (args.axis.name === 'primaryYAxis') args.text = formatCompactUsd(args.value);
              }}
            >
              <Inject services={[LineSeries, DateTime, DataLabel, Legend, Tooltip]} />
              <SeriesCollectionDirective>
                <SeriesDirective
                  dataSource={actualPoints}
                  xName="x"
                  yName="y"
                  type="Line"
                  name="Actual"
                  width={2}
                  fill={syncfusionTheme === 'Tailwind3Dark' ? '#93c5fd' : '#175cd3'}
                  marker={{ visible: true, width: 7, height: 7, shape: 'Circle', dataLabel: { visible: true, font: appChartLabelFont } }}
                />
                <SeriesDirective
                  dataSource={forecastPoints}
                  xName="x"
                  yName="y"
                  type="Line"
                  name="Forecast"
                  width={2}
                  dashArray="6,4"
                  fill={syncfusionTheme === 'Tailwind3Dark' ? '#fbbf24' : '#b54708'}
                  marker={{ visible: true, width: 7, height: 7, shape: 'Circle', dataLabel: { visible: true, font: appChartLabelFont } }}
                />
              </SeriesCollectionDirective>
            </ChartComponent>
          </div>
        ) : (
          <p style={{ color: 'var(--text-muted)' }}>Trend data is not available.</p>
        )}
      </div>
    </section>
  );
}
