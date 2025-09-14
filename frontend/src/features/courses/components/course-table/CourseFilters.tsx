import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Label } from '@/shared/components/ui/label';
import { Badge } from '@/shared/components/ui/badge';
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from '@/shared/components/ui/select';
import {
  Sheet,
  SheetClose,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from '@/shared/components/ui/sheet';
import { Separator } from '@/shared/components/ui/separator';
import { 
  Filter,
  X,
  RotateCcw,
  Search,
  Star,
  Clock,
  DollarSign,
  Users,
  BookOpen,
  ChevronDown,
  ChevronUp
} from 'lucide-react';
import { CourseLevel } from '../../types';
import type { UIFilters } from './filterMapping';
import { defaultUIFilters, getActiveFiltersCount } from './filterMapping';

// Re-export for convenience
export type { UIFilters };

interface CourseFiltersProps {
  filters: UIFilters;
  onFiltersChange: (filters: UIFilters) => void;
  categories: Array<{ id: string; name: string }>;
  className?: string;
}

export function CourseFilters({ 
  filters, 
  onFiltersChange, 
  categories,
  className = '' 
}: CourseFiltersProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [expandedSections, setExpandedSections] = useState({
    basic: true,
    pricing: false,
    advanced: false
  });

  const toggleSection = (section: keyof typeof expandedSections) => {
    setExpandedSections(prev => ({
      ...prev,
      [section]: !prev[section]
    }));
  };

  const handleFilterChange = (key: keyof UIFilters, value: any) => {
    onFiltersChange({
      ...filters,
      [key]: value,
    });
  };

  const handleReset = () => {
    onFiltersChange(defaultUIFilters);
  };

  const activeFiltersCount = getActiveFiltersCount(filters);

  const getLevelLabel = (level: CourseLevel | 'all') => {
    if (level === 'all') return 'All Levels';
    const levelNames = {
      [CourseLevel.Beginner]: 'Beginner',
      [CourseLevel.Intermediate]: 'Intermediate', 
      [CourseLevel.Advanced]: 'Advanced',
      [CourseLevel.Expert]: 'Expert'
    };
    return levelNames[level] || 'Unknown';
  };

  return (
    <div className={className}>
      <Sheet open={isOpen} onOpenChange={setIsOpen}>
        <SheetTrigger asChild>
          <Button 
            variant="outline" 
            className={`gap-2 relative transition-all duration-200 hover:shadow-sm ${
              activeFiltersCount > 0 
                ? 'border-primary bg-primary/5 text-primary hover:bg-primary/10' 
                : 'hover:border-muted-foreground/50'
            }`}
            aria-label={`Open filters${activeFiltersCount > 0 ? ` (${activeFiltersCount} active)` : ''}`}
          >
            <Filter className="h-4 w-4" />
            <span className="hidden sm:inline">Filters</span>
            <span className="sm:hidden">Filter</span>
            {activeFiltersCount > 0 && (
              <Badge 
                variant="secondary" 
                className="ml-1 h-5 w-5 p-0 text-xs bg-primary text-primary-foreground border-0 shadow-sm"
                aria-hidden="true"
              >
                {activeFiltersCount}
              </Badge>
            )}
          </Button>
        </SheetTrigger>
        <SheetContent 
          className="w-[50vw] min-w-[400px] overflow-y-auto"
          aria-describedby="filter-description"
        >
          <SheetHeader className="space-y-3 pb-6">
            <SheetTitle className="text-xl font-semibold">Filter Courses</SheetTitle>
            <SheetDescription id="filter-description" className="text-muted-foreground">
              Refine your course search with advanced filtering options. 
              {activeFiltersCount > 0 && ` ${activeFiltersCount} filter${activeFiltersCount > 1 ? 's' : ''} currently active.`}
            </SheetDescription>
          </SheetHeader>

          <div className="py-2 space-y-8">
            {/* Quick Search - Always Visible */}
            <div className="space-y-3">
              <Label className="text-sm font-medium flex items-center gap-2">
                <Search className="h-4 w-4 text-primary" />
                Quick Search
              </Label>
              <Input
                placeholder="Search courses, instructors, descriptions..."
                value={filters.searchTerm}
                onChange={(e) => handleFilterChange('searchTerm', e.target.value)}
                className="h-11"
                aria-label="Search courses"
              />
            </div>

            <Separator />

            {/* Basic Filters Section */}
            <div className="space-y-4">
              <button
                onClick={() => toggleSection('basic')}
                className="flex items-center justify-between w-full p-3 -m-3 rounded-lg hover:bg-muted/50 transition-colors group"
                aria-expanded={expandedSections.basic}
                aria-controls="basic-filters"
              >
                <h3 className="text-base font-semibold flex items-center gap-2">
                  <BookOpen className="h-5 w-5 text-primary" />
                  Basic Filters
                </h3>
                {expandedSections.basic ? (
                  <ChevronUp className="h-5 w-5 text-muted-foreground group-hover:text-foreground transition-colors" />
                ) : (
                  <ChevronDown className="h-5 w-5 text-muted-foreground group-hover:text-foreground transition-colors" />
                )}
              </button>
              
              {expandedSections.basic && (
                <div id="basic-filters" className="space-y-6 pl-7 animate-in slide-in-from-top-2 duration-200">
                  {/* Status */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium">Publication Status</Label>
                    <Select 
                      value={filters.status} 
                      onValueChange={(value) => handleFilterChange('status', value)}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Courses</SelectItem>
                        <SelectItem value="published">Published Only</SelectItem>
                        <SelectItem value="draft">Drafts Only</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Level */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium">Course Level</Label>
                    <Select 
                      value={filters.level.toString()} 
                      onValueChange={(value) => handleFilterChange('level', value === 'all' ? 'all' : parseInt(value))}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Levels</SelectItem>
                        <SelectItem value={CourseLevel.Beginner.toString()}>Beginner</SelectItem>
                        <SelectItem value={CourseLevel.Intermediate.toString()}>Intermediate</SelectItem>
                        <SelectItem value={CourseLevel.Advanced.toString()}>Advanced</SelectItem>
                        <SelectItem value={CourseLevel.Expert.toString()}>Expert</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Category */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium">Category</Label>
                    <Select 
                      value={filters.category} 
                      onValueChange={(value) => handleFilterChange('category', value)}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Categories</SelectItem>
                        {categories.map((category) => (
                          <SelectItem key={category.id} value={category.id}>
                            {category.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Featured */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium flex items-center gap-2">
                      <Star className="h-4 w-4 text-amber-500" />
                      Featured Status
                    </Label>
                    <Select 
                      value={filters.featured} 
                      onValueChange={(value) => handleFilterChange('featured', value)}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Courses</SelectItem>
                        <SelectItem value="featured">Featured Only</SelectItem>
                        <SelectItem value="regular">Regular Only</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              )}
            </div>

            <Separator />

            {/* Pricing Filters Section */}
            <div className="space-y-4">
              <button
                onClick={() => toggleSection('pricing')}
                className="flex items-center justify-between w-full p-3 -m-3 rounded-lg hover:bg-muted/50 transition-colors group"
                aria-expanded={expandedSections.pricing}
                aria-controls="pricing-filters"
              >
                <h3 className="text-base font-semibold flex items-center gap-2">
                  <DollarSign className="h-5 w-5 text-green-600" />
                  Pricing Options
                </h3>
                {expandedSections.pricing ? (
                  <ChevronUp className="h-5 w-5 text-muted-foreground group-hover:text-foreground transition-colors" />
                ) : (
                  <ChevronDown className="h-5 w-5 text-muted-foreground group-hover:text-foreground transition-colors" />
                )}
              </button>
              
              {expandedSections.pricing && (
                <div id="pricing-filters" className="space-y-6 pl-7 animate-in slide-in-from-top-2 duration-200">
                  {/* Price Range */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium">Price Range</Label>
                    <Select 
                      value={filters.priceRange} 
                      onValueChange={(value) => handleFilterChange('priceRange', value)}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Prices</SelectItem>
                        <SelectItem value="free">Free Courses</SelectItem>
                        <SelectItem value="paid">Paid Courses</SelectItem>
                        <SelectItem value="discounted">Discounted</SelectItem>
                        <SelectItem value="custom">Custom Range</SelectItem>
                      </SelectContent>
                    </Select>
                    
                    {/* Custom Price Range Inputs */}
                    {filters.priceRange === 'custom' && (
                      <div className="grid grid-cols-2 gap-4 mt-4 p-4 bg-muted/30 rounded-lg border">
                        <div>
                          <Label className="text-xs text-muted-foreground mb-2 block">Min Price</Label>
                          <Input
                            type="number"
                            placeholder="0"
                            value={filters.minPrice?.toString() || ''}
                            onChange={(e) => handleFilterChange('minPrice', e.target.value ? parseFloat(e.target.value) : null)}
                            className="text-sm h-10"
                            aria-label="Minimum price"
                          />
                        </div>
                        <div>
                          <Label className="text-xs text-muted-foreground mb-2 block">Max Price</Label>
                          <Input
                            type="number"
                            placeholder="999"
                            value={filters.maxPrice?.toString() || ''}
                            onChange={(e) => handleFilterChange('maxPrice', e.target.value ? parseFloat(e.target.value) : null)}
                            className="text-sm h-10"
                            aria-label="Maximum price"
                          />
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              )}
            </div>

            <Separator />

            {/* Advanced Filters Section */}
            <div className="space-y-4">
              <button
                onClick={() => toggleSection('advanced')}
                className="flex items-center justify-between w-full p-3 -m-3 rounded-lg hover:bg-muted/50 transition-colors group"
                aria-expanded={expandedSections.advanced}
                aria-controls="advanced-filters"
              >
                <h3 className="text-base font-semibold flex items-center gap-2">
                  <Star className="h-5 w-5 text-blue-600" />
                  Advanced Options
                </h3>
                {expandedSections.advanced ? (
                  <ChevronUp className="h-5 w-5 text-muted-foreground group-hover:text-foreground transition-colors" />
                ) : (
                  <ChevronDown className="h-5 w-5 text-muted-foreground group-hover:text-foreground transition-colors" />
                )}
              </button>
              
              {expandedSections.advanced && (
                <div id="advanced-filters" className="space-y-6 pl-7 animate-in slide-in-from-top-2 duration-200">
                  {/* Minimum Rating */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium flex items-center gap-2">
                      <Star className="h-4 w-4 text-amber-500" />
                      Minimum Rating
                    </Label>
                    <Select 
                      value={filters.minRating.toString()} 
                      onValueChange={(value) => handleFilterChange('minRating', parseFloat(value))}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="0">Any Rating</SelectItem>
                        <SelectItem value="3">3+ Stars</SelectItem>
                        <SelectItem value="4">4+ Stars</SelectItem>
                        <SelectItem value="4.5">4.5+ Stars</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Max Duration */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium flex items-center gap-2">
                      <Clock className="h-4 w-4 text-blue-500" />
                      Maximum Duration
                    </Label>
                    <Select 
                      value={filters.maxDuration?.toString() || 'all'} 
                      onValueChange={(value) => handleFilterChange('maxDuration', value === 'all' ? null : parseInt(value))}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">Any Duration</SelectItem>
                        <SelectItem value="5">Under 5 hours</SelectItem>
                        <SelectItem value="10">Under 10 hours</SelectItem>
                        <SelectItem value="20">Under 20 hours</SelectItem>
                        <SelectItem value="50">Under 50 hours</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Min Students */}
                  <div className="space-y-3">
                    <Label className="text-sm font-medium flex items-center gap-2">
                      <Users className="h-4 w-4 text-purple-500" />
                      Minimum Students
                    </Label>
                    <Select 
                      value={filters.minStudents?.toString() || 'all'} 
                      onValueChange={(value) => handleFilterChange('minStudents', value === 'all' ? null : parseInt(value))}
                    >
                      <SelectTrigger className="h-11">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">Any Number</SelectItem>
                        <SelectItem value="10">10+ Students</SelectItem>
                        <SelectItem value="50">50+ Students</SelectItem>
                        <SelectItem value="100">100+ Students</SelectItem>
                        <SelectItem value="500">500+ Students</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              )}
            </div>
          </div>
          <SheetFooter className="gap-3 pt-6 border-t">
            <Button 
              variant="outline" 
              onClick={handleReset}
              className="gap-2 flex-1 h-11"
              disabled={activeFiltersCount === 0}
            >
              <RotateCcw className="h-4 w-4" />
              Clear All
            </Button>
            <SheetClose asChild>
              <Button className="flex-1 h-11 shadow-sm">
                Apply Filters
                {activeFiltersCount > 0 && (
                  <Badge variant="secondary" className="ml-2 bg-white/20 text-white border-0">
                    {activeFiltersCount}
                  </Badge>
                )}
              </Button>
            </SheetClose>
          </SheetFooter>
        </SheetContent>
      </Sheet>

      {/* Enhanced Active Filters Display */}
      {activeFiltersCount > 0 && (
        <div className="mt-4 space-y-3">
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium text-muted-foreground">
              Active Filters ({activeFiltersCount})
            </span>
            <Button
              variant="ghost"
              size="sm"
              onClick={handleReset}
              className="h-8 px-2 text-xs text-muted-foreground hover:text-foreground"
            >
              Clear all
            </Button>
          </div>
          <div className="flex flex-wrap gap-2">
            {filters.searchTerm && (
              <Badge 
                variant="secondary" 
                className="gap-1 pr-1 max-w-[200px] truncate hover:bg-secondary/80 transition-colors"
                title={`Search: ${filters.searchTerm}`}
              >
                <Search className="h-3 w-3" />
                <span className="truncate">{filters.searchTerm}</span>
                <button
                  onClick={() => handleFilterChange('searchTerm', '')}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove search filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.status !== 'all' && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                <BookOpen className="h-3 w-3" />
                {filters.status}
                <button
                  onClick={() => handleFilterChange('status', 'all')}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove status filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.level !== 'all' && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                Level: {getLevelLabel(filters.level)}
                <button
                  onClick={() => handleFilterChange('level', 'all')}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove level filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.category !== 'all' && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                {categories.find(c => c.id === filters.category)?.name || filters.category}
                <button
                  onClick={() => handleFilterChange('category', 'all')}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove category filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.featured !== 'all' && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                <Star className="h-3 w-3" />
                {filters.featured}
                <button
                  onClick={() => handleFilterChange('featured', 'all')}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove featured filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.priceRange !== 'all' && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                <DollarSign className="h-3 w-3" />
                {filters.priceRange}
                <button
                  onClick={() => handleFilterChange('priceRange', 'all')}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove price range filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.minPrice !== null && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                Min: ${filters.minPrice}
                <button
                  onClick={() => handleFilterChange('minPrice', null)}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove minimum price filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.maxPrice !== null && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                Max: ${filters.maxPrice}
                <button
                  onClick={() => handleFilterChange('maxPrice', null)}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove maximum price filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.minRating > 0 && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                <Star className="h-3 w-3" />
                {filters.minRating}+ stars
                <button
                  onClick={() => handleFilterChange('minRating', 0)}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove rating filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.maxDuration !== null && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                <Clock className="h-3 w-3" />
                Under {filters.maxDuration}h
                <button
                  onClick={() => handleFilterChange('maxDuration', null)}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove duration filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
            {filters.minStudents !== null && (
              <Badge variant="secondary" className="gap-1 pr-1 hover:bg-secondary/80 transition-colors">
                <Users className="h-3 w-3" />
                {filters.minStudents}+ students
                <button
                  onClick={() => handleFilterChange('minStudents', null)}
                  className="ml-1 hover:bg-destructive/20 rounded-full p-0.5 transition-colors"
                  aria-label="Remove student count filter"
                >
                  <X className="h-3 w-3" />
                </button>
              </Badge>
            )}
          </div>
        </div>
      )}
    </div>
  );
}