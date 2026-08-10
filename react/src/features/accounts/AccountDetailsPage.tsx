import { useParams } from "react-router-dom";
import { PageHeader } from "../../shared/components/PageHeader";
import { LoadingState } from "../../shared/components/LoadingState";
import { EmptyState } from "../../shared/components/EmptyState";
import { ErrorState } from "../../shared/components/ErrorState";
import { AppIcon } from "../../shared/components/AppIcon";
import { ScoreBadge, StatusBadge } from "../../shared/components/Badges";
import { useAsyncData } from "../../shared/hooks/useAsyncData";
import { salesCrmApi } from "../../shared/api/salesCrmApi";
import { formatUsd } from "../../shared/utils/formatters";

export function AccountDetailsPage() {
  const { accountId = "" } = useParams();
  const {
    data: account,
    loading,
    error,
  } = useAsyncData(async () => {
    const accounts = await salesCrmApi.getAccounts();
    return (
      accounts.find(
        (item) => item.id.toLowerCase() === accountId.toLowerCase(),
      ) ?? null
    );
  }, [accountId]);

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!account) return <EmptyState message="Account not found." />;

  return (
    <section>
      <PageHeader
        title="Account Details"
        subtitle="Review account health, contacts, and pipeline context."
        icon={<AppIcon name="accounts" />}
      />
      <div className="account-details-grid">
        <div className="content-card">
          <h2>{account.accountName}</h2>
          <dl className="account-meta">
            <div>
              <dt>Industry</dt>
              <dd>{account.industry}</dd>
            </div>
            <div>
              <dt>Region</dt>
              <dd>{account.region}</dd>
            </div>
            <div>
              <dt>Status</dt>
              <dd>
                <StatusBadge value={account.status} />
              </dd>
            </div>
            <div>
              <dt>Owner</dt>
              <dd>{account.owner}</dd>
            </div>
            <div>
              <dt>Revenue</dt>
              <dd>{formatUsd(account.revenue)}</dd>
            </div>
            <div>
              <dt>Health Score</dt>
              <dd>
                <ScoreBadge value={account.healthScore} />
              </dd>
            </div>
          </dl>
        </div>
        <div className="content-card">
          <h2>Pipeline Context</h2>
          <p className="text-muted">
            Related opportunities and recent activities will appear here.
          </p>
        </div>
      </div>
    </section>
  );
}
