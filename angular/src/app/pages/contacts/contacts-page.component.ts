import { AsyncPipe } from '@angular/common';
import { Component, ViewChild, inject } from '@angular/core';
import {
  ColumnChooserService,
  ExcelExportService,
  FilterService,
  GridComponent,
  GridModule,
  GroupService,
  PageService,
  ResizeService,
  SortService,
  ToolbarItems,
  ToolbarService,
} from '@syncfusion/ej2-angular-grids';
import type { ClickEventArgs } from '@syncfusion/ej2-navigations';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-contacts-page',
  imports: [
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    GridModule,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  providers: [
    ColumnChooserService,
    ExcelExportService,
    FilterService,
    GroupService,
    PageService,
    ResizeService,
    SortService,
    ToolbarService,
  ],
  template: `
    <section class="fit-page">
      <app-page-header
        title="Contacts"
        subtitle="Track buyer relationships and communication roles."
        icon="contacts"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.length) {
          <div class="content-card grid-card">
            <ejs-grid
              #grid
              id="contactsGrid"
              width="100%"
              height="100%"
              [dataSource]="state.data"
              [allowPaging]="true"
              [allowSorting]="true"
              [allowFiltering]="true"
              [allowGrouping]="true"
              [allowResizing]="true"
              [allowTextWrap]="true"
              [allowExcelExport]="true"
              [toolbar]="toolbar"
              [pageSettings]="{ pageSize: 10 }"
              [filterSettings]="{ type: 'Menu' }"
              (toolbarClick)="onToolbarClick($event)"
            >
              <e-columns>
                <e-column field="name" headerText="Contact" width="150" />
                <e-column field="account" headerText="Account" width="200" />
                <e-column field="role" headerText="Role" width="160" />
                <e-column field="email" headerText="Email" width="310" />
              </e-columns>
            </ejs-grid>
          </div>
        } @else {
          <app-empty-state message="No contacts found." />
        }
      }
    </section>
  `,
})
export class ContactsPageComponent {
  private readonly api = inject(SalesCrmApiService);
  @ViewChild('grid') private grid?: GridComponent;
  readonly state$ = toLoadState(this.api.getContacts());
  protected readonly toolbar: ToolbarItems[] = [
    'ExcelExport',
    'CsvExport',
    'ColumnChooser',
    'Search',
  ];

  protected onToolbarClick(args: ClickEventArgs) {
    if (args.item.id === 'contactsGrid_excelexport')
      void this.grid?.excelExport({ fileName: 'contacts.xlsx' });
    if (args.item.id === 'contactsGrid_csvexport')
      void this.grid?.csvExport({ fileName: 'contacts.csv' });
  }
}
