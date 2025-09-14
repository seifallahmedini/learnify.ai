import { useState } from 'react';
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import type { CreateCourseRequest } from '../types';
import { CourseLevel } from '../types';
import { coursesApi } from '../services';

// Validation schema for course creation
const createCourseSchema = z.object({
  title: z.string()
    .min(3, 'Title must be at least 3 characters')
    .max(200, 'Title must be less than 200 characters'),
  shortDescription: z.string()
    .min(10, 'Short description must be at least 10 characters')
    .max(500, 'Short description must be less than 500 characters'),
  description: z.string()
    .min(50, 'Description must be at least 50 characters')
    .max(5000, 'Description must be less than 5000 characters'),
  learningObjectives: z.string()
    .min(20, 'Learning objectives must be at least 20 characters')
    .max(2000, 'Learning objectives must be less than 2000 characters'),
  categoryId: z.number().min(1, 'Please select a category'),
  instructorId: z.number().min(1, 'Please select an instructor'),
  level: z.nativeEnum(CourseLevel),
  price: z.number().min(0, 'Price must be 0 or greater'),
  discountPrice: z.number().min(0, 'Discount price must be 0 or greater').optional(),
  durationHours: z.number().min(0.5, 'Duration must be at least 0.5 hours'),
  maxStudents: z.number().min(1, 'Maximum students must be at least 1').optional(),
  language: z.string().optional(),
  prerequisites: z.string().optional(),
  isPublished: z.boolean(),
  isFeatured: z.boolean(),
}).refine((data) => {
  // Discount price must be less than regular price
  if (data.discountPrice && data.discountPrice >= data.price) {
    return false;
  }
  return true;
}, {
  message: 'Discount price must be less than regular price',
  path: ['discountPrice'],
});

export type CreateCourseFormData = z.infer<typeof createCourseSchema>;

export const useCreateCourseForm = () => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const form = useForm<CreateCourseFormData>({
    resolver: zodResolver(createCourseSchema),
    defaultValues: {
      title: '',
      shortDescription: '',
      description: '',
      learningObjectives: '',
      categoryId: 1, // Default to first category instead of 0
      instructorId: 1, // Default to first instructor instead of 0  
      level: CourseLevel.Beginner,
      price: 0,
      discountPrice: undefined,
      durationHours: 1,
      maxStudents: undefined,
      language: '',
      prerequisites: '',
      isPublished: false,
      isFeatured: false,
    },
    mode: 'onChange',
  });

  const submitForm = async (
    data: CreateCourseFormData,
    onSuccess?: (response: any) => void
  ) => {
    try {
      setIsSubmitting(true);
      setSubmitError(null);

      // Transform form data to API request format
      const requestData: CreateCourseRequest = {
        title: data.title.trim(),
        shortDescription: data.shortDescription.trim(),
        description: data.description.trim(),
        learningObjectives: data.learningObjectives.trim(),
        categoryId: data.categoryId,
        instructorId: data.instructorId,
        level: data.level,
        price: data.price,
        discountPrice: data.discountPrice,
        durationHours: data.durationHours,
        maxStudents: data.maxStudents,
        language: data.language?.trim() || undefined,
        prerequisites: data.prerequisites?.trim() || undefined,
        isPublished: data.isPublished,
        isFeatured: data.isFeatured,
      };

      // Call the actual API
      const response = await coursesApi.createCourse(requestData);

      onSuccess?.(response);
      form.reset();
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Failed to create course';
      setSubmitError(message);
      throw error;
    } finally {
      setIsSubmitting(false);
    }
  };

  const resetForm = () => {
    form.reset();
    setSubmitError(null);
  };

  const isDirty = form.formState.isDirty;
  const isValid = form.formState.isValid;
  const errors = form.formState.errors;

  // Helper methods for form state
  const getFieldError = (field: keyof CreateCourseFormData) => {
    return errors[field]?.message;
  };

  const setFieldValue = (field: keyof CreateCourseFormData, value: any) => {
    form.setValue(field, value, { shouldDirty: true, shouldValidate: true });
  };

  const getFieldValue = (field: keyof CreateCourseFormData) => {
    return form.getValues(field);
  };

  const validateField = async (field: keyof CreateCourseFormData) => {
    await form.trigger(field);
    return !errors[field];
  };

  // Auto-save functionality
  const enableAutoSave = (callback: (data: Partial<CreateCourseFormData>) => void) => {
    const subscription = form.watch((data) => {
      if (isDirty) {
        callback(data);
      }
    });
    return () => subscription.unsubscribe();
  };

  return {
    // Form instance
    form,
    
    // Form state
    isSubmitting,
    submitError,
    isDirty,
    isValid,
    errors,
    
    // Form actions
    submitForm,
    resetForm,
    
    // Field helpers
    getFieldError,
    setFieldValue,
    getFieldValue,
    validateField,
    
    // Advanced features
    enableAutoSave,
    
    // Validation schema (for external use)
    schema: createCourseSchema,
  };
};