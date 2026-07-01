import { useEffect, useMemo, useRef, useState } from 'react';
import { RouteMap, type RouteMapHandle } from './RouteMap';
import { useRouteOptimizer } from './useRouteOptimizer';
import { km, leg, mins } from './map-utils';
import type { MapPoint } from './types';
import type { RouteRequest, RoutePointInput } from '../../types/route';
import './RouteDemo.css';

function parseOriginId(jsonText: string): string {
  try {
    const parsed = JSON.parse(jsonText) as RouteRequest;
    return parsed.origin?.id || 'DC';
  } catch {
    return 'DC';
  }
}

/** Fallback coordinates taken from the request itself, used when the API response omits lat/lng. */
function inputCoords(jsonText: string): Record<string, { lat: number; lng: number }> {
  const out: Record<string, { lat: number; lng: number }> = {};
  try {
    const parsed = JSON.parse(jsonText) as RouteRequest;
    const points: RoutePointInput[] = [parsed.origin, ...(parsed.stops || [])];
    points.forEach((p) => {
      if (p && p.latitude != null && p.longitude != null) {
        out[p.id] = { lat: p.latitude, lng: p.longitude };
      }
    });
  } catch {
    // invalid JSON while typing — no fallback coords available yet
  }
  return out;
}

interface RouteDemoSectionProps {
  onCopy: (text: string, message: string) => void;
}

export function RouteDemoSection({ onCopy }: RouteDemoSectionProps) {
  const mapRef = useRef<RouteMapHandle>(null);
  const { apiUrl, setApiUrl, jsonText, setJsonText, loading, error, response, optimize } = useRouteOptimizer();
  const [mapHasRoute, setMapHasRoute] = useState(false);

  useEffect(() => {
    if (!response) return;
    const fallback = inputCoords(jsonText);
    const coordOf = (p: { id: string; latitude?: number; longitude?: number }) =>
      p.latitude != null && p.longitude != null ? { lat: p.latitude, lng: p.longitude } : fallback[p.id];

    const originCoord = coordOf(response.origin);
    const origin: MapPoint | null = originCoord ? { id: response.origin.id, ...originCoord, isOrigin: true } : null;
    const stops: MapPoint[] = response.stops
      .map((s): MapPoint | null => {
        const coord = coordOf(s);
        return coord ? { id: s.id, order: s.order, ...coord } : null;
      })
      .filter((p): p is MapPoint => p !== null);

    const points = origin ? [origin, ...stops] : stops;
    if (points.length === 0) return;

    setMapHasRoute(true);
    void mapRef.current?.renderRoute(points);
    // fallback coords depend on jsonText at the time of the response, not on later edits
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [response]);

  const originId = useMemo(() => (response ? response.origin.id : parseOriginId(jsonText)), [response, jsonText]);

  function handleCopyJson() {
    const payload = response ? JSON.stringify(response, null, 2) : jsonText;
    onCopy(payload, 'JSON copiado');
  }

  return (
    <section className="demo" id="roteirizar">
      <div className="wrap">
        <div className="sec-head no-print">
          <span className="sec-num">01</span>
          <div>
            <p className="eyebrow">WAYPOINT 01 · ROTEIRIZAR</p>
            <h2>Cole o JSON. Veja a rota.</h2>
            <p>Sua requisição vira um mapa interativo e uma sequência de entrega otimizada — pronta para relatório.</p>
          </div>
        </div>

        <div className="demo-card">
          <div className="demo-panel">
            <div className="no-print">
              <p className="field-label">ENDPOINT</p>
              <input className="input" value={apiUrl} spellCheck={false} onChange={(e) => setApiUrl(e.target.value)} />
            </div>
            <div className="no-print">
              <p className="field-label">REQUISIÇÃO</p>
              <textarea className="editor" spellCheck={false} value={jsonText} onChange={(e) => setJsonText(e.target.value)} />
            </div>
            <button className="btn btn-primary demo-run no-print" disabled={loading} onClick={() => void optimize()}>
              {loading ? 'Otimizando...' : 'Otimizar rota'}
            </button>

            {error && <div className="note note--error is-on no-print">{error}</div>}

            <div className="stats">
              <div className="stat">
                <b>{response ? km(response.totalDistanceMeters) : '—'}</b>
                <small>Distância otimizada</small>
              </div>
              <div className="stat">
                <b>{response ? mins(response.totalDurationSeconds) : '—'}</b>
                <small>Tempo estimado</small>
              </div>
            </div>

            <div>
              <p className="seq-label">SEQUÊNCIA DE ENTREGA</p>
              <div>
                <div className="seq-row">
                  <div className="chip chip--origin">CD</div>
                  <div className="seq-id">{originId}</div>
                  <div className="seq-leg">origem</div>
                </div>
                {response?.stops.map((s) => (
                  <div className="seq-row" key={s.id}>
                    <div className="chip">{s.order}</div>
                    <div className="seq-id">{s.id}</div>
                    <div className="seq-leg">{leg(s.legDistanceMeters)}</div>
                  </div>
                ))}
              </div>
            </div>

            <div className="demo-foot no-print">
              <button className="btn btn-ghost" onClick={() => window.print()}>
                <svg width="14" height="14" viewBox="0 0 16 16" fill="none" aria-hidden="true">
                  <path
                    d="M8 1v8m0 0 3-3M8 9 5 6M2.5 11v2.5h11V11"
                    stroke="currentColor"
                    strokeWidth="1.5"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
                Exportar PDF
              </button>
              <button className="btn btn-ghost" onClick={handleCopyJson}>
                <svg width="14" height="14" viewBox="0 0 16 16" fill="none" aria-hidden="true">
                  <rect x="5" y="5" width="8" height="9" rx="1.6" stroke="currentColor" strokeWidth="1.5" />
                  <path d="M3 10.5V3a1 1 0 0 1 1-1h6" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
                </svg>
                Copiar JSON
              </button>
            </div>
          </div>

          <div className="demo-map">
            <RouteMap ref={mapRef} />
            {!mapHasRoute && (
              <div className="map-empty">
                <span>A rota aparece aqui após otimizar</span>
              </div>
            )}
          </div>
        </div>
      </div>
    </section>
  );
}
