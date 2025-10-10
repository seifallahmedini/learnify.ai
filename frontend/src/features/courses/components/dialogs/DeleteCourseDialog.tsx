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
import { Badge } from '@/shared/components/ui/badge';
import { 
  Trash2, 
  AlertTriangle, 
  Users, 
  DollarSign, 
  X
} from 'lucide-react';
import { coursesApi } from '../../services';
import type { Course, CourseSummary } from '../../types';

interface DeleteCourseDialogProps {
  course: Course | CourseSummary | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCourseDeleted?: () => void;
  trigger?: React.ReactNode;
}

export function DeleteCourseDialog({ 
  course, 
  open, 
  onOpenChange, 
  onCourseDeleted, 
  trigger 
}: DeleteCourseDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Only render if we have a course to delete
  if (!course || course.id === 0) {
    return null;
  }

  const handleDelete = async () => {
    try {
      setIsDeleting(true);
      setError(null);
      
      console.log('Deleting course with ID:', course.id);
      await coursesApi.deleteCourse(course.id);
      
      console.log('Course deleted successfully');
      onCourseDeleted?.();
      onOpenChange(false);
      
    } catch (err) {
      console.error('Failed to delete course:', err);
      setError(err instanceof Error ? err.message : 'Failed to delete course');
    } finally {
      setIsDeleting(false);
    }
  };

  const handleClose = () => {
    if (!isDeleting) {
      setError(null);
      onOpenChange(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      {trigger && (
        <DialogTrigger asChild>
          {trigger}
        </DialogTrigger>
      )}
      <DialogContent className="w-[75vw] max-w-[1400px] min-w-[800px] max-h-[90vh] overflow-hidden flex flex-col">
        <DialogHeader className="space-y-6 pb-8 relative overflow-hidden">
          {/* Background Pattern */}
          <div className="absolute inset-0 bg-gradient-to-br from-red-500/5 via-transparent to-red-500/5 -z-10" />
          
          <div className="flex items-center justify-between relative">
            <div className="flex items-center space-x-4">
              <div className="h-14 w-14 rounded-2xl bg-gradient-to-br from-red-500/20 to-red-500/10 flex items-center justify-center border border-red-500/20 shadow-sm">
                <Trash2 className="h-7 w-7 text-red-600" />
              </div>
              <div className="space-y-1">
                <DialogTitle className="text-2xl font-bold tracking-tight bg-gradient-to-r from-foreground to-foreground/80 bg-clip-text">
                  Delete Course
                </DialogTitle>
                <DialogDescription className="text-base text-muted-foreground">
                  This action cannot be undone
                </DialogDescription>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <Badge variant="outline" className="bg-gradient-to-r from-red-50 to-red-100 text-red-700 border-red-200 px-4 py-2 font-medium shadow-sm">
                ID: {course.id}
              </Badge>
            </div>
          </div>
          
          
        </DialogHeader>

        <div className="flex-1 overflow-y-auto px-4 py-4">
          

          <div className="max-w-none mx-auto space-y-8">
            {/* Enhanced Course Preview */}
          <div className="bg-gradient-to-r from-red-50 via-red-50/80 to-red-50 rounded-2xl p-8 border border-red-200/50 shadow-sm backdrop-blur-sm">
            <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-center">
              <div className="lg:col-span-8 space-y-4">
                <div className="flex items-center space-x-2.5">
                  <div className="h-2.5 w-2.5 rounded-full bg-red-500 animate-pulse" />
                  <span className="text-sm font-medium text-muted-foreground uppercase tracking-wider">To Be Deleted</span>
                </div>
                <h4 className="text-2xl font-bold text-red-900 leading-tight">{course.title}</h4>
                <p className="text-sm text-red-700 opacity-80 leading-relaxed">This action cannot be undone</p>
              </div>
              <div className="lg:col-span-4 grid grid-cols-2 gap-6">
                <div className="text-center p-5 bg-gradient-to-br from-red-100 to-red-50 rounded-xl border border-red-200 shadow-sm">
                  <Users className="h-6 w-6 text-red-600 mx-auto mb-3" />
                  <p className="text-2xl font-bold text-red-900">{course.totalStudents || 0}</p>
                  <p className="text-xs text-red-700 font-medium mt-1">Students</p>
                </div>
                <div className="text-center p-5 bg-gradient-to-br from-red-100 to-red-50 rounded-xl border border-red-200 shadow-sm">
                  <DollarSign className="h-6 w-6 text-red-600 mx-auto mb-3" />
                  <p className="text-2xl font-bold text-red-900">${course.price}</p>
                  <p className="text-xs text-red-700 font-medium mt-1">Price</p>
                </div>
              </div>
            </div>
          </div>
            {/* Warning Section */}
            <div className="space-y-8 relative">
              <div className="relative">
                <div className="absolute inset-0 bg-gradient-to-r from-red-500/5 to-transparent rounded-2xl -z-10" />
                <div className="flex items-center space-x-4 p-6 rounded-2xl border border-red-500/20 bg-gradient-to-r from-red-50/50 to-transparent">
                  <div className="h-12 w-12 rounded-2xl bg-gradient-to-br from-red-100 to-red-50 flex items-center justify-center border border-red-200 shadow-sm">
                    <AlertTriangle className="h-6 w-6 text-red-600" />
                  </div>
                  <div className="flex-1">
                    <h3 className="text-xl font-bold text-foreground flex items-center space-x-2">
                      <span>Permanent Deletion Warning</span>
                      <div className="h-1 w-12 bg-gradient-to-r from-red-500 to-red-300 rounded-full" />
                    </h3>
                    <p className="text-sm text-muted-foreground mt-1">This will permanently delete the course</p>
                  </div>
                  <Badge variant="secondary" className="bg-red-100 text-red-700 border-red-200 px-3 py-1">Warning</Badge>
                </div>
              </div>
              
              <div className="space-y-4">
                <div className="flex items-start space-x-3 p-4 bg-red-50 border border-red-200 rounded-lg">
                  <AlertTriangle className="h-5 w-5 text-red-600 mt-0.5 flex-shrink-0" />
                  <div>
                    <h3 className="font-bold text-red-900 mb-2">⚠️ Permanent Deletion Warning</h3>
                    <p className="text-sm text-red-700 mb-2">
                      This will permanently delete the course and:
                    </p>
                    <ul className="text-sm text-red-700 space-y-1">
                      <li>• Remove all course content</li>
                      <li>• Unenroll all {course.totalStudents || 0} students</li>
                      <li>• Delete all progress data</li>
                      <li>• Remove all reviews and cancel subscriptions</li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
            
            {/* Confirmation Section */}
            <div className="space-y-8 relative">
              <div className="relative">
                <div className="absolute inset-0 bg-gradient-to-r from-gray-500/5 to-transparent rounded-2xl -z-10" />
                <div className="flex items-center space-x-4 p-6 rounded-2xl border border-gray-500/20 bg-gradient-to-r from-gray-50/50 to-transparent">
                  <div className="h-12 w-12 rounded-2xl bg-gradient-to-br from-gray-100 to-gray-50 flex items-center justify-center border border-gray-200 shadow-sm">
                    <Trash2 className="h-6 w-6 text-gray-600" />
                  </div>
                  <div className="flex-1">
                    <h3 className="text-xl font-bold text-foreground flex items-center space-x-2">
                      <span>Confirm Deletion</span>
                      <div className="h-1 w-12 bg-gradient-to-r from-gray-500 to-gray-300 rounded-full" />
                    </h3>
                    <p className="text-sm text-muted-foreground mt-1">Review before confirming</p>
                  </div>
                  <Badge variant="secondary" className="bg-gray-100 text-gray-700 border-gray-200 px-3 py-1">Required</Badge>
                </div>
              </div>
              
              <div className="p-4 bg-gray-50 border border-gray-200 rounded-lg">
                <h4 className="font-bold text-gray-900 mb-2">Confirm Deletion</h4>
                <p className="text-sm text-gray-700">
                  Course <strong className="text-red-700">"{course.title}"</strong> will be permanently deleted.
                </p>
                <p className="text-xs text-gray-600 mt-1">⚡ This action is immediate and irreversible.</p>
              </div>
            </div>

            {/* Error Display */}
            {error && (
              <div className="flex items-start space-x-3 p-4 bg-red-50 border border-red-200 rounded-lg">
                <AlertTriangle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
                <div>
                  <h4 className="text-sm font-medium text-red-800">Error deleting course</h4>
                  <p className="text-sm text-red-700 mt-1">{error}</p>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Modern Fluid Footer */}
        <div className="relative border-t-2 border-gradient-to-r from-border/50 to-border bg-gradient-to-r from-muted/40 via-muted/20 to-muted/40 px-8 py-6 mt-8">
          <div className="absolute inset-0 bg-gradient-to-r from-red-500/5 via-transparent to-red-500/5 opacity-50" />
          <DialogFooter className="gap-6 flex justify-between items-center relative">
            <div className="flex items-center space-x-3 text-sm text-muted-foreground">
              <div className="h-2 w-2 rounded-full bg-red-500 animate-pulse" />
              <span className="font-medium">This operation cannot be undone</span>
            </div>
            <div className="flex items-center gap-4">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isDeleting}
                className="h-12 px-8 rounded-xl border-2 transition-all duration-300 hover:border-gray-300 hover:bg-gray-50 hover:shadow-lg"
              >
                <X className="h-4 w-4 mr-2" />
                Cancel
              </Button>
              <Button
                type="button"
                variant="destructive"
                onClick={handleDelete}
                disabled={isDeleting}
                className="h-12 px-10 rounded-xl bg-gradient-to-r from-red-600 via-red-600 to-red-600/90 hover:from-red-700 hover:via-red-700 hover:to-red-700/90 shadow-lg hover:shadow-xl transition-all duration-300 hover:scale-105"
              >
                {isDeleting ? (
                  <>
                    <div className="h-4 w-4 mr-2 animate-spin rounded-full border-2 border-white border-t-transparent" />
                    Deleting...
                  </>
                ) : (
                  <>
                    <Trash2 className="h-4 w-4 mr-2" />
                    Delete Course
                  </>
                )}
              </Button>
            </div>
          </DialogFooter>
        </div>
      </DialogContent>
    </Dialog>
  );
}