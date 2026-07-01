import { DEFAULT_REQUEST, SAMPLE_RESPONSE } from '../route-demo/sample-data';
import './ApiDocs.css';

const REQUEST_FIELDS = [
  { name: 'origin.id', desc: 'Identificador do centro de distribuição.' },
  { name: 'origin.address', desc: 'Endereço de partida em texto livre.' },
  { name: 'stops[].id', desc: 'Código do cliente / ponto de entrega.' },
  { name: 'stops[].address', desc: 'Endereço da parada a ser visitada.' },
  { name: 'stops[].latitude', desc: 'Alternativa ao endereço: coordenada pronta.' },
];

const RESPONSE_FIELDS = [
  { name: 'totalDistanceMeters', desc: 'Distância total da rota, em metros.' },
  { name: 'totalDurationSeconds', desc: 'Tempo total estimado, em segundos.' },
  { name: 'stops[].order', desc: 'Posição da parada na sequência ótima.' },
  { name: 'stops[].latitude', desc: 'Coordenada resolvida da parada.' },
  { name: 'stops[].legDistanceMeters', desc: 'Distância do trecho até a parada.' },
];

const REQUEST_JSON = JSON.stringify(DEFAULT_REQUEST, null, 2);
const RESPONSE_JSON = JSON.stringify(SAMPLE_RESPONSE, null, 2);

interface ApiDocsProps {
  onCopy: (text: string, message: string) => void;
}

export function ApiDocs({ onCopy }: ApiDocsProps) {
  return (
    <section className="api no-print" id="api">
      <div className="wrap">
        <div className="sec-head">
          <span className="sec-num">03</span>
          <div>
            <p className="eyebrow">WAYPOINT 03 · API</p>
            <h2>Uma API. Uma chamada.</h2>
            <p>Envie endereços. Receba coordenadas, ordem ótima, distância e tempo por trecho. Sem SDK, sem fricção.</p>
          </div>
        </div>

        <div className="api-bar">
          <span className="pill-post">POST</span>
          <span className="api-url">/v1/routes/optimize</span>
          <span className="api-ct">Content-Type: application/json</span>
        </div>

        <div className="code-grid">
          <div className="code-card">
            <div className="code-head">
              <span>REQUEST</span>
              <button className="code-copy" onClick={() => onCopy(REQUEST_JSON, 'Request copiado')}>
                copiar
              </button>
            </div>
            <pre className="code-body">{REQUEST_JSON}</pre>
          </div>
          <div className="code-card">
            <div className="code-head">
              <span>RESPONSE · 200</span>
              <button className="code-copy" onClick={() => onCopy(RESPONSE_JSON, 'Response copiado')}>
                copiar
              </button>
            </div>
            <pre className="code-body">{RESPONSE_JSON}</pre>
          </div>
        </div>

        <div className="fields-grid">
          <div className="fields">
            <h3>Campos da requisição</h3>
            <div className="fields-list">
              {REQUEST_FIELDS.map((f) => (
                <div className="field-row" key={f.name}>
                  <span className="field-name">{f.name}</span>
                  <span className="field-desc">{f.desc}</span>
                </div>
              ))}
            </div>
          </div>
          <div className="fields">
            <h3>Campos da resposta</h3>
            <div className="fields-list">
              {RESPONSE_FIELDS.map((f) => (
                <div className="field-row" key={f.name}>
                  <span className="field-name">{f.name}</span>
                  <span className="field-desc">{f.desc}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
