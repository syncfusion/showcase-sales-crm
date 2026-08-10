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

const toolbarItems = ['ExcelExport', 'ColumnChooser', 'Search'];

export function ContactsPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getContacts(), []);
  const gridRef = useRef<GridComponent | null>(null);
  // Grow the grid on QHD / 4K so more rows remain visible on the taller viewport.
  const gridHeight = useResponsiveHeight('460px', '560px', '620px');

  // Explicit toolbarClick handler per Syncfusion docs:
  // https://ej2.syncfusion.com/react/documentation/grid/excel-export/excel-export-options
  // When the Excel Export toolbar button is clicked, we call gridRef.excelExport(ExcelExportProperties)
  // with a meaningfully-named file ("Contacts.xlsx") instead of the default "Export.xlsx" and
  // export-all-pages:true so every row is exported regardless of the current pagination state.
  const handleToolbarClick = (args: { item?: { text?: string } }) => {
    if (args.item?.text === 'Excel Export') {
      gridRef.current?.excelExport({
        fileName: 'Contacts.xlsx',
        exportType: 'AllPages'
      });
    }
  };

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No contacts found." />;

  return (
    <section>
      <PageHeader title="Contacts" subtitle="Track buyer relationships and communication roles." icon={<AppIcon name="contacts" />} />
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
              <ColumnDirective field="name" headerText="Contact" width="200" />
              <ColumnDirective field="account" headerText="Account" width="200" />
              <ColumnDirective field="role" headerText="Role" width="200" />
              <ColumnDirective field="email" headerText="Email" width="300" />
              {data[0]?.phone ? <ColumnDirective field="phone" headerText="Phone" width="180" /> : null}
            </ColumnsDirective>
          </GridComponent>
        </div>
      </div>
    </section>
  );
}
