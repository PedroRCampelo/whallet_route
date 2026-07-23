import type { ReactNode } from 'react';
import './PageHeader.css';

interface PageHeaderProps {
  eyebrow: string;
  title: string;
  action?: ReactNode;
}

export function PageHeader({ eyebrow, title, action }: PageHeaderProps) {
  return (
    <div className="page-header">
      <div>
        <p className="page-header__eyebrow">{eyebrow}</p>
        <h1 className="display page-header__title">{title}</h1>
      </div>
      {action && <div className="page-header__action">{action}</div>}
    </div>
  );
}
