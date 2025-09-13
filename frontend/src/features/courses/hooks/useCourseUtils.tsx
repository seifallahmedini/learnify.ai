import { useMemo } from 'react';
import type { Course, CourseSummary } from '../types';
import { 
  getCourseLevelLabel, 
  getCourseLevelColor, 
  formatCoursePrice, 
  formatCourseDuration 
} from '../types';

// Hook for course utility functions
export const useCourseUtils = () => {
  const utils = useMemo(() => ({
    // Course level utilities
    getCourseLevelLabel,
    getCourseLevelColor,
    
    // Formatting utilities
    formatPrice: formatCoursePrice,
    formatDuration: formatCourseDuration,
    
    // Course status utilities
    getCourseStatusColor: (isPublished: boolean): string => {
      return isPublished ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700';
    },
    
    getCourseStatusLabel: (isPublished: boolean): string => {
      return isPublished ? 'Published' : 'Draft';
    },
    
    // Feature status utilities
    getFeatureStatusColor: (isFeatured: boolean): string => {
      return isFeatured ? 'bg-purple-100 text-purple-700' : 'bg-gray-100 text-gray-700';
    },
    
    getFeatureStatusLabel: (isFeatured: boolean): string => {
      return isFeatured ? 'Featured' : 'Standard';
    },
    
    // Course initials for avatars
    getCourseInitials: (title: string): string => {
      return title
        .split(' ')
        .map(word => word.charAt(0))
        .join('')
        .toUpperCase()
        .slice(0, 2);
    },
    
    // Course URL slug
    getCourseSlug: (title: string): string => {
      return title
        .toLowerCase()
        .replace(/[^a-z0-9\s-]/g, '')
        .replace(/\s+/g, '-')
        .replace(/-+/g, '-')
        .trim();
    },
    
    // Enrollment capacity utilities
    getEnrollmentCapacityInfo: (totalStudents: number, maxStudents?: number) => {
      if (!maxStudents) {
        return {
          percentage: 0,
          label: 'Unlimited',
          status: 'available' as const
        };
      }
      
      const percentage = (totalStudents / maxStudents) * 100;
      
      return {
        percentage,
        label: `${totalStudents}/${maxStudents}`,
        status: percentage >= 100 ? 'full' as const : 
               percentage >= 80 ? 'almost-full' as const : 
               'available' as const
      };
    },
    
    // Price utilities
    getDiscountPercentage: (originalPrice: number, discountPrice?: number): number => {
      if (!discountPrice || discountPrice >= originalPrice) return 0;
      return Math.round(((originalPrice - discountPrice) / originalPrice) * 100);
    },
    
    getEffectivePrice: (price: number, discountPrice?: number): number => {
      return discountPrice && discountPrice < price ? discountPrice : price;
    },
    
    // Course completion utilities
    isCourseComplete: (course: Course | CourseSummary): boolean => {
      return !!(
        course.title &&
        course.shortDescription &&
        course.categoryName &&
        course.instructorName &&
        course.price >= 0
      );
    },
    
    canPublishCourse: (course: Course): boolean => {
      return !!(
        course.title &&
        course.description &&
        course.shortDescription &&
        course.learningObjectives &&
        course.categoryId &&
        course.instructorId &&
        course.price >= 0 &&
        course.durationHours > 0
      );
    },
    
    // Search utilities
    filterCoursesByTerm: (courses: CourseSummary[], searchTerm: string): CourseSummary[] => {
      if (!searchTerm.trim()) return courses;
      
      const term = searchTerm.toLowerCase();
      return courses.filter(course =>
        course.title.toLowerCase().includes(term) ||
        course.shortDescription.toLowerCase().includes(term) ||
        course.instructorName.toLowerCase().includes(term) ||
        course.categoryName.toLowerCase().includes(term)
      );
    },
    
    // Sort utilities
    sortCourses: (courses: CourseSummary[], field: string, direction: 'asc' | 'desc'): CourseSummary[] => {
      return [...courses].sort((a, b) => {
        let valueA: any;
        let valueB: any;
        
        switch (field) {
          case 'title':
            valueA = a.title.toLowerCase();
            valueB = b.title.toLowerCase();
            break;
          case 'price':
            valueA = a.effectivePrice;
            valueB = b.effectivePrice;
            break;
          case 'rating':
            valueA = a.averageRating;
            valueB = b.averageRating;
            break;
          case 'students':
            valueA = a.totalStudents;
            valueB = b.totalStudents;
            break;
          case 'createdAt':
            valueA = new Date(a.createdAt);
            valueB = new Date(b.createdAt);
            break;
          default:
            return 0;
        }
        
        if (valueA < valueB) return direction === 'asc' ? -1 : 1;
        if (valueA > valueB) return direction === 'asc' ? 1 : -1;
        return 0;
      });
    },
  }), []);

  return utils;
};