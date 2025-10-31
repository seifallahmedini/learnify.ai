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
import { Trash2, AlertTriangle } from 'lucide-react';
import { useLessonOperations } from '../../hooks/useLessonManagement';
import type { Lesson } from '../../types';

interface DeleteLessonDialogProps {
  lesson: Lesson;
  onLessonDeleted?: () => void;
  trigger?: React.ReactNode;
}

export function DeleteLessonDialog({ lesson, onLessonDeleted, trigger }: DeleteLessonDialogProps) {
  const [open, setOpen] = useState(false);
  const { deleteLesson, isLoading, error } = useLessonOperations();

  const handleDelete = async () => {
    const success = await deleteLesson(lesson.id);
    if (success) {
      onLessonDeleted?.();
      setOpen(false);
    }
  };

  const defaultTrigger = (
    <Button variant="destructive" size="sm">
      <Trash2 className="h-4 w-4 mr-2" />
      Delete
    </Button>
  );

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || defaultTrigger}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <AlertTriangle className="h-5 w-5 text-destructive" />
            Delete Lesson?
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-4">
          <p className="text-sm text-muted-foreground">
            Are you sure you want to delete <strong>&quot;{lesson.title}&quot;</strong>? 
            This action cannot be undone.
          </p>
          
          {error && (
            <div className="p-3 text-sm text-red-600 bg-red-50 rounded-md">
              {error}
            </div>
          )}
        </div>

        <DialogFooter>
          <Button 
            type="button" 
            variant="outline" 
            onClick={() => setOpen(false)}
            disabled={isLoading}
          >
            Cancel
          </Button>
          <Button 
            type="button" 
            variant="destructive" 
            onClick={handleDelete}
            disabled={isLoading}
          >
            {isLoading ? 'Deleting...' : 'Delete Lesson'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

