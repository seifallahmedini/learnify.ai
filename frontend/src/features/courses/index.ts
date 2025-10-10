// Public API - selective exports only
// Page components for routing
export { CoursesListPage, CourseDetailsPage } from './components';

// Main management hook for other features if needed
export { useCourseManagement } from './hooks';

// Essential types for cross-feature communication
export type { Course, CourseSummary, CourseLevel, CreateCourseRequest } from './types';

// Utility functions for cross-feature use
export { 
  getCourseLevelLabel, 
  getCourseLevelColor, 
  formatCoursePrice, 
  formatCourseDuration 
} from './lib';

// DO NOT export: internal components, services, utility hooks, form types