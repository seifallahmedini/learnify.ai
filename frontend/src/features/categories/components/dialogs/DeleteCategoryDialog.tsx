import { Button } from '@/shared/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog';
import { Trash2 } from 'lucide-react';
import type { CategorySummary } from '../../types';

interface DeleteCategoryDialogProps {
  category: CategorySummary | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCategoryDeleted?: () => void;
}

export function DeleteCategoryDialog({
  category,
  open,
  onOpenChange,
  onCategoryDeleted,
}: DeleteCategoryDialogProps) {
  if (!category) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-destructive">
            <Trash2 className="h-5 w-5" />
            Delete Category
          </DialogTitle>
          <DialogDescription>
            Are you sure you want to delete "{category.name}"? This action cannot be undone.
          </DialogDescription>
        </DialogHeader>

        <div className="py-4">
          <div className="bg-destructive/15 border border-destructive/20 rounded-lg p-3">
            <p className="text-sm text-destructive">
              This will permanently delete the category and may affect {category.courseCount} courses.
            </p>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button variant="destructive" onClick={() => {
            onCategoryDeleted?.();
            onOpenChange(false);
          }}>
            <Trash2 className="h-4 w-4 mr-2" />
            Delete Category
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}