import type { Lesson } from '../types';

// Helper functions for lesson-related operations
export const formatLessonDuration = (durationMinutes: number): string => {
  const hours = Math.floor(durationMinutes / 60);
  const minutes = durationMinutes % 60;
  
  if (hours > 0) {
    return minutes > 0 ? `${hours}h ${minutes}m` : `${hours}h`;
  }
  return `${minutes}m`;
};

export const parseDurationToMinutes = (duration: string): number => {
  // Parse duration string like "1h 30m" or "90m" or "1.5h"
  const trimmed = duration.trim();
  
  // Handle decimal hours (e.g., "1.5h" = 90 minutes)
  if (trimmed.endsWith('h') && !trimmed.includes('m')) {
    const hours = parseFloat(trimmed.replace('h', ''));
    return Math.round(hours * 60);
  }
  
  // Handle hours and minutes (e.g., "1h 30m")
  const hourMatch = trimmed.match(/(\d+)h/);
  const minuteMatch = trimmed.match(/(\d+)m/);
  
  const hours = hourMatch ? parseInt(hourMatch[1], 10) : 0;
  const minutes = minuteMatch ? parseInt(minuteMatch[1], 10) : 0;
  
  return hours * 60 + minutes;
};

export const validateLessonDuration = (duration: string): boolean => {
  const minutes = parseDurationToMinutes(duration);
  return !isNaN(minutes) && minutes > 0;
};

export const getLessonStatusColor = (isPublished: boolean): string => {
  return isPublished ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700';
};

export const getLessonStatusLabel = (isPublished: boolean): string => {
  return isPublished ? 'Published' : 'Draft';
};

export const getLessonAccessLabel = (isFree: boolean): string => {
  return isFree ? 'Free' : 'Premium';
};

export const getLessonAccessColor = (isFree: boolean): string => {
  return isFree ? 'bg-blue-100 text-blue-700' : 'bg-purple-100 text-purple-700';
};

export const isLessonPublished = (lesson: { isPublished: boolean }): boolean => {
  return lesson.isPublished;
};

export const isLessonFree = (lesson: { isFree: boolean }): boolean => {
  return lesson.isFree;
};

export const sortLessonsByOrder = (lessons: Lesson[]): Lesson[] => {
  return [...lessons].sort((a, b) => a.orderIndex - b.orderIndex);
};

export const getNextOrderIndex = (lessons: Lesson[]): number => {
  if (lessons.length === 0) return 1;
  const maxOrder = Math.max(...lessons.map(l => l.orderIndex));
  return maxOrder + 1;
};

