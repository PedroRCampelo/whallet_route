import './HowItWorks.css';

const STEPS = [
  {
    n: 1,
    title: 'Você envia',
    text: 'Um POST com origem e paradas em JSON — endereços ou coordenadas, como preferir.',
  },
  {
    n: 2,
    title: 'Nós otimizamos',
    text: 'Calculamos a sequência de menor custo, com distância e tempo de cada trecho.',
  },
  {
    n: 3,
    title: 'Você visualiza',
    text: 'Mapa interativo e relatório em PDF, prontos para integrar e mostrar ao cliente.',
  },
];

export function HowItWorks() {
  return (
    <section className="how no-print" id="como">
      <svg className="how-bg" viewBox="0 0 1440 320" width="100%" height="100%" preserveAspectRatio="xMidYMid slice" aria-hidden="true">
        <g fill="none" stroke="#6b54ff" strokeOpacity=".35" strokeWidth="1.2">
          <path d="M-40 240 C200 180 280 300 520 240 C760 180 840 300 1080 240 C1240 200 1340 270 1480 240" />
          <path d="M-40 280 C200 220 280 340 520 280 C760 220 840 340 1080 280 C1240 240 1340 310 1480 280" />
        </g>
      </svg>
      <div className="wrap">
        <div className="how-head">
          <span className="sec-num">02</span>
          <div>
            <p className="eyebrow">WAYPOINT 02 · COMO FUNCIONA</p>
            <h2>Da requisição ao relatório, em uma chamada.</h2>
          </div>
        </div>
        <div className="steps">
          <div className="steps-rail" aria-hidden="true" />
          {STEPS.map((step) => (
            <div className="step" key={step.n}>
              <div className="step-n">{step.n}</div>
              <h3>{step.title}</h3>
              <p>{step.text}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
