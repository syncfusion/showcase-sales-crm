import { MapsComponent, LayersDirective, LayerDirective, Inject, Marker, MapsTooltip, Zoom } from '@syncfusion/ej2-react-maps';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';
import { formatUsd } from '../../shared/utils/formatters';
import { WinRateBadge } from '../../shared/components/Badges';

export function TerritoryMapPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getTerritories(), []);
  // Taller map on QHD / 4K so the wide viewport does not squash thecoverage area.
  const mapHeight = useResponsiveHeight('480px', '540px', '600px');
  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No territory data available." />;

  return (
    <section className="territory-page">
      <PageHeader title="Territory Map" subtitle="Visualize coverage and performance by territory." icon={<AppIcon name="territories" />} />
      <div className="territory-layout">
        <div className="content-card territory-map-card">
          <div className="w-full overflow-hidden">
            <MapsComponent
              height={mapHeight}
              centerPosition={{ latitude: 39.5, longitude: -98.35 }}
              zoomSettings={{ enable: true, zoomFactor: 4, mouseWheelZoom: true, pinchZooming: true }}
            >
              <LayersDirective>
                <LayerDirective
                  urlTemplate="https://tile.openstreetmap.org/level/tileX/tileY.png"
                  markerSettings={[
                    {
                      visible: true,
                      dataSource: data,
                      latitudeValuePath: 'latitude',
                      longitudeValuePath: 'longitude',
                      shape: 'Circle',
                      height: 14,
                      width: 14,
                      fill: '#175cd3',
                      border: { color: '#ffffff', width: 2 },
                      tooltipSettings: {
                        visible: true,
                        valuePath: 'name'
                      }
                    }
                  ]}
                />
              </LayersDirective>
              <Inject services={[Marker, MapsTooltip, Zoom]} />
            </MapsComponent>
          </div>
        </div>
        <div className="territory-summary">
          {data.slice(0, 6).map((territory) => (
            <div className="info-card territory-card" key={territory.id}>
              <strong>{territory.name}</strong>
              <span>{territory.region}</span>
              <div>
                <b>{formatUsd(territory.revenue)}</b>
                <small>{territory.accounts} accounts · <WinRateBadge value={territory.winRate} /></small>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
