import { overviewData } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, CardDescription, Button } from '../components/ui';
import './Overview.css';

export function OverviewPage() {
  const { title, tagline, description, genre, platforms, targetAudience, features } = overviewData;

  return (
    <div className="overview-page">
      {/* Hero */}
      <section className="overview-hero">
        <div className="overview-hero-content">
          <h1 className="overview-title">{title}</h1>
          <p className="overview-tagline">{tagline}</p>
        </div>
      </section>

      {/* Description */}
      <Section padding="lg">
        <div className="overview-description">
          <SectionHeader
            title="Описание"
            subtitle="Погрузитесь в мир Kiarche Continuum War"
          />
          <div className="description-content">
            <p className="description-text">{description}</p>
          </div>
        </div>
      </Section>

      {/* Info Grid */}
      <Section variant="alt" padding="lg">
        <div className="overview-info">
          <div className="info-card">
            <h3 className="info-title">Жанр</h3>
            <ul className="info-list">
              {genre.map((g) => (
                <li key={g} className="info-item">{g}</li>
              ))}
            </ul>
          </div>
          <div className="info-card">
            <h3 className="info-title">Платформы</h3>
            <ul className="info-list">
              {platforms.map((p) => (
                <li key={p} className="info-item">{p}</li>
              ))}
            </ul>
          </div>
          <div className="info-card info-card-full">
            <h3 className="info-title">Целевая аудитория</h3>
            <p className="info-text">{targetAudience}</p>
          </div>
        </div>
      </Section>

      {/* Features */}
      <Section padding="lg">
        <SectionHeader
          title="Особенности игры"
          subtitle="Что делает Kiarche Continuum War уникальной"
        />
        <div className="features-grid">
          {features.map((feature) => (
            <Card key={feature.id} className="feature-card-large" hoverable>
              <CardBody>
                <CardTitle>{feature.title}</CardTitle>
                <CardDescription>{feature.description}</CardDescription>
              </CardBody>
            </Card>
          ))}
        </div>
      </Section>

      {/* CTA */}
      <Section variant="dark" padding="lg">
        <div className="overview-cta">
          <h2 className="cta-title">Готовы начать путешествие?</h2>
          <p className="cta-text">
            Добавьте игру в Wishlist в Steam и будьте готовы к релизу
          </p>
          <div className="cta-buttons">
            <Button
              variant="primary"
              size="lg"
              href="https://store.steampowered.com/app/placeholder"
              external
            >
              <svg viewBox="0 0 24 24" fill="currentColor" className="button-icon">
                <path d="M11.979 0C5.678 0 .511 4.86.022 11.037l4.455 1.841a3.001 3.001 0 012.822-1.841c.431 0 .837.115 1.192.316l2.892-4.185V7.1c0-.295.023-.584.066-.868L6.606 4.28A7.037 7.037 0 0111.979 2c3.896 0 7.037 3.14 7.037 7.037 0 1.218-.31 2.362-.853 3.363l4.682 2.81A11.935 11.935 0 0024 11.979C24 5.365 18.635 0 11.979 0z"/>
              </svg>
              Wishlist в Steam
            </Button>
            <Button variant="outline" size="lg" to="/lore">
              Узнать лор
            </Button>
          </div>
        </div>
      </Section>
    </div>
  );
}
