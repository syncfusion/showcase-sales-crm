import { AsyncPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import {
  ChartModule,
  DataLabelService,
  DateTimeService,
  IAxisLabelRenderEventArgs,
  ITextRenderEventArgs,
  ITooltipRenderEventArgs,
  LegendService,
  LineSeriesService,
  TooltipService,
} from '@syncfusion/ej2-angular-charts';
import { map } from 'rxjs';
import { ThemeService } from '../../layout/theme.service';
import { formatCompactUsd, formatDateReadable, formatUsd } from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import { ErrorStateComponent, LoadingStateComponent } from '../../shared/state.component';
import { buildForecastTrend } from './forecast-trend';

@Component({
  selector: 'app-forecast-page',
  imports: [
    AsyncPipe,
    ChartModule,
    ErrorStateComponent,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  providers: [DateTimeService, DataLabelService, LegendService, LineSeriesService, TooltipService],
  template: `
    <section>
      <app-page-header
        title="Forecast"
        subtitle="Commit, best-case, pipeline, and weighted forecast summary."
        icon="forecast"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else {
          @let data = state.data;
          <div class="forecast-summary-strip">
            @for (item of data.summaryData; track item.label) {
              <div class="e-card forecast-summary-item">
                <div class="e-card-header">
                  <div class="e-card-header-caption">
                    <div class="e-card-title">{{ item.label }}</div>
                  </div>
                </div>
                <div class="e-card-content">
                  <strong>{{ compactUsd(item.value) }}</strong>
                </div>
              </div>
            }
          </div>
          <div class="content-card">
            <h2>Forecast Trend</h2>
            @if (data.hasTrend) {
              <div class="w-full overflow-x-auto">
                <ejs-chart
                  width="100%"
                  height="320px"
                  [primaryXAxis]="primaryXAxis()"
                  [primaryYAxis]="primaryYAxis()"
                  [legendSettings]="legendSettings()"
                  (axisLabelRender)="onAxisLabelRender($event)"
                  (tooltipRender)="onTooltipRender($event)"
                  (textRender)="onTextRender($event)"
                >
                  <e-series-collection>
                    <e-series
                      [dataSource]="data.actualPoints"
                      xName="x"
                      yName="y"
                      type="Line"
                      name="Actual"
                      [width]="2"
                      [fill]="actualColor()"
                      [marker]="marker()"
                    />
                    <e-series
                      [dataSource]="data.forecastPoints"
                      xName="x"
                      yName="y"
                      type="Line"
                      name="Forecast"
                      [width]="2"
                      dashArray="6,4"
                      [fill]="forecastColor()"
                      [marker]="marker()"
                    />
                  </e-series-collection>
                </ejs-chart>
              </div>
            } @else {
              <p class="muted-copy">Trend data is not available.</p>
            }
          </div>
        }
      }
    </section>
  `,
})
export class ForecastPageComponent {
  private readonly api = inject(SalesCrmApiService);
  private readonly themeService = inject(ThemeService);
  readonly state$ = toLoadState(
    this.api.getForecast().pipe(map((forecast) => buildForecastTrend(forecast))),
  );
  protected readonly compactUsd = formatCompactUsd;
  protected readonly primaryXAxis = computed(() => ({
    valueType: 'DateTime',
    labelFormat: 'MMM-yyyy',
    intervalType: 'Months',
    interval: 1,
    edgeLabelPlacement: 'Shift',
    labelStyle: this.chartFont('500'),
    majorGridLines: { width: 0 },
    majorTickLines: { color: this.chartAxisColor() },
    lineStyle: { color: this.chartAxisColor() },
  }));
  protected readonly primaryYAxis = computed(() => ({
    title: 'Weighted Forecast',
    labelStyle: this.chartFont('500'),
    titleStyle: this.chartFont('600'),
    majorGridLines: { color: this.chartGridColor(), width: 1 },
    majorTickLines: { color: this.chartAxisColor() },
    lineStyle: { color: this.chartAxisColor() },
    interval: 40000,
  }));
  protected readonly legendSettings = computed(() => ({
    visible: true,
    toggleVisibility: true,
    textStyle: this.chartFont('500'),
  }));
  protected readonly marker = computed(() => ({
    visible: true,
    width: 7,
    height: 7,
    shape: 'Circle',
    dataLabel: { visible: true, font: this.chartFont('700') },
  }));
  protected readonly actualColor = computed(() =>
    this.themeService.theme() === 'dark' ? '#93c5fd' : '#175cd3',
  );
  protected readonly forecastColor = computed(() =>
    this.themeService.theme() === 'dark' ? '#fbbf24' : '#b54708',
  );

  onAxisLabelRender(args: IAxisLabelRenderEventArgs) {
    if (args.axis?.name === 'primaryYAxis') args.text = formatCompactUsd(Number(args.value));
  }

  onTooltipRender(args: ITooltipRenderEventArgs) {
    args.text = `${formatDateReadable(args.point?.x as Date)}: ${formatUsd(Number(args.point?.y))}`;
  }

  onTextRender(args: ITextRenderEventArgs) {
    if (args.point?.y !== undefined) args.text = formatCompactUsd(Number(args.point.y));
  }

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
