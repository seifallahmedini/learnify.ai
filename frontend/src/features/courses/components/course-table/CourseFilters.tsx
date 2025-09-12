import { useState } from 'react';
import { Card, CardContent } from "@/shared/components/ui/card"
import { Input } from "@/shared/components/ui/input"
import { Label } from "@/shared/components/ui/label"
import { Button } from "@/shared/components/ui/button"
import { Badge } from "@/shared/components/ui/badge"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select"
import { Search, Filter, RotateCcw, ChevronDown, ChevronUp, X } from "lucide-react"
import type { CourseFilterRequest, CourseLevel } from '../../types';

interface CourseFiltersProps {
  filters: CourseFilterRequest;
  onFiltersChange: (filters: CourseFilterRequest) => void;
  onClearFilters: () => void;
}

export function CourseFilters({
  filters,
  onFiltersChange,
  onClearFilters,
}: CourseFiltersProps) {
  const [isExpanded, setIsExpanded] = useState(false);

  const getActiveFiltersCount = () => {
    let count = 0;
    if (filters.searchTerm) count++;
    if (filters.categoryId) count++;
    if (filters.level) count++;
    if (filters.isPublished !== undefined || filters.isFeatured !== undefined) count++;
    return count;
  };

  const resetFilters = () => {
    onClearFilters();
  };

  const getCategoryLabel = (categoryId: number | undefined) => {
    if (!categoryId) return "All Categories";
    switch (categoryId) {
      case 1: return "Programming";
      case 2: return "Data Science";
      case 3: return "Design";
      case 4: return "Business";
      case 5: return "Marketing";
      default: return "Unknown";
    }
  };

  const getLevelLabel = (level: CourseLevel | undefined) => {
    if (!level) return "All Levels";
    switch (level) {
      case 1: return "Beginner";
      case 2: return "Intermediate";
      case 3: return "Advanced";
      case 4: return "Expert";
      default: return "Unknown";
    }
  };

  const getStatusLabel = () => {
    if (filters.isPublished === true) return "Published";
    if (filters.isPublished === false) return "Draft";
    if (filters.isFeatured === true) return "Featured";
    return "All Status";
  };

  const getCurrentStatus = () => {
    if (filters.isPublished === true) return "published";
    if (filters.isPublished === false) return "draft";
    if (filters.isFeatured === true) return "featured";
    return "all";
  };

  const handleSearchChange = (value: string) => {
    onFiltersChange({
      ...filters,
      searchTerm: value || undefined,
      page: 1
    });
  };

  const handleCategoryChange = (value: string) => {
    onFiltersChange({
      ...filters,
      categoryId: value === "all" ? undefined : parseInt(value),
      page: 1
    });
  };

  const handleLevelChange = (value: string) => {
    onFiltersChange({
      ...filters,
      level: value === "all" ? undefined : parseInt(value) as CourseLevel,
      page: 1
    });
  };

  const handleStatusChange = (value: string) => {
    let statusFilters = {};
    
    switch (value) {
      case "published":
        statusFilters = { isPublished: true, isFeatured: undefined };
        break;
      case "draft":
        statusFilters = { isPublished: false, isFeatured: undefined };
        break;
      case "featured":
        statusFilters = { isFeatured: true, isPublished: undefined };
        break;
      case "all":
      default:
        statusFilters = { isPublished: undefined, isFeatured: undefined };
        break;
    }

    onFiltersChange({
      ...filters,
      ...statusFilters,
      page: 1
    });
  };

  const activeFiltersCount = getActiveFiltersCount();

  return (
    <div className="space-y-3">
      {/* Compact Header with Quick Filters */}
      <div className="flex items-center justify-between p-3 bg-muted/30 rounded-lg border border-l-4 border-l-primary">
        <div className="flex items-center gap-3">
          <button
            onClick={() => setIsExpanded(!isExpanded)}
            className="flex items-center gap-2 text-sm font-medium hover:text-primary transition-colors"
          >
            <Filter className="h-4 w-4 text-primary" />
            Filters
            {activeFiltersCount > 0 && (
              <Badge variant="secondary" className="bg-primary text-primary-foreground text-xs px-1.5 py-0.5">
                {activeFiltersCount}
              </Badge>
            )}
            {isExpanded ? (
              <ChevronUp className="h-3 w-3 text-muted-foreground" />
            ) : (
              <ChevronDown className="h-3 w-3 text-muted-foreground" />
            )}
          </button>
        </div>
        
        {/* Quick Action Buttons */}
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={resetFilters}
            className="h-7 px-2 text-xs text-muted-foreground hover:text-foreground"
          >
            <RotateCcw className="h-3 w-3" />
          </Button>
        </div>
      </div>

      {/* Compact Active Filters Bar */}
      {!isExpanded && activeFiltersCount > 0 && (
        <div className="flex flex-wrap gap-1.5 px-3 py-2 bg-muted/20 rounded border border-dashed">
          {filters.searchTerm && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-blue-100 text-blue-800 border-blue-200 hover:bg-blue-200 transition-colors">
              Search: {filters.searchTerm}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-blue-900" 
                onClick={() => handleSearchChange('')}
              />
            </Badge>
          )}
          {filters.categoryId && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-purple-100 text-purple-800 border-purple-200 hover:bg-purple-200 transition-colors">
              Category: {getCategoryLabel(filters.categoryId)}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-purple-900" 
                onClick={() => handleCategoryChange('all')}
              />
            </Badge>
          )}
          {filters.level && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-orange-100 text-orange-800 border-orange-200 hover:bg-orange-200 transition-colors">
              Level: {getLevelLabel(filters.level)}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-orange-900" 
                onClick={() => handleLevelChange('all')}
              />
            </Badge>
          )}
          {(filters.isPublished !== undefined || filters.isFeatured !== undefined) && (
            <Badge variant="secondary" className="h-6 px-2 text-xs bg-green-100 text-green-800 border-green-200 hover:bg-green-200 transition-colors">
              Status: {getStatusLabel()}
              <X 
                className="h-3 w-3 ml-1 cursor-pointer hover:text-green-900" 
                onClick={() => handleStatusChange('all')}
              />
            </Badge>
          )}
        </div>
      )}

      {/* Compact Expanded Content */}
      {isExpanded && (
        <Card className="shadow-sm border-l-4 border-l-primary">
          <CardContent className="p-4 space-y-4">
            {/* Search Input */}
            <div className="space-y-1.5">
              <Label className="text-xs font-medium text-muted-foreground">Search Courses</Label>
              <div className="relative">
                <Search className="absolute left-2 top-2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Search by title, description..."
                  value={filters.searchTerm || ''}
                  onChange={(e) => handleSearchChange(e.target.value)}
                  className="h-8 text-sm pl-8"
                />
              </div>
            </div>

            {/* Category, Level and Status Filters */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
              <div className="space-y-1.5">
                <Label className="text-xs font-medium text-muted-foreground">Category</Label>
                <Select 
                  value={filters.categoryId?.toString() || "all"} 
                  onValueChange={handleCategoryChange}
                >
                  <SelectTrigger className="h-8 text-sm">
                    <SelectValue placeholder="All categories" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Categories</SelectItem>
                    <SelectItem value="1">Programming</SelectItem>
                    <SelectItem value="2">Data Science</SelectItem>
                    <SelectItem value="3">Design</SelectItem>
                    <SelectItem value="4">Business</SelectItem>
                    <SelectItem value="5">Marketing</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-1.5">
                <Label className="text-xs font-medium text-muted-foreground">Level</Label>
                <Select 
                  value={filters.level?.toString() || "all"} 
                  onValueChange={handleLevelChange}
                >
                  <SelectTrigger className="h-8 text-sm">
                    <SelectValue placeholder="All levels" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Levels</SelectItem>
                    <SelectItem value="1">Beginner</SelectItem>
                    <SelectItem value="2">Intermediate</SelectItem>
                    <SelectItem value="3">Advanced</SelectItem>
                    <SelectItem value="4">Expert</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-1.5">
                <Label className="text-xs font-medium text-muted-foreground">Status</Label>
                <Select value={getCurrentStatus()} onValueChange={handleStatusChange}>
                  <SelectTrigger className="h-8 text-sm">
                    <SelectValue placeholder="All statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Status</SelectItem>
                    <SelectItem value="published">Published</SelectItem>
                    <SelectItem value="draft">Draft</SelectItem>
                    <SelectItem value="featured">Featured</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            {/* Action Button */}
            <div className="flex gap-2 pt-2 border-t border-border/50">
              <Button variant="outline" onClick={resetFilters} size="sm" className="h-8">
                <RotateCcw className="h-3 w-3 mr-1" />
                Reset Filters
              </Button>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
