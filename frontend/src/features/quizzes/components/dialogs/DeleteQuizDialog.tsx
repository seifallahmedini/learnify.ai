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
import { Trash2 } from 'lucide-react';
import { useQuizOperations } from '../../hooks';
import type { Quiz, QuizSummary } from '../../types';

interface DeleteQuizDialogProps {
  quiz: Quiz | QuizSummary;
  onQuizDeleted?: () => void;
  trigger?: React.ReactNode;
}

export function DeleteQuizDialog({
  quiz,
  onQuizDeleted,
  trigger,
}: DeleteQuizDialogProps) {
  const { deleteQuiz, isLoading } = useQuizOperations();
  const [open, setOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleDelete = async () => {
    try {
      setError(null);
      const success = await deleteQuiz(quiz.id);
      
      if (success) {
        setOpen(false);
        onQuizDeleted?.();
      } else {
        setError('Failed to delete quiz');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete quiz');
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || (
          <Button variant="outline" size="sm" className="gap-1.5 sm:gap-2 text-destructive/70 hover:text-destructive hover:bg-destructive/10 hover:border-destructive/50">
            <Trash2 className="h-4 w-4" />
            <span className="hidden sm:inline">Delete</span>
          </Button>
        )}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Delete Quiz</DialogTitle>
          <DialogDescription>
            Are you sure you want to delete &quot;{quiz.title}&quot;? This action cannot be undone.
          </DialogDescription>
        </DialogHeader>

        {error && (
          <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-lg text-sm text-destructive">
            {error}
          </div>
        )}

        <DialogFooter>
          <Button type="button" variant="outline" onClick={() => setOpen(false)}>
            Cancel
          </Button>
          <Button 
            type="button" 
            variant="destructive" 
            onClick={handleDelete}
            disabled={isLoading}
          >
            {isLoading ? 'Deleting...' : 'Delete Quiz'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

