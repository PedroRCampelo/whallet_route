import { Outlet } from 'react-router-dom';
import { Nav } from '../components/marketing/Nav';
import { Footer } from '../components/marketing/Footer';

export function MarketingLayout() {
  return (
    <>
      <Nav />
      <main>
        <Outlet />
      </main>
      <Footer />
    </>
  );
}
