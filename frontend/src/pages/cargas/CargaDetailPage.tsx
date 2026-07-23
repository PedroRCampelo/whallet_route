import { useCallback, useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { cargosApi } from '../../api/cargos';
import { driversApi, vehiclesApi } from '../../api/fleet';
import { Badge } from '../../components/portal/Badge';
import { Button } from '../../components/portal/Button';
import { Field } from '../../components/portal/Field';
import { CargoMap } from '../../components/portal/CargoMap';
import { CARGO_STATUS, DELIVERY_STATUS, getAvailableActions } from '../../cargoState';
import type { Cargo, Driver, Vehicle } from '../../types/cargo';
import './CargaDetailPage.css';

export function CargaDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [cargo, setCargo] = useState<Cargo | null>(null);
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [actionPending, setActionPending] = useState(false);
  const [refusingId, setRefusingId] = useState<string | null>(null);
  const [refuseReason, setRefuseReason] = useState('');

  const loadCargo = useCallback(() => {
    if (!id) return Promise.resolve();
    return cargosApi.get(id).then(setCargo);
  }, [id]);

  useEffect(() => {
    setLoading(true);
    setLoadError(null);
    Promise.all([loadCargo(), driversApi.list().then(setDrivers), vehiclesApi.list().then(setVehicles)])
      .catch((err) => setLoadError(err.message))
      .finally(() => setLoading(false));
  }, [loadCargo]);

  async function runAction(fn: () => Promise<Cargo>) {
    setActionError(null);
    setActionPending(true);
    try {
      const updated = await fn();
      setCargo(updated);
    } catch (err) {
      setActionError(err instanceof Error ? err.message : String(err));
    } finally {
      setActionPending(false);
    }
  }

  function startRefuse(deliveryId: string) {
    setActionError(null);
    setRefusingId(deliveryId);
    setRefuseReason('');
  }

  function cancelRefuse() {
    setRefusingId(null);
    setRefuseReason('');
  }

  async function submitRefuse(deliveryId: string) {
    if (!id || !refuseReason.trim()) return;
    await runAction(() =>
      cargosApi.refuse(id, deliveryId, { reason: refuseReason.trim(), latitude: null, longitude: null })
    );
    setRefusingId(null);
    setRefuseReason('');
  }

  if (loading) return <p className="cargo-detail__hint">Carregando carga…</p>;
  if (loadError) return <p className="cargo-detail__error">{loadError}</p>;
  if (!cargo || !id) return null;

  const actions = getAvailableActions(cargo);
  const driver = drivers.find((d) => d.id === cargo.driverId);
  const vehicle = vehicles.find((v) => v.id === cargo.vehicleId);

  return (
    <div className="cargo-detail">
      <Link to="/cargas" className="cargo-detail__back">
        ← Cargas
      </Link>

      <div className="cargo-detail__title">
        <h1 className="display mono">{cargo.externalId}</h1>
        <Badge tone={CARGO_STATUS[cargo.status]?.tone ?? 'received'}>
          {CARGO_STATUS[cargo.status]?.label ?? cargo.status}
        </Badge>
      </div>

      {actionError && <p className="cargo-detail__error">{actionError}</p>}

      <div className="cargo-detail__body">
        <aside className="cargo-detail__panel">
          <Field label="Origem">{cargo.originAddress}</Field>

          <Field label="Motorista">
            {actions.assignDriver ? (
              <select
                className="field-select"
                value={cargo.driverId ?? ''}
                disabled={actionPending}
                onChange={(e) => runAction(() => cargosApi.assignDriver(id, e.target.value))}
              >
                <option value="" disabled>
                  Selecionar motorista
                </option>
                {drivers.map((d) => (
                  <option key={d.id} value={d.id}>
                    {d.name}
                  </option>
                ))}
              </select>
            ) : (
              driver?.name ?? '—'
            )}
          </Field>

          <Field label="Veículo">
            {actions.assignVehicle ? (
              <select
                className="field-select"
                value={cargo.vehicleId ?? ''}
                disabled={actionPending}
                onChange={(e) => runAction(() => cargosApi.assignVehicle(id, e.target.value))}
              >
                <option value="" disabled>
                  Selecionar veículo
                </option>
                {vehicles.map((v) => (
                  <option key={v.id} value={v.id}>
                    {v.plate}
                  </option>
                ))}
              </select>
            ) : (
              vehicle?.plate ?? '—'
            )}
          </Field>

          <div className="cargo-detail__actions">
            {actions.route && (
              <Button variant="secondary" disabled={actionPending} onClick={() => runAction(() => cargosApi.route(id))}>
                {cargo.status === 'Routed' ? 'Roteirizar novamente' : 'Roteirizar'}
              </Button>
            )}
            {actions.dispatch && (
              <Button disabled={actionPending} onClick={() => runAction(() => cargosApi.dispatch(id))}>
                Despachar
              </Button>
            )}
            {actions.cancelDispatch && (
              <Button
                variant="danger"
                disabled={actionPending}
                onClick={() => runAction(() => cargosApi.cancelDispatch(id))}
              >
                Cancelar envio
              </Button>
            )}
          </div>

          <div className="cargo-detail__deliveries">
            <span className="field__label">Entregas</span>
            {cargo.deliveries.map((delivery) => (
              <div key={delivery.id} className="delivery-row">
                <div className="delivery-row__main">
                  <span className="delivery-row__order">{delivery.order ?? '–'}</span>
                  <div className="delivery-row__info">
                    <strong>{delivery.clientName}</strong>
                    <span className="delivery-row__address">{delivery.address}</span>
                  </div>
                  <Badge tone={DELIVERY_STATUS[delivery.status]?.tone ?? 'pending'}>
                    {DELIVERY_STATUS[delivery.status]?.label ?? delivery.status}
                  </Badge>
                </div>

                {delivery.status === 'EnRoute' && refusingId !== delivery.id && (
                  <div className="delivery-row__buttons">
                    <Button
                      variant="secondary"
                      disabled={actionPending}
                      onClick={() =>
                        runAction(() => cargosApi.deliver(id, delivery.id, { latitude: null, longitude: null }))
                      }
                    >
                      Entregar
                    </Button>
                    <Button variant="danger" disabled={actionPending} onClick={() => startRefuse(delivery.id)}>
                      Recusar
                    </Button>
                  </div>
                )}

                {refusingId === delivery.id && (
                  <div className="delivery-row__refuse">
                    <input
                      type="text"
                      className="field-input"
                      placeholder="Motivo da recusa (obrigatório)"
                      value={refuseReason}
                      onChange={(e) => setRefuseReason(e.target.value)}
                      autoFocus
                    />
                    <div className="delivery-row__buttons">
                      <Button
                        variant="danger"
                        disabled={actionPending || !refuseReason.trim()}
                        onClick={() => submitRefuse(delivery.id)}
                      >
                        Confirmar recusa
                      </Button>
                      <Button variant="secondary" disabled={actionPending} onClick={cancelRefuse}>
                        Cancelar
                      </Button>
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </aside>

        <div className="cargo-detail__map">
          <CargoMap
            key={cargo.deliveries.map((d) => `${d.id}:${d.order}:${d.status}`).join(',')}
            deliveries={cargo.deliveries}
          />
        </div>
      </div>
    </div>
  );
}
