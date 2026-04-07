import { mechanicsData } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, CardDescription } from '../components/ui';
import './Mechanics.css';

export function MechanicsPage() {
  const {
    coreLoop,
    combat,
    progression,
    portalNetwork,
    campaignStructure,
    factionEndgames,
    economy,
    uxPrinciples,
  } = mechanicsData;

  return (
    <div className="mechanics-page">
      {/* Hero */}
      <section className="mechanics-hero">
        <div className="mechanics-hero-content">
          <h1 className="mechanics-title">Игровые Механики</h1>
          <p className="mechanics-tagline">
            Глубокая система геймплея для максимального погружения
          </p>
        </div>
      </section>

      {/* Core Loop */}
      <Section padding="lg">
        <SectionHeader
          title="Core Gameplay Loop"
          subtitle="Основной цикл игры"
        />
        <div className="core-loop">
          <p className="core-loop-description">{coreLoop.description}</p>
          <div className="loop-steps">
            {coreLoop.steps.map((step) => (
              <div key={step.order} className="loop-step">
                <span className="step-number">{step.order}</span>
                <div className="step-content">
                  <h3 className="step-title">{step.title}</h3>
                  <p className="step-description">{step.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </Section>

      {/* Combat System */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title="Боевая Система"
          subtitle="Тактический бой от третьего лица"
        />
        <p className="section-intro">{combat.description}</p>
        <div className="combat-features">
          {combat.features.map((feature) => (
            <Card key={feature.id} className="combat-card" hoverable>
              <CardBody>
                <CardTitle>{feature.name}</CardTitle>
                <CardDescription>{feature.description}</CardDescription>
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Progression */}
      <Section padding="lg">
        <SectionHeader
          title="Система Прогрессии"
          subtitle="Развивайте персонажа по своему стилю"
        />
        <p className="section-intro">{progression.description}</p>
        <div className="progression-types">
          {progression.types.map((type) => (
            <Card key={type.name} className="progression-card">
              <CardBody>
                <CardTitle>{type.name}</CardTitle>
                <CardDescription>{type.description}</CardDescription>
                {type.maxLevel && (
                  <div className="max-level">
                    <span className="level-label">Макс. уровень:</span>
                    <span className="level-value">{type.maxLevel}</span>
                  </div>
                )}
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Portal Network */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title={portalNetwork.title}
          subtitle="Транспортная сеть и логика войны"
        />
        <p className="section-intro">{portalNetwork.description}</p>
        <div className="principles-grid">
          {portalNetwork.principles.map((principle) => (
            <Card key={principle} className="principle-card">
              <CardBody>
                <CardDescription>{principle}</CardDescription>
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Campaign Structure */}
      <Section padding="lg">
        <SectionHeader
          title={campaignStructure.title}
          subtitle="Граф миров вместо линейного марша"
        />
        <p className="section-intro">{campaignStructure.description}</p>
        <div className="campaign-grid">
          {campaignStructure.nodeTypes.map((nodeType) => (
            <Card key={nodeType.name} className="campaign-card">
              <CardBody>
                <CardTitle>{nodeType.name}</CardTitle>
                <CardDescription>{nodeType.description}</CardDescription>
                <p className="campaign-role">{nodeType.role}</p>
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Faction Endgames */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title="Фракционные Финалы"
          subtitle="Как Родверы и Тал'Син закрывают сектор"
        />
        <div className="endgame-grid">
          {factionEndgames.map((endgame) => (
            <Card key={endgame.faction} className="endgame-card">
              <CardBody>
                <CardTitle>{endgame.faction}</CardTitle>
                <CardDescription>{endgame.summary}</CardDescription>
                <p className="endgame-line"><strong>Цель:</strong> {endgame.objective}</p>
                <p className="endgame-line"><strong>Контригра:</strong> {endgame.counterplay}</p>
                <p className="endgame-line"><strong>Итог:</strong> {endgame.outcome}</p>
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* Economy */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title="Экономика"
          subtitle="Валюты и ресурсы"
        />
        <p className="section-intro">{economy.description}</p>
        
        <div className="economy-section">
          <h3 className="economy-subtitle">Валюты</h3>
          <div className="currencies-grid">
            {economy.currencies.map((currency) => (
              <Card key={currency.name} className="currency-card">
                <CardBody>
                  <span className="currency-symbol">{currency.symbol}</span>
                  <CardTitle>{currency.name}</CardTitle>
                  <CardDescription>{currency.description}</CardDescription>
                </CardBody>
              </Card>
            ))}
          </div>
        </div>

        <div className="economy-section">
          <h3 className="economy-subtitle">Ресурсы</h3>
          <div className="resources-grid">
            {economy.resources.map((resource) => (
              <div
                key={resource.name}
                className={`resource-item resource-${resource.type}`}
              >
                <span className="resource-name">{resource.name}</span>
                <span className="resource-type">{resource.type}</span>
                <span className="resource-description">{resource.description}</span>
              </div>
            ))}
          </div>
        </div>
      </Section>

      {/* UX Principles */}
      <Section padding="lg">
        <SectionHeader
          title="UI/UX Принципы"
          subtitle="Философия дизайна интерфейса"
        />
        <div className="ux-principles">
          {uxPrinciples.map((principle) => (
            <Card key={principle.name} className="ux-card">
              <CardBody>
                <CardTitle>{principle.name}</CardTitle>
                <CardDescription>{principle.description}</CardDescription>
                {principle.examples.length > 0 && (
                  <ul className="ux-examples">
                    {principle.examples.map((example, i) => (
                      <li key={i}>{example}</li>
                    ))}
                  </ul>
                )}
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>
    </div>
  );
}
