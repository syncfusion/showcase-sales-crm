import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  AgendaService,
  DayService,
  MonthService,
  ScheduleModule,
  WeekService,
  WorkWeekService,
} from '@syncfusion/ej2-angular-schedule';
import { map } from 'rxjs';
import { toLoadState } from '../../shared/load-state';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';

@Component({
  selector: 'app-activities-calendar-page',
  imports: [
    AsyncPipe,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    PageHeaderComponent,
    ScheduleModule,
  ],
  providers: [AgendaService, DayService, MonthService, WeekService, WorkWeekService],
  template: `
    <section class="fit-page activities-page">
      <app-page-header
        title="Activities Calendar"
        subtitle="Schedule meetings, follow-ups, and tasks."
        icon="activities"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.length) {
          <div class="content-card schedule-card">
            <ejs-schedule
              height="100%"
              currentView="Month"
              [selectedDate]="selectedDate"
              [eventSettings]="{ dataSource: state.data, fields: eventFields }"
            >
              <e-views>
                <e-view option="Day" />
                <e-view option="Week" />
                <e-view option="WorkWeek" />
                <e-view option="Month" />
                <e-view option="Agenda" />
              </e-views>
            </ejs-schedule>
          </div>
        } @else {
          <app-empty-state message="No activities scheduled." />
        }
      }
    </section>
  `,
})
export class ActivitiesCalendarPageComponent {
  private readonly api = inject(SalesCrmApiService);
  readonly selectedDate = new Date();
  readonly eventFields = {
    id: 'Id',
    subject: { name: 'Subject' },
    startTime: { name: 'StartTime' },
    endTime: { name: 'EndTime' },
  };
  readonly state$ = toLoadState(
    this.api.getActivities().pipe(
      map((activities) =>
        activities.map((activity) => ({
          Id: activity.id,
          Subject: `${activity.subject} · ${activity.account}`,
          StartTime: new Date(activity.start),
          EndTime: new Date(activity.end),
          Owner: activity.owner,
          Status: activity.status,
          CategoryColor: activity.status === 'Completed' ? '#12b76a' : '#175cd3',
        })),
      ),
    ),
  );
}
