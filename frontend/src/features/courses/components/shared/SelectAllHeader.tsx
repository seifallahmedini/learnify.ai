import { Button } from '@/shared/components/ui/button';
import { Checkbox } from '@/shared/components/ui/checkbox';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/shared/components/ui/tooltip';

interface SelectAllHeaderProps {
  isAllSelected: boolean;
  isIndeterminate: boolean;
  selectedCount: number;
  totalCount: number;
  onSelectAll: (checked: boolean) => void;
  onClearSelection: () => void;
  className?: string;
}

export function SelectAllHeader({
  isAllSelected,
  isIndeterminate,
  selectedCount,
  totalCount,
  onSelectAll,
  onClearSelection,
  className = '',
}: SelectAllHeaderProps) {
  return (
    <TooltipProvider>
      <div className={`flex items-center justify-between p-4 bg-muted/30 hover:bg-muted/50 rounded-lg border transition-colors duration-200 ${className}`}>
        <div className="flex items-center gap-3">
          <Tooltip>
            <TooltipTrigger asChild>
              <div>
                <Checkbox
                  checked={isAllSelected}
                  onCheckedChange={onSelectAll}
                  aria-label="Select all items"
                  className="data-[state=indeterminate]:bg-primary data-[state=indeterminate]:text-primary-foreground"
                  {...(isIndeterminate && { 'data-state': 'indeterminate' })}
                />
              </div>
            </TooltipTrigger>
            <TooltipContent>
              <p>
                {isAllSelected 
                  ? 'Deselect all items' 
                  : isIndeterminate 
                    ? 'Select all remaining items'
                    : 'Select all items'
                } (Ctrl+A)
              </p>
            </TooltipContent>
          </Tooltip>
          <span className="text-sm font-medium">
            {isAllSelected 
              ? `All ${totalCount} items selected`
              : isIndeterminate 
                ? `${selectedCount} of ${totalCount} items selected`
                : `Select all ${totalCount} items`
            }
          </span>
        </div>
        {selectedCount > 0 && (
          <Button
            variant="outline"
            size="sm"
            onClick={onClearSelection}
            className="h-8 text-xs"
          >
            Clear Selection
          </Button>
        )}
      </div>
    </TooltipProvider>
  );
}