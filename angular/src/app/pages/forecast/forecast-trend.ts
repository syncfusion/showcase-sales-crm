import { parsePeriodToDate } from '../../shared/formatters';
import { ForecastSummary } from '../../shared/models';

type ForecastPoint = { x: Date; y: number };

export function buildForecastTrend(
  forecast: ForecastSummary,
  now = new Date(),
): {
  summaryData: Array<{ label: string; value: number }>;
  actualPoints: ForecastPoint[];
  forecastPoints: ForecastPoint[];
  hasTrend: boolean;
} {
  const summaryData = [
    { label: 'Commit', value: forecast.commit },
    { label: 'Best Case', value: forecast.bestCase },
    { label: 'Pipeline', value: forecast.pipeline },
    { label: 'Weighted', value: forecast.weighted },
  ];
  const currentMonth = new Date(now.getFullYear(), now.getMonth(), 1);
  const datedTrend = forecast.trend
    .map((point) => ({ x: parsePeriodToDate(point.period), y: point.weighted }))
    .filter((point): point is ForecastPoint => point.x !== null)
    .sort((a, b) => a.x.getTime() - b.x.getTime());
  const actualPoints = datedTrend.filter((point) => point.x < currentMonth);
  const forecastPoints = datedTrend.filter((point) => point.x >= currentMonth);
  const lastActual = actualPoints.at(-1);

  if (lastActual && forecastPoints.length) forecastPoints.unshift(lastActual);

  return {
    summaryData,
    actualPoints,
    forecastPoints,
    hasTrend: datedTrend.length > 0,
  };
}
