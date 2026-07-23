import { createBrowserRouter } from 'react-router-dom';
import { MarketingLayout } from './layouts/MarketingLayout';
import { PortalLayout } from './layouts/PortalLayout';
import { LandingPage } from './pages/LandingPage';
import { CargasListPage } from './pages/cargas/CargasListPage';
import { CargaNewPage } from './pages/cargas/CargaNewPage';
import { CargaDetailPage } from './pages/cargas/CargaDetailPage';
import { FleetPage } from './pages/FleetPage';

export const router = createBrowserRouter([
  {
    element: <MarketingLayout />,
    children: [{ index: true, element: <LandingPage /> }],
  },
  {
    element: <PortalLayout />,
    children: [
      { path: 'cargas', element: <CargasListPage /> },
      { path: 'cargas/nova', element: <CargaNewPage /> },
      { path: 'cargas/:id', element: <CargaDetailPage /> },
      { path: 'frota', element: <FleetPage /> },
    ],
  },
]);
