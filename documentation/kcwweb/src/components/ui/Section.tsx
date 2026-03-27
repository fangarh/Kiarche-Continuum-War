import type { ReactNode } from 'react';
import './Section.css';

interface SectionProps {
  children: ReactNode;
  className?: string;
  variant?: 'default' | 'alt' | 'dark';
  padding?: 'sm' | 'md' | 'lg';
}

export function Section({
  children,
  className = '',
  variant = 'default',
  padding = 'md',
}: SectionProps) {
  return (
    <section className={`section section--${variant} section--${padding} ${className}`.trim()}>
      {children}
    </section>
  );
}

interface SectionHeaderProps {
  title: string;
  subtitle?: string;
  className?: string;
}

export function SectionHeader({ title, subtitle, className = '' }: SectionHeaderProps) {
  return (
    <div className={`section-header ${className}`}>
      <h2 className="section-title">{title}</h2>
      {subtitle && <p className="section-subtitle">{subtitle}</p>}
    </div>
  );
}

interface SectionContentProps {
  children: ReactNode;
  className?: string;
}

export function SectionContent({ children, className = '' }: SectionContentProps) {
  return <div className={`section-content ${className}`}>{children}</div>;
}
