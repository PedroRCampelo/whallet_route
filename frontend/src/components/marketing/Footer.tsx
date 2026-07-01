import logo from '../../assets/logo.svg';
import './Footer.css';

export function Footer() {
  return (
    <footer className="footer no-print">
      <div className="footer-inner wrap">
        <div className="brand">
          <img src={logo} width={24} height={24} alt="" aria-hidden="true" />
          <span className="brand-name" style={{ fontSize: 16 }}>
            Whallet<span> Route</span>
          </span>
        </div>
        <small>Roteirização para ERP · feito no Recife</small>
      </div>
    </footer>
  );
}
