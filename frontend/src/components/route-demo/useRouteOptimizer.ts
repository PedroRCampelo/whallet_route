import { useCallback, useState } from 'react';
import type { RouteResponse } from '../../types/route';
import { SAMPLE_RESPONSE } from './sample-data';

/**
 * Demo estático da landing page — não é mais uma chamada real.
 *
 * Esta seção é pública e sem login. Uma chamada real bateria no backend com a chave
 * de API exposta no bundle do navegador e dispararia geocodificação paga a cada clique
 * de qualquer visitante. Por isso a "otimização" aqui é só uma simulação com dados
 * fixos, para o cliente ver como fica — nada sai do navegador.
 */
export function useRouteOptimizer() {
  const [loading, setLoading] = useState(false);
  const [response, setResponse] = useState<RouteResponse | null>(null);

  const optimize = useCallback(async (): Promise<RouteResponse> => {
    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 450));
    setResponse(SAMPLE_RESPONSE);
    setLoading(false);
    return SAMPLE_RESPONSE;
  }, []);

  return { loading, response, optimize };
}
