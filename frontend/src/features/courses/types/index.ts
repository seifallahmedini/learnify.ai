// Course-related types and interfaces based on the CoursesController.cs

// API Response wrapper
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors?: string[];
}

export interface Course {
  id: number;
  title: string;
  description: string;
  shortDescription: string;
  instructorId: number;
  instructorName: string;
  categoryId: number;
  categoryName: string;
  price: number;
  discountPrice?: number;
  durationHours: number;
  level: CourseLevel;
  language: string;
  thumbnailUrl?: string;
  videoPreviewUrl?: string;
  isPublished: boolean;
  isFeatured: boolean;
  maxStudents?: number;
  prerequisites?: string;
  learningObjectives: string;
  createdAt: string;
  updatedAt: string;
  effectivePrice: number;
  isDiscounted: boolean;
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
  instructorName: string;
  categoryName: string;
  price: number;
  discountPrice?: number;
  level: CourseLevel;
  thumbnailUrl?: string;
  isPublished: boolean;
  isFeatured: boolean;
  totalStudents: number;
  averageRating: number;
  createdAt: string;
  effectivePrice: number;
  isDiscounted: boolean;
}

export interface CourseListResponse {
  courses: CourseSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CourseDetailResponse {
  course: Course;
  lessons: LessonSummary[];
  statistics: CourseStatistics;
}

export interface LessonSummary {
  id: number;
  title: string;
  description: string;
  duration: number;
  orderIndex: number;
  isFree: boolean;
  isPublished: boolean;
}

export interface CourseStatistics {
  totalStudents: number;
  totalLessons: number;
  totalDuration: number;
  averageRating: number;
  reviewCount: number;
}

// Request Types
export interface CreateCourseRequest {
  title: string;
  description: string;
  shortDescription: string;
  instructorId: number;
  categoryId: number;
  price: number;
  discountPrice?: number;
  durationHours: number;
  level: CourseLevel;
  language: string;
  thumbnailUrl?: string;
  videoPreviewUrl?: string;
  isPublished: boolean;
  isFeatured: boolean;
  maxStudents?: number;
  prerequisites?: string;
  learningObjectives: string;
}

export interface UpdateCourseRequest {
  title?: string;
  description?: string;
  shortDescription?: string;
  categoryId?: number;
  price?: number;
  discountPrice?: number;
  durationHours?: number;
  level?: CourseLevel;
  language?: string;
  thumbnailUrl?: string;
  videoPreviewUrl?: string;
  isPublished?: boolean;
  isFeatured?: boolean;
  maxStudents?: number;
  prerequisites?: string;
  learningObjectives?: string;
}

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
  instructorId: number;
  categoryId: number;
  price: string; // String for form input
  discountPrice?: string;
  durationHours: string;
  level: CourseLevel;
  language: string;
  thumbnailUrl?: string;
  videoPreviewUrl?: string;
  isPublished: boolean;
  isFeatured: boolean;
  maxStudents?: string;
  prerequisites?: string;
  learningObjectives: string;
}

export interface UpdateCourseFormData extends Partial<CreateCourseFormData> {}

// Utility Types
export interface CourseFilters {
  categoryId?: number;
  instructorId?: number;
  level?: CourseLevel;
  isPublished?: boolean;
  isFeatured?: boolean;
  minPrice?: number;
  maxPrice?: number;
  searchTerm?: string;
}

export interface CourseSortOptions {
  field: 'title' | 'createdAt' | 'price' | 'rating' | 'totalStudents';
  direction: 'asc' | 'desc';
}

// Helper functions
export const getCourseLevelLabel = (level: CourseLevel): string => {
  switch (level) {
    case CourseLevel.Beginner:
      return 'Beginner';
    case CourseLevel.Intermediate:
      return 'Intermediate';
    case CourseLevel.Advanced:
      return 'Advanced';
    case CourseLevel.Expert:
      return 'Expert';
    default:
      return 'Unknown';
  }
};

export const getCourseLevelColor = (level: CourseLevel): string => {
  switch (level) {
    case CourseLevel.Beginner:
      return 'bg-green-100 text-green-700';
    case CourseLevel.Intermediate:
      return 'bg-blue-100 text-blue-700';
    case CourseLevel.Advanced:
      return 'bg-orange-100 text-orange-700';
    case CourseLevel.Expert:
      return 'bg-red-100 text-red-700';
    default:
      return 'bg-gray-100 text-gray-700';
  }
};

export const formatCoursePrice = (price: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(price);
};

export const formatCourseDuration = (hours: number): string => {
  if (hours < 1) {
    return `${Math.round(hours * 60)} minutes`;
  }
  
  const wholehours = Math.floor(hours);
  const minutes = Math.round((hours - wholehours) * 60);
  
  if (minutes === 0) {
    return `${wholehours} ${wholehours === 1 ? 'hour' : 'hours'}`;
  }
  
  return `${wholehours}h ${minutes}m`;
};
