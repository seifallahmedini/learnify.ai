import { useState, useCallback } from 'react';
import { coursesApi } from '../services';
import type { Course, UpdateCourseRequest } from '../types';

export const useBulkOperations = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const bulkUpdate = useCallback(async (courseIds: number[], updates: UpdateCourseRequest): Promise<Course[]> => {
    try {
      setIsLoading(true);
      setError(null);
      const updatedCourses = await coursesApi.bulkUpdateCourses(courseIds, updates);
      return updatedCourses;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to update courses';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const bulkDelete = useCallback(async (courseIds: number[]): Promise<void> => {
    try {
      setIsLoading(true);
      setError(null);
      await coursesApi.bulkDeleteCourses(courseIds);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete courses';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const bulkPublish = useCallback(async (courseIds: number[]): Promise<Course[]> => {
    try {
      setIsLoading(true);
      setError(null);
      const publishedCourses = await coursesApi.bulkPublishCourses(courseIds);
      return publishedCourses;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to publish courses';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const bulkUnpublish = useCallback(async (courseIds: number[]): Promise<Course[]> => {
    try {
      setIsLoading(true);
      setError(null);
      const unpublishedCourses = await coursesApi.bulkUnpublishCourses(courseIds);
      return unpublishedCourses;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to unpublish courses';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const exportCourses = useCallback(async (filters?: any): Promise<Blob> => {
    try {
      setIsLoading(true);
      setError(null);
      const blob = await coursesApi.exportCourses(filters);
      return blob;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to export courses';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    isLoading,
    error,
    bulkUpdate,
    bulkDelete,
    bulkPublish,
    bulkUnpublish,
    exportCourses,
    clearError,
  };
};