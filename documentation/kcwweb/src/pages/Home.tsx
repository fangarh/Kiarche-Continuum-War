import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { homePageData } from '../data';
import { Section, SectionHeader, Button, Gallery, Card, CardBody, CardTitle, CardDescription } from '../components/ui';
import './Home.css';

export function HomePage() {
  const { hero, gameInfo, featuredConcepts } = homePageData;
  const [activeVideo, setActiveVideo] = useState(0);
  const [isTransitioning, setIsTransitioning] = useState(false);
  const video1Ref = useRef<HTMLVideoElement>(null);
  const video2Ref = useRef<HTMLVideoElement>(null);

  const handleVideoEnd = () => {
    setIsTransitioning(true);
    setTimeout(() => {
      setActiveVideo((prev) => (prev + 1) % 2);
      setIsTransitioning(false);
    }, 1000); // 1s transition
  };

  useEffect(() => {
    const activeVideoEl = activeVideo === 0 ? video1Ref.current : video2Ref.current;
    if (activeVideoEl) {
      activeVideoEl.currentTime = 0;
      activeVideoEl.play().catch(() => {});
    }
  }, [activeVideo]);

  return (
    <div className="home">
      {/* Hero Section */}
      <section className={`hero ${activeVideo === 1 ? 'video-2-active' : ''}`}>
        <div className="hero-background">
          <video
            ref={video1Ref}
            className={`hero-video ${activeVideo === 0 ? 'active' : ''} ${isTransitioning && activeVideo === 0 ? 'fade-out' : ''}`}
            autoPlay
            muted
            playsInline
            onEnded={handleVideoEnd}
            poster="/assets/concepts/kiarche/1774598701.png"
          >
            <source src="/assets/concepts/kiarche-hero.mp4" type="video/mp4" />
          </video>
          <video
            ref={video2Ref}
            className={`hero-video ${activeVideo === 1 ? 'active' : ''} ${isTransitioning && activeVideo === 1 ? 'fade-out' : ''}`}
            autoPlay
            muted
            playsInline
            onEnded={handleVideoEnd}
          >
            <source src="/assets/concepts/pixverse-v5-hero.mp4" type="video/mp4" />
          </video>
          <div className="hero-gradient"></div>
          <div className="hero-gradient hero-gradient-2"></div>
          <div className="hero-particles"></div>
        </div>
        <div className="hero-content">
          <h1 className="hero-title" data-text={hero.title}>{hero.title}</h1>
          <p className="hero-tagline">{hero.tagline}</p>
          <div className="hero-actions">
            <Button variant="primary" size="lg" to="/overview">
              Узнать больше
            </Button>
            <Button
              variant="outline"
              size="lg"
              href="https://store.steampowered.com/app/placeholder"
              external
            >
              <svg viewBox="0 0 24 24" fill="currentColor" className="button-icon">
                <path d="M11.979 0C5.678 0 .511 4.86.022 11.037l4.455 1.841a3.001 3.001 0 012.822-1.841c.431 0 .837.115 1.192.316l2.892-4.185V7.1c0-.295.023-.584.066-.868L6.606 4.28A7.037 7.037 0 0111.979 2c3.896 0 7.037 3.14 7.037 7.037 0 1.218-.31 2.362-.853 3.363l4.682 2.81A11.935 11.935 0 0024 11.979C24 5.365 18.635 0 11.979 0z"/>
              </svg>
              Wishlist в Steam
            </Button>
          </div>
        </div>
        <div className="hero-scroll">
          <span>Листайте вниз</span>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M12 5v14M5 12l7 7 7-7" />
          </svg>
        </div>
      </section>

      {/* About Preview */}
      <Section variant="alt" padding="lg">
        <SectionHeader
          title="Об игре"
          subtitle="Научно-фантастическое приключение в мире звёздных врат"
        />
        <div className="about-preview">
          <div className="about-content">
            <p className="about-text">{gameInfo.description}</p>
            <Button variant="primary" to="/overview">
              Подробнее об игре
            </Button>
          </div>
          <div className="about-features">
            {gameInfo.features.slice(0, 4).map((feature) => (
              <Card key={feature.id} className="feature-card">
                <CardBody>
                  <CardTitle>{feature.title}</CardTitle>
                  <CardDescription>{feature.description}</CardDescription>
                </CardBody>
              </Card>
            ))}
          </div>
        </div>
      </Section>

      {/* Concept Art Gallery Preview */}
      <Section padding="lg">
        <SectionHeader
          title="Концепт-арты"
          subtitle="Визуальное воплощение мира Kiarche Continuum War"
        />
        <Gallery items={featuredConcepts} />
        <div className="gallery-more">
          <Button variant="outline" size="lg" to="/lore#concepts">
            Смотреть все концепт-арты
          </Button>
        </div>
      </Section>

      {/* Quick Links */}
      <Section variant="dark" padding="lg">
        <div className="quick-links">
          <Link to="/mechanics" className="quick-link-card">
            <div className="quick-link-icon">⚔️</div>
            <h3>Механики</h3>
            <p>Боевая система, прогрессия и экономика</p>
          </Link>
          <Link to="/lore" className="quick-link-card">
            <div className="quick-link-icon">📖</div>
            <h3>Лор</h3>
            <p>Мир, история, фракции и персонажи</p>
          </Link>
          <Link to="/roadmap" className="quick-link-card">
            <div className="quick-link-icon">🗺️</div>
            <h3>Roadmap</h3>
            <p>План разработки и статусы</p>
          </Link>
          <Link to="/devlog" className="quick-link-card">
            <div className="quick-link-icon">📝</div>
            <h3>Devlog</h3>
            <p>История разработки и заметки</p>
          </Link>
        </div>
      </Section>

      {/* CTA Section */}
      <Section padding="lg">
        <div className="cta-section">
          <h2 className="cta-title">Следите за разработкой</h2>
          <p className="cta-text">
            Присоединяйтесь к нашему сообществу и узнавайте первыми о новостях разработки
          </p>
          <div className="cta-actions">
            <Button variant="primary" size="lg" to="/team">
              Познакомиться с командой
            </Button>
            <Button variant="ghost" size="lg" to="/docs">
              Документация
            </Button>
          </div>
        </div>
      </Section>
    </div>
  );
}
