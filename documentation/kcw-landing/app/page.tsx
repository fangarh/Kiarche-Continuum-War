import { landingContent } from "./landing-content";

const characterImageSizes =
    "(max-width: 760px) calc(100vw - 32px), (max-width: 1120px) calc((100vw - 66px) / 2), 410px";

function getOptimizedCharacterSrc(imageUrl: string, width: number) {
    return imageUrl.replace("/heroes/", "/heroes/optimized/").replace(".png", `-${width}.webp`);
}

function CharacterImage({ imageUrl, alt }: { imageUrl: string; alt: string }) {
    const srcSet = [480, 720, 960]
        .map((width) => `${getOptimizedCharacterSrc(imageUrl, width)} ${width}w`)
        .join(", ");

    return (
        <picture className="character-picture">
            <source type="image/webp" srcSet={srcSet} sizes={characterImageSizes} />
            <img
                src={getOptimizedCharacterSrc(imageUrl, 720)}
                alt={alt}
                loading="lazy"
                decoding="async"
                className="character-image"
            />
        </picture>
    );
}

function SectionHeader({
    eyebrow,
    title,
    description,
}: {
    eyebrow: string;
    title: string;
    description: string;
}) {
    return (
        <div className="section-header">
            <span className="section-eyebrow">{eyebrow}</span>
            <h2>{title}</h2>
            <p>{description}</p>
        </div>
    );
}

export default function HomePage() {
    const { navigation, hero, portalNetwork, gameplayLoop, heroes, factions, worlds, cta } = landingContent;

    return (
        <main className="landing-shell" id="top">
            <div className="page-noise" aria-hidden="true" />
            <div className="aurora aurora-left" aria-hidden="true" />
            <div className="aurora aurora-right" aria-hidden="true" />

            <header className="topbar">
                <a className="brand" href="#top">
                    <span className="brand-mark" aria-hidden="true" />
                    <span className="brand-copy">
                        <strong>Ки&#39;Архе</strong>
                        <span>Война Континуума</span>
                    </span>
                </a>

                <nav className="topnav" aria-label="Навигация по лендингу">
                    {navigation.map((item) => (
                        <a key={item.href} href={item.href}>
                            {item.label}
                        </a>
                    ))}
                </nav>
            </header>

            <section className="hero" aria-labelledby="hero-title">
                <div className="hero-copy">
                    <span className="section-eyebrow">{hero.eyebrow}</span>
                    <h1 id="hero-title">{hero.title}</h1>
                    <p>{hero.subtitle}</p>

                    <div className="hero-actions">
                        <a className="button button-primary" href={hero.primaryAction.href}>
                            {hero.primaryAction.label}
                        </a>
                        <a className="button button-secondary" href={hero.secondaryAction.href}>
                            {hero.secondaryAction.label}
                        </a>
                    </div>

                    <ul className="hero-highlights" aria-label="Ключевые преимущества">
                        {hero.highlights.map((item) => (
                            <li key={item}>{item}</li>
                        ))}
                    </ul>
                </div>

                <div className="hero-visual" aria-hidden="true">
                    <div className="monument-scene">
                        <div className="monument-base" />
                        <div className="monument-spire monument-spire-left" />
                        <div className="monument-spire monument-spire-center" />
                        <div className="monument-spire monument-spire-right" />
                        <div className="monument-core" />
                        <div className="monument-runes monument-runes-left" />
                        <div className="monument-runes monument-runes-right" />
                        <span className="portal-label">Космический монумент Синтекс</span>
                    </div>
                </div>
            </section>

            <section className="stats-band" aria-label="Ключевые факты">
                {hero.stats.map((stat) => (
                    <article className="stat-card" key={stat.label}>
                        <strong>{stat.value}</strong>
                        <span>{stat.label}</span>
                    </article>
                ))}
            </section>

            <section className="content-section portal-section" id="portal-network">
                <SectionHeader
                    eyebrow={portalNetwork.eyebrow}
                    title={portalNetwork.title}
                    description={portalNetwork.description}
                />

                <div className="portal-layout">
                    <div className="network-diagram" aria-hidden="true">
                        <div className="network-center">
                            <span>Центральный узел</span>
                        </div>
                        {portalNetwork.nodes.map((node, index) => (
                            <div
                                key={node.name}
                                className={`network-node network-node-${index + 1}`}
                            >
                                <strong>{node.name}</strong>
                                <span>{node.type}</span>
                            </div>
                        ))}
                    </div>

                    <div className="feature-stack">
                        {portalNetwork.points.map((point) => (
                            <article className="feature-card" key={point.title}>
                                <h3>{point.title}</h3>
                                <p>{point.description}</p>
                            </article>
                        ))}
                    </div>
                </div>
            </section>

            <section className="content-section" id="gameplay-loop">
                <SectionHeader
                    eyebrow={gameplayLoop.eyebrow}
                    title={gameplayLoop.title}
                    description={gameplayLoop.description}
                />

                <div className="loop-grid">
                    {gameplayLoop.steps.map((step) => (
                        <article className="loop-card" key={step.index}>
                            <span className="loop-index">{step.index}</span>
                            <h3>{step.title}</h3>
                            <p>{step.description}</p>
                        </article>
                    ))}
                </div>
            </section>

            <section className="content-section" id="heroes">
                <SectionHeader
                    eyebrow={heroes.eyebrow}
                    title={heroes.title}
                    description={heroes.description}
                />

                <div className="hero-card-grid">
                    {heroes.cards.map((card) => (
                        <article className="character-card" key={card.name}>
                            <div className="character-media">
                                <CharacterImage imageUrl={card.imageUrl} alt={card.name} />
                            </div>
                            <div className="character-content">
                                <span className="character-faction">{card.faction}</span>
                                <h3>{card.name}</h3>
                                <strong>{card.role}</strong>
                                <span className="character-era">{card.era}</span>
                                <p>{card.description}</p>
                            </div>
                        </article>
                    ))}
                </div>
            </section>

            <section className="content-section" id="factions">
                <SectionHeader
                    eyebrow={factions.eyebrow}
                    title={factions.title}
                    description={factions.description}
                />

                <div className="faction-grid">
                    {factions.cards.map((card) => (
                        <article className="faction-card" key={card.name}>
                            <span className="faction-accent">{card.accent}</span>
                            <h3>{card.name}</h3>
                            <p>{card.summary}</p>
                        </article>
                    ))}
                </div>
            </section>

            <section className="content-section" id="worlds">
                <SectionHeader
                    eyebrow={worlds.eyebrow}
                    title={worlds.title}
                    description={worlds.description}
                />

                <div className="world-grid">
                    {worlds.cards.map((card) => (
                        <article className="world-card" key={card.name}>
                            <span className="world-label">{card.label}</span>
                            <h3>{card.name}</h3>
                            <p>{card.description}</p>
                        </article>
                    ))}
                </div>
            </section>

            <section className="cta-panel">
                <SectionHeader
                    eyebrow={cta.eyebrow}
                    title={cta.title}
                    description={cta.description}
                />

                <div className="hero-actions">
                    <a className="button button-primary" href={cta.primaryAction.href}>
                        {cta.primaryAction.label}
                    </a>
                    <a className="button button-secondary" href={cta.secondaryAction.href}>
                        {cta.secondaryAction.label}
                    </a>
                </div>
            </section>
        </main>
    );
}
