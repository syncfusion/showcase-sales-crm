import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  MapsModule,
  MarkerService,
  MapsTooltipService,
  ZoomService,
} from '@syncfusion/ej2-angular-maps';
import { formatUsd, metricCss } from '../../shared/formatters';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-territory-map-page',
  imports: [
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    MapsModule,
    PageHeaderComponent,
  ],
  providers: [MarkerService, MapsTooltipService, ZoomService],
  template: `
    <section class="territory-page">
      <app-page-header
        title="Territory Map"
        subtitle="Visualize coverage and performance by territory."
        icon="territories"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.length) {
          @let territories = state.data;
          <div class="territory-layout">
            <div class="content-card territory-map-card">
              <ejs-maps
                width="100%"
                height="100%"
                [margin]="{ left: 0, right: 0, top: 0, bottom: 0 }"
                [centerPosition]="{ latitude: 39.5, longitude: -98.35 }"
                [zoomSettings]="{
                  enable: true,
                  zoomFactor: 4,
                  mouseWheelZoom: true,
                  pinchZooming: true,
                }"
              >
                <e-layers>
                  <e-layer
                    urlTemplate="https://tile.openstreetmap.org/level/tileX/tileY.png"
                    [markerSettings]="[
                      {
                        visible: true,
                        dataSource: territories,
                        latitudeValuePath: 'latitude',
                        longitudeValuePath: 'longitude',
                        shape: 'Circle',
                        height: 14,
                        width: 14,
                        fill: '#175cd3',
                        border: { color: '#ffffff', width: 2 },
                        tooltipSettings: { visible: true, valuePath: 'name' },
                      },
                    ]"
                  />
                </e-layers>
              </ejs-maps>
            </div>
            <div class="territory-summary">
              @for (territory of territories; track territory.id) {
                <div class="info-card territory-card">
                  <strong>{{ territory.name }}</strong>
                  <span>{{ territory.region }}</span>
                  <div>
                    <b>{{ usd(territory.revenue) }}</b>
                    <small
                      >{{ territory.accounts }} accounts ·
                      <span [class]="metricClass(territory.winRate)"
                        >{{ territory.winRate }}% win rate</span
                      ></small
                    >
                  </div>
                </div>
              }
            </div>
          </div>
        } @else {
          <app-empty-state message="No territory data available." />
        }
      }
    </section>
  `,
})
export class TerritoryMapPageComponent {
  private readonly api = inject(SalesCrmApiService);
  readonly state$ = toLoadState(this.api.getTerritories());
  protected readonly usd = formatUsd;
  protected readonly metricClass = metricCss;
}
