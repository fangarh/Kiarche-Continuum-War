import { roadmapData } from '../data';
import { Section, SectionHeader, Card, CardBody, CardTitle, CardDescription } from '../components/ui';
import type { MilestoneStatus } from '../types';
import './Roadmap.css';

function getStatusLabel(status: MilestoneStatus): string {
  switch (status) {
    case 'done':
      return 'Готово';
    case 'in_progress':
      return 'В работе';
    case 'planned':
      return 'Запланировано';
    case 'delayed':
      return 'Отложено';
    default:
      return status;
  }
}

function getStatusClass(status: MilestoneStatus): string {
  return `status-${status}`;
}

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('ru-RU', { month: 'short', year: 'numeric' });
}

export function RoadmapPage() {
  const { phases } = roadmapData;

  return (
    <div className="roadmap-page">
      {/* Hero */}
      <section className="roadmap-hero">
        <div className="roadmap-hero-content">
          <h1 className="roadmap-title">Roadmap</h1>
          <p className="roadmap-tagline">
            План разработки и текущий статус проекта
          </p>
        </div>
      </section>

      {/* Phases */}
      {phases.map((phase, index) => (
        <Section key={phase.id} variant={index % 2 === 0 ? 'default' : 'alt'} padding="lg">
          <div className="phase-container">
            <div className="phase-header">
              <span className="phase-badge">Фаза {index + 1}</span>
              <h2 className="phase-title">{phase.name}</h2>
              <p className="phase-description">{phase.description}</p>
            </div>

            <div className="milestones-list">
              {phase.milestones.map((milestone) => (
                <Card key={milestone.id} className={`milestone-card milestone-${getStatusClass(milestone.status)}`}>
                  <CardBody>
                    <div className="milestone-header">
                      <CardTitle>{milestone.title}</CardTitle>
                      <span className={`milestone-status status-${milestone.status}`}>
                        {getStatusLabel(milestone.status)}
                      </span>
                    </div>
                    <CardDescription>{milestone.description}</CardDescription>
                    
                    <div className="milestone-dates">
                      <span className="milestone-date">
                        <span className="date-label">Начало:</span> {formatDate(milestone.startDate)}
                      </span>
                      <span className="milestone-date">
                        <span className="date-label">Конец:</span> {formatDate(milestone.endDate)}
                      </span>
                    </div>

                    <div className="milestone-tasks">
                      <span className="tasks-label">Задачи:</span>
                      <div className="tasks-list">
                        {milestone.tasks.map((task) => (
                          <div
                            key={task.id}
                            className={`task-item ${task.completed ? 'task-completed' : ''}`}
                          >
                            <span className="task-checkbox">
                              {task.completed ? '✓' : '○'}
                            </span>
                            <span className="task-title">{task.title}</span>
                          </div>
                        ))}
                      </div>
                    </div>

                    <div className="milestone-progress">
                      <div className="progress-bar">
                        <div
                          className="progress-fill"
                          style={{
                            width: `${(milestone.tasks.filter(t => t.completed).length / milestone.tasks.length) * 100}%`,
                          }}
                        />
                      </div>
                      <span className="progress-text">
                        {milestone.tasks.filter(t => t.completed).length} / {milestone.tasks.length}
                      </span>
                    </div>
                  </CardBody>
                </Card>
              ))}
            </div>
          </div>
        </Section>
      ))}

      {/* Legend */}
      <Section variant="dark" padding="lg">
        <SectionHeader
          title="Условные обозначения"
          subtitle="Статусы этапов разработки"
        />
        <div className="legend">
          <div className="legend-item">
            <span className="legend-dot status-done"></span>
            <span>Готово</span>
          </div>
          <div className="legend-item">
            <span className="legend-dot status-in_progress"></span>
            <span>В работе</span>
          </div>
          <div className="legend-item">
            <span className="legend-dot status-planned"></span>
            <span>Запланировано</span>
          </div>
          <div className="legend-item">
            <span className="legend-dot status-delayed"></span>
            <span>Отложено</span>
          </div>
        </div>
      </Section>
    </div>
  );
}
