import { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Polyline } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { DELIVERY_MARKER_COLOR } from '../../cargoState';
import type { Delivery } from '../../types/cargo';
import './CargoMap.css';

const OSRM_URL = 'https://router.project-osrm.org/route/v1/driving/';

type MapDelivery = Delivery & { latitude: number; longitude: number };

function markerIcon(order: number | string, color: string) {
  return L.divIcon({
    className: 'cargo-map-marker',
    html: `<span style="background:${color}">${order}</span>`,
    iconSize: [28, 28],
    iconAnchor: [14, 14],
  });
}

interface CargoMapProps {
  deliveries: Delivery[];
}

export function CargoMap({ deliveries }: CargoMapProps) {
  const points = deliveries.filter(
    (d): d is MapDelivery => d.latitude != null && d.longitude != null
  );
  const [routeLine, setRouteLine] = useState<[number, number][] | null>(null);

  useEffect(() => {
    if (points.length < 2) {
      setRouteLine(null);
      return;
    }
    let cancelled = false;
    const coords = points.map((p) => `${p.longitude},${p.latitude}`).join(';');
    fetch(`${OSRM_URL}${coords}?overview=full&geometries=geojson`)
      .then((res) => {
        if (!res.ok) throw new Error('osrm failed');
        return res.json();
      })
      .then((data) => {
        if (cancelled) return;
        const geometry: [number, number][] | undefined = data.routes?.[0]?.geometry?.coordinates;
        if (!geometry) throw new Error('sem rota');
        setRouteLine(geometry.map(([lng, lat]) => [lat, lng]));
      })
      .catch(() => {
        if (!cancelled) setRouteLine(points.map((p): [number, number] => [p.latitude, p.longitude]));
      });
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [points.map((p) => `${p.id}:${p.latitude}:${p.longitude}`).join('|')]);

  if (points.length === 0) {
    return (
      <div className="cargo-map cargo-map--empty">
        <p>Roteirize a carga para ver as entregas no mapa.</p>
      </div>
    );
  }

  const bounds: [number, number][] = points.map((p) => [p.latitude, p.longitude]);

  return (
    <MapContainer bounds={bounds} boundsOptions={{ padding: [40, 40] }} className="cargo-map" scrollWheelZoom>
      <TileLayer
        url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
        attribution="&copy; OpenStreetMap &copy; CARTO"
      />
      {routeLine && (
        <Polyline positions={routeLine} pathOptions={{ color: '#5b3df5', weight: 3, dashArray: '6 8' }} />
      )}
      {points.map((delivery) => (
        <Marker
          key={delivery.id}
          position={[delivery.latitude, delivery.longitude]}
          icon={markerIcon(delivery.order ?? '?', DELIVERY_MARKER_COLOR[delivery.status] ?? '#9aa0b0')}
        />
      ))}
    </MapContainer>
  );
}
