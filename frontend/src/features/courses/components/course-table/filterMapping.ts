import type { CourseFilterRequest, CourseLevel } from '../../types';

export interface UIFilters {
  searchTerm: string;
  status: 'all' | 'published' | 'draft';
  level: 'all' | CourseLevel;
  category: string;
  featured: 'all' | 'featured' | 'regular';
  priceRange: 'all' | 'free' | 'paid' | 'discounted' | 'custom';
  minRating: number;
  maxDuration: number | null;
  minStudents: number | null;
  minPrice: number | null;
  maxPrice: number | null;
}

export const defaultUIFilters: UIFilters = {
  searchTerm: '',
  status: 'all',
  level: 'all',
  category: 'all',
  featured: 'all',
  priceRange: 'all',
  minRating: 0,
  maxDuration: null,
  minStudents: null,
  minPrice: null,
  maxPrice: null,
};

/**
 * Maps UI filters to API filter request
 */
export function mapUIFiltersToAPI(uiFilters: UIFilters): CourseFilterRequest {
  const apiFilters: CourseFilterRequest = {};

  // Search term
  if (uiFilters.searchTerm.trim()) {
    apiFilters.searchTerm = uiFilters.searchTerm.trim();
  }

  // Publication status
  if (uiFilters.status !== 'all') {
    apiFilters.isPublished = uiFilters.status === 'published';
  }

  // Course level
  if (uiFilters.level !== 'all') {
    apiFilters.level = uiFilters.level;
  }

  // Featured status
  if (uiFilters.featured !== 'all') {
    apiFilters.isFeatured = uiFilters.featured === 'featured';
  }

  // Price range
  if (uiFilters.priceRange !== 'all') {
    switch (uiFilters.priceRange) {
      case 'free':
        apiFilters.minPrice = 0;
        apiFilters.maxPrice = 0;
        break;
      case 'paid':
        apiFilters.minPrice = 0.01;
        break;
      case 'discounted':
        // This would require additional API support for discount detection
        // For now, we'll handle this in the API or skip it
        break;
      case 'custom':
        // Custom price range is handled by separate min/max price fields
        break;
    }
  }

  // Custom price range
  if (uiFilters.minPrice !== null) {
    apiFilters.minPrice = uiFilters.minPrice;
  }
  if (uiFilters.maxPrice !== null) {
    apiFilters.maxPrice = uiFilters.maxPrice;
  }

  // Note: The following filters would need API support:
  // - minRating (would need API enhancement)
  // - maxDuration (would need API enhancement) 
  // - minStudents (would need API enhancement)
  // - category by name (current API uses categoryId)

  return apiFilters;
}

/**
 * Checks if any filters are active
 */
export function hasActiveFilters(uiFilters: UIFilters): boolean {
  return (
    uiFilters.searchTerm !== '' ||
    uiFilters.status !== 'all' ||
    uiFilters.level !== 'all' ||
    uiFilters.category !== 'all' ||
    uiFilters.featured !== 'all' ||
    uiFilters.priceRange !== 'all' ||
    uiFilters.minRating > 0 ||
    uiFilters.maxDuration !== null ||
    uiFilters.minStudents !== null ||
    uiFilters.minPrice !== null ||
    uiFilters.maxPrice !== null
  );
}

/**
 * Counts active filters
 */
export function getActiveFiltersCount(uiFilters: UIFilters): number {
  let count = 0;
  if (uiFilters.searchTerm) count++;
  if (uiFilters.status !== 'all') count++;
  if (uiFilters.level !== 'all') count++;
  if (uiFilters.category !== 'all') count++;
  if (uiFilters.featured !== 'all') count++;
  if (uiFilters.priceRange !== 'all') count++;
  if (uiFilters.minRating > 0) count++;
  if (uiFilters.maxDuration !== null) count++;
  if (uiFilters.minStudents !== null) count++;
  if (uiFilters.minPrice !== null) count++;
  if (uiFilters.maxPrice !== null) count++;
  return count;
}