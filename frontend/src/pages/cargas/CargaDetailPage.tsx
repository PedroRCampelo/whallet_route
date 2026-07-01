import { useParams } from 'react-router-dom';
import { ComingSoon } from '../../components/ui/ComingSoon';

export function CargaDetailPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <ComingSoon
      eyebrow={`PORTAL · CARGA ${id ?? ''}`}
      title="Detalhe da carga"
      description="Próxima etapa: completar cadastro, roteirizar, enviar ao motorista, acompanhar entregas e gerar o retorno para o ERP."
      items={[
        'Completar cadastro (motorista, veículo, peso)',
        'Roteirizar — editar motorista, veículo e paradas',
        'Enviar ao motorista / cancelar envio',
        'Acompanhar entregas — entregue ou recusada',
        'Concluir e gerar JSON de retorno para o ERP',
      ]}
    />
  );
}
