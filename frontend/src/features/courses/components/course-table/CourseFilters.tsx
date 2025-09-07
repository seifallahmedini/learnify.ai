import { useState, useEffect } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Label } from '@/shared/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { Switch } from '@/shared/components/ui/switch';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { X, Filter, RotateCcw } from 'lucide-react';
import type { CourseFilterRequest, CourseLevel } from '../../types';
import { CourseLevel as CourseLevelEnum, getCourseLevelLabel } from '../../types';

interface CourseFiltersProps {
  filters: CourseFilterRequest;
  onFiltersChange: (filters: CourseFilterRequest) => void;
  onClearFilters: () => void;
}

interface FilterState {
  categoryId?: number;
  instructorId?: number;
  level?: CourseLevel;
  isPublished?: boolean;
  isFeatured?: boolean;
  minPrice?: string;
  maxPrice?: string;
  searchTerm?: string;
}

// Mock data - in a real app, these would come from API
const mockCategories = [
  { id: 1, name: 'Programming' },
  { id: 2, name: 'Data Science' },
  { id: 3, name: 'Design' },
  { id: 4, name: 'Business' },
  { id: 5, name: 'Marketing' },
];

const mockInstructors = [
  { id: 1, name: 'John Doe' },
  { id: 2, name: 'Jane Smith' },
  { id: 3, name: 'Bob Johnson' },
  { id: 4, name: 'Alice Brown' },
];

export function CourseFilters({ filters, onFiltersChange, onClearFilters }: CourseFiltersProps) {
  const [localFilters, setLocalFilters] = useState<FilterState>({
    categoryId: filters.categoryId,
    instructorId: filters.instructorId,
    level: filters.level,
    isPublished: filters.isPublished,
    isFeatured: filters.isFeatured,
    minPrice: filters.minPrice?.toString() || '',
    maxPrice: filters.maxPrice?.toString() || '',
    searchTerm: filters.searchTerm || '',
  });

  useEffect(() => {
    setLocalFilters({
      categoryId: filters.categoryId,
      instructorId: filters.instructorId,
      level: filters.level,
      isPublished: filters.isPublished,
      isFeatured: filters.isFeatured,
      minPrice: filters.minPrice?.toString() || '',
      maxPrice: filters.maxPrice?.toString() || '',
      searchTerm: filters.searchTerm || '',
    });
  }, [filters]);

  const handleFilterChange = (key: keyof FilterState, value: any) => {
    setLocalFilters(prev => ({
      ...prev,
      [key]: value,
    }));
  };

  const applyFilters = () => {
    const newFilters: CourseFilterRequest = {
      ...filters,
      categoryId: localFilters.categoryId || undefined,
      instructorId: localFilters.instructorId || undefined,
      level: localFilters.level || undefined,
      isPublished: localFilters.isPublished,
      isFeatured: localFilters.isFeatured,
      minPrice: localFilters.minPrice ? parseFloat(localFilters.minPrice) : undefined,
      maxPrice: localFilters.maxPrice ? parseFloat(localFilters.maxPrice) : undefined,
      searchTerm: localFilters.searchTerm || undefined,
      page: 1, // Reset to first page when filters change
    };

    onFiltersChange(newFilters);
  };

  const resetFilters = () => {
    setLocalFilters({
      categoryId: undefined,
      instructorId: undefined,
      level: undefined,
      isPublished: undefined,
      isFeatured: undefined,
      minPrice: '',
      maxPrice: '',
      searchTerm: '',
    });
    onClearFilters();
  };

  const getActiveFiltersCount = () => {
    let count = 0;
    if (localFilters.categoryId) count++;
    if (localFilters.instructorId) count++;
    if (localFilters.level) count++;
    if (localFilters.isPublished !== undefined) count++;
    if (localFilters.isFeatured !== undefined) count++;
    if (localFilters.minPrice) count++;
    if (localFilters.maxPrice) count++;
    return count;
  };

  const activeFiltersCount = getActiveFiltersCount();

  return (
    <Card>
      <CardHeader className="pb-4">
        <div className="flex items-center justify-between">
          <CardTitle className="text-lg flex items-center gap-2">
            <Filter className="h-5 w-5" />
            Course Filters
            {activeFiltersCount > 0 && (
              <Badge variant="secondary" className="ml-2">
                {activeFiltersCount} active
              </Badge>
            )}
          </CardTitle>
          <Button
            variant="ghost"
            size="sm"
            onClick={resetFilters}
            className="flex items-center gap-2"
          >
            <RotateCcw className="h-4 w-4" />
            Reset
          </Button>
        </div>
      </CardHeader>
      
      <CardContent className="space-y-6">
        {/* Category and Instructor */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="space-y-2">
            <Label htmlFor="category">Category</Label>
            <Select
              value={localFilters.categoryId?.toString() || ''}
              onValueChange={(value) => handleFilterChange('categoryId', value ? parseInt(value) : undefined)}
            >
              <SelectTrigger>
                <SelectValue placeholder="All categories" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All categories</SelectItem>
                {mockCategories.map((category) => (
                  <SelectItem key={category.id} value={category.id.toString()}>
                    {category.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="instructor">Instructor</Label>
            <Select
              value={localFilters.instructorId?.toString() || ''}
              onValueChange={(value) => handleFilterChange('instructorId', value ? parseInt(value) : undefined)}
            >
              <SelectTrigger>
                <SelectValue placeholder="All instructors" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All instructors</SelectItem>
                {mockInstructors.map((instructor) => (
                  <SelectItem key={instructor.id} value={instructor.id.toString()}>
                    {instructor.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        {/* Level */}
        <div className="space-y-2">
          <Label htmlFor="level">Course Level</Label>
          <Select
            value={localFilters.level?.toString() || ''}
            onValueChange={(value) => handleFilterChange('level', value ? parseInt(value) as CourseLevel : undefined)}
          >
            <SelectTrigger>
              <SelectValue placeholder="All levels" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="">All levels</SelectItem>
              {Object.entries(CourseLevelEnum).map(([, value]) => (
                <SelectItem key={value} value={value.toString()}>
                  {getCourseLevelLabel(value)}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {/* Price Range */}
        <div className="space-y-2">
          <Label>Price Range</Label>
          <div className="grid grid-cols-2 gap-2">
            <div className="space-y-1">
              <Label htmlFor="minPrice" className="text-sm text-muted-foreground">Min Price</Label>
              <Input
                id="minPrice"
                type="number"
                placeholder="0"
                value={localFilters.minPrice}
                onChange={(e) => handleFilterChange('minPrice', e.target.value)}
                min="0"
                step="0.01"
              />
            </div>
            <div className="space-y-1">
              <Label htmlFor="maxPrice" className="text-sm text-muted-foreground">Max Price</Label>
              <Input
                id="maxPrice"
                type="number"
                placeholder="999"
                value={localFilters.maxPrice}
                onChange={(e) => handleFilterChange('maxPrice', e.target.value)}
                min="0"
                step="0.01"
              />
            </div>
          </div>
        </div>

        {/* Status Toggles */}
        <div className="space-y-4">
          <Label>Course Status</Label>
          
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label htmlFor="published" className="text-sm font-normal">
                  Published Only
                </Label>
                <p className="text-xs text-muted-foreground">
                  Show only published courses
                </p>
              </div>
              <Switch
                id="published"
                checked={localFilters.isPublished === true}
                onCheckedChange={(checked) => handleFilterChange('isPublished', checked || undefined)}
              />
            </div>

            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label htmlFor="featured" className="text-sm font-normal">
                  Featured Only
                </Label>
                <p className="text-xs text-muted-foreground">
                  Show only featured courses
                </p>
              </div>
              <Switch
                id="featured"
                checked={localFilters.isFeatured === true}
                onCheckedChange={(checked) => handleFilterChange('isFeatured', checked || undefined)}
              />
            </div>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex gap-3 pt-4 border-t">
          <Button onClick={applyFilters} className="flex-1">
            Apply Filters
          </Button>
          <Button variant="outline" onClick={resetFilters}>
            Clear All
          </Button>
        </div>

        {/* Active Filters Display */}
        {activeFiltersCount > 0 && (
          <div className="space-y-2 pt-4 border-t">
            <Label className="text-sm font-medium">Active Filters:</Label>
            <div className="flex flex-wrap gap-2">
              {localFilters.categoryId && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Category: {mockCategories.find(c => c.id === localFilters.categoryId)?.name}
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('categoryId', undefined)}
                  />
                </Badge>
              )}
              {localFilters.instructorId && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Instructor: {mockInstructors.find(i => i.id === localFilters.instructorId)?.name}
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('instructorId', undefined)}
                  />
                </Badge>
              )}
              {localFilters.level && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Level: {getCourseLevelLabel(localFilters.level)}
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('level', undefined)}
                  />
                </Badge>
              )}
              {localFilters.isPublished === true && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Published
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('isPublished', undefined)}
                  />
                </Badge>
              )}
              {localFilters.isFeatured === true && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Featured
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => handleFilterChange('isFeatured', undefined)}
                  />
                </Badge>
              )}
              {(localFilters.minPrice || localFilters.maxPrice) && (
                <Badge variant="secondary" className="flex items-center gap-1">
                  Price: ${localFilters.minPrice || '0'} - ${localFilters.maxPrice || '∞'}
                  <X 
                    className="h-3 w-3 cursor-pointer" 
                    onClick={() => {
                      handleFilterChange('minPrice', '');
                      handleFilterChange('maxPrice', '');
                    }}
                  />
                </Badge>
              )}
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
