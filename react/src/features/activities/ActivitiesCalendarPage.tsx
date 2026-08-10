import { ScheduleComponent, ViewsDirective, ViewDirective, Inject, Day, Week, WorkWeek, Month, Agenda } from '@syncfusion/ej2-react-schedule';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';
import React, { useState } from 'react';


export function ActivitiesCalendarPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getActivities(), []);
  const [currentDate, setCurrentDate] = useState(new Date());
  // Taller calendar on QHD / 4K so month view shows more week rows without scrolling.
  const scheduleHeight = useResponsiveHeight('640px', '720px', '800px');

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No activities scheduled." />;

  const events = data.map((activity) => ({
    Id: activity.id,
    Subject: `${activity.subject} · ${activity.account}`,
    StartTime: new Date(activity.start),
    EndTime: new Date(activity.end),
    Owner: activity.owner,
    Status: activity.status,
    CategoryColor: activity.status === 'Completed' ? '#12b76a' : '#175cd3'
  }));

  return (
    <section>
      <PageHeader title="Activities Calendar" subtitle="Schedule meetings, follow-ups, and tasks." icon={<AppIcon name="activities" />} />
      <div className="content-card">
        <div className="w-full overflow-x-auto">
          <ScheduleComponent
            eventSettings={{
              dataSource: events,
              fields: {
                id: 'Id',
                subject: { name: 'Subject' },
                startTime: { name: 'StartTime' },
                endTime: { name: 'EndTime' }
              }
            }}
            selectedDate={currentDate}
            currentView="Month"
            height={scheduleHeight}
          >
            <ViewsDirective>
              <ViewDirective option="Day" />
              <ViewDirective option="Week" />
              <ViewDirective option="WorkWeek" />
              <ViewDirective option="Month" />
              <ViewDirective option="Agenda" />
            </ViewsDirective>
            <Inject services={[Day, Week, WorkWeek, Month, Agenda]} />
          </ScheduleComponent>
        </div>
      </div>
    </section>
  );
}
