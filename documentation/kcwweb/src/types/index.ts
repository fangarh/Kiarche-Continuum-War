// ============================================================================
// Основные типы данных для проекта Kiarche Continuum War
// ============================================================================

// ----------------------------------------------------------------------------
// Навигация
// ----------------------------------------------------------------------------
export interface NavItem {
  label: string;
  path: string;
}

// ----------------------------------------------------------------------------
// Концепт-арты и галерея
// ----------------------------------------------------------------------------
export interface ConceptArt {
  id: string;
  title: string;
  description: string;
  imageUrl?: string;
  category: ConceptArtCategory;
  tags: string[];
}

export type ConceptArtCategory = 
  | 'characters'
  | 'environments'
  | 'vehicles'
  | 'weapons'
  | 'ui'
  | 'other';

export interface GalleryProps {
  items: ConceptArt[];
  title?: string;
  filterByCategory?: ConceptArtCategory;
}

// ----------------------------------------------------------------------------
// Лор и мир игры
// ----------------------------------------------------------------------------
export interface WorldInfo {
  name: string;
  description: string;
  imageUrl?: string;
}

export interface HistoryEvent {
  id: string;
  title: string;
  year: string;
  description: string;
  era: string;
}

export interface Faction {
  id: string;
  name: string;
  description: string;
  alignment: 'good' | 'neutral' | 'evil' | 'unknown';
  playable: boolean; // true = играбельная фракция, false = NPC/антагонист
  symbol?: string;
  colors: string[];
}

export interface Character {
  id: string;
  name: string;
  role: string;
  description: string;
  factionId?: string;
  imageUrl?: string;
  abilities?: string[];
}

export interface LoreData {
  worlds: WorldInfo[];
  history: HistoryEvent[];
  factions: Faction[];
  characters: Character[];
}

// ----------------------------------------------------------------------------
// Игровые механики
// ----------------------------------------------------------------------------
export interface GameplayLoop {
  title: string;
  description: string;
  steps: LoopStep[];
}

export interface LoopStep {
  order: number;
  title: string;
  description: string;
  icon?: string;
}

export interface CombatSystem {
  title: string;
  description: string;
  features: CombatFeature[];
}

export interface CombatFeature {
  id: string;
  name: string;
  description: string;
}

export interface ProgressionSystem {
  title: string;
  description: string;
  types: ProgressionType[];
}

export interface ProgressionType {
  name: string;
  description: string;
  maxLevel?: number;
}

export interface EconomySystem {
  title: string;
  description: string;
  currencies: Currency[];
  resources: Resource[];
}

export interface Currency {
  name: string;
  symbol: string;
  description: string;
}

export interface Resource {
  name: string;
  type: 'common' | 'rare' | 'legendary';
  description: string;
}

export interface UXPrinciple {
  name: string;
  description: string;
  examples: string[];
}

// ----------------------------------------------------------------------------
// Механики молодых рас
// ----------------------------------------------------------------------------
export interface EraInfo {
  name: string;
  tech: string;
  key: string;
}

export interface YoungRaceMechanic {
  id: string;
  name: string;
  description: string;
  mechanics: string[];
  techTree: string;
  victoryCondition: string;
  eras?: EraInfo[];
}

export interface YoungRacesSystem {
  title: string;
  description: string;
  races: YoungRaceMechanic[];
}

export interface MechanicsData {
  coreLoop: GameplayLoop;
  combat: CombatSystem;
  progression: ProgressionSystem;
  youngRaces: YoungRacesSystem;
  economy: EconomySystem;
  uxPrinciples: UXPrinciple[];
}

// ----------------------------------------------------------------------------
// Архитектура проекта
// ----------------------------------------------------------------------------
export interface ProjectModule {
  name: string;
  description: string;
  path: string;
  dependencies: string[];
}

export interface DataFlow {
  name: string;
  description: string;
  from: string;
  to: string;
  type: 'sync' | 'async' | 'event';
}

export interface ArchitectureData {
  modules: ProjectModule[];
  dataFlows: DataFlow[];
  diagramPlaceholders: DiagramPlaceholder[];
}

export interface DiagramPlaceholder {
  id: string;
  title: string;
  description: string;
  type: 'flowchart' | 'sequence' | 'class' | 'component';
}

// ----------------------------------------------------------------------------
// Документация
// ----------------------------------------------------------------------------
export interface DocSection {
  id: string;
  title: string;
  content: string;
  subsections?: DocSection[];
}

export interface AiInstruction {
  id: string;
  name: string;
  description: string;
  instructions: string[];
  examples?: string[];
}

export interface Specification {
  id: string;
  title: string;
  version: string;
  status: 'draft' | 'review' | 'approved' | 'deprecated';
  sections: DocSection[];
}

export interface DocumentationData {
  devGuide: DocSection[];
  aiInstructions: AiInstruction[];
  specifications: Specification[];
}

// ----------------------------------------------------------------------------
// Roadmap
// ----------------------------------------------------------------------------
export type MilestoneStatus = 'planned' | 'in_progress' | 'done' | 'delayed';

export interface Milestone {
  id: string;
  title: string;
  description: string;
  status: MilestoneStatus;
  startDate: string;
  endDate: string;
  tasks: MilestoneTask[];
}

export interface MilestoneTask {
  id: string;
  title: string;
  completed: boolean;
}

export interface RoadmapPhase {
  id: string;
  name: string;
  description: string;
  milestones: Milestone[];
}

export interface RoadmapData {
  phases: RoadmapPhase[];
}

// ----------------------------------------------------------------------------
// Devlog / Заметки
// ----------------------------------------------------------------------------
export interface DevlogEntry {
  id: string;
  title: string;
  date: string;
  category: DevlogCategory;
  content: string;
  tags: string[];
}

export type DevlogCategory = 
  | 'development'
  | 'design'
  | 'idea'
  | 'decision'
  | 'bugfix'
  | 'feature';

export interface DevlogData {
  entries: DevlogEntry[];
}

// ----------------------------------------------------------------------------
// Команда
// ----------------------------------------------------------------------------
export interface TeamMember {
  id: string;
  name: string;
  role: string;
  description?: string;
  imageUrl?: string;
  socialLinks?: SocialLink[];
}

export interface SocialLink {
  platform: 'github' | 'twitter' | 'linkedin' | 'email';
  url: string;
  label: string;
}

export interface TeamData {
  members: TeamMember[];
}

// ----------------------------------------------------------------------------
// Игра (основная информация)
// ----------------------------------------------------------------------------
export interface GameInfo {
  title: string;
  tagline: string;
  description: string;
  genre: string[];
  platforms: string[];
  targetAudience: string;
  features: GameFeature[];
  steamUrl?: string;
  trailerUrl?: string;
}

export interface GameFeature {
  id: string;
  title: string;
  description: string;
  icon?: string;
}

export interface HomePageData {
  hero: {
    title: string;
    tagline: string;
    ctaPrimary?: {
      label: string;
      path: string;
    };
    ctaSecondary?: {
      label: string;
      path: string;
    };
  };
  gameInfo: GameInfo;
  featuredConcepts: ConceptArt[];
}
