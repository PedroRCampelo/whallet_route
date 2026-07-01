import { useCallback, useState } from 'react';
import type { RouteRequest, RouteResponse } from '../../types/route';
import { DEFAULT_REQUEST } from './sample-data';

const DEFAULT_API_URL = 'http://localhost:5036';
const DEMO_API_KEY = 'wr_dev_local_key_123';

export function useRouteOptimizer() {
  const [apiUrl, setApiUrl] = useState(DEFAULT_API_URL);
  const [jsonText, setJsonText] = useState(() => JSON.stringify(DEFAULT_REQUEST, null, 2));
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [response, setResponse] = useState<RouteResponse | null>(null);

  const optimize = useCallback(async (): Promise<RouteResponse | null> => {
    setError(null);

    let body: RouteRequest;
    try {
      body = JSON.parse(jsonText);
    } catch {
      setError('O JSON está inválido. Revise as chaves e vírgulas.');
      return null;
    }

    setLoading(true);
    try {
      const res = await fetch(`${apiUrl.trim().replace(/\/$/, '')}/v1/routes/optimize`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'X-Api-Key': DEMO_API_KEY },
        body: JSON.stringify(body),
      });
      const data = await res.json();
      if (!res.ok) {
        setError(data.error || 'A API recusou a requisição.');
        return null;
      }
      setResponse(data);
      return data as RouteResponse;
    } catch {
      setError('Não consegui falar com a API. Ela está rodando? Confira a URL acima e o CORS.');
      return null;
    } finally {
      setLoading(false);
    }
  }, [apiUrl, jsonText]);

  return { apiUrl, setApiUrl, jsonText, setJsonText, loading, error, response, optimize };
}
