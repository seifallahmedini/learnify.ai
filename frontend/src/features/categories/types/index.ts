// Category-related types and interfaces

// API Response wrapper
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface Category {
  id: number;
  name: string;
  description: string;
  slug: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  sortOrder: number;
  courseCount: number;
  totalStudents: number;
  parentId?: number;
  parentName?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CategorySummary {
  id: number;
  name: string;
  description: string;
  slug: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  sortOrder: number;
  courseCount: number;
  totalStudents: number;
  parentName?: string;
}

export interface CategoryListResponse {
  categories: CategorySummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CategoryDetailResponse extends Category {
  subcategories: CategorySummary[];
  topCourses: CategoryCourse[];
  statistics: CategoryStatistics;
}

export interface CategoryCourse {
  id: number;
  title: string;
  thumbnailUrl?: string;
  price: number;
  averageRating?: number;
  totalStudents: number;
  instructorName: string;
}

export interface CategoryStatistics {
  totalCourses: number;
  totalStudents: number;
  averageRating: number;
  popularityRank: number;
  growthRate: number;
}

// Request Types
export interface CreateCategoryRequest {
  name: string;
  description: string;
  slug: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  sortOrder: number;
  parentId?: number;
}

export interface UpdateCategoryRequest extends Partial<CreateCategoryRequest> {}

export interface CategoryFilterRequest {
  parentId?: number | null;
  isActive?: boolean;
  searchTerm?: string;
  sortBy?: 'name' | 'courseCount' | 'sortOrder' | 'createdAt';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

// Form Types
export interface CreateCategoryFormData {
  name: string;
  description: string;
  slug: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  sortOrder: string;
  parentId: number | '';
}

export interface UpdateCategoryFormData extends Partial<CreateCategoryFormData> {}

// Utility Types
export interface CategoryFilters {
  parent?: number | null;
  status?: 'active' | 'inactive';
  searchTerm?: string;
}

export interface CategorySortOptions {
  field: 'name' | 'courseCount' | 'sortOrder' | 'createdAt';
  direction: 'asc' | 'desc';
}

// Category tree structure for hierarchical display
export interface CategoryTreeNode {
  category: CategorySummary;
  children: CategoryTreeNode[];
  level: number;
}