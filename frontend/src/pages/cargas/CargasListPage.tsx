import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { cargosApi } from '../../api/cargos';
import { Badge } from '../../components/portal/Badge';
import { Button } from '../../components/portal/Button';
import { CARGO_STATUS } from '../../cargoState';
import type { CargoSummary, CargoStatus } from '../../types/cargo';
import './CargasListPage.css';

const STATUS_FILTERS: Array<CargoStatus | 'Todos'> = ['Todos', 'Received', 'Routed', 'Dispatched', 'Completed'];

export function CargasListPage() {
  const [cargos, setCargos] = useState<CargoSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [statusFilter, setStatusFilter] = useState<CargoStatus | 'Todos'>('Todos');

  useEffect(() => {
    cargosApi
      .list()
      .then(setCargos)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(() => {
    if (statusFilter === 'Todos') return cargos;
    return cargos.filter((cargo) => cargo.status === statusFilter);
  }, [cargos, statusFilter]);

  return (
    <div className="cargas-list">
      <div className="cargas-list__header">
        <h1 className="display">Cargas</h1>
        <Button as={Link} to="/cargas/nova">
          Nova carga
        </Button>
      </div>

      <div className="cargas-list__filters">
        {STATUS_FILTERS.map((status) => (
          <button
            key={status}
            className={`filter-chip ${statusFilter === status ? 'filter-chip--active' : ''}`}
            onClick={() => setStatusFilter(status)}
          >
            {status === 'Todos' ? 'Todos' : CARGO_STATUS[status].label}
          </button>
        ))}
      </div>

      {loading && <p className="cargas-list__hint">Carregando cargas…</p>}
      {error && <p className="cargas-list__error">{error}</p>}

      {!loading && !error && (
        <table className="cargas-table">
          <thead>
            <tr>
              <th>Código</th>
              <th>Status</th>
              <th>Entregas</th>
              <th>Criada em</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((cargo) => (
              <tr key={cargo.id}>
                <td>
                  <Link className="cargas-table__link mono" to={`/cargas/${cargo.id}`}>
                    {cargo.externalId}
                  </Link>
                </td>
                <td>
                  <Badge tone={CARGO_STATUS[cargo.status]?.tone ?? 'received'}>
                    {CARGO_STATUS[cargo.status]?.label ?? cargo.status}
                  </Badge>
                </td>
                <td>{cargo.deliveryCount}</td>
                <td>{new Date(cargo.createdAt).toLocaleString('pt-BR')}</td>
              </tr>
            ))}
            {filtered.length === 0 && (
              <tr>
                <td colSpan={4} className="cargas-list__hint">
                  Nenhuma carga encontrada.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      )}
    </div>
  );
}
