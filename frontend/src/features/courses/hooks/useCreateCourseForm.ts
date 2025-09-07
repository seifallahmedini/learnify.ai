import { useState, useCallback } from 'react';
import type { CreateCourseFormData, CreateCourseRequest } from '../types';

interface FormErrors {
  title?: string;
  description?: string;
  shortDescription?: string;
  instructorId?: string;
  categoryId?: string;
  price?: string;
  discountPrice?: string;
  durationHours?: string;
  level?: string;
  language?: string;
  learningObjectives?: string;
  maxStudents?: string;
}

const initialFormData: CreateCourseFormData = {
  title: '',
  description: '',
  shortDescription: '',
  instructorId: 0,
  categoryId: 0,
  price: '',
  discountPrice: '',
  durationHours: '',
  level: 1, // Beginner by default
  language: 'English',
  thumbnailUrl: '',
  videoPreviewUrl: '',
  isPublished: false,
  isFeatured: false,
  maxStudents: '',
  prerequisites: '',
  learningObjectives: '',
};

export const useCreateCourseForm = () => {
  const [formData, setFormData] = useState<CreateCourseFormData>(initialFormData);
  const [errors, setErrors] = useState<FormErrors>({});
  const [isValidating, setIsValidating] = useState(false);

  const updateField = useCallback((field: keyof CreateCourseFormData, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));

    // Clear error for this field when user starts typing
    if (errors[field as keyof FormErrors]) {
      setErrors(prev => ({
        ...prev,
        [field]: undefined,
      }));
    }
  }, [errors]);

  const validateForm = useCallback((): boolean => {
    setIsValidating(true);
    const newErrors: FormErrors = {};

    // Required field validations
    if (!formData.title.trim()) {
      newErrors.title = 'Course title is required';
    } else if (formData.title.length < 3) {
      newErrors.title = 'Course title must be at least 3 characters';
    } else if (formData.title.length > 200) {
      newErrors.title = 'Course title must be less than 200 characters';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'Course description is required';
    } else if (formData.description.length < 50) {
      newErrors.description = 'Course description must be at least 50 characters';
    }

    if (!formData.shortDescription.trim()) {
      newErrors.shortDescription = 'Short description is required';
    } else if (formData.shortDescription.length < 20) {
      newErrors.shortDescription = 'Short description must be at least 20 characters';
    } else if (formData.shortDescription.length > 500) {
      newErrors.shortDescription = 'Short description must be less than 500 characters';
    }

    if (!formData.instructorId || formData.instructorId === 0) {
      newErrors.instructorId = 'Instructor selection is required';
    }

    if (!formData.categoryId || formData.categoryId === 0) {
      newErrors.categoryId = 'Category selection is required';
    }

    if (!formData.price.trim()) {
      newErrors.price = 'Course price is required';
    } else {
      const price = parseFloat(formData.price);
      if (isNaN(price) || price < 0) {
        newErrors.price = 'Please enter a valid price (0 or greater)';
      } else if (price > 999999) {
        newErrors.price = 'Price must be less than $999,999';
      }
    }

    if (formData.discountPrice && formData.discountPrice.trim()) {
      const discountPrice = parseFloat(formData.discountPrice);
      const originalPrice = parseFloat(formData.price);
      
      if (isNaN(discountPrice) || discountPrice < 0) {
        newErrors.discountPrice = 'Please enter a valid discount price';
      } else if (!isNaN(originalPrice) && discountPrice >= originalPrice) {
        newErrors.discountPrice = 'Discount price must be less than original price';
      }
    }

    if (!formData.durationHours.trim()) {
      newErrors.durationHours = 'Course duration is required';
    } else {
      const duration = parseFloat(formData.durationHours);
      if (isNaN(duration) || duration <= 0) {
        newErrors.durationHours = 'Please enter a valid duration (greater than 0)';
      } else if (duration > 1000) {
        newErrors.durationHours = 'Duration must be less than 1000 hours';
      }
    }

    if (!formData.language.trim()) {
      newErrors.language = 'Course language is required';
    }

    if (!formData.learningObjectives.trim()) {
      newErrors.learningObjectives = 'Learning objectives are required';
    } else if (formData.learningObjectives.length < 20) {
      newErrors.learningObjectives = 'Learning objectives must be at least 20 characters';
    }

    if (formData.maxStudents && formData.maxStudents.trim()) {
      const maxStudents = parseInt(formData.maxStudents);
      if (isNaN(maxStudents) || maxStudents < 1) {
        newErrors.maxStudents = 'Maximum students must be a positive number';
      } else if (maxStudents > 100000) {
        newErrors.maxStudents = 'Maximum students must be less than 100,000';
      }
    }

    setErrors(newErrors);
    setIsValidating(false);

    return Object.keys(newErrors).length === 0;
  }, [formData]);

  const resetForm = useCallback(() => {
    setFormData(initialFormData);
    setErrors({});
    setIsValidating(false);
  }, []);

  const setFormData_outer = useCallback((data: Partial<CreateCourseFormData>) => {
    setFormData(prev => ({
      ...prev,
      ...data,
    }));
  }, []);

  const transformToApiRequest = useCallback((): CreateCourseRequest => {
    return {
      title: formData.title.trim(),
      description: formData.description.trim(),
      shortDescription: formData.shortDescription.trim(),
      instructorId: formData.instructorId,
      categoryId: formData.categoryId,
      price: parseFloat(formData.price),
      discountPrice: formData.discountPrice && formData.discountPrice.trim() 
        ? parseFloat(formData.discountPrice) 
        : undefined,
      durationHours: parseFloat(formData.durationHours),
      level: formData.level,
      language: formData.language.trim(),
      thumbnailUrl: formData.thumbnailUrl && formData.thumbnailUrl.trim() 
        ? formData.thumbnailUrl.trim() 
        : undefined,
      videoPreviewUrl: formData.videoPreviewUrl && formData.videoPreviewUrl.trim() 
        ? formData.videoPreviewUrl.trim() 
        : undefined,
      isPublished: formData.isPublished,
      isFeatured: formData.isFeatured,
      maxStudents: formData.maxStudents && formData.maxStudents.trim() 
        ? parseInt(formData.maxStudents) 
        : undefined,
      prerequisites: formData.prerequisites && formData.prerequisites.trim() 
        ? formData.prerequisites.trim() 
        : undefined,
      learningObjectives: formData.learningObjectives.trim(),
    };
  }, [formData]);

  const isValid = Object.keys(errors).length === 0 && !isValidating;

  return {
    formData,
    errors,
    isValidating,
    isValid,
    updateField,
    validateForm,
    resetForm,
    setFormData: setFormData_outer,
    transformToApiRequest,
  };
};
