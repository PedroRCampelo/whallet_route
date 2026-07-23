import type { Cargo, CargoStatus, DeliveryStatus } from './types/cargo';

// Máquina de estado da carga e da entrega — única fonte de verdade sobre
// o que pode ser feito em cada status.

export const CARGO_STATUS: Record<CargoStatus, { label: string; tone: string }> = {
  Received: { label: 'Recebida', tone: 'received' },
  Routed: { label: 'Roteirizada', tone: 'routed' },
  Dispatched: { label: 'Despachada', tone: 'dispatched' },
  Completed: { label: 'Concluída', tone: 'completed' },
};

export const DELIVERY_STATUS: Record<DeliveryStatus, { label: string; tone: string }> = {
  Pending: { label: 'Pendente', tone: 'pending' },
  EnRoute: { label: 'Em rota', tone: 'enroute' },
  Delivered: { label: 'Entregue', tone: 'delivered' },
  Refused: { label: 'Recusada', tone: 'refused' },
};

// espelha as cores de --delivery-* em variables.css — Leaflet precisa de hex, não var(--x)
export const DELIVERY_MARKER_COLOR: Record<DeliveryStatus, string> = {
  Pending: '#9aa0b0',
  EnRoute: '#b45309',
  Delivered: '#15803d',
  Refused: '#c2410c',
};

export interface CargoActions {
  route: boolean;
  assignDriver: boolean;
  assignVehicle: boolean;
  dispatch: boolean;
  cancelDispatch: boolean;
}

export function getAvailableActions(cargo: Cargo): CargoActions {
  switch (cargo.status) {
    case 'Received':
      return { route: true, assignDriver: true, assignVehicle: true, dispatch: false, cancelDispatch: false };
    case 'Routed':
      return { route: true, assignDriver: true, assignVehicle: true, dispatch: true, cancelDispatch: false };
    case 'Dispatched':
      return { route: false, assignDriver: false, assignVehicle: false, dispatch: false, cancelDispatch: true };
    case 'Completed':
    default:
      return { route: false, assignDriver: false, assignVehicle: false, dispatch: false, cancelDispatch: false };
  }
}
