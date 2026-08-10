const usdFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

const compactUsdFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  notation: 'compact',
  minimumFractionDigits: 0,
  maximumFractionDigits: 1,
});

const percentFormatter = new Intl.NumberFormat('en-US', {
  style: 'percent',
  minimumFractionDigits: 0,
  maximumFractionDigits: 0,
});

function safeNumber(value: number | null | undefined) {
  return Number.isFinite(value ?? NaN) ? (value as number) : 0;
}

export function formatUsd(value: number | null | undefined) {
  return usdFormatter.format(safeNumber(value));
}

export function formatCompactUsd(value: number | null | undefined) {
  return compactUsdFormatter.format(safeNumber(value));
}

export function formatPercent(value: number | null | undefined) {
  return percentFormatter.format(safeNumber(value) / 100);
}

export function formatDateUs(value: string | Date | null | undefined) {
  if (!value) return '';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${month}-${day}-${date.getFullYear()}`;
}

export function formatDateReadable(value: string | Date | null | undefined) {
  if (!value) return '';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: '2-digit',
    year: 'numeric',
  });
}

export function parsePeriodToDate(period: string | null | undefined): Date | null {
  if (!period) return null;
  const match = /^(\d{4})-(\d{2})(?:-(\d{2}))?$/.exec(period);
  if (!match) return null;

  const year = Number(match[1]);
  const month = Number(match[2]) - 1;
  const day = match[3] ? Number(match[3]) : 1;
  const date = new Date(year, month, day);

  if (
    Number.isNaN(date.getTime()) ||
    date.getFullYear() !== year ||
    date.getMonth() !== month ||
    date.getDate() !== day
  ) {
    return null;
  }

  return date;
}

export const appChartFont = {
  fontFamily: 'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
  size: '13px',
  fontWeight: '600',
};

export const appChartLabelFont = {
  ...appChartFont,
  fontWeight: '500',
};

export const appChartTitleFont = {
  ...appChartFont,
  size: '14px',
  fontWeight: '700',
};

export function statusCss(value: string) {
  return `status-badge status-${(value ?? 'unknown').replace(/\s+/g, '').toLowerCase()}`;
}

export function metricCss(value: number) {
  if (value >= 75) return 'metric-badge metric-strong';
  if (value >= 50) return 'metric-badge metric-medium';
  if (value > 0) return 'metric-badge metric-low';
  return 'metric-badge metric-risk';
}
