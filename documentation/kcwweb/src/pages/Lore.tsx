import { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { loreData, conceptArts, extendedLore } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, CardDescription, Gallery } from '../components/ui';
import type { Faction } from '../types';
import './Lore.css';

type LoreTab = 'worlds' | 'history' | 'factions' | 'characters' | 'details';

// Играбельные фракции — определяются по полю playable в данных
// PLAYABLE_FACTION_IDS больше не используется, см. faction.playable

const tabLabels: Record<LoreTab, string> = {
  worlds: 'Миры',
  history: 'История',
  factions: 'Фракции',
  characters: 'Персонажи',
  details: 'Детали',
};

function getFactionColor(alignment: Faction['alignment']): string {
  switch (alignment) {
    case 'good':
      return '#3a86ff';
    case 'neutral':
      return '#ffb703';
    case 'evil':
      return '#d00000';
    case 'unknown':
      return '#7b2cbf';
    default:
      return '#8b8ba7';
  }
}

export function LorePage() {
  const [activeTab, setActiveTab] = useState<LoreTab>('worlds');
  const [selectedFaction, setSelectedFaction] = useState<string>('kiarche');
  const [selectedCharacter, setSelectedCharacter] = useState<string | null>(null);

  // Список фракций для таба "Детали"
  const factionList = [
    // Древние фракции
    { id: 'kiarche', name: "Ки'Архе", colors: ['#6B46C1', '#553C9A', '#44337A'] },
    { id: 'syntex', name: 'Синтекс', colors: ['#4A90D9', '#2C5282', '#1A365D'] },
    { id: 'eterns', name: 'Этерны', colors: ['#E2E8F0', '#CBD5E0', '#A0AEC0'] },
    // Современные фракции
    { id: 'talsin', name: "Тал'Син", colors: ['#805AD5', '#6B46C1', '#553C9A'] },
    { id: 'keshari', name: 'Кешари', colors: ['#38A169', '#2F855A', '#276749'] },
    { id: 'sylni', name: "Сил'Ни", colors: ['#48BB78', '#38A169', '#2F855A'] },
    { id: 'velketh', name: "Вел'Кеты", colors: ['#C53030', '#9B2C2C', '#82227A'] },
    // Молодые расы
    { id: 'rodver', name: 'Родверы', colors: ['#3182CE', '#2B6CB0', '#1A365D'] },
    { id: 'rezir', name: "Ре'Зиры", colors: ['#C53030', '#9B2C2C', '#742A2A'] },
    { id: 'tion', name: "Ти'Оны", colors: ['#38A169', '#2F855A', '#276749'] },
    { id: 'veori', name: "Ве'Ори", colors: ['#E2E8F0', '#CBD5E0', '#A0AEC0'] },
  ];

  const renderFactionContent = (factionId: string) => {
    switch (factionId) {
      case 'kiarche':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.kiarche.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.kiarche.philosophy}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.kiarche.mechanics}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.kiarche.conflict}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.kiarche.relations}</ReactMarkdown>
            </div>
          </div>
        );
      case 'talsin':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.talsin.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.talsin.philosophy}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.talsin.mechanics}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.talsin.quests}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.talsin.gameplay}</ReactMarkdown>
            </div>
          </div>
        );
      case 'syntex':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.syntex.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.syntex.purpose}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.syntex.conflict}</ReactMarkdown>
            </div>
          </div>
        );
      case 'eterns':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.eterns.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.eterns.nature}</ReactMarkdown>
            </div>
          </div>
        );
      case 'keshari':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.keshari.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.keshari.values}</ReactMarkdown>
            </div>
          </div>
        );
      case 'sylni':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.sylni.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.sylni.philosophy}</ReactMarkdown>
            </div>
          </div>
        );
      case 'velketh':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.velketh.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.velketh.biology}</ReactMarkdown>
            </div>
          </div>
        );

      // =========================================================================
      // МОЛОДЫЕ РАСЫ
      // =========================================================================

      case 'rodver':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rodver.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rodver.biology}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rodver.culture}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rodver.mechanics}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rodver.gameplay}</ReactMarkdown>
            </div>
          </div>
        );

      case 'rezir':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rezir.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rezir.biology}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rezir.culture}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rezir.mechanics}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.rezir.gameplay}</ReactMarkdown>
            </div>
          </div>
        );

      case 'tion':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.tion.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.tion.biology}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.tion.culture}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.tion.mechanics}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.tion.gameplay}</ReactMarkdown>
            </div>
          </div>
        );

      case 'veori':
        return (
          <div className="details-content">
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.veori.overview}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.veori.biology}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.veori.culture}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.veori.mechanics}</ReactMarkdown>
            </div>
            <div className="details-markdown">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>{extendedLore.veori.gameplay}</ReactMarkdown>
            </div>
          </div>
        );

      default:
        return null;
    }
  };

  const renderContent = () => {
    switch (activeTab) {
      case 'worlds':
        return (
          <div className="worlds-grid">
            {loreData.worlds.map((world) => (
              <Card key={world.name} className="world-card" hoverable>
                {world.imageUrl && (
                  <img src={world.imageUrl} alt={world.name} className="world-image" />
                )}
                <CardBody>
                  <CardTitle>{world.name}</CardTitle>
                  <CardDescription>{world.description}</CardDescription>
                </CardBody>
              </Card>
            ))}
          </div>
        );

      case 'history':
        return (
          <div className="timeline">
            {loreData.history.map((event) => (
              <div key={event.id} className="timeline-item">
                <div className="timeline-marker"></div>
                <div className="timeline-content">
                  <span className="timeline-year">{event.year}</span>
                  <span className="timeline-era">{event.era}</span>
                  <h3 className="timeline-title">{event.title}</h3>
                  <p className="timeline-description">{event.description}</p>
                </div>
              </div>
            ))}
          </div>
        );

      case 'factions': {
        const playableFactions = loreData.factions.filter(f => f.playable);
        const nonPlayableFactions = loreData.factions.filter(f => !f.playable);

        const renderFactionCard = (faction: Faction, isPlayable: boolean) => (
          <Card key={faction.id} className={`faction-card ${isPlayable ? 'faction-card-playable' : ''}`}>
            <div
              className="faction-header"
              style={{ borderColor: getFactionColor(faction.alignment) }}
            >
              <div className="faction-header-left">
                <div className="faction-colors">
                  {faction.colors.map((color, i) => (
                    <span
                      key={i}
                      className="faction-color"
                      style={{ backgroundColor: color }}
                    />
                  ))}
                </div>
                {isPlayable && <span className="faction-playable-badge">Играбельная</span>}
              </div>
              <span
                className="faction-alignment"
                style={{ color: getFactionColor(faction.alignment) }}
              >
                {faction.alignment === 'good' && 'Добро'}
                {faction.alignment === 'neutral' && 'Нейтрал'}
                {faction.alignment === 'evil' && 'Зло'}
                {faction.alignment === 'unknown' && 'Неизвестно'}
              </span>
            </div>
            <CardBody>
              <CardTitle>{faction.name}</CardTitle>
              <CardDescription>{faction.description}</CardDescription>
            </CardBody>
          </Card>
        );

        return (
          <div className="factions-container">
            {playableFactions.length > 0 && (
              <div className="factions-section">
                <h2 className="factions-section-title">Фракции игрока</h2>
                <div className="factions-grid factions-grid-playable">
                  {playableFactions.map(f => renderFactionCard(f, true))}
                </div>
              </div>
            )}
            {nonPlayableFactions.length > 0 && (
              <div className="factions-section">
                <h2 className="factions-section-title">Остальные фракции</h2>
                <div className="factions-grid">
                  {nonPlayableFactions.map(f => renderFactionCard(f, false))}
                </div>
              </div>
            )}
          </div>
        );
      }

      case 'characters': {
        const selectedChar = loreData.characters.find(c => c.id === selectedCharacter);

        return (
          <div className="characters-container">
            {/* Character Navigation */}
            <div className="characters-nav">
              {loreData.characters.map((char) => (
                <button
                  key={char.id}
                  className={`character-nav-btn ${selectedCharacter === char.id ? 'character-nav-btn-active' : ''}`}
                  onClick={() => setSelectedCharacter(char.id)}
                >
                  {char.name}
                </button>
              ))}
            </div>

            {/* Selected Character Detail */}
            {selectedChar && (
              <div className="character-detail">
                <Card className="character-card-detail" hoverable>
                  {selectedChar.imageUrl && (
                    <img src={selectedChar.imageUrl} alt={selectedChar.name} className="character-image" />
                  )}
                  <CardBody>
                    <CardTitle>{selectedChar.name}</CardTitle>
                    <span className="character-role">{selectedChar.role}</span>
                    <CardDescription>{selectedChar.description}</CardDescription>
                    {selectedChar.abilities && selectedChar.abilities.length > 0 && (
                      <div className="character-abilities">
                        <span className="abilities-label">Способности:</span>
                        <div className="abilities-list">
                          {selectedChar.abilities.map((ability) => (
                            <span key={ability} className="ability-tag">{ability}</span>
                          ))}
                        </div>
                      </div>
                    )}
                  </CardBody>
                </Card>
              </div>
            )}
          </div>
        );
      }

      case 'details':
        return (
          <div className="details-container">
            {/* Faction Selector */}
            <div className="details-faction-nav">
              {factionList.map((faction) => (
                <button
                  key={faction.id}
                  className={`details-faction-btn ${selectedFaction === faction.id ? 'details-faction-btn-active' : ''}`}
                  onClick={() => setSelectedFaction(faction.id)}
                >
                  <span className="faction-colors-inline">
                    {faction.colors.map((color, i) => (
                      <span
                        key={i}
                        className="faction-color-dot"
                        style={{ backgroundColor: color }}
                      />
                    ))}
                  </span>
                  {faction.name}
                </button>
              ))}
            </div>

            {/* Selected Faction Content */}
            {selectedFaction && (
              <div className="details-section">
                <h2 className="details-title">
                  {factionList.find(f => f.id === selectedFaction)?.name}
                </h2>
                {renderFactionContent(selectedFaction)}
              </div>
            )}
          </div>
        );

      default:
        return null;
    }
  };

  return (
    <div className="lore-page">
      {/* Hero */}
      <section className="lore-hero">
        <div className="lore-hero-content">
          <h1 className="lore-title">Лор и Мир</h1>
          <p className="lore-tagline">
            Погрузитесь в историю вселенной Kiarche Continuum War
          </p>
        </div>
      </section>

      {/* Tabs */}
      <div className="lore-tabs-wrapper">
        <div className="lore-tabs">
          {(Object.keys(tabLabels) as LoreTab[]).map((tab) => (
            <button
              key={tab}
              className={`lore-tab ${activeTab === tab ? 'lore-tab-active' : ''}`}
              onClick={() => {
                setActiveTab(tab);
                if (tab === 'details') {
                  setSelectedFaction('kiarche');
                } else if (tab === 'characters') {
                  setSelectedCharacter(loreData.characters[0]?.id || null);
                }
              }}
            >
              {tabLabels[tab]}
            </button>
          ))}
        </div>
      </div>

      {/* Content */}
      <Section padding="lg">
        {renderContent()}
      </Section>

      {/* Concept Arts */}
      <Section variant="alt" padding="lg">
        <div id="concepts"></div>
        <SectionHeader
          title="Концепт-арты"
          subtitle="Визуальное воплощение мира"
        />
        <Gallery items={conceptArts} />
      </Section>
    </div>
  );
}
