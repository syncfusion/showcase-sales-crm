import { describe, expect, it } from 'vitest';
import { ForecastSummary } from '../../shared/models';
import { buildForecastTrend } from './forecast-trend';

const forecast: ForecastSummary = {
  commit: 100,
  bestCase: 200,
  pipeline: 300,
  weighted: 150,
  trend: [
    { period: '2026-08', commit: 0, bestCase: 0, pipeline: 0, weighted: 180 },
    { period: '2026-06', commit: 0, bestCase: 0, pipeline: 0, weighted: 120 },
    { period: '2026-07', commit: 0, bestCase: 0, pipeline: 0, weighted: 150 },
  ],
};

describe('buildForecastTrend', () => {
  it('splits actual and forecast months and connects both series', () => {
    const result = buildForecastTrend(forecast, new Date(2026, 6, 20));

    expect(result.actualPoints.map((point) => point.y)).toEqual([120]);
    expect(result.forecastPoints.map((point) => point.y)).toEqual([120, 150, 180]);
    expect(result.hasTrend).toBe(true);
  });

  it('ignores invalid period values', () => {
    const result = buildForecastTrend(
      { ...forecast, trend: [{ ...forecast.trend[0], period: 'not-a-period' }] },
      new Date(2026, 6, 20),
    );

    expect(result.actualPoints).toEqual([]);
    expect(result.forecastPoints).toEqual([]);
    expect(result.hasTrend).toBe(false);
  });
});
