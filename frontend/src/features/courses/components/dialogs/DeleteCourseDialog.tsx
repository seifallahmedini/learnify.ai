import { useState } from 'react';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog';
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar';
import { Badge } from '@/shared/components/ui/badge';
import { 
  Trash2,
  AlertTriangle,
  Loader2,
  BookOpen,
  Users,
  Star,
} from 'lucide-react';
import { useCourseOperations, useCourseUtils } from '../../hooks';
import { CourseLevelBadge, CourseStatusBadge, CourseFeatureBadge } from '../shared';
import type { CourseSummary } from '../../types';

interface DeleteCourseDialogProps {
  course: CourseSummary | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCourseDeleted: (deletedCourseId: number) => void;
}

export function DeleteCourseDialog({ 
  course, 
  open, 
  onOpenChange, 
  onCourseDeleted 
}: DeleteCourseDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const { deleteCourse } = useCourseOperations();
  const { getCourseInitials, formatPrice } = useCourseUtils();

  // Early return to prevent any processing if no course
  if (!course) return null;

  // Simple computed values (avoid complex hooks that might cause re-renders)
  const courseInitials = getCourseInitials(course.title || '');
  const formattedPrice = formatPrice(course.price || 0);
  const formattedEffectivePrice = formatPrice(course.effectivePrice || 0);

  const handleDelete = async () => {
    if (!course) return;

    try {
      setIsDeleting(true);
      setError(null);

      const success = await deleteCourse(course.id);
      
      if (success) {
        onCourseDeleted(course.id);
        onOpenChange(false);
      } else {
        setError('Failed to delete course. Please try again.');
      }
    } catch (err) {
      console.error('Failed to delete course:', err);
      setError('Failed to delete course. Please try again.');
    } finally {
      setIsDeleting(false);
    }
  };

  const handleCancel = () => {
    setError(null);
    onOpenChange(false);
  };

  return (
    <AlertDialog open={open && !!course} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-md">
        <AlertDialogHeader>
          <AlertDialogTitle className="flex items-center gap-2 text-red-600">
            <AlertTriangle className="h-5 w-5" />
            Delete Course
          </AlertDialogTitle>
          <AlertDialogDescription className="text-sm text-muted-foreground">
            Are you sure you want to permanently delete this course? This action cannot be undone.
          </AlertDialogDescription>
          
          <div className="space-y-4">
            
            {/* Course Preview */}
            <div className="flex items-start gap-3 p-4 bg-red-50 border border-red-200 rounded-lg">
              <Avatar className="h-16 w-16 flex-shrink-0">
                <AvatarImage src={course.thumbnailUrl} alt={course.title} />
                <AvatarFallback className="text-sm font-medium bg-red-100 text-red-700">
                  {courseInitials}
                </AvatarFallback>
              </Avatar>
              
              <div className="flex-1 space-y-2 min-w-0">
                <div>
                  <div className="font-medium text-sm line-clamp-2">{course.title || 'Untitled Course'}</div>
                  <div className="text-xs text-muted-foreground line-clamp-2 mt-1">
                    {course.shortDescription || 'No description available'}
                  </div>
                </div>
                
                <div className="flex items-center gap-2 text-xs">
                  <Badge variant="outline" className="text-xs">
                    {course.categoryName || 'Uncategorized'}
                  </Badge>
                  {course.level && <CourseLevelBadge level={course.level} className="text-xs" />}
                </div>
                
                <div className="grid grid-cols-2 gap-2 text-xs">
                  <div className="flex items-center gap-1">
                    <Users className="h-3 w-3" />
                    <span>{course.totalStudents || 0} students</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <Star className="h-3 w-3" />
                    <span>{course.averageRating?.toFixed(1) || '0.0'} rating</span>
                  </div>
                </div>
                
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <CourseStatusBadge isPublished={course.isPublished ?? false} />
                    <CourseFeatureBadge isFeatured={course.isFeatured ?? false} />
                  </div>
                  <div className="text-sm font-medium text-right">
                    {course.isDiscounted ? (
                      <div className="space-y-0.5">
                        <div className="text-red-600 font-semibold">
                          {formattedEffectivePrice}
                        </div>
                        <div className="text-xs text-muted-foreground line-through">
                          {formattedPrice}
                        </div>
                      </div>
                    ) : (
                      <div className="text-gray-900">
                        {formattedPrice}
                      </div>
                    )}
                  </div>
                </div>
                
                <div className="text-xs text-muted-foreground">
                  Instructor: {course.instructorName || 'Unknown Instructor'}
                </div>
              </div>
            </div>

            {/* Warning Information */}
            <div className="space-y-3 text-sm">
              <div className="font-medium text-red-700">This will permanently delete:</div>
              <ul className="list-disc list-inside space-y-1 text-muted-foreground ml-2 text-xs">
                <li>Course content and materials</li>
                <li>All lessons and modules</li>
                <li>Student enrollment data</li>
                <li>Progress tracking information</li>
                <li>Course reviews and ratings</li>
                <li>Associated assignments and quizzes</li>
              </ul>
            </div>

            {/* Impact Warning */}
            {course.totalStudents > 0 && (
              <div className="p-3 bg-amber-50 border border-amber-200 rounded-lg">
                <div className="flex items-start gap-2">
                  <AlertTriangle className="h-4 w-4 text-amber-600 mt-0.5 flex-shrink-0" />
                  <div className="text-xs text-amber-800">
                    <div className="font-medium">High Impact Warning!</div>
                    <div className="mt-1">
                      This course has <strong>{course.totalStudents || 0} enrolled students</strong> who will 
                      lose access to their course content and progress. Consider archiving the course 
                      instead of deleting it.
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Alternative Suggestion */}
            <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg">
              <div className="flex items-start gap-2">
                <BookOpen className="h-4 w-4 text-blue-600 mt-0.5 flex-shrink-0" />
                <div className="text-xs text-blue-800">
                  <div className="font-medium">Alternative Options:</div>
                  <div className="mt-1">
                    Consider <strong>unpublishing</strong> the course instead of deleting it. 
                    This hides the course from new students while preserving existing enrollments 
                    and data.
                  </div>
                </div>
              </div>
            </div>

            {error && (
              <div className="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
                {error}
              </div>
            )}
          </div>
        </AlertDialogHeader>
        
        <AlertDialogFooter className="gap-2">
          <AlertDialogCancel 
            onClick={handleCancel}
            disabled={isDeleting}
            className="flex items-center gap-2"
          >
            <BookOpen className="h-4 w-4" />
            Keep Course
          </AlertDialogCancel>
          <AlertDialogAction
            onClick={handleDelete}
            disabled={isDeleting}
            className="bg-red-600 hover:bg-red-700 focus:ring-red-600 flex items-center gap-2"
          >
            {isDeleting ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <Trash2 className="h-4 w-4" />
            )}
            {isDeleting ? 'Deleting...' : 'Delete Course'}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
