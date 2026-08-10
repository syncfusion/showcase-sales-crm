import { AsyncPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import {
  CategoryService,
  ChartModule,
  ColumnSeriesService,
  DataLabelService,
  IAxisLabelRenderEventArgs,
  ITextRenderEventArgs,
  ITooltipRenderEventArgs,
  LegendService,
  TooltipService,
} from '@syncfusion/ej2-angular-charts';
import { GridModule, PageService, SortService } from '@syncfusion/ej2-angular-grids';
import { map } from 'rxjs';
import { formatCompactUsd, formatDateUs, formatUsd, statusCss } from '../../shared/formatters';
import { ThemeService } from '../../layout/theme.service';
import { AppIconComponent } from '../../shared/app-icon.component';
import { DashboardSummary } from '../../shared/models';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

type DashboardView = DashboardSummary & {
  kpis: Array<{ label: string; value: string | number; icon: string }>;
};

@Component({
  selector: 'app-dashboard-page',
  imports: [
    AppIconComponent,
    AsyncPipe,
    ChartModule,
    EmptyStateComponent,
    ErrorStateComponent,
    GridModule,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  providers: [
    CategoryService,
    ColumnSeriesService,
    DataLabelService,
    LegendService,
    PageService,
    SortService,
    TooltipService,
  ],
  template: `
    <section class="dashboard-page">
      <app-page-header
        title="Dashboard"
        subtitle="Executive summary for pipeline health and revenue outlook."
        icon="dashboard"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.recentOpportunities.length || state.data.stageBreakdown.length) {
          @let data = state.data;
          <div class="dashboard-kpi-card content-card">
            <div class="kpi-grid">
              @for (kpi of data.kpis; track kpi.label) {
                <div class="e-card metric-card">
                  <div class="e-card-header">
                    <div class="e-card-header-caption">
                      <div class="e-card-title metric-label">{{ kpi.label }}</div>
                    </div>
                    <span class="metric-card-icon e-card-header-icon">
                      <app-icon [name]="kpi.icon" />
                    </span>
                  </div>
                  <div class="e-card-content">
                    <div class="metric-value">{{ kpi.value }}</div>
                  </div>
                </div>
              }
            </div>
          </div>

          <div class="content-card dashboard-chart-card">
            <h2>Pipeline Stage Breakdown</h2>
            <div class="w-full overflow-x-auto">
              <ejs-chart
                width="100%"
                height="320px"
                title="Stage Breakdown"
                [primaryXAxis]="primaryXAxis()"
                [primaryYAxis]="primaryYAxis()"
                [legendSettings]="legendSettings()"
                [titleStyle]="titleStyle()"
                (axisLabelRender)="onAxisLabelRender($event)"
                (tooltipRender)="onTooltipRender($event)"
                (textRender)="onTextRender($event)"
              >
                <e-series-collection>
                  <e-series
                    [dataSource]="data.stageBreakdown"
                    xName="stage"
                    yName="value"
                    type="Column"
                    name="Pipeline Value"
                    [columnWidth]="0.6"
                    [marker]="marker()"
                  />
                </e-series-collection>
              </ejs-chart>
            </div>
          </div>

          <div class="content-card dashboard-table-card">
            <h2>Recent Opportunities</h2>
            <div class="w-full overflow-x-auto">
              <ejs-grid
                width="100%"
                height="360px"
                [dataSource]="data.recentOpportunities"
                [allowSorting]="true"
                [allowPaging]="true"
                [allowTextWrap]="true"
                [pageSettings]="{ pageSize: 8 }"
              >
                <e-columns>
                  <e-column field="name" headerText="Opportunity" width="220" />
                  <e-column field="account" headerText="Account" width="200" />
                  <e-column field="stage" headerText="Stage" width="140"
                    ><ng-template #template let-row
                      ><span [class]="statusClass(row.stage)">{{
                        stageLabel(row.stage)
                      }}</span></ng-template
                    ></e-column
                  >
                  <e-column field="expectedCloseDate" headerText="Expected Close Date" width="160"
                    ><ng-template #template let-row>{{
                      dateUs(row.expectedCloseDate)
                    }}</ng-template></e-column
                  >
                  <e-column field="amount" headerText="Amount" width="150" textAlign="Right">
                    <ng-template #template let-data>{{ usd(data.amount) }}</ng-template>
                  </e-column>
                </e-columns>
              </ejs-grid>
            </div>
          </div>
        } @else {
          <app-empty-state message="No dashboard data available." />
        }
      }
    </section>
  `,
})
export class DashboardPageComponent {
  private readonly api = inject(SalesCrmApiService);
  private readonly themeService = inject(ThemeService);
  readonly state$ = toLoadState(
    this.api.getDashboard().pipe(
      map((data): DashboardView => ({
        ...data,
        kpis: [
          { label: 'Revenue', value: formatCompactUsd(data.revenue), icon: 'revenue' },
          {
            label: 'Pipeline Value',
            value: formatCompactUsd(data.pipelineValue),
            icon: 'pipeline',
          },
          {
            label: 'Weighted Forecast',
            value: formatCompactUsd(data.weightedForecast),
            icon: 'forecast',
          },
          {
            label: 'Open Opportunities',
            value: data.stageBreakdown
              .filter((item) => !['ClosedWon', 'ClosedLost'].includes(item.stage))
              .reduce((sum, item) => sum + item.count, 0),
            icon: 'work',
          },
          {
            label: 'Win Rate',
            value: `${Math.round(data.winRate)}%`,
            icon: 'target',
          },
        ],
      })),
    ),
  );
  protected readonly usd = formatUsd;
  protected readonly dateUs = formatDateUs;
  protected readonly statusClass = statusCss;
  protected readonly stageLabel = (stage: string) => stage.replace(/([a-z])([A-Z])/g, '$1 $2');
  protected readonly primaryXAxis = computed(() => ({
    valueType: 'Category',
    labelStyle: this.chartFont('500'),
    majorGridLines: { width: 0 },
    majorTickLines: { color: this.chartAxisColor() },
    lineStyle: { color: this.chartAxisColor() },
  }));
  protected readonly primaryYAxis = computed(() => ({
    title: 'Pipeline Value',
    labelStyle: this.chartFont('500'),
    titleStyle: this.chartFont('600'),
    majorGridLines: { color: this.chartGridColor(), width: 1 },
    majorTickLines: { color: this.chartAxisColor() },
    lineStyle: { color: this.chartAxisColor() },
    minimum: 0,
    interval: 500000,
  }));
  protected readonly legendSettings = computed(() => ({
    visible: true,
    toggleVisibility: true,
    textStyle: this.chartFont('500'),
  }));
  protected readonly marker = computed(() => ({
    dataLabel: { visible: true, font: this.chartFont('700') },
  }));

  onAxisLabelRender(args: IAxisLabelRenderEventArgs) {
    if (args.axis?.name === 'primaryYAxis') args.text = formatCompactUsd(Number(args.value));
  }

  onTooltipRender(args: ITooltipRenderEventArgs) {
    args.text = `${args.point?.x}: ${formatUsd(Number(args.point?.y))}`;
  }

  onTextRender(args: ITextRenderEventArgs) {
    if (args.point?.y !== undefined) args.text = formatCompactUsd(Number(args.point.y));
  }

  protected readonly titleStyle = computed(() => this.chartFont('700', '14px'));

  private chartFont(fontWeight = '600', size = '13px') {
    return {
      fontFamily: 'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
      size,
      fontWeight,
      color: this.themeService.theme() === 'dark' ? '#e5e7eb' : '#344054',
    };
  }

  private chartGridColor() {
    return this.themeService.theme() === 'dark' ? '#334155' : '#e5e9f0';
  }

  private chartAxisColor() {
    return this.themeService.theme() === 'dark' ? '#475569' : '#d0d5dd';
  }
}
