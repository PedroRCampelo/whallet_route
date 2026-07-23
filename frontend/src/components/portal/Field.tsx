import type { ReactNode } from 'react';
import './Field.css';

interface FieldProps {
  label: string;
  children: ReactNode;
}

export function Field({ label, children }: FieldProps) {
  return (
    <div className="field">
      <span className="field__label">{label}</span>
      <div className="field__value">{children}</div>
    </div>
  );
}
