import { useState, useEffect } from 'react';
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
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Label } from '@/shared/components/ui/label';
import { Switch } from '@/shared/components/ui/switch';
import { Edit } from 'lucide-react';
import { useQuizOperations } from '../../hooks';
import type { Quiz, QuizSummary, UpdateQuizRequest } from '../../types';

interface EditQuizDialogProps {
  quiz: Quiz | QuizSummary;
  onQuizUpdated?: (updatedQuiz: Quiz) => void;
  trigger?: React.ReactNode;
}

export function EditQuizDialog({
  quiz,
  onQuizUpdated,
  trigger,
}: EditQuizDialogProps) {
  const { updateQuiz, isLoading } = useQuizOperations();
  const [open, setOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<UpdateQuizRequest>({
    title: quiz.title,
    description: quiz.description,
    timeLimit: quiz.timeLimit,
    passingScore: quiz.passingScore,
    maxAttempts: 'maxAttempts' in quiz ? quiz.maxAttempts : undefined,
    isActive: quiz.isActive,
  });

  useEffect(() => {
    if (open) {
      setFormData({
        title: quiz.title,
        description: quiz.description,
        timeLimit: quiz.timeLimit,
        passingScore: quiz.passingScore,
        maxAttempts: 'maxAttempts' in quiz ? quiz.maxAttempts : undefined,
        isActive: quiz.isActive,
      });
      setError(null);
    }
  }, [open, quiz]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setError(null);
      const updatedQuiz = await updateQuiz(quiz.id, formData);
      
      if (updatedQuiz) {
        setOpen(false);
        onQuizUpdated?.(updatedQuiz);
      } else {
        setError('Failed to update quiz');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update quiz');
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || (
          <Button variant="outline" size="sm" className="gap-1.5 sm:gap-2">
            <Edit className="h-4 w-4" />
            <span className="hidden sm:inline">Edit</span>
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-[600px]">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Edit Quiz</DialogTitle>
            <DialogDescription>
              Update quiz information and settings.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            {error && (
              <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-lg text-sm text-destructive">
                {error}
              </div>
            )}

            <div className="space-y-2">
              <Label htmlFor="title">Quiz Title *</Label>
              <Input
                id="title"
                value={formData.title || ''}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                placeholder="Enter quiz title"
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                value={formData.description || ''}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                placeholder="Enter quiz description"
                rows={3}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="timeLimit">Time Limit (minutes)</Label>
                <Input
                  id="timeLimit"
                  type="number"
                  min="0"
                  value={formData.timeLimit || ''}
                  onChange={(e) => setFormData({ 
                    ...formData, 
                    timeLimit: e.target.value ? parseInt(e.target.value) : undefined 
                  })}
                  placeholder="No limit"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="passingScore">Passing Score (%)</Label>
                <Input
                  id="passingScore"
                  type="number"
                  min="0"
                  max="100"
                  value={formData.passingScore || 70}
                  onChange={(e) => setFormData({ 
                    ...formData, 
                    passingScore: parseInt(e.target.value) || 70 
                  })}
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="maxAttempts">Max Attempts</Label>
              <Input
                id="maxAttempts"
                type="number"
                min="1"
                value={formData.maxAttempts || 3}
                onChange={(e) => setFormData({ 
                  ...formData, 
                  maxAttempts: parseInt(e.target.value) || 3 
                })}
              />
            </div>

            <div className="flex items-center space-x-2">
              <Switch
                id="isActive"
                checked={formData.isActive ?? true}
                onCheckedChange={(checked) => setFormData({ ...formData, isActive: checked })}
              />
              <Label htmlFor="isActive">Active</Label>
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Updating...' : 'Update Quiz'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

