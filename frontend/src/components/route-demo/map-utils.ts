import L from 'leaflet';
import type { MapPoint } from './types';

export function km(meters: number): string {
  return (meters / 1000).toFixed(1).replace('.', ',') + ' km';
}

export function leg(meters: number): string {
  return meters < 1000 ? `${meters} m` : km(meters);
}

export function mins(seconds: number): string {
  return `${Math.round(seconds / 60)} min`;
}

export function pinDivIcon(order: number | undefined, isOrigin?: boolean): L.DivIcon {
  const className = isOrigin ? 'wr-pin wr-pin--origin' : 'wr-pin';
  return L.divIcon({
    html: `<div class="${className}">${isOrigin ? 'CD' : order}</div>`,
    className: '',
    iconSize: [30, 30],
    iconAnchor: [15, 15],
  });
}

/** Traces road geometry between ordered points via OSRM's public demo server. */
export async function roadGeometry(points: MapPoint[]): Promise<[number, number][]> {
  const coords = points.map((p) => `${p.lng},${p.lat}`).join(';');
  const res = await fetch(`https://router.project-osrm.org/route/v1/driving/${coords}?overview=full&geometries=geojson`);
  if (!res.ok) throw new Error('osrm request failed');
  const data = await res.json();
  if (!data.routes?.length) throw new Error('osrm returned no routes');
  return data.routes[0].geometry.coordinates.map((c: [number, number]) => [c[1], c[0]]);
}
