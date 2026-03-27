import type { TeamData } from '../types';

// ----------------------------------------------------------------------------
// Команда проекта Kiarche Continuum War
// ----------------------------------------------------------------------------
//
// Примечание: Замените данные на актуальных участников проекта
//
// ----------------------------------------------------------------------------

export const teamData: TeamData = {
  members: [
    {
      id: 'member-001',
      name: 'Разработчик',
      role: 'Lead Developer / Game Designer',
      description: 'Основатель проекта, ведущий разработчик и дизайнер. Ответственен за архитектуру игры, лор и дизайн фракций.',
      imageUrl: '/assets/team/placeholder.jpg',
      socialLinks: [
        { platform: 'github', url: 'https://github.com', label: 'GitHub' },
        { platform: 'twitter', url: 'https://twitter.com', label: 'Twitter' },
      ],
    },
    // --------------------------------------------------------------------------
    // ШАБЛОНЫ ДЛЯ ДОБАВЛЕНИЯ НОВЫХ УЧАСТНИКОВ
    // --------------------------------------------------------------------------
    // Скопируйте блок ниже и заполните реальными данными:
    //
    // {
    //   id: 'member-XXX',
    //   name: 'Имя Фамилия',
    //   role: 'Должность',
    //   description: 'Краткое описание роли и ответственности',
    //   imageUrl: '/assets/team/photo.jpg',
    //   socialLinks: [
    //     { platform: 'github', url: 'https://github.com/username', label: 'GitHub' },
    //     { platform: 'twitter', url: 'https://twitter.com/username', label: 'Twitter' },
    //     { platform: 'linkedin', url: 'https://linkedin.com/in/username', label: 'LinkedIn' },
    //     { platform: 'email', url: 'mailto:email@example.com', label: 'Email' },
    //   ],
    // },
    // --------------------------------------------------------------------------
  ],
};

// ----------------------------------------------------------------------------
// РОЛИ В ПРОЕКТЕ (для справки)
// ----------------------------------------------------------------------------
//
// **Разработка:**
// - Lead Developer — ведущий разработчик
// - Unity Developer — разработчик Unity
// - AI Programmer — программист ИИ
// - Network Programmer — сетевой программист
//
// **Дизайн:**
// - Game Designer — дизайнер игры
// - Level Designer — дизайнер уровней
// - UI/UX Designer — дизайнер интерфейсов
//
// **Арт:**
// - Concept Artist — концепт-художник
// - 3D Artist — 3D-моделлер
// - VFX Artist — художник эффектов
// - Animator — аниматор
//
// **Звук:**
// - Composer — композитор
// - Sound Designer — звуковой дизайнер
//
// **Продюсирование:**
// - Producer — продюсер
// - Community Manager — менеджер сообщества
// - QA Lead — руководитель тестирования
//
// ----------------------------------------------------------------------------
