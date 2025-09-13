import { useState, useEffect, useCallback } from 'react';
import type {
  Course,
  CourseSummary,
  CourseListResponse,
  CourseFilterRequest,
  CreateCourseRequest,
  UpdateCourseRequest,
  CourseDetailResponse,
} from '../types';
import { coursesApi } from '../services';

// Hook for managing course list with filtering and pagination
export const useCourseManagement = (initialFilters?: CourseFilterRequest) => {
  const [courses, setCourses] = useState<CourseSummary[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [filters, setFilters] = useState<CourseFilterRequest>(initialFilters || {});

  const loadCourses = useCallback(async (newFilters?: CourseFilterRequest) => {
    try {
      setIsLoading(true);
      setError(null);
      
      const currentFilters = newFilters || filters;
      const response: CourseListResponse = await coursesApi.getCourses(currentFilters);
      
      setCourses(response.courses);
      setTotalCount(response.totalCount);
      setCurrentPage(response.page);
      setTotalPages(response.totalPages);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load courses');
      setCourses([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateFilters = useCallback((newFilters: CourseFilterRequest) => {
    setFilters(newFilters);
    loadCourses(newFilters);
  }, [loadCourses]);

  const goToPage = useCallback((page: number) => {
    const newFilters = { ...filters, page };
    setFilters(newFilters);
    loadCourses(newFilters);
  }, [loadCourses, filters]);

  const refreshCourses = useCallback(() => {
    loadCourses();
  }, [loadCourses]);

  useEffect(() => {
    loadCourses();
  }, []);

  return {
    courses,
    isLoading,
    error,
    totalCount,
    currentPage,
    totalPages,
    filters,
    updateFilters,
    goToPage,
    refreshCourses,
  };
};

// Hook for managing individual course operations
export const useCourseOperations = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createCourse = useCallback(async (courseData: CreateCourseRequest): Promise<Course | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const course = await coursesApi.createCourse(courseData);
      return course;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create course');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateCourse = useCallback(async (id: number, courseData: UpdateCourseRequest): Promise<Course | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const course = await coursesApi.updateCourse(id, courseData);
      return course;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update course');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const deleteCourse = useCallback(async (id: number): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);
      await coursesApi.deleteCourse(id);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete course');
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const publishCourse = useCallback(async (id: number): Promise<Course | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const course = await coursesApi.publishCourse(id);
      return course;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to publish course');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const unpublishCourse = useCallback(async (id: number): Promise<Course | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const course = await coursesApi.unpublishCourse(id);
      return course;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to unpublish course');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const featureCourse = useCallback(async (id: number): Promise<Course | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const course = await coursesApi.featureCourse(id);
      return course;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to feature course');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const unfeatureCourse = useCallback(async (id: number): Promise<Course | null> => {
    try {
      setIsLoading(true);
      setError(null);
      const course = await coursesApi.unfeatureCourse(id);
      return course;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to unfeature course');
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    isLoading,
    error,
    createCourse,
    updateCourse,
    deleteCourse,
    publishCourse,
    unpublishCourse,
    featureCourse,
    unfeatureCourse,
  };
};

// Hook for loading individual course details
export const useCourseDetails = (courseId: number | null) => {
  const [course, setCourse] = useState<Course | null>(null);
  const [courseDetails, setCourseDetails] = useState<CourseDetailResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadCourse = useCallback(async (id: number) => {
    try {
      setIsLoading(true);
      setError(null);
      const courseData = await coursesApi.getCourseById(id);
      setCourse(courseData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load course');
      setCourse(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const loadCourseDetails = useCallback(async (id: number) => {
    try {
      setIsLoading(true);
      setError(null);
      const details = await coursesApi.getCourseDetails(id);
      setCourseDetails(details);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load course details');
      setCourseDetails(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (courseId) {
      loadCourse(courseId);
    } else {
      setCourse(null);
    }
  }, [courseId, loadCourse]);

  return {
    course,
    courseDetails,
    isLoading,
    error,
    loadCourse,
    loadCourseDetails,
  };
};

// Hook for bulk operations
export const useBulkCourseOperations = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const bulkUpdateCourses = useCallback(async (courseIds: number[], updates: UpdateCourseRequest): Promise<Course[]> => {
    try {
      setIsLoading(true);
      setError(null);
      const courses = await coursesApi.bulkUpdateCourses(courseIds, updates);
      return courses;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update courses');
      return [];
    } finally {
      setIsLoading(false);
    }
  }, []);

  const bulkDeleteCourses = useCallback(async (courseIds: number[]): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);
      await coursesApi.bulkDeleteCourses(courseIds);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete courses');
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const bulkPublishCourses = useCallback(async (courseIds: number[]): Promise<Course[]> => {
    try {
      setIsLoading(true);
      setError(null);
      const courses = await coursesApi.bulkPublishCourses(courseIds);
      return courses;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to publish courses');
      return [];
    } finally {
      setIsLoading(false);
    }
  }, []);

  const bulkUnpublishCourses = useCallback(async (courseIds: number[]): Promise<Course[]> => {
    try {
      setIsLoading(true);
      setError(null);
      const courses = await coursesApi.bulkUnpublishCourses(courseIds);
      return courses;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to unpublish courses');
      return [];
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    isLoading,
    error,
    bulkUpdateCourses,
    bulkDeleteCourses,
    bulkPublishCourses,
    bulkUnpublishCourses,
  };
};