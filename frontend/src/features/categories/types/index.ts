// Category-related types and interfaces

// API Response wrapper
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

// Main category interface matching CategoryResponse from backend
export interface Category {
  id: number;
  name: string;
  description: string;
  iconUrl?: string;
  parentCategoryId?: number;
  parentCategoryName?: string;
  isActive: boolean;
  courseCount: number;
  subcategoryCount: number;
  createdAt: string;
  updatedAt: string;
  isRootCategory: boolean;
}

// Category summary matching CategorySummaryResponse from backend
export interface CategorySummary {
  id: number;
  name: string;
  description: string;
  iconUrl?: string;
  parentCategoryId?: number;
  isActive: boolean;
  courseCount: number;
  createdAt: string;
}

// Category list response matching CategoryListResponse from backend
export interface CategoryListResponse {
  categories: CategorySummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Category hierarchy for tree display
export interface CategoryHierarchy {
  id: number;
  name: string;
  description: string;
  iconUrl?: string;
  isActive: boolean;
  courseCount: number;
  children: CategoryHierarchy[];
}

// Breadcrumb navigation
export interface CategoryBreadcrumb {
  breadcrumbs: CategoryBreadcrumbItem[];
}

export interface CategoryBreadcrumbItem {
  id: number;
  name: string;
  iconUrl?: string;
}

// Analytics data
export interface CategoryAnalytics {
  categoryId: number;
  categoryName: string;
  directCourseCount: number;
  totalCourseCount: number;
  subcategoryCount: number;
  totalEnrollments: number;
  averageRating: number;
  analyticsDate: string;
}

// Trending categories
export interface TrendingCategory {
  id: number;
  name: string;
  description: string;
  iconUrl?: string;
  courseCount: number;
  recentEnrollments: number;
  growthRate: number;
  averageRating: number;
}

// Course count analytics
export interface CategoryCoursesCount {
  categoryId: number;
  categoryName: string;
  directCourseCount: number;
  totalCourseCount: number;
  subcategoryCount: number;
}

// Request Types
export interface CreateCategoryRequest {
  name: string;
  description: string;
  iconUrl?: string;
  parentCategoryId?: number;
  isActive?: boolean;
}

export interface UpdateCategoryRequest {
  name?: string;
  description?: string;
  iconUrl?: string;
  parentCategoryId?: number;
  isActive?: boolean;
}

export interface CategoryFilterRequest {
  isActive?: boolean;
  parentCategoryId?: number;
  rootOnly?: boolean;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

// Form Types for UI components
export interface CreateCategoryFormData {
  name: string;
  description: string;
  iconUrl?: string;
  parentCategoryId: number | '';
  isActive: boolean;
}

export interface UpdateCategoryFormData extends Partial<CreateCategoryFormData> {}

// Utility Types for frontend state management
export interface CategoryFilters {
  parentCategoryId?: number | null;
  isActive?: boolean;
  rootOnly?: boolean;
  searchTerm?: string;
}

export interface CategorySortOptions {
  field: 'name' | 'courseCount' | 'createdAt';
  direction: 'asc' | 'desc';
}

// Category tree structure for hierarchical display
export interface CategoryTreeNode {
  category: CategorySummary;
  children: CategoryTreeNode[];
  level: number;
}