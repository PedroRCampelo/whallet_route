import { apiClient } from './client';
import type {
  Cargo,
  CargoSummary,
  CreateCargoPayload,
  DeliverPayload,
  RefusePayload,
} from '../types/cargo';

export const cargosApi = {
  list: () => apiClient.get<CargoSummary[]>('/v1/cargos'),
  get: (id: string) => apiClient.get<Cargo>(`/v1/cargos/${id}`),
  create: (payload: CreateCargoPayload) => apiClient.post<Cargo>('/v1/cargos', payload),
  route: (id: string) => apiClient.post<Cargo>(`/v1/cargos/${id}/route`),
  dispatch: (id: string) => apiClient.post<Cargo>(`/v1/cargos/${id}/dispatch`),
  cancelDispatch: (id: string) => apiClient.post<Cargo>(`/v1/cargos/${id}/cancel-dispatch`),
  assignDriver: (id: string, driverId: string) =>
    apiClient.post<Cargo>(`/v1/cargos/${id}/assign-driver`, { driverId }),
  assignVehicle: (id: string, vehicleId: string) =>
    apiClient.post<Cargo>(`/v1/cargos/${id}/assign-vehicle`, { vehicleId }),
  deliver: (cargoId: string, deliveryId: string, payload: DeliverPayload) =>
    apiClient.post<Cargo>(`/v1/cargos/${cargoId}/deliveries/${deliveryId}/deliver`, payload),
  refuse: (cargoId: string, deliveryId: string, payload: RefusePayload) =>
    apiClient.post<Cargo>(`/v1/cargos/${cargoId}/deliveries/${deliveryId}/refuse`, payload),
};
