import './ComingSoon.css';

interface ComingSoonProps {
  eyebrow: string;
  title: string;
  description: string;
  items?: string[];
}

export function ComingSoon({ eyebrow, title, description, items }: ComingSoonProps) {
  return (
    <div className="coming-soon">
      <p className="eyebrow">{eyebrow}</p>
      <h1 className="display">{title}</h1>
      <p>{description}</p>
      {items && items.length > 0 && (
        <ul>
          {items.map((item) => (
            <li key={item}>{item}</li>
          ))}
        </ul>
      )}
    </div>
  );
}
