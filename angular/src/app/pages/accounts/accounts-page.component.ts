import { AsyncPipe } from '@angular/common';
import { Component, ViewChild, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
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
import { formatUsd, metricCss, statusCss } from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-accounts-page',
  imports: [
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    GridModule,
    LoadingStateComponent,
    PageHeaderComponent,
    RouterLink,
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
        title="Accounts"
        subtitle="Manage account records and territory ownership."
        icon="accounts"
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
              id="accountsGrid"
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
                <e-column field="accountName" headerText="Account Name" width="220">
                  <ng-template #template let-data
                    ><a [routerLink]="['/accounts', data.id]">{{
                      data.accountName
                    }}</a></ng-template
                  >
                </e-column>
                <e-column field="industry" headerText="Industry" width="150" />
                <e-column field="region" headerText="Region" width="140" />
                <e-column field="status" headerText="Status" width="130">
                  <ng-template #template let-data
                    ><span [class]="statusClass(data.status)">{{ data.status }}</span></ng-template
                  >
                </e-column>
                <e-column field="owner" headerText="Owner" width="160" />
                <e-column field="revenue" headerText="Revenue" width="150" textAlign="Right">
                  <ng-template #template let-data>{{ usd(data.revenue) }}</ng-template>
                </e-column>
                <e-column
                  field="healthScore"
                  headerText="Health Score"
                  width="130"
                  textAlign="Right"
                >
                  <ng-template #template let-data
                    ><span [class]="scoreClass(data.healthScore)">{{
                      data.healthScore
                    }}</span></ng-template
                  >
                </e-column>
              </e-columns>
            </ejs-grid>
          </div>
        } @else {
          <app-empty-state message="No accounts found." />
        }
      }
    </section>
  `,
})
export class AccountsPageComponent {
  private readonly api = inject(SalesCrmApiService);
  @ViewChild('grid') private grid?: GridComponent;
  readonly state$ = toLoadState(this.api.getAccounts());
  protected readonly toolbar: ToolbarItems[] = [
    'ExcelExport',
    'CsvExport',
    'ColumnChooser',
    'Search',
  ];
  protected readonly usd = formatUsd;
  protected readonly statusClass = statusCss;
  protected readonly scoreClass = metricCss;

  protected onToolbarClick(args: ClickEventArgs) {
    if (args.item.id === 'accountsGrid_excelexport')
      void this.grid?.excelExport({ fileName: 'accounts.xlsx' });
    if (args.item.id === 'accountsGrid_csvexport')
      void this.grid?.csvExport({ fileName: 'accounts.csv' });
  }
}
