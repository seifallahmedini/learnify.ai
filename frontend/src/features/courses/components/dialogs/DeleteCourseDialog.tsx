import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/shared/components/ui/dialog';
import { AlertTriangle, Trash2 } from 'lucide-react';
import type { Course } from '../../types';

interface DeleteCourseDialogProps {
  course: Course;
  onCourseDeleted?: (courseId: number) => void;
  trigger?: React.ReactNode;
}

export function DeleteCourseDialog({ course, onCourseDeleted, trigger }: DeleteCourseDialogProps) {
  const [open, setOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  const handleDelete = async () => {
    try {
      setIsDeleting(true);
      
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      onCourseDeleted?.(course.id);
      setOpen(false);
    } catch (error) {
      console.error('Failed to delete course:', error);
    } finally {
      setIsDeleting(false);
    }
  };

  const defaultTrigger = (
    <Button variant="outline" size="sm">
      <Trash2 className="h-4 w-4 mr-2" />
      Delete
    </Button>
  );

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || defaultTrigger}
      </DialogTrigger>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center space-x-2">
            <AlertTriangle className="h-5 w-5 text-red-500" />
            <span>Delete Course</span>
          </DialogTitle>
          <DialogDescription>
            Are you sure you want to delete this course? This action cannot be undone.
          </DialogDescription>
        </DialogHeader>

        <div className="py-4">
          <div className="bg-gray-50 rounded-lg p-4">
            <h4 className="font-medium text-gray-900">{course.title}</h4>
            <p className="text-sm text-gray-600 mt-1">{course.shortDescription}</p>
            <div className="flex items-center space-x-4 mt-3 text-xs text-gray-500">
              <span>{course.totalStudents} students enrolled</span>
              <span>{course.totalReviews} reviews</span>
            </div>
          </div>

          {course.totalStudents > 0 && (
            <div className="mt-4 p-3 bg-amber-50 border border-amber-200 rounded-md">
              <div className="flex items-center">
                <AlertTriangle className="h-4 w-4 text-amber-600 mr-2" />
                <span className="text-sm text-amber-800">
                  This course has {course.totalStudents} enrolled students. 
                  Deleting it will remove their access to the course content.
                </span>
              </div>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => setOpen(false)}
            disabled={isDeleting}
          >
            Cancel
          </Button>
          <Button
            variant="destructive"
            onClick={handleDelete}
            disabled={isDeleting}
          >
            {isDeleting ? 'Deleting...' : 'Delete Course'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}