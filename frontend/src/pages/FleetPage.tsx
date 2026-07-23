import { useEffect, useState, type FormEvent } from 'react';
import { driversApi, vehiclesApi } from '../api/fleet';
import { Button } from '../components/portal/Button';
import { Field } from '../components/portal/Field';
import type { CreateDriverPayload, CreateVehiclePayload, Driver, Vehicle } from '../types/cargo';
import './FleetPage.css';

interface DriverFormState {
  name: string;
  document: string;
  phone: string;
  licenseNumber: string;
  licenseCategory: string;
  licenseExpiry: string;
}

interface VehicleFormState {
  plate: string;
  capacityKg: string;
  capacityM3: string;
  description: string;
}

function emptyDriver(): DriverFormState {
  return { name: '', document: '', phone: '', licenseNumber: '', licenseCategory: '', licenseExpiry: '' };
}

function emptyVehicle(): VehicleFormState {
  return { plate: '', capacityKg: '', capacityM3: '', description: '' };
}

export function FleetPage() {
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [driverForm, setDriverForm] = useState<DriverFormState>(emptyDriver());
  const [driverError, setDriverError] = useState<string | null>(null);
  const [driverSubmitting, setDriverSubmitting] = useState(false);

  const [vehicleForm, setVehicleForm] = useState<VehicleFormState>(emptyVehicle());
  const [vehicleError, setVehicleError] = useState<string | null>(null);
  const [vehicleSubmitting, setVehicleSubmitting] = useState(false);

  useEffect(() => {
    Promise.all([driversApi.list().then(setDrivers), vehiclesApi.list().then(setVehicles)])
      .catch((err) => setLoadError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleCreateDriver(e: FormEvent) {
    e.preventDefault();
    setDriverError(null);
    setDriverSubmitting(true);
    try {
      const payload: CreateDriverPayload = {
        name: driverForm.name,
        document: driverForm.document || null,
        phone: driverForm.phone || null,
        licenseNumber: driverForm.licenseNumber || null,
        licenseCategory: driverForm.licenseCategory || null,
        licenseExpiry: driverForm.licenseExpiry || null,
      };
      const created = await driversApi.create(payload);
      setDrivers((prev) => [...prev, created]);
      setDriverForm(emptyDriver());
    } catch (err) {
      setDriverError(err instanceof Error ? err.message : String(err));
    } finally {
      setDriverSubmitting(false);
    }
  }

  async function handleCreateVehicle(e: FormEvent) {
    e.preventDefault();
    setVehicleError(null);
    setVehicleSubmitting(true);
    try {
      const payload: CreateVehiclePayload = {
        plate: vehicleForm.plate,
        capacityKg: vehicleForm.capacityKg === '' ? 0 : Number(vehicleForm.capacityKg),
        capacityM3: vehicleForm.capacityM3 === '' ? 0 : Number(vehicleForm.capacityM3),
        description: vehicleForm.description || null,
      };
      const created = await vehiclesApi.create(payload);
      setVehicles((prev) => [...prev, created]);
      setVehicleForm(emptyVehicle());
    } catch (err) {
      setVehicleError(err instanceof Error ? err.message : String(err));
    } finally {
      setVehicleSubmitting(false);
    }
  }

  if (loading) return <p className="fleet__hint">Carregando frota…</p>;
  if (loadError) return <p className="fleet__error">{loadError}</p>;

  return (
    <div className="fleet">
      <h1 className="display">Frota</h1>

      <div className="fleet__columns">
        <section className="fleet__column">
          <h2>Motoristas</h2>

          <ul className="fleet__list">
            {drivers.map((driver) => (
              <li key={driver.id} className="fleet__list-item">
                <strong>{driver.name}</strong>
                <span className="fleet__list-meta">
                  {[driver.licenseNumber, driver.phone].filter(Boolean).join(' · ') || '—'}
                </span>
              </li>
            ))}
            {drivers.length === 0 && <li className="fleet__hint">Nenhum motorista cadastrado.</li>}
          </ul>

          <form onSubmit={handleCreateDriver} className="fleet__form">
            {driverError && <p className="fleet__error">{driverError}</p>}
            <Field label="Nome">
              <input
                className="field-input"
                value={driverForm.name}
                onChange={(e) => setDriverForm((f) => ({ ...f, name: e.target.value }))}
                required
              />
            </Field>
            <div className="fleet__form-row">
              <Field label="Documento">
                <input
                  className="field-input"
                  value={driverForm.document}
                  onChange={(e) => setDriverForm((f) => ({ ...f, document: e.target.value }))}
                />
              </Field>
              <Field label="Telefone">
                <input
                  className="field-input"
                  value={driverForm.phone}
                  onChange={(e) => setDriverForm((f) => ({ ...f, phone: e.target.value }))}
                />
              </Field>
            </div>
            <div className="fleet__form-row">
              <Field label="CNH — número">
                <input
                  className="field-input"
                  value={driverForm.licenseNumber}
                  onChange={(e) => setDriverForm((f) => ({ ...f, licenseNumber: e.target.value }))}
                />
              </Field>
              <Field label="CNH — categoria">
                <input
                  className="field-input"
                  value={driverForm.licenseCategory}
                  onChange={(e) => setDriverForm((f) => ({ ...f, licenseCategory: e.target.value }))}
                />
              </Field>
            </div>
            <Field label="CNH — validade">
              <input
                className="field-input"
                type="date"
                value={driverForm.licenseExpiry}
                onChange={(e) => setDriverForm((f) => ({ ...f, licenseExpiry: e.target.value }))}
              />
            </Field>
            <Button type="submit" disabled={driverSubmitting}>
              {driverSubmitting ? 'Cadastrando…' : 'Cadastrar motorista'}
            </Button>
          </form>
        </section>

        <section className="fleet__column">
          <h2>Veículos</h2>

          <ul className="fleet__list">
            {vehicles.map((vehicle) => (
              <li key={vehicle.id} className="fleet__list-item">
                <strong className="mono">{vehicle.plate}</strong>
                <span className="fleet__list-meta">
                  {vehicle.capacityKg} kg · {vehicle.capacityM3} m³
                  {vehicle.description ? ` · ${vehicle.description}` : ''}
                </span>
              </li>
            ))}
            {vehicles.length === 0 && <li className="fleet__hint">Nenhum veículo cadastrado.</li>}
          </ul>

          <form onSubmit={handleCreateVehicle} className="fleet__form">
            {vehicleError && <p className="fleet__error">{vehicleError}</p>}
            <Field label="Placa">
              <input
                className="field-input"
                value={vehicleForm.plate}
                onChange={(e) => setVehicleForm((f) => ({ ...f, plate: e.target.value }))}
                required
              />
            </Field>
            <div className="fleet__form-row">
              <Field label="Capacidade (kg)">
                <input
                  className="field-input"
                  type="number"
                  value={vehicleForm.capacityKg}
                  onChange={(e) => setVehicleForm((f) => ({ ...f, capacityKg: e.target.value }))}
                  required
                />
              </Field>
              <Field label="Capacidade (m³)">
                <input
                  className="field-input"
                  type="number"
                  value={vehicleForm.capacityM3}
                  onChange={(e) => setVehicleForm((f) => ({ ...f, capacityM3: e.target.value }))}
                  required
                />
              </Field>
            </div>
            <Field label="Descrição">
              <input
                className="field-input"
                value={vehicleForm.description}
                onChange={(e) => setVehicleForm((f) => ({ ...f, description: e.target.value }))}
              />
            </Field>
            <Button type="submit" disabled={vehicleSubmitting}>
              {vehicleSubmitting ? 'Cadastrando…' : 'Cadastrar veículo'}
            </Button>
          </form>
        </section>
      </div>
    </div>
  );
}
