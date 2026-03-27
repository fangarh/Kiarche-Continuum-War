import { useState } from 'react';
import { documentationData } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, CardDescription, Button } from '../components/ui';
import type { Specification } from '../types';
import './Docs.css';

type DocTab = 'guide' | 'ai' | 'specs';

const tabLabels: Record<DocTab, string> = {
  guide: 'Руководство',
  ai: 'AI Инструкции',
  specs: 'Спецификации',
};

export function DocsPage() {
  const [activeTab, setActiveTab] = useState<DocTab>('guide');
  const [expandedAi, setExpandedAi] = useState<string | null>(null);
  const [expandedSpec, setExpandedSpec] = useState<string | null>(null);

  const { devGuide, aiInstructions, specifications } = documentationData;

  const getStatusColor = (status: Specification['status']) => {
    switch (status) {
      case 'approved':
        return '#06d6a0';
      case 'review':
        return '#ffb703';
      case 'draft':
        return '#3a86ff';
      case 'deprecated':
        return '#ef476f';
      default:
        return '#8b8ba7';
    }
  };

  const getStatusLabel = (status: Specification['status']) => {
    switch (status) {
      case 'approved':
        return 'Утверждено';
      case 'review':
        return 'На проверке';
      case 'draft':
        return 'Черновик';
      case 'deprecated':
        return 'Устарело';
      default:
        return status;
    }
  };

  return (
    <div className="docs-page">
      {/* Hero */}
      <section className="docs-hero">
        <div className="docs-hero-content">
          <h1 className="docs-title">Документация</h1>
          <p className="docs-tagline">
            Руководства, инструкции и спецификации проекта
          </p>
        </div>
      </section>

      {/* Tabs */}
      <Section padding="sm">
        <div className="docs-tabs">
          {(Object.keys(tabLabels) as DocTab[]).map((tab) => (
            <button
              key={tab}
              className={`docs-tab ${activeTab === tab ? 'docs-tab-active' : ''}`}
              onClick={() => setActiveTab(tab)}
            >
              {tabLabels[tab]}
            </button>
          ))}
        </div>
      </Section>

      {/* Content */}
      {activeTab === 'guide' && (
        <Section padding="lg">
          <SectionHeader
            title="Руководство разработчика"
            subtitle="Документация для команды разработки"
          />
          <div className="dev-guide">
            {devGuide.map((section) => (
              <Card key={section.id} className="guide-card">
                <CardBody>
                  <CardTitle>{section.title}</CardTitle>
                  <div className="guide-content" dangerouslySetInnerHTML={{ __html: section.content.replace(/\n/g, '<br />') }} />
                  {section.subsections && (
                    <div className="guide-subsections">
                      {section.subsections.map((sub) => (
                        <div key={sub.id} className="guide-subsection">
                          <h4 className="guide-subsection-title">{sub.title}</h4>
                          <div className="guide-subsection-content" dangerouslySetInnerHTML={{ __html: sub.content.replace(/\n/g, '<br />') }} />
                        </div>
                      ))}
                    </div>
                  )}
                </CardBody>
              </Card>
            ))}
          </div>
        </Section>
      )}

      {activeTab === 'ai' && (
        <Section padding="lg">
          <SectionHeader
            title="Инструкции для AI"
            subtitle="Гайдлайны для работы с AI-ассистентами"
          />
          <div className="ai-instructions">
            {aiInstructions.map((instruction) => (
              <Card key={instruction.id} className="ai-card">
                <CardBody>
                  <div className="ai-card-header">
                    <CardTitle>{instruction.name}</CardTitle>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => setExpandedAi(expandedAi === instruction.id ? null : instruction.id)}
                    >
                      {expandedAi === instruction.id ? 'Свернуть' : 'Развернуть'}
                    </Button>
                  </div>
                  <CardDescription>{instruction.description}</CardDescription>
                  {expandedAi === instruction.id && (
                    <div className="ai-card-content">
                      <div className="ai-instructions-list">
                        <h4>Инструкции:</h4>
                        <ul>
                          {instruction.instructions.map((inst, i) => (
                            <li key={i}>{inst}</li>
                          ))}
                        </ul>
                      </div>
                      {instruction.examples && instruction.examples.length > 0 && (
                        <div className="ai-examples">
                          <h4>Примеры:</h4>
                          {instruction.examples.map((example, i) => (
                            <pre key={i} className="ai-example-code">{example}</pre>
                          ))}
                        </div>
                      )}
                    </div>
                  )}
                </CardBody>
              </Card>
            ))}
          </div>
        </Section>
      )}

      {activeTab === 'specs' && (
        <Section padding="lg">
          <SectionHeader
            title="Спецификации"
            subtitle="Технические документы и стандарты"
          />
          <div className="specifications">
            {specifications.map((spec) => (
              <Card key={spec.id} className="spec-card">
                <CardBody>
                  <div className="spec-header">
                    <div>
                      <CardTitle>{spec.title}</CardTitle>
                      <span className="spec-version">v{spec.version}</span>
                    </div>
                    <span
                      className="spec-status"
                      style={{ backgroundColor: getStatusColor(spec.status) }}
                    >
                      {getStatusLabel(spec.status)}
                    </span>
                  </div>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setExpandedSpec(expandedSpec === spec.id ? null : spec.id)}
                  >
                    {expandedSpec === spec.id ? 'Свернуть' : 'Развернуть'}
                  </Button>
                  {expandedSpec === spec.id && (
                    <div className="spec-content">
                      {spec.sections.map((section) => (
                        <div key={section.id} className="spec-section">
                          <h4 className="spec-section-title">{section.title}</h4>
                          <div className="spec-section-content" dangerouslySetInnerHTML={{ __html: section.content.replace(/\n/g, '<br />') }} />
                        </div>
                      ))}
                    </div>
                  )}
                </CardBody>
              </Card>
            ))}
          </div>
        </Section>
      )}
    </div>
  );
}
