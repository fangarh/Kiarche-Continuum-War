import type { ArchitectureData } from '../types';

// ----------------------------------------------------------------------------
// Архитектура проекта
// ----------------------------------------------------------------------------
//
// Модульная архитектура игры Kiarche Continuum War
//
// Основные системы:
// - Portal Network: исследование и колонизация через порталы
// - Hero System: наём, прокачка, способности героев
// - Tech Tree: развитие технологий
// - Faction System: дипломатия и конфликты
//
// ----------------------------------------------------------------------------

export const architectureData: ArchitectureData = {
  modules: [
    // TODO: Обновить модули под новую архитектуру
    // {
    //   name: '...',
    //   description: '...',
    //   path: 'src/...',
    //   dependencies: ['...'],
    // },
  ],

  dataFlows: [
    // TODO: Добавить потоки данных
    // {
    //   name: '...',
    //   description: '...',
    //   from: '...',
    //   to: '...',
    //   type: 'sync' | 'async' | 'event',
    // },
  ],

  diagramPlaceholders: [
    // TODO: Добавить диаграммы
    // {
    //   id: 'diagram-...',
    //   title: '...',
    //   description: '...',
    //   type: 'flowchart' | 'sequence' | 'class' | 'component',
    // },
  ],
};
