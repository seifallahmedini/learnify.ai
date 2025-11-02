import type { QuestionAnswersResponse } from '../types';
import ApiClient from '@/lib/api-client';

const API_BASE_URL = ApiClient.getUrl('answers');

interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface CreateAnswerRequest {
  questionId: number;
  answerText: string;
  isCorrect?: boolean;
  orderIndex?: number;
}

export interface UpdateAnswerRequest {
  answerText?: string;
  isCorrect?: boolean;
  orderIndex?: number;
}

export interface AnswerResponse {
  id: number;
  questionId: number;
  questionText: string;
  answerText: string;
  isCorrect: boolean;
  orderIndex: number;
  createdAt: string;
  updatedAt: string;
}

class AnswersApiService {
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
      const errorText = await response.text();
      throw new Error(`API request failed: ${response.statusText} - ${errorText}`);
    }

    const result: ApiResponse<T> = await response.json();
    return result.data;
  }

  /**
   * Get all answers for a specific question
   */
  async getQuestionAnswers(questionId: number): Promise<QuestionAnswersResponse> {
    return this.request<QuestionAnswersResponse>(`/question/${questionId}`);
  }

  /**
   * Create a new answer
   */
  async createAnswer(data: CreateAnswerRequest): Promise<AnswerResponse> {
    return this.request<AnswerResponse>('', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  /**
   * Update an existing answer
   */
  async updateAnswer(answerId: number, data: UpdateAnswerRequest): Promise<AnswerResponse> {
    return this.request<AnswerResponse>(`/${answerId}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  /**
   * Delete an answer
   */
  async deleteAnswer(answerId: number): Promise<void> {
    return this.request<void>(`/${answerId}`, {
      method: 'DELETE',
    });
  }
}

// Export singleton instance
export const answersApi = new AnswersApiService();

// Export class for testing
export { AnswersApiService };

// Default export
export default answersApi;

