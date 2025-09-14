// Course-related types and interfaces

// API Response wrapper
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface Course {
  id: number;
  title: string;
  description: string;
  shortDescription: string;
  thumbnailUrl?: string;
  price: number;
  discountPrice?: number;
  effectivePrice: number;
  durationHours: number;
  level: CourseLevel;
  language?: string;
  isPublished: boolean;
  isFeatured: boolean;
  maxStudents?: number;
  totalStudents: number;
  averageRating?: number;
  totalReviews: number;
  categoryId: number;
  categoryName: string;
  instructorId: number;
  instructorName: string;
  prerequisites?: string;
  learningObjectives: string;
  createdAt: string;
  updatedAt: string;
}

export const CourseLevel = {
  Beginner: 1,
  Intermediate: 2,
  Advanced: 3,
  Expert: 4
} as const;

export type CourseLevel = typeof CourseLevel[keyof typeof CourseLevel];

export interface CourseSummary {
  id: number;
  title: string;
  shortDescription: string;
  thumbnailUrl?: string;
  price: number;
  discountPrice?: number;
  effectivePrice: number;
  durationHours: number;
  level: CourseLevel;
  isPublished: boolean;
  isFeatured: boolean;
  totalStudents: number;
  averageRating?: number;
  totalReviews: number;
  categoryName: string;
  instructorName: string;
  createdAt: string;
}

export interface CourseListResponse {
  courses: CourseSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CourseDetailResponse extends Course {
  lessons: LessonSummary[];
  statistics: CourseStatistics;
}

export interface LessonSummary {
  id: number;
  title: string;
  durationMinutes: number;
  isCompleted: boolean;
  order: number;
}

export interface CourseStatistics {
  totalLessons: number;
  totalDuration: number;
  completionRate: number;
  avgRating: number;
  enrollmentTrend: number[];
}

// Request Types
export interface CreateCourseRequest {
  title: string;
  description: string;
  shortDescription: string;
  price: number;
  discountPrice?: number;
  durationHours: number;
  level: CourseLevel;
  language?: string;
  maxStudents?: number;
  categoryId: number;
  instructorId: number;
  prerequisites?: string;
  learningObjectives: string;
  isPublished: boolean;
  isFeatured: boolean;
}

export interface UpdateCourseRequest extends Partial<CreateCourseRequest> {}

export interface CourseFilterRequest {
  categoryId?: number;
  instructorId?: number;
  level?: CourseLevel;
  isPublished?: boolean;
  isFeatured?: boolean;
  minPrice?: number;
  maxPrice?: number;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

// Form Types
export interface CreateCourseFormData {
  title: string;
  description: string;
  shortDescription: string;
  price: string;
  discountPrice?: string;
  durationHours: string;
  level: CourseLevel | '';
  language?: string;
  maxStudents?: string;
  categoryId: number | '';
  instructorId: number | '';
  prerequisites?: string;
  learningObjectives: string;
}

export interface UpdateCourseFormData extends Partial<CreateCourseFormData> {}

// Utility Types
export interface CourseFilters {
  category?: string;
  level?: CourseLevel;
  status?: 'published' | 'draft';
  featured?: boolean;
  searchTerm?: string;
}

export interface CourseSortOptions {
  field: 'title' | 'price' | 'rating' | 'students' | 'createdAt';
  direction: 'asc' | 'desc';
}

// Helper functions
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