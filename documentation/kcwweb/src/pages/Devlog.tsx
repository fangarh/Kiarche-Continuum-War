import { useState } from 'react';
import { devlogData } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, Button, MarkdownContent } from '../components/ui';
import type { DevlogCategory } from '../types';
import './Devlog.css';

type CategoryFilter = DevlogCategory | 'all';

const categoryLabels: Record<DevlogCategory | 'all', string> = {
  all: 'Все',
  development: 'Разработка',
  design: 'Дизайн',
  idea: 'Идеи',
  decision: 'Решения',
  bugfix: 'Багфикс',
  feature: 'Фичи',
};

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
}

export function DevlogPage() {
  const [selectedCategory, setSelectedCategory] = useState<CategoryFilter>('all');
  const [expandedEntry, setExpandedEntry] = useState<string | null>(null);

  const filteredEntries = devlogData.entries.filter(
    (entry) => selectedCategory === 'all' || entry.category === selectedCategory
  );

  const toggleEntry = (id: string) => {
    setExpandedEntry(expandedEntry === id ? null : id);
  };

  return (
    <div className="devlog-page">
      {/* Hero */}
      <section className="devlog-hero">
        <div className="devlog-hero-content">
          <h1 className="devlog-title">Devlog</h1>
          <p className="devlog-tagline">
            История разработки, идеи и принятые решения
          </p>
        </div>
      </section>

      {/* Filters */}
      <Section padding="sm">
        <div className="devlog-filters">
          {(Object.keys(categoryLabels) as CategoryFilter[]).map((category) => (
            <Button
              key={category}
              variant={selectedCategory === category ? 'primary' : 'outline'}
              size="sm"
              onClick={() => setSelectedCategory(category)}
            >
              {categoryLabels[category]}
            </Button>
          ))}
        </div>
      </Section>

      {/* Entries */}
      <Section padding="lg">
        <div className="devlog-entries">
          {filteredEntries.map((entry) => (
            <Card key={entry.id} className={`devlog-entry ${expandedEntry === entry.id ? 'entry-expanded' : ''}`}>
              <CardBody>
                <div className="entry-header">
                  <div className="entry-meta">
                    <span className="entry-date">{formatDate(entry.date)}</span>
                    <span className={`entry-category category-${entry.category}`}>
                      {categoryLabels[entry.category]}
                    </span>
                  </div>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => toggleEntry(entry.id)}
                  >
                    {expandedEntry === entry.id ? 'Свернуть' : 'Читать'}
                  </Button>
                </div>
                <CardTitle>{entry.title}</CardTitle>
                {expandedEntry === entry.id && (
                  <div className="entry-content">
                    <MarkdownContent content={entry.content} className="entry-text" />
                    {entry.tags.length > 0 && (
                      <div className="entry-tags">
                        {entry.tags.map((tag) => (
                          <span key={tag} className="entry-tag">#{tag}</span>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </CardBody>
            </Card>
          ))}
        </div>

        {filteredEntries.length === 0 && (
          <p className="devlog-empty">Записей в этой категории пока нет</p>
        )}
      </Section>

      {/* Stats */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title="Статистика"
          subtitle="Количество записей по категориям"
        />
        <div className="devlog-stats">
          {(Object.keys(categoryLabels) as CategoryFilter[]).filter(c => c !== 'all').map((category) => {
            const count = devlogData.entries.filter(e => e.category === category).length;
            return (
              <div key={category} className="stat-item">
                <span className="stat-count">{count}</span>
                <span className="stat-label">{categoryLabels[category]}</span>
              </div>
            );
          })}
        </div>
      </Section>
    </div>
  );
}
