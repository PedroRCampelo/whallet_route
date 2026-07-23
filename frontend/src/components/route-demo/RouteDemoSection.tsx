import { useEffect, useRef, useState } from 'react';
import { RouteMap, type RouteMapHandle } from './RouteMap';
import { useRouteOptimizer } from './useRouteOptimizer';
import { km, leg, mins } from './map-utils';
import { DEFAULT_REQUEST_JSON } from './sample-data';
import type { MapPoint } from './types';
import './RouteDemo.css';

interface RouteDemoSectionProps {
  onCopy: (text: string, message: string) => void;
}

export function RouteDemoSection({ onCopy }: RouteDemoSectionProps) {
  const mapRef = useRef<RouteMapHandle>(null);
  const { loading, response, optimize } = useRouteOptimizer();
  const [mapHasRoute, setMapHasRoute] = useState(false);

  useEffect(() => {
    if (!response) return;

    const origin: MapPoint = { id: response.origin.id, lat: response.origin.latitude, lng: response.origin.longitude, isOrigin: true };
    const stops: MapPoint[] = response.stops.map((s) => ({ id: s.id, order: s.order, lat: s.latitude, lng: s.longitude }));

    setMapHasRoute(true);
    void mapRef.current?.renderRoute([origin, ...stops]);
  }, [response]);

  const originId = response ? response.origin.id : 'DC';

  function handleCopyJson() {
    const payload = response ? JSON.stringify(response, null, 2) : DEFAULT_REQUEST_JSON;
    onCopy(payload, 'JSON copiado');
  }

  return (
    <section className="demo" id="roteirizar">
      <div className="wrap">
        <div className="sec-head no-print">
          <span className="sec-num">01</span>
          <div>
            <p className="eyebrow">WAYPOINT 01 · ROTEIRIZAR</p>
            <h2>Veja como fica a rota otimizada.</h2>
            <p>A requisição vira um mapa interativo e uma sequência de entrega otimizada — pronta para relatório.</p>
          </div>
        </div>

        <div className="demo-card">
          <div className="demo-panel">
            <div className="no-print">
              <p className="field-label">REQUISIÇÃO · EXEMPLO</p>
              <textarea className="editor editor--readonly" spellCheck={false} readOnly value={DEFAULT_REQUEST_JSON} />
            </div>
            <button className="btn btn-primary demo-run no-print" disabled={loading} onClick={() => void optimize()}>
              {loading ? 'Otimizando...' : 'Otimizar rota'}
            </button>
            <p className="demo-static-note no-print">
              Simulação com dados de exemplo.
            </p>

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
