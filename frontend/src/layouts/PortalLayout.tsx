import { NavLink, Outlet } from 'react-router-dom';
import logo from '../assets/logo.svg';
import './PortalLayout.css';

export function PortalLayout() {
  return (
    <div className="portal">
      <header className="portal-topbar">
        <div className="portal-brand">
          <img src={logo} width={28} height={28} alt="" aria-hidden="true" />
          <span className="portal-brand-name">
            Whallet<span> Route</span>
          </span>
        </div>
        <nav className="portal-nav">
          <NavLink to="/cargas" className={({ isActive }) => (isActive ? 'is-active' : '')}>
            Cargas
          </NavLink>
          <NavLink to="/frota" className={({ isActive }) => (isActive ? 'is-active' : '')}>
            Frota
          </NavLink>
        </nav>
      </header>
      <main className="portal-main">
        <Outlet />
      </main>
    </div>
  );
}
