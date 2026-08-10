import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';

export function AdminSeedDataPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.refreshSeedData(), []);
  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;

  return (
    <section>
      <PageHeader title="Admin Seed Data" subtitle="Inspect and refresh deterministic demo data." icon={<AppIcon name="admin" />} />
      <div className="content-card">
        {data ? <strong>Seed refresh status: {data.status}</strong> : <EmptyState message="No seed batch data available." />}
      </div>
    </section>
  );
}
