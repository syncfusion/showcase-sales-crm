import { useRef } from 'react';
import { GridComponent, ColumnsDirective, ColumnDirective, Inject, Page, Sort, Filter, Group, ExcelExport, Resize, ColumnMenu, Toolbar } from '@syncfusion/ej2-react-grids';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';
import { formatDateNumeric, formatUsd } from '../../shared/utils/formatters';
import { ProbabilityBadge, StatusBadge } from '../../shared/components/Badges';

const toolbarItems = ['ExcelExport', 'ColumnChooser', 'Search'];

export function OpportunitiesPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getOpportunities(), []);
  const gridRef = useRef<GridComponent | null>(null);
  // Grow the grid on QHD / 4K so more rows remain visible on the taller viewport.
  const gridHeight = useResponsiveHeight('560px', '640px', '700px');

  // Explicit toolbarClick handler per Syncfusion docs:
  // https://ej2.syncfusion.com/react/documentation/grid/excel-export/excel-export-options
  // When the Excel Export toolbar button is clicked, we call gridRef.excelExport(ExcelExportProperties)
  // with a meaningfully-named file ("Opportunities.xlsx") instead of the default "Export.xlsx" and
  // export-all-pages:true so every row is exported regardless of the current pagination state.
  const handleToolbarClick = (args: { item?: { text?: string } }) => {
    if (args.item?.text === 'Excel Export') {
      gridRef.current?.excelExport({
        fileName: 'Opportunities.xlsx',
        exportType: 'AllPages'
      });
    }
  };

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No opportunities found." />;

  return (
    <section>
      <PageHeader title="Opportunities" subtitle="Manage deal stages, values, and next actions." icon={<AppIcon name="opportunities" />} />
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
              <ColumnDirective field="opportunityName" headerText="Opportunity Name" width="230" />
              <ColumnDirective field="account" headerText="Account" width="180" />
              <ColumnDirective field="stage" headerText="Stage" width="140" template={(row: any) => <StatusBadge value={row.stage} />} />
              <ColumnDirective field="amount" headerText="Amount" width="150" textAlign="Right" template={(row: any) => formatUsd(row.amount)} />
              <ColumnDirective field="probability" headerText="Probability" width="130" textAlign="Right" template={(row: any) => <ProbabilityBadge value={row.probability} />} />
              <ColumnDirective field="expectedCloseDate" headerText="Expected Close Date" width="160" template={(row: any) => formatDateNumeric(row.expectedCloseDate)} />
              <ColumnDirective field="owner" headerText="Owner" width="170" />
            </ColumnsDirective>
          </GridComponent>
        </div>
      </div>
    </section>
  );
}
