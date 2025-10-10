import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { 
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { Badge } from '@/shared/components/ui/badge';
import { Search, Filter, X } from 'lucide-react';
import type { CategoryFilterRequest } from '../../types';

interface CategoryFiltersProps {
  onFiltersChange: (filters: Partial<CategoryFilterRequest>) => void;
  totalCount: number;
}

export function CategoryFilters({ onFiltersChange, totalCount }: CategoryFiltersProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [sortBy, setSortBy] = useState<string>('name');
  const [sortDirection, setSortDirection] = useState<string>('asc');

  const handleSearchChange = (value: string) => {
    setSearchTerm(value);
    onFiltersChange({ 
      searchTerm: value || undefined,
      page: 1 
    });
  };

  const handleStatusChange = (value: string) => {
    setStatusFilter(value);
    onFiltersChange({ 
      isActive: value === 'active' ? true : value === 'inactive' ? false : undefined,
      page: 1 
    });
  };

  const handleSortChange = (field: string) => {
    setSortBy(field);
    onFiltersChange({ 
      sortBy: field as 'name' | 'courseCount' | 'sortOrder' | 'createdAt',
      page: 1 
    });
  };

  const handleSortDirectionChange = (direction: string) => {
    setSortDirection(direction);
    onFiltersChange({ 
      sortDirection: direction as 'asc' | 'desc',
      page: 1 
    });
  };

  const clearFilters = () => {
    setSearchTerm('');
    setStatusFilter('all');
    setSortBy('name');
    setSortDirection('asc');
    onFiltersChange({
      searchTerm: undefined,
      isActive: undefined,
      sortBy: 'name',
      sortDirection: 'asc',
      page: 1,
    });
  };

  const hasActiveFilters = searchTerm || statusFilter !== 'all';

  return (
    <div className="space-y-4">
      {/* Filters Row */}
      <div className="flex flex-col sm:flex-row gap-4 p-4 bg-muted/50 rounded-lg border">
        {/* Search */}
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search categories..."
            value={searchTerm}
            onChange={(e) => handleSearchChange(e.target.value)}
            className="pl-10"
          />
        </div>

        {/* Status Filter */}
        <Select value={statusFilter} onValueChange={handleStatusChange}>
          <SelectTrigger className="w-full sm:w-[150px]">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Status</SelectItem>
            <SelectItem value="active">Active</SelectItem>
            <SelectItem value="inactive">Inactive</SelectItem>
          </SelectContent>
        </Select>

        {/* Sort By */}
        <Select value={sortBy} onValueChange={handleSortChange}>
          <SelectTrigger className="w-full sm:w-[150px]">
            <SelectValue placeholder="Sort by" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="name">Name</SelectItem>
            <SelectItem value="courseCount">Courses</SelectItem>
            <SelectItem value="sortOrder">Sort Order</SelectItem>
            <SelectItem value="createdAt">Created</SelectItem>
          </SelectContent>
        </Select>

        {/* Sort Direction */}
        <Select value={sortDirection} onValueChange={handleSortDirectionChange}>
          <SelectTrigger className="w-full sm:w-[120px]">
            <SelectValue placeholder="Order" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="asc">Ascending</SelectItem>
            <SelectItem value="desc">Descending</SelectItem>
          </SelectContent>
        </Select>

        {/* Clear Filters */}
        {hasActiveFilters && (
          <Button variant="outline" onClick={clearFilters} className="gap-2">
            <X className="h-4 w-4" />
            Clear
          </Button>
        )}
      </div>

      {/* Results Summary */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Filter className="h-4 w-4 text-muted-foreground" />
          <span className="text-sm text-muted-foreground">
            {totalCount} {totalCount === 1 ? 'category' : 'categories'} found
          </span>
          {hasActiveFilters && (
            <Badge variant="secondary" className="ml-2">
              Filtered
            </Badge>
          )}
        </div>
      </div>
    </div>
  );
}