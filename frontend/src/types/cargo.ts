export type CargoStatus = 'Received' | 'Routed' | 'Dispatched' | 'Completed';
export type DeliveryStatus = 'Pending' | 'EnRoute' | 'Delivered' | 'Refused';

export interface Delivery {
  id: string;
  externalId: string;
  clientId: string;
  clientName: string;
  address: string;
  latitude: number | null;
  longitude: number | null;
  order: number | null;
  status: DeliveryStatus;
}

export interface Cargo {
  id: string;
  externalId: string;
  originAddress: string;
  status: CargoStatus;
  driverId: string | null;
  vehicleId: string | null;
  createdAt: string;
  deliveries: Delivery[];
}

export interface CargoSummary {
  id: string;
  externalId: string;
  status: CargoStatus;
  deliveryCount: number;
  createdAt: string;
}

export interface CreateDeliveryPayload {
  externalId: string;
  clientId: string;
  clientName: string;
  address: string;
  latitude: number | null;
  longitude: number | null;
  weightKg: number;
  volumeM3: number;
  windowFrom: string | null;
  windowTo: string | null;
  phone: string | null;
  instructions: string | null;
}

export interface CreateCargoPayload {
  externalId: string;
  originAddress: string;
  originLatitude: number | null;
  originLongitude: number | null;
  driverId: string | null;
  vehicleId: string | null;
  deliveries: CreateDeliveryPayload[];
}

export interface DeliverPayload {
  latitude: number | null;
  longitude: number | null;
  note?: string;
}

export interface RefusePayload {
  reason: string;
  latitude: number | null;
  longitude: number | null;
}

export interface Driver {
  id: string;
  name: string;
  document?: string | null;
  phone?: string | null;
  licenseNumber?: string | null;
  licenseCategory?: string | null;
  licenseExpiry?: string | null;
}

export interface CreateDriverPayload {
  name: string;
  document: string | null;
  phone: string | null;
  licenseNumber: string | null;
  licenseCategory: string | null;
  licenseExpiry: string | null;
}

export interface Vehicle {
  id: string;
  plate: string;
  capacityKg: number;
  capacityM3: number;
  description?: string | null;
}

export interface CreateVehiclePayload {
  plate: string;
  capacityKg: number;
  capacityM3: number;
  description: string | null;
}
