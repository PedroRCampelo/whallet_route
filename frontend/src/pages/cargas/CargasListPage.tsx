import { ComingSoon } from '../../components/ui/ComingSoon';

export function CargasListPage() {
  return (
    <ComingSoon
      eyebrow="PORTAL · CARGAS"
      title="Gestão de cargas"
      description="Próxima etapa: listar as cargas recebidas do ERP e seus status ao longo do fluxo operacional."
      items={['Recebida', 'Roteirizada', 'Despachada', 'Concluída']}
    />
  );
}
