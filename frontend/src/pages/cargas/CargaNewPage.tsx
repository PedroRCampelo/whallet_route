import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { cargosApi } from '../../api/cargos';
import { Button } from '../../components/portal/Button';
import { Field } from '../../components/portal/Field';
import { PageHeader } from '../../components/portal/PageHeader';
import type { CreateCargoPayload } from '../../types/cargo';
import './CargaNewPage.css';

interface DeliveryFormState {
  externalId: string;
  clientId: string;
  clientName: string;
  address: string;
  latitude: string;
  longitude: string;
  weightKg: string;
  volumeM3: string;
  windowFrom: string;
  windowTo: string;
  phone: string;
  instructions: string;
}

function emptyDelivery(): DeliveryFormState {
  return {
    externalId: '',
    clientId: '',
    clientName: '',
    address: '',
    latitude: '',
    longitude: '',
    weightKg: '',
    volumeM3: '',
    windowFrom: '',
    windowTo: '',
    phone: '',
    instructions: '',
  };
}

function deliveryFromJson(raw: Record<string, unknown>): DeliveryFormState {
  const str = (v: unknown) => (v == null ? '' : String(v));
  return {
    externalId: str(raw.externalId),
    clientId: str(raw.clientId),
    clientName: str(raw.clientName),
    address: str(raw.address),
    latitude: str(raw.latitude),
    longitude: str(raw.longitude),
    weightKg: str(raw.weightKg),
    volumeM3: str(raw.volumeM3),
    windowFrom: str(raw.windowFrom),
    windowTo: str(raw.windowTo),
    phone: str(raw.phone),
    instructions: str(raw.instructions),
  };
}

export function CargaNewPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<'form' | 'json'>('form');
  const [externalId, setExternalId] = useState('');
  const [originAddress, setOriginAddress] = useState('');
  const [deliveries, setDeliveries] = useState<DeliveryFormState[]>([emptyDelivery()]);
  const [expanded, setExpanded] = useState<Record<number, boolean>>({});
  const [jsonText, setJsonText] = useState('');
  const [jsonError, setJsonError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  function updateDelivery(index: number, field: keyof DeliveryFormState, value: string) {
    setDeliveries((prev) => prev.map((d, i) => (i === index ? { ...d, [field]: value } : d)));
  }

  function addDelivery() {
    setDeliveries((prev) => [...prev, emptyDelivery()]);
  }

  function removeDelivery(index: number) {
    setDeliveries((prev) => (prev.length > 1 ? prev.filter((_, i) => i !== index) : prev));
  }

  function toggleExpanded(index: number) {
    setExpanded((prev) => ({ ...prev, [index]: !prev[index] }));
  }

  function applyJson() {
    setJsonError(null);
    try {
      const parsed = JSON.parse(jsonText);
      if (!parsed.externalId || !parsed.originAddress || !Array.isArray(parsed.deliveries)) {
        throw new Error('JSON precisa de externalId, originAddress e deliveries.');
      }
      setExternalId(parsed.externalId);
      setOriginAddress(parsed.originAddress);
      setDeliveries(parsed.deliveries.length > 0 ? parsed.deliveries.map(deliveryFromJson) : [emptyDelivery()]);
      setActiveTab('form');
    } catch (err) {
      setJsonError(err instanceof Error ? err.message : 'JSON inválido.');
    }
  }

  function buildPayload(): CreateCargoPayload {
    return {
      externalId,
      originAddress,
      originLatitude: null,
      originLongitude: null,
      driverId: null,
      vehicleId: null,
      deliveries: deliveries.map((d) => ({
        externalId: d.externalId,
        clientId: d.clientId,
        clientName: d.clientName,
        address: d.address,
        latitude: d.latitude === '' ? null : Number(d.latitude),
        longitude: d.longitude === '' ? null : Number(d.longitude),
        weightKg: d.weightKg === '' ? 0 : Number(d.weightKg),
        volumeM3: d.volumeM3 === '' ? 0 : Number(d.volumeM3),
        windowFrom: d.windowFrom || null,
        windowTo: d.windowTo || null,
        phone: d.phone || null,
        instructions: d.instructions || null,
      })),
    };
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setSubmitError(null);
    setSubmitting(true);
    try {
      const created = await cargosApi.create(buildPayload());
      navigate(`/cargas/${created.id}`);
    } catch (err) {
      setSubmitError(err instanceof Error ? err.message : String(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="cargo-new">
      <PageHeader eyebrow="PORTAL · NOVA CARGA" title="Nova carga" />

      <div className="cargo-new__tabs">
        <button
          className={`cargo-new__tab ${activeTab === 'form' ? 'cargo-new__tab--active' : ''}`}
          onClick={() => setActiveTab('form')}
        >
          Formulário
        </button>
        <button
          className={`cargo-new__tab ${activeTab === 'json' ? 'cargo-new__tab--active' : ''}`}
          onClick={() => setActiveTab('json')}
        >
          Colar JSON
        </button>
      </div>

      {activeTab === 'json' ? (
        <div className="cargo-new__json">
          <textarea
            className="field-textarea"
            placeholder="Cole aqui o JSON da carga…"
            value={jsonText}
            onChange={(e) => setJsonText(e.target.value)}
          />
          {jsonError && <p className="cargo-new__error">{jsonError}</p>}
          <Button type="button" onClick={applyJson}>
            Preencher formulário
          </Button>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="cargo-new__form">
          {submitError && <p className="cargo-new__error">{submitError}</p>}

          <div className="cargo-new__origin">
            <Field label="Código (externalId)">
              <input
                className="field-input"
                value={externalId}
                onChange={(e) => setExternalId(e.target.value)}
                required
              />
            </Field>
            <Field label="Endereço de origem">
              <input
                className="field-input"
                value={originAddress}
                onChange={(e) => setOriginAddress(e.target.value)}
                required
              />
            </Field>
          </div>

          <div className="cargo-new__deliveries">
            <div className="cargo-new__deliveries-header">
              <span className="field__label">Entregas</span>
              <Button type="button" variant="secondary" onClick={addDelivery}>
                + Adicionar entrega
              </Button>
            </div>

            {deliveries.map((delivery, index) => (
              <div key={index} className="delivery-form-row">
                <div className="delivery-form-row__main">
                  <Field label="Código">
                    <input
                      className="field-input"
                      value={delivery.externalId}
                      onChange={(e) => updateDelivery(index, 'externalId', e.target.value)}
                      required
                    />
                  </Field>
                  <Field label="Cliente (ID)">
                    <input
                      className="field-input"
                      value={delivery.clientId}
                      onChange={(e) => updateDelivery(index, 'clientId', e.target.value)}
                      required
                    />
                  </Field>
                  <Field label="Nome do cliente">
                    <input
                      className="field-input"
                      value={delivery.clientName}
                      onChange={(e) => updateDelivery(index, 'clientName', e.target.value)}
                      required
                    />
                  </Field>
                  <Field label="Endereço">
                    <input
                      className="field-input"
                      value={delivery.address}
                      onChange={(e) => updateDelivery(index, 'address', e.target.value)}
                      required
                    />
                  </Field>
                </div>

                <div className="delivery-form-row__footer">
                  <button type="button" className="delivery-form-row__toggle" onClick={() => toggleExpanded(index)}>
                    {expanded[index] ? 'Menos campos ▲' : 'Mais campos ▾'}
                  </button>
                  {deliveries.length > 1 && (
                    <button type="button" className="delivery-form-row__remove" onClick={() => removeDelivery(index)}>
                      Remover
                    </button>
                  )}
                </div>

                {expanded[index] && (
                  <div className="delivery-form-row__extra">
                    <Field label="Peso (kg)">
                      <input
                        className="field-input"
                        type="number"
                        value={delivery.weightKg}
                        onChange={(e) => updateDelivery(index, 'weightKg', e.target.value)}
                      />
                    </Field>
                    <Field label="Volume (m³)">
                      <input
                        className="field-input"
                        type="number"
                        value={delivery.volumeM3}
                        onChange={(e) => updateDelivery(index, 'volumeM3', e.target.value)}
                      />
                    </Field>
                    <Field label="Latitude">
                      <input
                        className="field-input"
                        type="number"
                        value={delivery.latitude}
                        onChange={(e) => updateDelivery(index, 'latitude', e.target.value)}
                      />
                    </Field>
                    <Field label="Longitude">
                      <input
                        className="field-input"
                        type="number"
                        value={delivery.longitude}
                        onChange={(e) => updateDelivery(index, 'longitude', e.target.value)}
                      />
                    </Field>
                    <Field label="Janela de (data/hora)">
                      <input
                        className="field-input"
                        type="datetime-local"
                        value={delivery.windowFrom}
                        onChange={(e) => updateDelivery(index, 'windowFrom', e.target.value)}
                      />
                    </Field>
                    <Field label="Janela até (data/hora)">
                      <input
                        className="field-input"
                        type="datetime-local"
                        value={delivery.windowTo}
                        onChange={(e) => updateDelivery(index, 'windowTo', e.target.value)}
                      />
                    </Field>
                    <Field label="Telefone">
                      <input
                        className="field-input"
                        value={delivery.phone}
                        onChange={(e) => updateDelivery(index, 'phone', e.target.value)}
                      />
                    </Field>
                    <Field label="Instruções">
                      <input
                        className="field-input"
                        value={delivery.instructions}
                        onChange={(e) => updateDelivery(index, 'instructions', e.target.value)}
                      />
                    </Field>
                  </div>
                )}
              </div>
            ))}
          </div>

          <Button type="submit" disabled={submitting}>
            {submitting ? 'Criando…' : 'Criar carga'}
          </Button>
        </form>
      )}
    </div>
  );
}
