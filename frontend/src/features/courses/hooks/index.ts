// Main management hook - consolidates CRUD operations and state
export { useCourseManagement, useCourseDetails } from './useCourseManagement';

// Form hooks for course creation and editing
export { useCreateCourseForm } from './useCreateCourseForm';
export { useEditCourseForm } from './useEditCourseForm';

// Utility hooks for feature-specific operations
export { useCourseUtils } from './useCourseUtils';
export { useSelectionManager } from './useSelectionManager';
export { useBulkOperations } from './useBulkOperations';