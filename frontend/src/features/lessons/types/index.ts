// Lesson-related types and interfaces

// API Response wrapper
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface Lesson {
  id: number;
  courseId: number;
  courseTitle: string;
  title: string;
  description: string;
  content: string;
  videoUrl?: string;
  duration: number; // Duration in minutes
  orderIndex: number;
  isFree: boolean;
  isPublished: boolean;
  learningObjectives?: string; // Comma or newline separated list
  resources?: string; // JSON string of resource objects
  formattedDuration: string;
  createdAt: string;
  updatedAt: string;
}

export interface LessonSummary {
  id: number;
  courseId: number;
  title: string;
  description: string;
  duration: number;
  orderIndex: number;
  isFree: boolean;
  isPublished: boolean;
  formattedDuration: string;
  createdAt: string;
}

export interface LessonListResponse {
  lessons: LessonSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface LessonNavigationResponse {
  previousLesson?: Lesson;
  currentLesson: Lesson;
  nextLesson?: Lesson;
  totalLessons: number;
  currentPosition: number;
}

export interface LessonResourcesResponse {
  lessonId: number;
  resources: LessonResource[];
}

export interface LessonResource {
  id?: number;
  name: string;
  url: string;
  type: 'download' | 'external' | 'code';
  size?: number;
  uploadedAt?: string;
}

// Request Types
export interface CreateLessonRequest {
  title: string;
  description: string;
  content: string;
  videoUrl?: string;
  duration: number;
  isFree?: boolean;
  isPublished?: boolean;
  learningObjectives?: string;
  resources?: string;
}

export interface UpdateLessonRequest {
  title?: string;
  description?: string;
  content?: string;
  videoUrl?: string;
  duration?: number;
  orderIndex?: number;
  isFree?: boolean;
  isPublished?: boolean;
  learningObjectives?: string;
  resources?: string;
}

export interface ReorderLessonRequest {
  newOrderIndex: number;
}

export interface UploadVideoRequest {
  videoUrl: string;
}

export interface UpdateContentRequest {
  content: string;
}

// Form Types
export interface CreateLessonFormData {
  title: string;
  description: string;
  content: string;
  videoUrl: string;
  duration: string;
  isFree: boolean;
  isPublished: boolean;
}

export interface UpdateLessonFormData extends Partial<CreateLessonFormData> {
  orderIndex?: string;
}

// Utility Types
export interface LessonFilters {
  courseId?: number;
  isPublished?: boolean;
  isFree?: boolean;
  searchTerm?: string;
}

export interface LessonSortOptions {
  field: 'title' | 'orderIndex' | 'duration' | 'createdAt';
  direction: 'asc' | 'desc';
}

