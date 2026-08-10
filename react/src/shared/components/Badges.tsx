import { formatPercent, stageLabel } from '../utils/formatters';

function normalize(value: string | number) {
  return String(value).replace(/\s+/g, '').toLowerCase();
}

function probabilityTone(value: number) {
  if (value >= 75) return 'strong';
  if (value >= 50) return 'medium';
  if (value >= 30) return 'low';
  return 'risk';
}

export function StatusBadge({ value }: { value: string }) {
  // stageLabel converts "ClosedWon" / "ClosedLost" => "Closed Won" / "Closed Lost".
  const display = stageLabel(value);
  return <span className={`status-badge status-${normalize(value)}`}>{display}</span>;
}

export function ProbabilityBadge({ value }: { value: number }) {
  return <span className={`metric-badge metric-${probabilityTone(value)}`}>{formatPercent(value)}</span>;
}

export function ScoreBadge({ value }: { value: number }) {
  return <span className={`metric-badge metric-${probabilityTone(value)}`}>{value}</span>;
}

export function WinRateBadge({ value }: { value: number }) {
  return <span className={`metric-badge metric-${probabilityTone(value)}`}>{formatPercent(value)} win rate</span>;
}
