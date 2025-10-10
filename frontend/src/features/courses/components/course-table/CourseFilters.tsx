import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/shared/components/ui/select';
import { Card, CardContent } from '@/shared/components/ui/card';
import { Search, Filter, X } from 'lucide-react';
import { CourseLevel, type CourseFilterRequest } from '../../types';
import { getCourseLevelLabel } from '../../lib/utils';

interface CourseFiltersProps {
  filters: CourseFilterRequest;
  onFiltersChange: (filters: CourseFilterRequest) => void;
  onClearFilters: () => void;
}

export function CourseFilters({ filters, onFiltersChange, onClearFilters }: CourseFiltersProps) {
  const [isExpanded, setIsExpanded] = useState(false);

  const handleFilterChange = (key: keyof CourseFilterRequest, value: any) => {
    onFiltersChange({
      ...filters,
      [key]: value === '' ? undefined : value,
    });
  };

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
                placeholder="Search courses..."
                value={filters.searchTerm || ''}
                onChange={(e) => handleFilterChange('searchTerm', e.target.value)}
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

          {/* Expanded Filters */}
          {isExpanded && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 pt-4 border-t">
              <div>
                <label className="text-sm font-medium mb-2 block">Level</label>
                <Select
                  value={filters.level?.toString() || ''}
                  onValueChange={(value) => handleFilterChange('level', value ? parseInt(value) : undefined)}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All levels" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All levels</SelectItem>
                    <SelectItem value={CourseLevel.Beginner.toString()}>
                      {getCourseLevelLabel(CourseLevel.Beginner)}
                    </SelectItem>
                    <SelectItem value={CourseLevel.Intermediate.toString()}>
                      {getCourseLevelLabel(CourseLevel.Intermediate)}
                    </SelectItem>
                    <SelectItem value={CourseLevel.Advanced.toString()}>
                      {getCourseLevelLabel(CourseLevel.Advanced)}
                    </SelectItem>
                    <SelectItem value={CourseLevel.Expert.toString()}>
                      {getCourseLevelLabel(CourseLevel.Expert)}
                    </SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium mb-2 block">Status</label>
                <Select
                  value={filters.isPublished?.toString() || ''}
                  onValueChange={(value) => handleFilterChange('isPublished', value === '' ? undefined : value === 'true')}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All statuses</SelectItem>
                    <SelectItem value="true">Published</SelectItem>
                    <SelectItem value="false">Draft</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium mb-2 block">Min Price</label>
                <Input
                  type="number"
                  placeholder="$0"
                  value={filters.minPrice || ''}
                  onChange={(e) => handleFilterChange('minPrice', e.target.value ? parseFloat(e.target.value) : undefined)}
                  min="0"
                  step="0.01"
                />
              </div>

              <div>
                <label className="text-sm font-medium mb-2 block">Max Price</label>
                <Input
                  type="number"
                  placeholder="No limit"
                  value={filters.maxPrice || ''}
                  onChange={(e) => handleFilterChange('maxPrice', e.target.value ? parseFloat(e.target.value) : undefined)}
                  min="0"
                  step="0.01"
                />
              </div>

              <div>
                <label className="text-sm font-medium mb-2 block">Featured</label>
                <Select
                  value={filters.isFeatured?.toString() || ''}
                  onValueChange={(value) => handleFilterChange('isFeatured', value === '' ? undefined : value === 'true')}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All courses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All courses</SelectItem>
                    <SelectItem value="true">Featured only</SelectItem>
                    <SelectItem value="false">Not featured</SelectItem>
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