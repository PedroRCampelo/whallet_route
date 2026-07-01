import logo from '../../assets/logo.svg';
import './Nav.css';

export function Nav() {
  return (
    <nav className="nav no-print" id="top">
      <div className="wrap nav-inner">
        <a className="brand" href="#top" aria-label="Whallet Route">
          <img src={logo} width={32} height={32} alt="" aria-hidden="true" />
          <span className="brand-name">
            Whallet<span> Route</span>
          </span>
        </a>
        <div className="nav-links">
          <a href="#roteirizar">Roteirizar</a>
          <a href="#como">Como funciona</a>
          <a href="#api">API</a>
          <a className="btn btn-dark" href="#roteirizar">
            Começar
          </a>
        </div>
      </div>
    </nav>
  );
}
