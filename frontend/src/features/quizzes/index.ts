// Public API - selective exports only
// Page components for routing
export { QuizzesListPage, QuizDetailsPage } from './components';

// Main management hook for other features if needed
export { useQuizManagement, useQuizDetails, useQuizOperations } from './hooks';

// Essential types for cross-feature communication
export type { Quiz, QuizSummary, QuestionType, CreateQuizRequest } from './types';

// Utility functions for cross-feature use
export { 
  formatTimeLimit, 
  formatPassingScore,
  calculateCompletionPercentage,
  isQuizPassed
} from './lib';

