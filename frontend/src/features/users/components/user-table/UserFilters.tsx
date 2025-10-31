import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/shared/components/ui/select';
import { Button } from '@/shared/components/ui/button';
import { Badge } from '@/shared/components/ui/badge';
import { X } from 'lucide-react';
import type { UserRole } from '../../types';

interface UserFiltersProps {
  selectedRole: UserRole | "all"
  selectedStatus: "all" | "active" | "inactive"
  onRoleChange: (role: string) => void
  onStatusChange: (status: string) => void
}

export function UserFilters({
  selectedRole,
  selectedStatus,
  onRoleChange,
  onStatusChange,
}: UserFiltersProps) {
  const hasActiveFilters = selectedRole !== "all" || selectedStatus !== "all";

  const clearFilters = () => {
    onRoleChange('all');
    onStatusChange('all');
  };

  return (
    <div className="flex flex-col sm:flex-row gap-3 items-start sm:items-center">
      {/* Filter Controls */}
      <div className="flex flex-1 gap-2 w-full sm:w-auto">
        {/* Role Filter */}
        <div className="flex-1 sm:flex-initial sm:w-[140px]">
          <Select 
            value={selectedRole === "all" ? "all" : selectedRole.toString()} 
            onValueChange={onRoleChange}
          >
            <SelectTrigger className="w-full">
              <SelectValue placeholder="All roles" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Roles</SelectItem>
              <SelectItem value="1">Students</SelectItem>
              <SelectItem value="2">Instructors</SelectItem>
              <SelectItem value="3">Admins</SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Status Filter */}
        <div className="flex-1 sm:flex-initial sm:w-[140px]">
          <Select 
            value={selectedStatus} 
            onValueChange={onStatusChange}
          >
            <SelectTrigger className="w-full">
              <SelectValue placeholder="All statuses" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Status</SelectItem>
              <SelectItem value="active">Active</SelectItem>
              <SelectItem value="inactive">Inactive</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Clear Button - Only show when filters are active */}
      {hasActiveFilters && (
        <Button
          variant="ghost"
          size="sm"
          onClick={clearFilters}
          className="flex items-center gap-1.5 h-9 text-muted-foreground hover:text-foreground shrink-0"
        >
          <X className="h-4 w-4" />
          <span className="hidden sm:inline">Clear</span>
        </Button>
      )}

      {/* Active Filter Indicator */}
      {hasActiveFilters && (
        <div className="flex items-center gap-1.5 shrink-0">
          <Badge 
            variant="secondary" 
            className="h-7 px-2.5 text-xs font-medium"
          >
            {[selectedRole !== "all" ? 1 : 0, selectedStatus !== "all" ? 1 : 0].reduce((a, b) => a + b, 0)} active
          </Badge>
        </div>
      )}
    </div>
  );
}
