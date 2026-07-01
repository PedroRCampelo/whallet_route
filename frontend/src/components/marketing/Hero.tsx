import './Hero.css';

export function Hero() {
  return (
    <header className="hero no-print">
      <div className="wrap hero-grid">
        <div>
          <div className="hero-tag">
            <svg width="11" height="11" viewBox="0 0 12 12" aria-hidden="true">
              <rect x="6" y="0" width="6" height="6" transform="rotate(45 6 6)" fill="#5b3df5" />
            </svg>
            <span>origem · CD · −8.119, −34.905 · recife–pe</span>
          </div>
          <h1 className="display">
            A rota mais econômica, pronta para integrar ao seu <span>ERP</span>.
          </h1>
          <p className="hero-sub">
            Envie origem e paradas. Receba a sequência ótima de entrega, distância, tempo, e um mapa pronto para
            mostrar ao cliente.
          </p>
          <div className="hero-actions">
            <a className="btn btn-primary" href="#roteirizar">
              Roteirizar agora
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none" aria-hidden="true">
                <path
                  d="M3 8h10m0 0-4-4m4 4-4 4"
                  stroke="currentColor"
                  strokeWidth="1.6"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            </a>
            <a className="btn btn-ghost" href="#api">
              Ver a documentação
            </a>
          </div>
          <div className="hero-meta">
            <span className="pill-post">POST</span>
            <span>/v1/routes/optimize · resposta em segundos</span>
          </div>
        </div>

        <div className="hero-visual">
          <svg className="hero-map" viewBox="0 0 520 460" preserveAspectRatio="xMidYMid slice" aria-hidden="true">
            <g stroke="#e7e6f3" strokeWidth="1">
              <path d="M0 92H520M0 184H520M0 276H520M0 368H520M104 0V460M208 0V460M312 0V460M416 0V460" />
            </g>
            <g fill="none" stroke="#d9d6f0" strokeWidth="1.2" opacity=".9">
              <path d="M-20 300 C90 250 120 360 230 320 C340 280 360 180 470 150" />
              <path d="M-20 360 C90 310 120 420 230 380 C340 340 360 240 470 210" />
            </g>
            <path
              className="wr-routeline"
              d="M86 392 C150 360 150 300 214 296 C300 290 300 196 360 192 C424 188 416 110 462 92"
              fill="none"
              stroke="#5b3df5"
              strokeWidth="3.5"
              strokeDasharray="2 11"
              strokeLinecap="round"
            />
            <g fill="#fff" fontFamily="Space Grotesk, sans-serif" fontSize="13" fontWeight="600" textAnchor="middle">
              <rect x="69" y="375" width="34" height="34" rx="9" fill="#14161d" />
              <text x="86" y="397">
                CD
              </text>
              <circle cx="214" cy="296" r="15" fill="#5b3df5" stroke="#fff" strokeWidth="2" />
              <text x="214" y="301">
                2
              </text>
              <circle cx="360" cy="192" r="15" fill="#5b3df5" stroke="#fff" strokeWidth="2" />
              <text x="360" y="197">
                3
              </text>
              <circle cx="462" cy="92" r="15" fill="#5b3df5" stroke="#fff" strokeWidth="2" />
              <text x="462" y="97">
                5
              </text>
            </g>
          </svg>
          <div className="hero-cards">
            <div className="hero-card">
              <b>37,5 km</b>
              <small>distância otimizada</small>
            </div>
            <div className="hero-card">
              <b>75 min</b>
              <small>tempo estimado</small>
            </div>
          </div>
        </div>
      </div>

      <svg className="wave" viewBox="0 0 1440 150" width="100%" preserveAspectRatio="none" aria-hidden="true">
        <path
          d="M0,84 C240,30 480,128 720,84 C960,40 1200,128 1440,84 L1440,150 L0,150 Z"
          fill="#f5f4fb"
        />
        <path
          className="wr-routeline"
          d="M0,84 C240,30 480,128 720,84 C960,40 1200,128 1440,84"
          fill="none"
          stroke="#5b3df5"
          strokeWidth="2.4"
          strokeDasharray="2 12"
          strokeLinecap="round"
          opacity=".5"
        />
      </svg>
    </header>
  );
}
