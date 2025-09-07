import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Label } from '@/shared/components/ui/label';
import { Textarea } from '@/shared/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { Switch } from '@/shared/components/ui/switch';
import { Loader2, Plus, AlertCircle } from 'lucide-react';
import { useCreateCourseForm, useCourseOperations } from '../../hooks';
import { CourseLevel as CourseLevelEnum, getCourseLevelLabel } from '../../types';
import type { Course } from '../../types';

interface CreateCourseDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCourseCreated: (course: Course) => void;
}

// Mock data - in a real app, these would come from API
const mockCategories = [
  { id: 1, name: 'Programming' },
  { id: 2, name: 'Data Science' },
  { id: 3, name: 'Design' },
  { id: 4, name: 'Business' },
  { id: 5, name: 'Marketing' },
];

const mockInstructors = [
  { id: 1, name: 'John Doe' },
  { id: 2, name: 'Jane Smith' },
  { id: 3, name: 'Bob Johnson' },
  { id: 4, name: 'Alice Brown' },
];

export function CreateCourseDialog({ open, onOpenChange, onCourseCreated }: CreateCourseDialogProps) {
  const [submitError, setSubmitError] = useState<string | null>(null);
  
  const {
    formData,
    errors,
    isValidating,
    isValid,
    updateField,
    validateForm,
    resetForm,
    transformToApiRequest,
  } = useCreateCourseForm();

  const { createCourse, isLoading } = useCourseOperations();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    try {
      setSubmitError(null);
      const courseData = transformToApiRequest();
      const newCourse = await createCourse(courseData);
      
      if (newCourse) {
        onCourseCreated(newCourse);
        resetForm();
        onOpenChange(false);
      }
    } catch (error) {
      setSubmitError(error instanceof Error ? error.message : 'Failed to create course');
    }
  };

  const handleCancel = () => {
    resetForm();
    setSubmitError(null);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Plus className="h-5 w-5" />
            Create New Course
          </DialogTitle>
          <DialogDescription>
            Fill in the course details to create a new course. Required fields are marked with an asterisk (*).
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Basic Information */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium">Basic Information</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2 space-y-2">
                <Label htmlFor="title">Course Title *</Label>
                <Input
                  id="title"
                  value={formData.title}
                  onChange={(e) => updateField('title', e.target.value)}
                  placeholder="Enter course title..."
                  className={errors.title ? 'border-red-500' : ''}
                />
                {errors.title && (
                  <p className="text-sm text-red-600 flex items-center gap-1">
                    <AlertCircle className="h-4 w-4" />
                    {errors.title}
                  </p>
                )}
              </div>

              <div className="md:col-span-2 space-y-2">
                <Label htmlFor="shortDescription">Short Description *</Label>
                <Textarea
                  id="shortDescription"
                  value={formData.shortDescription}
                  onChange={(e) => updateField('shortDescription', e.target.value)}
                  placeholder="Brief description of the course (20-500 characters)..."
                  className={errors.shortDescription ? 'border-red-500' : ''}
                  rows={2}
                />
                <div className="flex justify-between text-xs text-muted-foreground">
                  <span>{formData.shortDescription.length}/500 characters</span>
                  {errors.shortDescription && (
                    <span className="text-red-600">{errors.shortDescription}</span>
                  )}
                </div>
              </div>

              <div className="md:col-span-2 space-y-2">
                <Label htmlFor="description">Full Description *</Label>
                <Textarea
                  id="description"
                  value={formData.description}
                  onChange={(e) => updateField('description', e.target.value)}
                  placeholder="Detailed description of the course content, objectives, and what students will learn..."
                  className={errors.description ? 'border-red-500' : ''}
                  rows={4}
                />
                {errors.description && (
                  <p className="text-sm text-red-600">{errors.description}</p>
                )}
              </div>
            </div>
          </div>

          {/* Course Details */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium">Course Details</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="category">Category *</Label>
                <Select
                  value={formData.categoryId.toString() || ''}
                  onValueChange={(value) => updateField('categoryId', parseInt(value) || 0)}
                >
                  <SelectTrigger className={errors.categoryId ? 'border-red-500' : ''}>
                    <SelectValue placeholder="Select category" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockCategories.map((category) => (
                      <SelectItem key={category.id} value={category.id.toString()}>
                        {category.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.categoryId && (
                  <p className="text-sm text-red-600">{errors.categoryId}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="instructor">Instructor *</Label>
                <Select
                  value={formData.instructorId.toString() || ''}
                  onValueChange={(value) => updateField('instructorId', parseInt(value) || 0)}
                >
                  <SelectTrigger className={errors.instructorId ? 'border-red-500' : ''}>
                    <SelectValue placeholder="Select instructor" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockInstructors.map((instructor) => (
                      <SelectItem key={instructor.id} value={instructor.id.toString()}>
                        {instructor.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.instructorId && (
                  <p className="text-sm text-red-600">{errors.instructorId}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="level">Course Level *</Label>
                <Select
                  value={formData.level.toString()}
                  onValueChange={(value) => updateField('level', parseInt(value))}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.entries(CourseLevelEnum).map(([, value]) => (
                      <SelectItem key={value} value={value.toString()}>
                        {getCourseLevelLabel(value)}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="language">Language *</Label>
                <Input
                  id="language"
                  value={formData.language}
                  onChange={(e) => updateField('language', e.target.value)}
                  placeholder="e.g., English, Spanish..."
                  className={errors.language ? 'border-red-500' : ''}
                />
                {errors.language && (
                  <p className="text-sm text-red-600">{errors.language}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="duration">Duration (hours) *</Label>
                <Input
                  id="duration"
                  type="number"
                  value={formData.durationHours}
                  onChange={(e) => updateField('durationHours', e.target.value)}
                  placeholder="e.g., 10.5"
                  min="0"
                  step="0.5"
                  className={errors.durationHours ? 'border-red-500' : ''}
                />
                {errors.durationHours && (
                  <p className="text-sm text-red-600">{errors.durationHours}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="maxStudents">Max Students</Label>
                <Input
                  id="maxStudents"
                  type="number"
                  value={formData.maxStudents}
                  onChange={(e) => updateField('maxStudents', e.target.value)}
                  placeholder="Leave empty for unlimited"
                  min="1"
                  className={errors.maxStudents ? 'border-red-500' : ''}
                />
                {errors.maxStudents && (
                  <p className="text-sm text-red-600">{errors.maxStudents}</p>
                )}
              </div>
            </div>
          </div>

          {/* Pricing */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium">Pricing</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="price">Price (USD) *</Label>
                <Input
                  id="price"
                  type="number"
                  value={formData.price}
                  onChange={(e) => updateField('price', e.target.value)}
                  placeholder="0.00"
                  min="0"
                  step="0.01"
                  className={errors.price ? 'border-red-500' : ''}
                />
                {errors.price && (
                  <p className="text-sm text-red-600">{errors.price}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="discountPrice">Discount Price (USD)</Label>
                <Input
                  id="discountPrice"
                  type="number"
                  value={formData.discountPrice}
                  onChange={(e) => updateField('discountPrice', e.target.value)}
                  placeholder="Optional discount price"
                  min="0"
                  step="0.01"
                  className={errors.discountPrice ? 'border-red-500' : ''}
                />
                {errors.discountPrice && (
                  <p className="text-sm text-red-600">{errors.discountPrice}</p>
                )}
              </div>
            </div>
          </div>

          {/* Content Details */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium">Content Details</h3>
            
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="learningObjectives">Learning Objectives *</Label>
                <Textarea
                  id="learningObjectives"
                  value={formData.learningObjectives}
                  onChange={(e) => updateField('learningObjectives', e.target.value)}
                  placeholder="Describe what students will learn and achieve after completing this course..."
                  className={errors.learningObjectives ? 'border-red-500' : ''}
                  rows={3}
                />
                {errors.learningObjectives && (
                  <p className="text-sm text-red-600">{errors.learningObjectives}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="prerequisites">Prerequisites</Label>
                <Textarea
                  id="prerequisites"
                  value={formData.prerequisites}
                  onChange={(e) => updateField('prerequisites', e.target.value)}
                  placeholder="List any required knowledge or skills students should have before taking this course..."
                  rows={2}
                />
              </div>
            </div>
          </div>

          {/* Media */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium">Media</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="thumbnailUrl">Thumbnail URL</Label>
                <Input
                  id="thumbnailUrl"
                  value={formData.thumbnailUrl}
                  onChange={(e) => updateField('thumbnailUrl', e.target.value)}
                  placeholder="https://example.com/thumbnail.jpg"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="videoPreviewUrl">Preview Video URL</Label>
                <Input
                  id="videoPreviewUrl"
                  value={formData.videoPreviewUrl}
                  onChange={(e) => updateField('videoPreviewUrl', e.target.value)}
                  placeholder="https://example.com/preview.mp4"
                />
              </div>
            </div>
          </div>

          {/* Status Options */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium">Publication Settings</h3>
            
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="isPublished" className="text-sm font-normal">
                    Publish Course
                  </Label>
                  <p className="text-xs text-muted-foreground">
                    Make this course available to students immediately
                  </p>
                </div>
                <Switch
                  id="isPublished"
                  checked={formData.isPublished}
                  onCheckedChange={(checked) => updateField('isPublished', checked)}
                />
              </div>

              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="isFeatured" className="text-sm font-normal">
                    Feature Course
                  </Label>
                  <p className="text-xs text-muted-foreground">
                    Highlight this course in featured sections
                  </p>
                </div>
                <Switch
                  id="isFeatured"
                  checked={formData.isFeatured}
                  onCheckedChange={(checked) => updateField('isFeatured', checked)}
                />
              </div>
            </div>
          </div>

          {/* Error Display */}
          {submitError && (
            <div className="p-4 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
              {submitError}
            </div>
          )}

          {/* Form Actions */}
          <DialogFooter className="gap-2">
            <Button 
              type="button" 
              variant="outline" 
              onClick={handleCancel}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              disabled={isLoading || isValidating || !isValid}
              className="flex items-center gap-2"
            >
              {isLoading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                <Plus className="h-4 w-4" />
              )}
              {isLoading ? 'Creating...' : 'Create Course'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
