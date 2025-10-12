import { useState, useEffect, useRef } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/shared/components/ui/select';
import { Card, CardContent } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Search, Filter, X } from 'lucide-react';
import type { CategoryFilterRequest } from '../../types';

interface CategoryFiltersProps {
  filters: CategoryFilterRequest;
  onFiltersChange: (filters: CategoryFilterRequest) => void;
  onClearFilters: () => void;
}

export function CategoryFilters({ filters, onFiltersChange, onClearFilters }: CategoryFiltersProps) {
  const [isExpanded, setIsExpanded] = useState(false);
  const [localSearchTerm, setLocalSearchTerm] = useState(filters.searchTerm || '');
  const debounceRef = useRef<NodeJS.Timeout | null>(null);

  const handleSearchChange = (value: string) => {
    setLocalSearchTerm(value);
    
    // Clear existing timeout
    if (debounceRef.current) {
      clearTimeout(debounceRef.current);
    }

    // Set new timeout for debounced search
    debounceRef.current = setTimeout(() => {
      onFiltersChange({
        ...filters,
        searchTerm: value.trim() || undefined,
      });
    }, 300);
  };

  const handleFilterChange = (key: keyof CategoryFilterRequest, value: any) => {
    // If clearing search term, also clear local state
    if (key === 'searchTerm' && (value === undefined || value === '')) {
      setLocalSearchTerm('');
    }
    
    onFiltersChange({
      ...filters,
      [key]: value === '' ? undefined : value,
    });
  };

  // Handle external filter changes (like clear all filters)
  useEffect(() => {
    if (!filters.searchTerm && localSearchTerm) {
      setLocalSearchTerm('');
    }
  }, [filters.searchTerm]);

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (debounceRef.current) {
        clearTimeout(debounceRef.current);
      }
    };
  }, []);

  const hasActiveFilters = Object.values(filters).some(value => 
    value !== undefined && value !== '' && value !== null
  );

  return (
    <Card>
      <CardContent className="p-4">
        <div className="space-y-4">
          {/* Search and Quick Filters */}
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Search categories by name..."
                value={localSearchTerm}
                onChange={(e) => handleSearchChange(e.target.value)}
                className="pl-10"
              />
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setIsExpanded(!isExpanded)}
                className="flex items-center gap-2"
              >
                <Filter className="h-4 w-4" />
                Filters
              </Button>
              {hasActiveFilters && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onClearFilters}
                  className="flex items-center gap-2"
                >
                  <X className="h-4 w-4" />
                  Clear
                </Button>
              )}
            </div>
          </div>

          {/* Active Filters Display */}
          {hasActiveFilters && (
            <div className="flex flex-wrap gap-2">
              {filters.searchTerm && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Search: {filters.searchTerm}
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('searchTerm', undefined)}
                  />
                </Badge>
              )}
              {filters.isActive !== undefined && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Status: {filters.isActive ? 'Active' : 'Inactive'}
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('isActive', undefined)}
                  />
                </Badge>
              )}
              {filters.rootOnly && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Root Only
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('rootOnly', undefined)}
                  />
                </Badge>
              )}
            </div>
          )}

          {/* Expanded Filters */}
          {isExpanded && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 border-t">
              <div>
                <label className="text-sm font-medium mb-2 block">Status</label>
                <Select
                  value={filters.isActive?.toString() || ''}
                  onValueChange={(value) => handleFilterChange('isActive', value === '' ? undefined : value === 'true')}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All statuses</SelectItem>
                    <SelectItem value="true">Active</SelectItem>
                    <SelectItem value="false">Inactive</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium mb-2 block">Type</label>
                <Select
                  value={filters.rootOnly?.toString() || ''}
                  onValueChange={(value) => {
                    if (value === 'true') {
                      handleFilterChange('rootOnly', true);
                      handleFilterChange('parentCategoryId', undefined);
                    } else {
                      handleFilterChange('rootOnly', undefined);
                    }
                  }}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All categories" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All categories</SelectItem>
                    <SelectItem value="true">Root categories only</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}