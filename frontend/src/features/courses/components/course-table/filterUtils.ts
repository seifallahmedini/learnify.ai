import type { CourseSummary } from '../../types';
import type { CourseFilters } from '../course-table/CourseFilters';

export function applyFilters(courses: CourseSummary[], filters: CourseFilters): CourseSummary[] {
  return courses.filter(course => {
    // Search term filter
    if (filters.searchTerm) {
      const searchTerm = filters.searchTerm.toLowerCase();
      const matchesSearch = 
        course.title.toLowerCase().includes(searchTerm) ||
        course.shortDescription.toLowerCase().includes(searchTerm) ||
        course.instructorName.toLowerCase().includes(searchTerm) ||
        course.categoryName.toLowerCase().includes(searchTerm);
      
      if (!matchesSearch) return false;
    }

    // Status filter
    if (filters.status !== 'all') {
      if (filters.status === 'published' && !course.isPublished) return false;
      if (filters.status === 'draft' && course.isPublished) return false;
    }

    // Level filter
    if (filters.level !== 'all') {
      if (course.level !== filters.level) return false;
    }

    // Category filter (using category name instead of ID)
    if (filters.category !== 'all') {
      if (course.categoryName !== filters.category) return false;
    }

    // Featured filter
    if (filters.featured !== 'all') {
      if (filters.featured === 'featured' && !course.isFeatured) return false;
      if (filters.featured === 'regular' && course.isFeatured) return false;
    }

    // Price range filter
    if (filters.priceRange !== 'all') {
      if (filters.priceRange === 'free' && course.effectivePrice > 0) return false;
      if (filters.priceRange === 'paid' && course.effectivePrice === 0) return false;
      if (filters.priceRange === 'discounted' && (!course.discountPrice || course.discountPrice >= course.price)) return false;
    }

    // Minimum rating filter
    if (filters.minRating > 0) {
      if (!course.averageRating || course.averageRating < filters.minRating) return false;
    }

    // Maximum duration filter
    if (filters.maxDuration !== null) {
      if (course.durationHours > filters.maxDuration) return false;
    }

    // Minimum students filter
    if (filters.minStudents !== null) {
      if (course.totalStudents < filters.minStudents) return false;
    }

    return true;
  });
}

export function getUniqueCategories(courses: CourseSummary[]): Array<{ id: string; name: string }> {
  const categorySet = new Set<string>();
  
  courses.forEach(course => {
    categorySet.add(course.categoryName);
  });
  
  return Array.from(categorySet)
    .map(name => ({ id: name, name }))
    .sort((a, b) => a.name.localeCompare(b.name));
}