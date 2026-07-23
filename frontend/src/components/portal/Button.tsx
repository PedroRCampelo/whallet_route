import type { ElementType, ComponentPropsWithoutRef } from 'react';

const VARIANT_CLASS = {
  primary: 'btn-primary',
  secondary: 'btn-ghost',
  danger: 'btn-danger',
} as const;

interface ButtonOwnProps {
  variant?: keyof typeof VARIANT_CLASS;
  className?: string;
}

type ButtonProps<T extends ElementType> = ButtonOwnProps & { as?: T } & Omit<
    ComponentPropsWithoutRef<T>,
    keyof ButtonOwnProps | 'as'
  >;

export function Button<T extends ElementType = 'button'>(props: ButtonProps<T>) {
  const { variant = 'primary', as, className = '', ...rest } = props;
  const Component = (as ?? 'button') as ElementType;
  return <Component className={`btn ${VARIANT_CLASS[variant]} ${className}`} {...rest} />;
}
