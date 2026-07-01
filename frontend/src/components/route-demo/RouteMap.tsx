import { forwardRef, useEffect, useImperativeHandle, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { pinDivIcon, roadGeometry } from './map-utils';
import type { MapPoint } from './types';

export interface RouteMapHandle {
  renderRoute: (points: MapPoint[]) => Promise<void>;
  clear: () => void;
}

export const RouteMap = forwardRef<RouteMapHandle>(function RouteMap(_props, ref) {
  const containerRef = useRef<HTMLDivElement>(null);
  const mapRef = useRef<L.Map | null>(null);
  const layersRef = useRef<L.Layer[]>([]);

  useEffect(() => {
    if (!containerRef.current || mapRef.current) return;
    const map = L.map(containerRef.current, { zoomControl: true, attributionControl: true }).setView([-8.05, -34.9], 12);
    L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
      attribution: '&copy; OpenStreetMap &copy; CARTO',
      maxZoom: 19,
    }).addTo(map);
    mapRef.current = map;

    const fixMap = () => map.invalidateSize();
    const fixTimer = window.setTimeout(fixMap, 200);
    window.addEventListener('resize', fixMap);

    return () => {
      window.clearTimeout(fixTimer);
      window.removeEventListener('resize', fixMap);
      map.remove();
      mapRef.current = null;
    };
  }, []);

  const clearLayers = () => {
    const map = mapRef.current;
    if (!map) return;
    layersRef.current.forEach((layer) => map.removeLayer(layer));
    layersRef.current = [];
  };

  useImperativeHandle(
    ref,
    () => ({
      clear: clearLayers,
      async renderRoute(points) {
        const map = mapRef.current;
        if (!map || points.length === 0) return;
        clearLayers();

        points.forEach((p) => {
          const marker = L.marker([p.lat, p.lng], { icon: pinDivIcon(p.order, p.isOrigin) }).addTo(map);
          layersRef.current.push(marker);
        });

        let line: [number, number][];
        try {
          line = await roadGeometry(points);
        } catch {
          line = points.map((p): [number, number] => [p.lat, p.lng]);
        }
        const polyline = L.polyline(line, { color: '#5b3df5', weight: 4.5, opacity: 0.95, className: 'wr-route' }).addTo(map);
        layersRef.current.push(polyline);

        map.fitBounds(
          L.latLngBounds(points.map((p): [number, number] => [p.lat, p.lng])),
          { padding: [55, 55] },
        );
      },
    }),
    [],
  );

  return <div ref={containerRef} className="wr-map" />;
});
