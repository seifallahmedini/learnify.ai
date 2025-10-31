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
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Label } from '@/shared/components/ui/label';
import { Switch } from '@/shared/components/ui/switch';
import { Plus } from 'lucide-react';
import { useQuizOperations } from '../../hooks';
import type { CreateQuizRequest } from '../../types';

interface CreateQuizDialogProps {
  courseId?: number | null;
  lessonId?: number | null;
  onQuizCreated?: () => void;
  trigger?: React.ReactNode;
}

export function CreateQuizDialog({
  courseId,
  lessonId,
  onQuizCreated,
  trigger,
}: CreateQuizDialogProps) {
  const { createQuiz, isLoading } = useQuizOperations();
  const [open, setOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<CreateQuizRequest>({
    courseId: courseId || 0,
    lessonId: lessonId || undefined,
    title: '',
    description: '',
    timeLimit: undefined,
    passingScore: 70,
    maxAttempts: 3,
    isActive: true,
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!courseId) {
      setError('Course ID is required');
      return;
    }

    try {
      setError(null);
      await createQuiz({
        ...formData,
        courseId,
      });
      
      // Reset form
      setFormData({
        courseId,
        lessonId: lessonId || undefined,
        title: '',
        description: '',
        timeLimit: undefined,
        passingScore: 70,
        maxAttempts: 3,
        isActive: true,
      });
      
      setOpen(false);
      onQuizCreated?.();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create quiz');
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || (
          <Button size="sm" className="gap-1.5 sm:gap-2">
            <Plus className="h-4 w-4" />
            <span className="hidden sm:inline">Create Quiz</span>
            <span className="sm:hidden">New</span>
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-[600px]">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Create New Quiz</DialogTitle>
            <DialogDescription>
              Create a new quiz for this course. You can add questions after creation.
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
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                placeholder="Enter quiz title"
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                value={formData.description}
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
                  value={formData.passingScore}
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
                value={formData.maxAttempts}
                onChange={(e) => setFormData({ 
                  ...formData, 
                  maxAttempts: parseInt(e.target.value) || 3 
                })}
              />
            </div>

            <div className="flex items-center space-x-2">
              <Switch
                id="isActive"
                checked={formData.isActive}
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
              {isLoading ? 'Creating...' : 'Create Quiz'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

