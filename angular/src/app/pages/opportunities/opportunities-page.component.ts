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
import {
  formatDateUs,
  formatPercent,
  formatUsd,
  metricCss,
  statusCss,
} from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-opportunities-page',
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
        title="Opportunities"
        subtitle="Manage deal stages, values, and next actions."
        icon="opportunities"
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
              id="opportunitiesGrid"
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
                <e-column field="opportunityName" headerText="Opportunity Name" width="190" />
                <e-column field="account" headerText="Account" width="170" />
                <e-column field="stage" headerText="Stage" width="130"
                  ><ng-template #template let-data
                    ><span [class]="statusClass(data.stage)">{{ data.stage }}</span></ng-template
                  ></e-column
                >
                <e-column field="amount" headerText="Amount" width="130" textAlign="Right"
                  ><ng-template #template let-data>{{ usd(data.amount) }}</ng-template></e-column
                >
                <e-column field="probability" headerText="Probability" width="120" textAlign="Right"
                  ><ng-template #template let-data
                    ><span [class]="metricClass(data.probability)">{{
                      percent(data.probability)
                    }}</span></ng-template
                  ></e-column
                >
                <e-column field="expectedCloseDate" headerText="Expected Close Date" width="150"
                  ><ng-template #template let-data>{{
                    dateUs(data.expectedCloseDate)
                  }}</ng-template></e-column
                >
                <e-column field="owner" headerText="Owner" width="140" />
              </e-columns>
            </ejs-grid>
          </div>
        } @else {
          <app-empty-state message="No opportunities found." />
        }
      }
    </section>
  `,
})
export class OpportunitiesPageComponent {
  private readonly api = inject(SalesCrmApiService);
  @ViewChild('grid') private grid?: GridComponent;
  readonly state$ = toLoadState(this.api.getOpportunities());
  protected readonly toolbar: ToolbarItems[] = [
    'ExcelExport',
    'CsvExport',
    'ColumnChooser',
    'Search',
  ];
  protected readonly usd = formatUsd;
  protected readonly percent = formatPercent;
  protected readonly dateUs = formatDateUs;
  protected readonly statusClass = statusCss;
  protected readonly metricClass = metricCss;

  protected onToolbarClick(args: ClickEventArgs) {
    if (args.item.id === 'opportunitiesGrid_excelexport')
      void this.grid?.excelExport({ fileName: 'opportunities.xlsx' });
    if (args.item.id === 'opportunitiesGrid_csvexport')
      void this.grid?.csvExport({ fileName: 'opportunities.csv' });
  }
}
