import { useState, useEffect } from 'react';
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
import { Badge } from '@/shared/components/ui/badge';
import { 
  Trash2,
  AlertTriangle,
  Loader2,
  BookOpen,
  Users,
  Calendar,
  DollarSign
} from 'lucide-react';
import type { CourseSummary } from '../../types';
import { CourseFeatureBadge } from '../shared';

interface DeleteCourseDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  course: CourseSummary | null;
  onCourseDeleted: (course: CourseSummary) => void;
}

export function DeleteCourseDialog({ 
  open, 
  onOpenChange, 
  course, 
  onCourseDeleted 
}: DeleteCourseDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Reset error state when dialog opens/closes or course changes
  useEffect(() => {
    if (!open || !course) {
      setError(null);
      setIsDeleting(false);
    }
  }, [open, course]);

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  const handleDelete = async () => {
    if (!course) return;

    try {
      setIsDeleting(true);
      setError(null);

      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1500));
      
      // Simulate potential error for courses with students
      if (course.totalStudents > 0) {
        throw new Error('Cannot delete course with enrolled students. Please unenroll all students first.');
      }

      onCourseDeleted(course);
      onOpenChange(false);
    } catch (err) {
      console.error('Failed to delete course:', err);
      setError(err instanceof Error ? err.message : 'Failed to delete course');
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
        {course && (
          <>
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center gap-2 text-red-600">
                <AlertTriangle className="h-5 w-5" />
                Delete Course
              </AlertDialogTitle>
              <AlertDialogDescription className="space-y-4">
                <div className="text-sm text-muted-foreground">
                  Are you sure you want to permanently delete this course? This action cannot be undone.
                </div>
                
                {/* Course Preview */}
                <div className="flex items-center gap-3 p-3 bg-red-50 border border-red-200 rounded-lg">
                  <div className="flex-shrink-0 w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
                    <BookOpen className="h-6 w-6 text-red-600" />
                  </div>
                  <div className="flex-1 space-y-1">
                    <h4 className="font-medium text-sm">{course.title}</h4>
                    <div className="flex items-center gap-2 text-xs text-muted-foreground">
                      <Calendar className="h-3 w-3" />
                      Created: {new Date(course.createdAt).toLocaleDateString()}
                    </div>
                    <div className="flex items-center gap-2">
                      {course.isFeatured && <CourseFeatureBadge isFeatured={course.isFeatured} />}
                      <Badge 
                        variant={course.isPublished ? 'default' : 'secondary'} 
                        className="text-xs"
                      >
                        {course.isPublished ? 'Published' : 'Draft'}
                      </Badge>
                      <Badge variant="outline" className="text-xs">
                        ID: #{course.id}
                      </Badge>
                    </div>
                  </div>
                </div>

                {/* Course Stats */}
                <div className="grid grid-cols-2 gap-2 text-xs">
                  <div className="flex items-center gap-1">
                    <Users className="h-3 w-3 text-gray-500" />
                    <span>{course.totalStudents} students</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <DollarSign className="h-3 w-3 text-gray-500" />
                    <span>{formatPrice(course.price)}</span>
                  </div>
                </div>

                {/* Warning Information */}
                <div className="space-y-2 text-sm">
                  <div className="font-medium text-red-700">This will permanently delete:</div>
                  <ul className="list-disc list-inside space-y-1 text-muted-foreground ml-2">
                    <li>Course content and materials</li>
                    <li>Student enrollments and progress data</li>
                    <li>Quiz attempts and assessment results</li>
                    <li>Reviews and ratings for this course</li>
                    <li>All associated lesson content</li>
                  </ul>
                </div>

                {course.totalStudents > 0 && (
                  <div className="p-3 bg-amber-50 border border-amber-200 rounded-lg">
                    <div className="flex items-start gap-2">
                      <AlertTriangle className="h-4 w-4 text-amber-600 mt-0.5 flex-shrink-0" />
                      <div className="text-xs text-amber-800">
                        <div className="font-medium">Cannot Delete:</div>
                        <div className="mt-1">
                          This course has <strong>{course.totalStudents} enrolled student{course.totalStudents !== 1 ? 's' : ''}</strong>. 
                          You must unenroll all students before deleting this course.
                        </div>
                      </div>
                    </div>
                  </div>
                )}

                {error && (
                  <div className="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
                    {error}
                  </div>
                )}
              </AlertDialogDescription>
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
                disabled={isDeleting || !course || course.totalStudents > 0}
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
          </>
        )}
      </AlertDialogContent>
    </AlertDialog>
  );
}