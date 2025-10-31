// Quiz utility functions

import type { QuizSummary } from '../types';

/**
 * Format time limit to readable string
 */
export function formatTimeLimit(minutes?: number): string {
  if (!minutes) return 'No limit';
  
  if (minutes < 60) {
    return `${minutes} min`;
  }
  
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  
  if (mins === 0) {
    return `${hours} ${hours === 1 ? 'hour' : 'hours'}`;
  }
  
  return `${hours}h ${mins}m`;
}

/**
 * Format passing score as percentage
 */
export function formatPassingScore(score: number): string {
  return `${score}%`;
}

/**
 * Get quiz status label
 */
export function getQuizStatusLabel(isActive: boolean): string {
  return isActive ? 'Active' : 'Inactive';
}

/**
 * Sort quizzes by creation date (newest first)
 */
export function sortQuizzesByDate(quizzes: QuizSummary[]): QuizSummary[] {
  return [...quizzes].sort((a, b) => {
    const dateA = new Date(a.createdAt).getTime();
    const dateB = new Date(b.createdAt).getTime();
    return dateB - dateA;
  });
}

/**
 * Sort quizzes by title
 */
export function sortQuizzesByTitle(quizzes: QuizSummary[]): QuizSummary[] {
  return [...quizzes].sort((a, b) => a.title.localeCompare(b.title));
}

/**
 * Calculate quiz completion percentage
 */
export function calculateCompletionPercentage(score?: number, maxScore: number = 100): number {
  if (!score) return 0;
  return Math.round((score / maxScore) * 100);
}

/**
 * Check if quiz is passed based on score and passing score
 */
export function isQuizPassed(score: number, passingScore: number): boolean {
  return score >= passingScore;
}

