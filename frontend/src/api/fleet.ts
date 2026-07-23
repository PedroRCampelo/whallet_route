import { apiClient } from './client';
import type { CreateDriverPayload, CreateVehiclePayload, Driver, Vehicle } from '../types/cargo';

export const driversApi = {
  list: () => apiClient.get<Driver[]>('/v1/drivers'),
  get: (id: string) => apiClient.get<Driver>(`/v1/drivers/${id}`),
  create: (payload: CreateDriverPayload) => apiClient.post<Driver>('/v1/drivers', payload),
};

export const vehiclesApi = {
  list: () => apiClient.get<Vehicle[]>('/v1/vehicles'),
  get: (id: string) => apiClient.get<Vehicle>(`/v1/vehicles/${id}`),
  create: (payload: CreateVehiclePayload) => apiClient.post<Vehicle>('/v1/vehicles', payload),
};
