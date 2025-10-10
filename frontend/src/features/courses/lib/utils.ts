import { CourseLevel } from '../types';

// Helper functions for course-related operations
export const getCourseLevelLabel = (level: CourseLevel): string => {
  switch (level) {
    case CourseLevel.Beginner: return 'Beginner';
    case CourseLevel.Intermediate: return 'Intermediate';
    case CourseLevel.Advanced: return 'Advanced';
    case CourseLevel.Expert: return 'Expert';
    default: return 'Unknown';
  }
};

export const getCourseLevelColor = (level: CourseLevel): string => {
  switch (level) {
    case CourseLevel.Beginner: return 'bg-green-100 text-green-700';
    case CourseLevel.Intermediate: return 'bg-blue-100 text-blue-700';
    case CourseLevel.Advanced: return 'bg-orange-100 text-orange-700';
    case CourseLevel.Expert: return 'bg-red-100 text-red-700';
    default: return 'bg-gray-100 text-gray-700';
  }
};

export const formatCoursePrice = (price: number): string => {
  return price === 0 ? 'Free' : `$${price.toFixed(2)}`;
};

export const formatCourseDuration = (hours: number): string => {
  if (hours < 1) {
    return `${Math.round(hours * 60)} minutes`;
  }
  return `${hours}h`;
};

// Course validation utilities
export const validateCoursePrice = (price: string): boolean => {
  const numPrice = parseFloat(price);
  return !isNaN(numPrice) && numPrice >= 0;
};

export const validateCourseDuration = (duration: string): boolean => {
  const numDuration = parseFloat(duration);
  return !isNaN(numDuration) && numDuration > 0;
};

// Course status utilities
export const isCoursePublished = (course: { isPublished: boolean }): boolean => {
  return course.isPublished;
};

export const isCourseFeatured = (course: { isFeatured: boolean }): boolean => {
  return course.isFeatured;
};

export const getCourseStatusColor = (isPublished: boolean): string => {
  return isPublished ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700';
};

export const getCourseStatusLabel = (isPublished: boolean): string => {
  return isPublished ? 'Published' : 'Draft';
};