import type {
  Lesson,
  LessonSummary,
  CreateLessonRequest,
  UpdateLessonRequest,
  ReorderLessonRequest,
  UploadVideoRequest,
  UpdateContentRequest,
  LessonResourcesResponse,
} from '../types';
import ApiClient from '@/lib/api-client';

const API_BASE_URL = ApiClient.getUrl('lessons');

interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

class LessonsApiService {
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

  // Lesson CRUD Operations
  async getLessonById(id: number): Promise<Lesson> {
    return this.request<Lesson>(`/${id}`);
  }

  async updateLesson(id: number, lessonData: UpdateLessonRequest): Promise<Lesson> {
    return this.request<Lesson>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(lessonData),
    });
  }

  async deleteLesson(id: number): Promise<void> {
    return this.request<void>(`/${id}`, {
      method: 'DELETE',
    });
  }

  // Lesson Organization
  async reorderLesson(id: number, request: ReorderLessonRequest): Promise<Lesson> {
    return this.request<Lesson>(`/${id}/reorder`, {
      method: 'PUT',
      body: JSON.stringify(request),
    });
  }

  async getNextLesson(id: number): Promise<Lesson | null> {
    try {
      return await this.request<Lesson>(`/${id}/next`);
    } catch {
      return null;
    }
  }

  async getPreviousLesson(id: number): Promise<Lesson | null> {
    try {
      return await this.request<Lesson>(`/${id}/previous`);
    } catch {
      return null;
    }
  }

  // Lesson Content
  async uploadLessonVideo(id: number, request: UploadVideoRequest): Promise<Lesson> {
    return this.request<Lesson>(`/${id}/video`, {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }

  async updateLessonContent(id: number, request: UpdateContentRequest): Promise<Lesson> {
    return this.request<Lesson>(`/${id}/content`, {
      method: 'PUT',
      body: JSON.stringify(request),
    });
  }

  async getLessonResources(id: number): Promise<LessonResourcesResponse> {
    return this.request<LessonResourcesResponse>(`/${id}/resources`);
  }

  // Lesson Access Control
  async publishLesson(id: number): Promise<Lesson> {
    return this.request<Lesson>(`/${id}/publish`, {
      method: 'PUT',
    });
  }

  async unpublishLesson(id: number): Promise<Lesson> {
    return this.request<Lesson>(`/${id}/unpublish`, {
      method: 'PUT',
    });
  }

  async makeLessonFree(id: number, isFree: boolean = true): Promise<Lesson> {
    return this.request<Lesson>(`/${id}/free?isFree=${isFree}`, {
      method: 'PUT',
    });
  }
}

// Course Lessons API (Lessons are created through the Courses API)
class CourseLessonsApiService {
  private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const url = `${ApiClient.getUrl('courses')}${endpoint}`;
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

  async getCourseLessons(courseId: number, isPublished?: boolean): Promise<LessonSummary[]> {
    const params = new URLSearchParams();
    if (isPublished !== undefined) {
      params.append('isPublished', isPublished.toString());
    }
    const queryString = params.toString();
    const endpoint = `/${courseId}/lessons${queryString ? `?${queryString}` : ''}`;
    
    const response = await this.request<{ lessons: LessonSummary[] }>(endpoint);
    return response.lessons || [];
  }

  async createCourseLesson(courseId: number, lessonData: CreateLessonRequest): Promise<Lesson> {
    return this.request<Lesson>(`/${courseId}/lessons`, {
      method: 'POST',
      body: JSON.stringify(lessonData),
    });
  }
}

// Export singleton instances
export const lessonsApi = new LessonsApiService();
export const courseLessonsApi = new CourseLessonsApiService();

// Export classes for testing
export { LessonsApiService, CourseLessonsApiService };

// Default export
export default lessonsApi;

