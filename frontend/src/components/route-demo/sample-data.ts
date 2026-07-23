import type { RouteRequest, RouteResponse } from '../../types/route';

export const DEFAULT_REQUEST: RouteRequest = {
  origin: { id: 'DC', address: 'Rua Padre Carapuceiro, 777, Recife, PE' },
  stops: [
    { id: 'C0001-01', address: 'Av. Conselheiro Aguiar, 2738, Boa Viagem, Recife, PE' },
    { id: 'C0002-01', address: 'Rua da Aurora, 295, Boa Vista, Recife, PE' },
    { id: 'C0003-01', address: 'Av. Caxangá, 1200, Madalena, Recife, PE' },
    { id: 'C0004-01', address: 'Rua Amália Bernardino de Sousa, 532, Boa Viagem, Recife, PE' },
    { id: 'C0005-01', address: 'Av. Fagundes Varela, 111, Jardim Atlântico, Olinda, PE' },
  ],
};

export const SAMPLE_RESPONSE: RouteResponse = {
  origin: { id: 'DC', latitude: -8.1190289, longitude: -34.9046335 },
  totalDistanceMeters: 37465,
  totalDurationSeconds: 4498,
  stops: [
    { id: 'C0004-01', order: 1, latitude: -8.1263192, longitude: -34.909066, legDistanceMeters: 946, legDurationSeconds: 114 },
    { id: 'C0003-01', order: 2, latitude: -8.051578, longitude: -34.9188818, legDistanceMeters: 8381, legDurationSeconds: 1006 },
    { id: 'C0005-01', order: 3, latitude: -7.9786555, longitude: -34.8393271, legDistanceMeters: 11937, legDurationSeconds: 1433 },
    { id: 'C0002-01', order: 4, latitude: -8.0609708, longitude: -34.8813796, legDistanceMeters: 10258, legDurationSeconds: 1231 },
    { id: 'C0001-01', order: 5, latitude: -8.1131332, longitude: -34.89312779999999, legDistanceMeters: 5943, legDurationSeconds: 713 },
  ],
};

// exibida somente como exemplo, somente-leitura — não é enviada a lugar nenhum
export const DEFAULT_REQUEST_JSON = JSON.stringify(DEFAULT_REQUEST, null, 2);
