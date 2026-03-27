import { useState } from 'react';
import { conventionsData } from '../data';
import { Section } from '../components/ui';
import './Conventions.css';

export function ConventionsPage() {
  const [expandedItems, setExpandedItems] = useState<Set<string>>(new Set());

  const toggleItem = (id: string) => {
    const newSet = new Set(expandedItems);
    if (newSet.has(id)) {
      newSet.delete(id);
    } else {
      newSet.add(id);
    }
    setExpandedItems(newSet);
  };

  const getStatusClass = (status: string) => {
    switch (status) {
      case 'active':
        return 'status-active';
      case 'deprecated':
        return 'status-deprecated';
      case 'proposed':
        return 'status-proposed';
      default:
        return '';
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'active':
        return 'Активно';
      case 'deprecated':
        return 'Устарело';
      case 'proposed':
        return 'Предложено';
      default:
        return status;
    }
  };

  return (
    <div className="conventions-page">
      {/* Hero */}
      <section className="conventions-hero">
        <div className="conventions-hero-content">
          <h1 className="conventions-title">Конвенции</h1>
          <p className="conventions-tagline">
            Правила, соглашения и условности проекта Kiarche Continuum War
          </p>
          <p className="conventions-updated">
            Последнее обновление: {conventionsData.lastUpdated}
          </p>
        </div>
      </section>

      {/* Content */}
      <Section padding="lg">
        <div className="conventions-container">
          {conventionsData.categories.map((category) => (
            <div key={category.id} className="conventions-category">
              <div className="category-header">
                <h2 className="category-name">{category.name}</h2>
                <p className="category-description">{category.description}</p>
              </div>

              <div className="category-items">
                {category.items.map((item) => (
                  <div
                    key={item.id}
                    className={`convention-item ${expandedItems.has(item.id) ? 'convention-item-expanded' : ''}`}
                  >
                    <button
                      className="item-header"
                      onClick={() => toggleItem(item.id)}
                    >
                      <div className="item-header-left">
                        <span className={`item-status ${getStatusClass(item.status)}`}>
                          {getStatusLabel(item.status)}
                        </span>
                        <h3 className="item-title">{item.title}</h3>
                      </div>
                      <span className="item-toggle">
                        {expandedItems.has(item.id) ? '−' : '+'}
                      </span>
                    </button>

                    {expandedItems.has(item.id) && (
                      <div className="item-content">
                        <p className="item-description">{item.description}</p>

                        {item.rationale && (
                          <div className="item-section">
                            <h4 className="item-section-title">Обоснование</h4>
                            <p className="item-section-content">{item.rationale}</p>
                          </div>
                        )}

                        {item.examples && item.examples.length > 0 && (
                          <div className="item-section">
                            <h4 className="item-section-title">Примеры</h4>
                            <ul className="item-examples">
                              {item.examples.map((example, i) => (
                                <li key={i}>{example}</li>
                              ))}
                            </ul>
                          </div>
                        )}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      </Section>
    </div>
  );
}
