import { useState, useEffect, useCallback } from 'react';
import type {
  Lesson,
  LessonSummary,
  CreateLessonRequest,
  UpdateLessonRequest,
} from '../types';
import { lessonsApi, courseLessonsApi } from '../services';

// Hook for managing lesson list within a course
export const useLessonManagement = (courseId: number | null) => {
  const [lessons, setLessons] = useState<LessonSummary[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showPublishedOnly, setShowPublishedOnly] = useState(false);

  const loadLessons = useCallback(async (isPublished?: boolean) => {
    if (!courseId) {
      setLessons([]);
      return;
    }

    try {
      setIsLoading(true);
      setError(null);
      const lessonsData = await courseLessonsApi.getCourseLessons(courseId, isPublished);
      setLessons(lessonsData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load lessons');
      setLessons([]);
    } finally {
      setIsLoading(false);
    }
  }, [courseId]);

  const refreshLessons = useCallback(() => {
    loadLessons(showPublishedOnly ? true : undefined);
  }, [loadLessons, showPublishedOnly]);

  const togglePublishedFilter = useCallback(() => {
    const newValue = !showPublishedOnly;
    setShowPublishedOnly(newValue);
    loadLessons(newValue ? true : undefined);
  }, [showPublishedOnly, loadLessons]);

  useEffect(() => {
    if (courseId) {
      loadLessons(showPublishedOnly ? true : undefined);
    } else {
      setLessons([]);
    }
  }, [courseId, loadLessons, showPublishedOnly]);

  return {
    lessons,
    isLoading,
    error,
    showPublishedOnly,
    refreshLessons,
    togglePublishedFilter,
  };
};

// Hook for managing individual lesson operations
export const useLessonOperations = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateLesson = useCallback(async (id: number, lessonData: UpdateLessonRequest): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.updateLesson(id, lessonData);
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update lesson');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const deleteLesson = useCallback(async (id: number): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);
      await lessonsApi.deleteLesson(id);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete lesson');
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const publishLesson = useCallback(async (id: number): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.publishLesson(id);
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to publish lesson');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const unpublishLesson = useCallback(async (id: number): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.unpublishLesson(id);
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to unpublish lesson');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const makeLessonFree = useCallback(async (id: number, isFree: boolean = true): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.makeLessonFree(id, isFree);
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update lesson access');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const reorderLesson = useCallback(async (id: number, newOrderIndex: number): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.reorderLesson(id, { newOrderIndex });
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to reorder lesson');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateLessonContent = useCallback(async (id: number, content: string): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.updateLessonContent(id, { content });
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update lesson content');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const uploadLessonVideo = useCallback(async (id: number, videoUrl: string): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await lessonsApi.uploadLessonVideo(id, { videoUrl });
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to upload lesson video');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    isLoading,
    error,
    updateLesson,
    deleteLesson,
    publishLesson,
    unpublishLesson,
    makeLessonFree,
    reorderLesson,
    updateLessonContent,
    uploadLessonVideo,
  };
};

// Hook for loading individual lesson details
export const useLessonDetails = (lessonId: number | null) => {
  const [lesson, setLesson] = useState<Lesson | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadLesson = useCallback(async (id: number) => {
    try {
      setIsLoading(true);
      setError(null);
      const lessonData = await lessonsApi.getLessonById(id);
      setLesson(lessonData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load lesson');
      setLesson(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (lessonId) {
      loadLesson(lessonId);
    } else {
      setLesson(null);
    }
  }, [lessonId, loadLesson]);

  return {
    lesson,
    isLoading,
    error,
    loadLesson,
  };
};

// Hook for lesson navigation
export const useLessonNavigation = (lessonId: number | null) => {
  const [previousLesson, setPreviousLesson] = useState<Lesson | null>(null);
  const [nextLesson, setNextLesson] = useState<Lesson | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadNavigation = useCallback(async (id: number) => {
    try {
      setIsLoading(true);
      setError(null);
      const [prev, next] = await Promise.all([
        lessonsApi.getPreviousLesson(id),
        lessonsApi.getNextLesson(id),
      ]);
      setPreviousLesson(prev);
      setNextLesson(next);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load lesson navigation');
      setPreviousLesson(null);
      setNextLesson(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (lessonId) {
      loadNavigation(lessonId);
    } else {
      setPreviousLesson(null);
      setNextLesson(null);
    }
  }, [lessonId, loadNavigation]);

  return {
    previousLesson,
    nextLesson,
    isLoading,
    error,
    loadNavigation,
  };
};

// Hook for creating lessons
export const useCreateLesson = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createLesson = useCallback(async (
    courseId: number,
    lessonData: CreateLessonRequest
  ): Promise<Lesson | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const lesson = await courseLessonsApi.createCourseLesson(courseId, lessonData);
      return lesson;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create lesson');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    isLoading,
    error,
    createLesson,
  };
};

