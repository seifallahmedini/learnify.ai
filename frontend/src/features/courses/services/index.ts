import type {
    CourseListResponse,
    Course,
    CourseFilterRequest,
    CreateCourseRequest,
    UpdateCourseRequest,
    CourseDetailResponse,
    CourseSummary,
    CourseLevel,
} from '../types';
import ApiClient from '@/lib/api-client';

const API_BASE_URL = ApiClient.getUrl('courses'); // Using a simple URL for now

interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

class CoursesApiService {
  private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      throw new Error(`API request failed: ${response.statusText}`);
    }

    const result: ApiResponse<T> = await response.json();
    return result.data;
  }

  // Course CRUD Operations
  async getCourses(filters?: CourseFilterRequest): Promise<CourseListResponse> {
    const params = new URLSearchParams();
    
    if (filters?.categoryId) params.append('categoryId', filters.categoryId.toString());
    if (filters?.instructorId) params.append('instructorId', filters.instructorId.toString());
    if (filters?.level) params.append('level', filters.level.toString());
    if (filters?.isPublished !== undefined) params.append('isPublished', filters.isPublished.toString());
    if (filters?.isFeatured !== undefined) params.append('isFeatured', filters.isFeatured.toString());
    if (filters?.minPrice) params.append('minPrice', filters.minPrice.toString());
    if (filters?.maxPrice) params.append('maxPrice', filters.maxPrice.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = queryString ? `?${queryString}` : '';
    
    return this.request<CourseListResponse>(endpoint);
  }

  async getCourseById(id: number): Promise<Course> {
    return this.request<Course>(`/${id}`);
  }

  async getCourseDetails(id: number): Promise<CourseDetailResponse> {
    return this.request<CourseDetailResponse>(`/${id}/details`);
  }

  async createCourse(courseData: CreateCourseRequest): Promise<Course> {
    return this.request<Course>('', {
      method: 'POST',
      body: JSON.stringify(courseData),
    });
  }

  async updateCourse(id: number, courseData: UpdateCourseRequest): Promise<Course> {
    return this.request<Course>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(courseData),
    });
  }

  async deleteCourse(id: number): Promise<void> {
    return this.request<void>(`/${id}`, {
      method: 'DELETE',
    });
  }

  // Course Status Operations
  async publishCourse(id: number): Promise<Course> {
    return this.request<Course>(`/${id}/publish`, {
      method: 'POST',
    });
  }

  async unpublishCourse(id: number): Promise<Course> {
    return this.request<Course>(`/${id}/unpublish`, {
      method: 'POST',
    });
  }

  async featureCourse(id: number): Promise<Course> {
    return this.request<Course>(`/${id}/feature`, {
      method: 'POST',
    });
  }

  async unfeatureCourse(id: number): Promise<Course> {
    return this.request<Course>(`/${id}/unfeature`, {
      method: 'POST',
    });
  }

  // Course Analytics and Statistics
  async getCourseAnalytics(id: number): Promise<any> {
    return this.request<any>(`/${id}/analytics`);
  }

  async getCourseCompletionRate(id: number): Promise<any> {
    return this.request<any>(`/${id}/completion-rate`);
  }

  async getCourseStudents(id: number): Promise<any> {
    return this.request<any>(`/${id}/students`);
  }

  async getCourseLessons(id: number): Promise<any> {
    return this.request<any>(`/${id}/lessons`);
  }

  // Featured and Popular Courses
  async getFeaturedCourses(): Promise<CourseSummary[]> {
    return this.request<CourseSummary[]>('/featured');
  }

  async getPopularCourses(): Promise<CourseSummary[]> {
    return this.request<CourseSummary[]>('/popular');
  }

  async getRecentCourses(): Promise<CourseSummary[]> {
    return this.request<CourseSummary[]>('/recent');
  }

  // Bulk Operations
  async bulkUpdateCourses(courseIds: number[], updates: UpdateCourseRequest): Promise<Course[]> {
    return this.request<Course[]>('/bulk-update', {
      method: 'POST',
      body: JSON.stringify({ courseIds, updates }),
    });
  }

  async bulkDeleteCourses(courseIds: number[]): Promise<void> {
    return this.request<void>('/bulk-delete', {
      method: 'POST',
      body: JSON.stringify({ courseIds }),
    });
  }

  async bulkPublishCourses(courseIds: number[]): Promise<Course[]> {
    return this.request<Course[]>('/bulk-publish', {
      method: 'POST',
      body: JSON.stringify({ courseIds }),
    });
  }

  async bulkUnpublishCourses(courseIds: number[]): Promise<Course[]> {
    return this.request<Course[]>('/bulk-unpublish', {
      method: 'POST',
      body: JSON.stringify({ courseIds }),
    });
  }

  // Export/Import Operations
  async exportCourses(filters?: CourseFilterRequest): Promise<Blob> {
    const params = new URLSearchParams();
    if (filters?.categoryId) params.append('categoryId', filters.categoryId.toString());
    if (filters?.instructorId) params.append('instructorId', filters.instructorId.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);

    const queryString = params.toString();
    const endpoint = `/export${queryString ? `?${queryString}` : ''}`;
    
    const response = await fetch(`${API_BASE_URL}${endpoint}`);
    return response.blob();
  }

  // Search and Filter Helpers
  async searchCourses(searchTerm: string, filters?: Omit<CourseFilterRequest, 'searchTerm'>): Promise<CourseListResponse> {
    return this.getCourses({ ...filters, searchTerm });
  }

  async getCoursesByCategory(categoryId: number, filters?: Omit<CourseFilterRequest, 'categoryId'>): Promise<CourseListResponse> {
    return this.getCourses({ ...filters, categoryId });
  }

  async getCoursesByInstructor(instructorId: number, filters?: Omit<CourseFilterRequest, 'instructorId'>): Promise<CourseListResponse> {
    return this.getCourses({ ...filters, instructorId });
  }

  async getCoursesByLevel(level: CourseLevel, filters?: Omit<CourseFilterRequest, 'level'>): Promise<CourseListResponse> {
    return this.getCourses({ ...filters, level });
  }
}

// Export singleton instance
export const coursesApi = new CoursesApiService();

// Export class for testing
export { CoursesApiService };

// Default export
export default coursesApi;