import { architectureData } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, CardDescription } from '../components/ui';
import './Architecture.css';

export function ArchitecturePage() {
  const { modules, dataFlows, diagramPlaceholders } = architectureData;

  return (
    <div className="architecture-page">
      {/* Hero */}
      <section className="architecture-hero">
        <div className="architecture-hero-content">
          <h1 className="architecture-title">Архитектура</h1>
          <p className="architecture-tagline">
            Техническая документация для разработчиков
          </p>
        </div>
      </section>

      {/* Modules */}
      <Section padding="lg">
        <SectionHeader
          title="Модули проекта"
          subtitle="Структура и зависимости компонентов"
        />
        <div className="modules-grid">
          {modules.map((module) => (
            <Card key={module.name} className="module-card">
              <CardBody>
                <div className="module-header">
                  <CardTitle>{module.name}</CardTitle>
                  <code className="module-path">{module.path}</code>
                </div>
                <CardDescription>{module.description}</CardDescription>
                {module.dependencies.length > 0 && (
                  <div className="module-dependencies">
                    <span className="deps-label">Зависимости:</span>
                    <div className="deps-list">
                      {module.dependencies.map((dep) => (
                        <span key={dep} className="dep-tag">{dep}</span>
                      ))}
                    </div>
                  </div>
                )}
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Data Flows */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title="Потоки данных"
          subtitle="Как данные перемещаются между системами"
        />
        <div className="dataflows-list">
          {dataFlows.map((flow) => (
            <div key={flow.name} className="dataflow-item">
              <div className="dataflow-source">
                <span className="dataflow-name">{flow.from}</span>
              </div>
              <div className="dataflow-connection">
                <div className={`dataflow-line dataflow-${flow.type}`}></div>
                <span className={`dataflow-type dataflow-${flow.type}`}>
                  {flow.type === 'sync' && 'Синхронно'}
                  {flow.type === 'async' && 'Асинхронно'}
                  {flow.type === 'event' && 'Событие'}
                </span>
              </div>
              <div className="dataflow-target">
                <span className="dataflow-name">{flow.to}</span>
              </div>
              <span className="dataflow-description">{flow.description}</span>
            </div>
          ))}
        </div>
      </Section>

      {/* Diagram Placeholders */}
      <Section padding="lg">
        <SectionHeader
          title="Диаграммы"
          subtitle="Визуальное представление архитектуры"
        />
        <div className="diagrams-grid">
          {diagramPlaceholders.map((diagram) => (
            <Card key={diagram.id} className="diagram-card">
              <CardBody>
                <span className={`diagram-type diagram-type-${diagram.type}`}>
                  {diagram.type === 'flowchart' && '📊'}
                  {diagram.type === 'sequence' && '🔄'}
                  {diagram.type === 'class' && '🏗️'}
                  {diagram.type === 'component' && '🧩'}
                  {' '}{diagram.type}
                </span>
                <CardTitle>{diagram.title}</CardTitle>
                <CardDescription>{diagram.description}</CardDescription>
                <div className="diagram-placeholder">
                  <span>Диаграмма будет добавлена</span>
                </div>
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Tech Stack Info */}
      <Section variant="dark" padding="lg">
        <SectionHeader
          title="Технологический стек"
          subtitle="Инструменты и технологии проекта"
        />
        <div className="tech-stack">
          <div className="tech-category">
            <h3 className="tech-category-title">Движок</h3>
            <ul className="tech-list">
              <li>Unreal Engine 5</li>
              <li>Blueprints</li>
              <li>C++</li>
            </ul>
          </div>
          <div className="tech-category">
            <h3 className="tech-category-title">Документация</h3>
            <ul className="tech-list">
              <li>React 19</li>
              <li>TypeScript</li>
              <li>Vite</li>
              <li>React Router</li>
            </ul>
          </div>
          <div className="tech-category">
            <h3 className="tech-category-title">Инструменты</h3>
            <ul className="tech-list">
              <li>Git</li>
              <li>ESLint</li>
              <li>Node.js</li>
            </ul>
          </div>
        </div>
      </Section>
    </div>
  );
}
