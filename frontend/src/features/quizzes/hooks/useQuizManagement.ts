import { useState, useEffect, useCallback } from 'react';
import { quizzesApi } from '../services';
import type { Quiz, QuizSummary, QuizListResponse, CreateQuizRequest, UpdateQuizRequest } from '../types';

interface UseQuizManagementOptions {
  courseId?: number | null;
  lessonId?: number | null;
  isActive?: boolean;
}

export const useQuizManagement = (options: UseQuizManagementOptions = {}) => {
  const [quizzes, setQuizzes] = useState<QuizSummary[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
  });
  const [showActiveOnly, setShowActiveOnly] = useState(false);

  const loadQuizzes = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const params = {
        courseId: options.courseId || undefined,
        lessonId: options.lessonId || undefined,
        isActive: showActiveOnly || options.isActive || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
      };
      
      const response = await quizzesApi.getQuizzes(params);
      setQuizzes(response.quizzes);
      setPagination({
        page: response.page,
        pageSize: response.pageSize,
        totalCount: response.totalCount,
        totalPages: response.totalPages,
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load quizzes');
      setQuizzes([]);
    } finally {
      setIsLoading(false);
    }
  }, [options.courseId, options.lessonId, options.isActive, showActiveOnly, pagination.page, pagination.pageSize]);

  useEffect(() => {
    loadQuizzes();
  }, [loadQuizzes]);

  const refreshQuizzes = useCallback(() => {
    loadQuizzes();
  }, [loadQuizzes]);

  const toggleActiveFilter = useCallback(() => {
    setShowActiveOnly(prev => !prev);
  }, []);

  return {
    quizzes,
    isLoading,
    error,
    pagination,
    showActiveOnly,
    refreshQuizzes,
    toggleActiveFilter,
    setPage: (page: number) => setPagination(prev => ({ ...prev, page })),
  };
};

export const useQuizDetails = (quizId: number | null) => {
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadQuiz = useCallback(async () => {
    if (!quizId) return;
    
    setIsLoading(true);
    setError(null);
    try {
      const data = await quizzesApi.getQuizById(quizId);
      setQuiz(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load quiz');
      setQuiz(null);
    } finally {
      setIsLoading(false);
    }
  }, [quizId]);

  useEffect(() => {
    loadQuiz();
  }, [loadQuiz]);

  return {
    quiz,
    isLoading,
    error,
    loadQuiz,
  };
};

export const useQuizOperations = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createQuiz = useCallback(async (quizData: CreateQuizRequest): Promise<Quiz | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const quiz = await quizzesApi.createQuiz(quizData);
      return quiz;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create quiz');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateQuiz = useCallback(async (id: number, quizData: UpdateQuizRequest): Promise<Quiz | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const quiz = await quizzesApi.updateQuiz(id, quizData);
      return quiz;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update quiz');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const deleteQuiz = useCallback(async (id: number): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);
      await quizzesApi.deleteQuiz(id);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete quiz');
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const activateQuiz = useCallback(async (id: number): Promise<Quiz | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const quiz = await quizzesApi.activateQuiz(id);
      return quiz;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to activate quiz');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const deactivateQuiz = useCallback(async (id: number): Promise<Quiz | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const quiz = await quizzesApi.deactivateQuiz(id);
      return quiz;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to deactivate quiz');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    isLoading,
    error,
    createQuiz,
    updateQuiz,
    deleteQuiz,
    activateQuiz,
    deactivateQuiz,
  };
};

