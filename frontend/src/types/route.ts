export interface RoutePointInput {
  id: string;
  address?: string;
  latitude?: number;
  longitude?: number;
}

export interface RouteRequest {
  origin: RoutePointInput;
  stops: RoutePointInput[];
}

export interface RouteResponseOrigin {
  id: string;
  latitude: number;
  longitude: number;
}

export interface RouteResponseStop {
  id: string;
  order: number;
  latitude: number;
  longitude: number;
  legDistanceMeters: number;
  legDurationSeconds: number;
}

export interface RouteResponse {
  origin: RouteResponseOrigin;
  totalDistanceMeters: number;
  totalDurationSeconds: number;
  stops: RouteResponseStop[];
}
