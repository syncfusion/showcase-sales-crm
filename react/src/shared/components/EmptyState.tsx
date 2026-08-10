export function EmptyState({ message = 'No records found.' }: { message?: string }) {
  return <div style={{ padding: 24 }}>{message}</div>;
}
