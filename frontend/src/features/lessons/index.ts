// Public API - selective exports only
// Page components for routing
export { LessonsListPage, LessonDetailsPage, AllLessonsPage } from './components';

// Main management hook for other features if needed
export { useLessonManagement } from './hooks';

// Essential types for cross-feature communication
export type { Lesson, LessonSummary, CreateLessonRequest } from './types';

// Utility functions for cross-feature use
export { 
  formatLessonDuration,
  getLessonStatusLabel,
  getLessonAccessLabel,
  sortLessonsByOrder,
} from './lib';

// DO NOT export: internal components, services, utility hooks, form types

