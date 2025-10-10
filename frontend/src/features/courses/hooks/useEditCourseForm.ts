import { useState, useCallback, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { CourseLevel, type Course, type UpdateCourseRequest } from '../types';

const editCourseSchema = z.object({
  title: z.string().min(1, 'Title is required').max(200, 'Title too long'),
  description: z.string().min(1, 'Description is required'),
  shortDescription: z.string().min(1, 'Short description is required').max(500, 'Short description too long'),
  price: z.string().refine((val) => !isNaN(parseFloat(val)) && parseFloat(val) >= 0, {
    message: 'Price must be a valid non-negative number'
  }),
  discountPrice: z.string().optional().refine((val) => !val || (!isNaN(parseFloat(val)) && parseFloat(val) >= 0), {
    message: 'Discount price must be a valid non-negative number'
  }),
  durationHours: z.string().refine((val) => !isNaN(parseFloat(val)) && parseFloat(val) >= 0.1, {
    message: 'Duration must be at least 0.1 hours'
  }),
  level: z.number().min(1).max(4),
  maxStudents: z.string().optional().refine((val) => !val || (!isNaN(parseInt(val)) && parseInt(val) > 0), {
    message: 'Max students must be a positive number'
  }),
  prerequisites: z.string().optional(),
  learningObjectives: z.string().min(1, 'Learning objectives are required'),
  isPublished: z.boolean(),
  isFeatured: z.boolean(),
});

type EditCourseFormData = z.infer<typeof editCourseSchema>;

export const useEditCourseForm = (course: Course, onSuccess?: (updatedCourse: Course) => void) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const form = useForm<EditCourseFormData>({
    resolver: zodResolver(editCourseSchema),
    defaultValues: {
      title: course.title || '',
      description: course.description || '',
      shortDescription: course.shortDescription || '',
      price: course.price?.toString() || '0',
      discountPrice: course.discountPrice?.toString() || '',
      durationHours: course.durationHours?.toString() || '0',
      level: course.level || 1,
      maxStudents: course.maxStudents?.toString() || '',
      prerequisites: course.prerequisites || '',
      learningObjectives: course.learningObjectives || '',
      isPublished: course.isPublished || false,
      isFeatured: course.isFeatured || false,
    },
  });

  // Update form when course changes
  useEffect(() => {
    if (course && course.id !== 0) { // Only update if we have a real course (not the default)
      form.reset({
        title: course.title || '',
        description: course.description || '',
        shortDescription: course.shortDescription || '',
        price: course.price?.toString() || '0',
        discountPrice: course.discountPrice?.toString() || '',
        durationHours: course.durationHours?.toString() || '0',
        level: course.level || 1,
        maxStudents: course.maxStudents?.toString() || '',
        prerequisites: course.prerequisites || '',
        learningObjectives: course.learningObjectives || '',
        isPublished: course.isPublished || false,
        isFeatured: course.isFeatured || false,
      });
    }
  }, [course.id, course.title, course.description, course.shortDescription, course.price, course.discountPrice, course.durationHours, course.level, course.maxStudents, course.prerequisites, course.learningObjectives, course.isPublished, course.isFeatured, form]);

  const submitForm = useCallback(async (data: EditCourseFormData) => {
    try {
      setIsSubmitting(true);
      setError(null);
      
      const updateData: UpdateCourseRequest = {
        title: data.title,
        description: data.description,
        shortDescription: data.shortDescription,
        price: parseFloat(data.price),
        discountPrice: data.discountPrice ? parseFloat(data.discountPrice) : undefined,
        durationHours: parseFloat(data.durationHours),
        level: data.level as CourseLevel,
        maxStudents: data.maxStudents ? parseInt(data.maxStudents) : undefined,
        prerequisites: data.prerequisites || undefined,
        learningObjectives: data.learningObjectives,
        isPublished: data.isPublished,
        isFeatured: data.isFeatured,
      };

      // TODO: Implement the actual API call
      // const updatedCourse = await coursesApi.updateCourse(course.id, updateData);
      
      // Mock successful update for now
      const updatedCourse: Course = { ...course, ...updateData };
      
      if (onSuccess) {
        onSuccess(updatedCourse);
      }
      
      return updatedCourse;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to update course';
      setError(errorMessage);
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  }, [course, onSuccess]);

  const resetForm = useCallback(() => {
    form.reset();
    setError(null);
  }, [form]);

  return {
    form,
    isSubmitting,
    error,
    submitForm,
    resetForm,
  };
};