# WhalletRoute

API de roteirização: recebe um ponto de origem e uma lista de paradas e devolve a melhor ordem de entrega.

## Endpoint

```
POST /v1/routes/optimize
```

## Request

Cada parada aceita um **endereço** ou uma **coordenada** (`latitude` + `longitude`). Se vier endereço, a API resolve a coordenada automaticamente.

```json
{
  "origin": { "id": "DC", "address": "Rua Padre Carapuceiro, 777, Recife, PE" },
  "stops": [
    { "id": "C0001-01", "address": "Av. Conselheiro Aguiar, 2738, Boa Viagem, Recife, PE" },
    { "id": "C0002-01", "address": "Rua da Aurora, 295, Boa Vista, Recife, PE" },
    { "id": "C0003-01", "address": "Av. Caxangá, 1200, Madalena, Recife, PE" },
    { "id": "C0004-01", "address": "Rua Amália Bernardino de Sousa, 532, Boa Viagem, Recife, PE" },
    { "id": "C0005-01", "address": "Av. Fagundes Varela, 111, Jardim Atlântico, Olinda, PE" }
  ]
}
```

Exemplo de parada com coordenada pronta (pula o geocoding):

```json
{ "id": "C0001-01", "latitude": -8.1131, "longitude": -34.8931 }
```

## Response

As paradas voltam reordenadas, cada uma com sua posição (`order`) e o trecho percorrido até ela.

```json
{
  "totalDistanceMeters": 37465,
  "totalDurationSeconds": 4498,
  "stops": [
    { "id": "C0004-01", "order": 1, "legDistanceMeters": 946, "legDurationSeconds": 114 },
    { "id": "C0003-01", "order": 2, "legDistanceMeters": 8381, "legDurationSeconds": 1006 },
    { "id": "C0005-01", "order": 3, "legDistanceMeters": 11937, "legDurationSeconds": 1433 },
    { "id": "C0002-01", "order": 4, "legDistanceMeters": 10258, "legDurationSeconds": 1231 },
    { "id": "C0001-01", "order": 5, "legDistanceMeters": 5943, "legDurationSeconds": 713 }
  ]
}
```