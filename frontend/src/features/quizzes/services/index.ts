import type {
  Quiz,
  QuizListResponse,
  CourseQuizzesResponse,
  QuizQuestionsResponse,
  QuizAttemptsResponse,
  StartQuizAttemptResponse,
  CreateQuizRequest,
  UpdateQuizRequest,
  GetQuizzesRequest,
  AddQuestionToQuizRequest,
  StartQuizAttemptRequest,
} from '../types';
import ApiClient from '@/lib/api-client';

const API_BASE_URL = ApiClient.getUrl('quizzes');

interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

class QuizzesApiService {
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

  // Quiz CRUD Operations
  async getQuizzes(params?: GetQuizzesRequest): Promise<QuizListResponse> {
    const queryParams = new URLSearchParams();
    if (params?.courseId) queryParams.append('courseId', params.courseId.toString());
    if (params?.lessonId) queryParams.append('lessonId', params.lessonId.toString());
    if (params?.isActive !== undefined) queryParams.append('isActive', params.isActive.toString());
    if (params?.page) queryParams.append('page', params.page.toString());
    if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    
    const queryString = queryParams.toString();
    const endpoint = queryString ? `?${queryString}` : '';
    
    return this.request<QuizListResponse>(endpoint);
  }

  async getQuizById(id: number): Promise<Quiz> {
    return this.request<Quiz>(`/${id}`);
  }

  async createQuiz(quizData: CreateQuizRequest): Promise<Quiz> {
    return this.request<Quiz>('', {
      method: 'POST',
      body: JSON.stringify(quizData),
    });
  }

  async updateQuiz(id: number, quizData: UpdateQuizRequest): Promise<Quiz> {
    return this.request<Quiz>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(quizData),
    });
  }

  async deleteQuiz(id: number): Promise<void> {
    return this.request<void>(`/${id}`, {
      method: 'DELETE',
    });
  }

  // Quiz Organization
  async getCourseQuizzes(courseId: number): Promise<CourseQuizzesResponse> {
    return this.request<CourseQuizzesResponse>(`/courses/${courseId}/quizzes`);
  }

  async activateQuiz(id: number): Promise<Quiz> {
    return this.request<Quiz>(`/${id}/activate`, {
      method: 'PUT',
    });
  }

  async deactivateQuiz(id: number): Promise<Quiz> {
    return this.request<Quiz>(`/${id}/deactivate`, {
      method: 'PUT',
    });
  }

  // Question Management
  async getQuizQuestions(quizId: number): Promise<QuizQuestionsResponse> {
    return this.request<QuizQuestionsResponse>(`/${quizId}/questions`);
  }

  async addQuestionToQuiz(quizId: number, questionData: AddQuestionToQuizRequest): Promise<any> {
    return this.request<any>(`/${quizId}/questions`, {
      method: 'POST',
      body: JSON.stringify(questionData),
    });
  }

  // Quiz Attempts
  async startQuizAttempt(quizId: number, request: StartQuizAttemptRequest): Promise<StartQuizAttemptResponse> {
    return this.request<StartQuizAttemptResponse>(`/${quizId}/start`, {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }

  async getQuizAttempts(quizId: number, page: number = 1, pageSize: number = 10): Promise<QuizAttemptsResponse> {
    return this.request<QuizAttemptsResponse>(`/${quizId}/attempts?page=${page}&pageSize=${pageSize}`);
  }
}

// Export singleton instance
export const quizzesApi = new QuizzesApiService();

// Export class for testing
export { QuizzesApiService };

// Default export
export default quizzesApi;

