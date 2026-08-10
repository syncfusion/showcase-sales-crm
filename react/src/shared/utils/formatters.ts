const usdFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2
});

const compactUsdFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  notation: 'compact',
  minimumFractionDigits: 0,
  maximumFractionDigits: 1
});

const percentFormatter = new Intl.NumberFormat('en-US', {
  style: 'percent',
  minimumFractionDigits: 0,
  maximumFractionDigits: 0
});

function safeNumber(value: number | null | undefined) {
  return Number.isFinite(value ?? NaN) ? value as number : 0;
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

export function formatMonthDayYear(value: string | Date | null | undefined) {
  if (!value) return '';

  if (typeof value === 'string') {
    const dateOnly = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
    if (dateOnly) return `${dateOnly[2]}-${dateOnly[3]}-${dateOnly[1]}`;
  }

  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';

  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${month}-${day}-${date.getFullYear()}`;
}

/**
 * Readable chart-friendly date, e.g. "Jul 21, 2026". Used in chart axis labels and data labels
 * where compact human-readable dates read better than the MM-DD-YYYY form used in grids.
 */
export function formatDateReadable(value: string | Date | null | undefined) {
  if (!value) return '';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';
  return date.toLocaleDateString('en-US', { month: 'short', day: '2-digit', year: 'numeric' });
}

/**
 * Numeric MM/DD/YYYY date used in grid cells (sortable, compact, unambiguous for US CRM use).
 */
export function formatDateNumeric(value: string | Date | null | undefined) {
  if (!value) return '';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${month}/${day}/${date.getFullYear()}`;
}

/**
 * Converts a PascalCase enum name into a display label. E.g. "ClosedWon" => "Closed Won",
 * "ClosedLost" => "Closed Lost". Used so grids, badges and chart axes never leak the raw enum
 * string returned by the Opportunities endpoint.
 */
export function stageLabel(stage: string | null | undefined) {
  if (!stage) return '';
  return stage.replace(/([a-z])([A-Z])/g, '$1 $2');
}

/**
 * Parses a "yyyy-MM" period string (e.g. "2026-07") to a JS Date (1st of month, local midnight).
 * Used by the forecast trend chart's DateTime axis. Returns null if the input cannot be parsed.
 */
export function parsePeriodtoDate(period: string | null | undefined): Date | null {
  if (!period) return null;
  // Accept both "yyyy-MM" and full "yyyy-MM-dd" shapes.
  const match = /^(\d{4})-(\d{2})(?:-(\d{2}))?$/.exec(period);
  if (!match) return null;
  const year = Number(match[1]);
  const month = Number(match[2]) - 1;
  const day = match[3] ? Number(match[3]) : 1;
  const date = new Date(year, month, day);
  return Number.isNaN(date.getTime()) ? null : date;
}
