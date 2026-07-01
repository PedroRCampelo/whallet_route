import { createBrowserRouter } from 'react-router-dom';
import { MarketingLayout } from './layouts/MarketingLayout';
import { PortalLayout } from './layouts/PortalLayout';
import { LandingPage } from './pages/LandingPage';
import { CargasListPage } from './pages/cargas/CargasListPage';
import { CargaDetailPage } from './pages/cargas/CargaDetailPage';

export const router = createBrowserRouter([
  {
    element: <MarketingLayout />,
    children: [{ index: true, element: <LandingPage /> }],
  },
  {
    path: 'cargas',
    element: <PortalLayout />,
    children: [
      { index: true, element: <CargasListPage /> },
      { path: ':id', element: <CargaDetailPage /> },
    ],
  },
]);
