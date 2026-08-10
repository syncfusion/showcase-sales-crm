import { useRef } from 'react';
import { GridComponent, ColumnsDirective, ColumnDirective, Inject, Page, Sort, Filter, Group, ExcelExport, Resize, ColumnMenu, Toolbar } from '@syncfusion/ej2-react-grids';
import { Link } from 'react-router-dom';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';
import { formatUsd } from '../../shared/utils/formatters';
import { ScoreBadge, StatusBadge } from '../../shared/components/Badges';

const toolbarItems = ['ExcelExport', 'ColumnChooser', 'Search'];

export function AccountsPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getAccounts(), []);
  const gridRef = useRef<GridComponent | null>(null);
  // Grow the grid on QHD / 4K so more rows remain visible on the taller viewport.
  const gridHeight = useResponsiveHeight('560px', '640px', '700px');

  const handleToolbarClick = (args: { item?: { text?: string } }) => {
    if (args.item?.text === 'Excel Export') {
      gridRef.current?.excelExport({
        fileName: 'Accounts.xlsx',
        exportType: 'AllPages'
      });
    }
  };

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No accounts found." />;

  return (
    <section>
      <PageHeader title="Accounts" subtitle="Manage account records and territory ownership." icon={<AppIcon name="accounts" />} />
      <div className="content-card">
        <div className="w-full overflow-x-auto">
          <GridComponent
            ref={gridRef}
            width="100%"
            dataSource={data}
            allowPaging={true}
            pageSettings={{ pageSize: 10, pageCount: 5 }}
            allowSorting={true}
            allowFiltering={true}
            filterSettings={{ type: 'Menu' }}
            allowGrouping={true}
            groupSettings={{ showDropArea: true, showUngroupButton: true }}
            allowTextWrap={true}
            allowResizing={true}
            allowExcelExport={true}
            showColumnMenu={true}
            toolbar={toolbarItems}
            toolbarClick={handleToolbarClick}
            height={gridHeight}
          >
            <Inject services={[Page, Sort, Filter, Group, ExcelExport, Resize, ColumnMenu, Toolbar]} />
            <ColumnsDirective>
              <ColumnDirective field="accountName" headerText="Account Name" width="260" template={(row: any) => <Link to={`/accounts/${row.id}`} className="text-brand">{row.accountName}</Link>} />
              <ColumnDirective field="industry" headerText="Industry" width="180" />
              <ColumnDirective field="region" headerText="Region" width="150" />
              <ColumnDirective field="status" headerText="Status" width="140" template={(row: any) => <StatusBadge value={row.status} />} />
              <ColumnDirective field="owner" headerText="Owner" width="150" />
              <ColumnDirective field="revenue" headerText="Revenue" width="140" textAlign="Right" template={(row: any) => formatUsd(row.revenue)} />
              <ColumnDirective field="healthScore" headerText="Health Score" width="150" textAlign="Right" template={(row: any) => <ScoreBadge value={row.healthScore} />} />
            </ColumnsDirective>
          </GridComponent>
        </div>
      </div>
    </section>
  );
}
