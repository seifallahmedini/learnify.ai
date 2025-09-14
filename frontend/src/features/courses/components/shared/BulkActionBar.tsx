import { Button } from '@/shared/components/ui/button';
import { Edit, Trash2, CheckSquare } from 'lucide-react';

interface BulkActionBarProps {
  selectedCount: number;
  totalCount?: number;
  onClearSelection: () => void;
  onSelectAll?: () => void;
  onBulkEdit?: () => void;
  onBulkDelete?: () => void;
  className?: string;
}

export function BulkActionBar({
  selectedCount,
  totalCount = 0,
  onClearSelection,
  onSelectAll,
  onBulkEdit,
  onBulkDelete,
  className = '',
}: BulkActionBarProps) {
  // Show "Select All" button when no items are selected and onSelectAll is provided
  if (selectedCount === 0 && onSelectAll && totalCount > 0) {
    return (
      <div className={`flex items-center justify-between p-3 bg-muted/30 border border-dashed rounded-lg ${className}`}>
        <span className="text-sm text-muted-foreground">
          No items selected
        </span>
        <Button
          variant="outline"
          size="sm"
          onClick={onSelectAll}
          className="h-8 text-xs gap-1 hover:bg-primary hover:text-primary-foreground"
        >
          <CheckSquare className="h-3 w-3" />
          Select All ({totalCount})
        </Button>
      </div>
    );
  }

  // Hide completely when no items selected and no select all function
  if (selectedCount === 0) return null;

  return (
    <div className={`flex items-center justify-between p-3 bg-primary/5 border rounded-lg ${className}`}>
      <div className="flex items-center gap-3">
        <span className="text-sm font-medium">
          {selectedCount} item{selectedCount !== 1 ? 's' : ''} selected
        </span>
        <Button
          variant="outline"
          size="sm"
          onClick={onClearSelection}
          className="h-8 text-xs"
        >
          Clear Selection
        </Button>
      </div>
      <div className="flex items-center gap-2">
        {onBulkEdit && (
          <Button variant="outline" size="sm" className="h-8 text-xs gap-1" onClick={onBulkEdit}>
            <Edit className="h-3 w-3" />
            Bulk Edit
          </Button>
        )}
        {onBulkDelete && (
          <Button 
            variant="outline" 
            size="sm" 
            className="h-8 text-xs gap-1 text-destructive hover:text-destructive"
            onClick={onBulkDelete}
          >
            <Trash2 className="h-3 w-3" />
            Delete Selected
          </Button>
        )}
      </div>
    </div>
  );
}