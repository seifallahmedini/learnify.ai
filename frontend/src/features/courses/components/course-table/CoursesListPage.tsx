import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader } from '@/shared/components/ui/card';
import { Input } from '@/shared/components/ui/input';
import { 
  Plus, 
  List, 
  Grid, 
  BookOpen, 
  Search, 
  Filter
} from 'lucide-react';
import { CreateCourseDialog } from '../dialogs';
import { CourseGridCard } from './CourseGridCard';
import { CourseTable } from './CourseTable';
import { BulkActionBar } from '../shared';
import { useCourseManagement, useSelectionManager } from '../../hooks';
import type { CourseSummary } from '../../types';

type ViewMode = 'list' | 'grid';

export function CoursesListPage() {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [viewMode, setViewMode] = useState<ViewMode>('list');

  const {
    courses,
    isLoading,
    error,
    totalCount,
    currentPage,
    totalPages,
    refreshCourses,
  } = useCourseManagement();

  // Filter courses based on search term
  const filteredCourses = courses.filter(course =>
    course.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    course.shortDescription.toLowerCase().includes(searchTerm.toLowerCase()) ||
    course.instructorName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Selection management
  const selection = useSelectionManager({
    items: filteredCourses,
    getItemId: (course) => course.id,
  });

  const handleViewCourse = (courseId: number) => {
    navigate(`/courses/${courseId}`);
  };

  const handleEditCourse = (course: CourseSummary) => {
    console.log('Edit course:', course.id);
    // TODO: Implement edit functionality
  };

  const handleDeleteCourse = (course: CourseSummary) => {
    console.log('Delete course:', course.id);
    // TODO: Implement delete functionality
  };

  const handleBulkEdit = () => {
    console.log('Bulk edit courses:', selection.selectedIds);
    // TODO: Implement bulk edit functionality
  };

  const handleBulkDelete = () => {
    console.log('Bulk delete courses:', selection.selectedIds);
    // TODO: Implement bulk delete functionality
  };

  // Handle select all for grid view
  const handleSelectAllGrid = () => {
    selection.handleSelectAll(true);
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        {/* Header Skeleton */}
        <div className="flex flex-col sm:flex-row gap-4 sm:items-center sm:justify-between">
          <div className="space-y-2">
            <div className="h-8 w-32 bg-muted rounded animate-pulse" />
            <div className="h-4 w-64 bg-muted rounded animate-pulse" />
          </div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-24 bg-muted rounded animate-pulse" />
            <div className="h-10 w-32 bg-muted rounded animate-pulse" />
          </div>
        </div>

        {/* Search and Filters Skeleton */}
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="h-10 flex-1 bg-muted rounded animate-pulse" />
          <div className="h-10 w-32 bg-muted rounded animate-pulse" />
        </div>

        {/* Cards Skeleton */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {Array.from({ length: 8 }).map((_, i) => (
            <Card key={i} className="overflow-hidden">
              <div className="aspect-video bg-muted animate-pulse" />
              <CardHeader className="space-y-3">
                <div className="h-6 bg-muted rounded animate-pulse" />
                <div className="space-y-2">
                  <div className="h-4 bg-muted rounded animate-pulse" />
                  <div className="h-4 w-3/4 bg-muted rounded animate-pulse" />
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex items-center gap-2">
                  <div className="h-5 w-16 bg-muted rounded animate-pulse" />
                  <div className="h-5 w-20 bg-muted rounded animate-pulse" />
                </div>
                <div className="flex items-center justify-between">
                  <div className="h-4 w-24 bg-muted rounded animate-pulse" />
                  <div className="h-6 w-16 bg-muted rounded animate-pulse" />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] space-y-6">
        <div className="text-center space-y-4">
          <div className="mx-auto h-16 w-16 rounded-full bg-destructive/10 flex items-center justify-center">
            <BookOpen className="h-8 w-8 text-destructive" />
          </div>
          <div className="space-y-2">
            <h3 className="text-xl font-semibold">Unable to Load Courses</h3>
            <p className="text-muted-foreground max-w-md">
              {error || 'We encountered an issue while loading your courses. Please try again.'}
            </p>
          </div>
        </div>
        <Button onClick={refreshCourses} variant="outline" className="gap-2">
          <BookOpen className="h-4 w-4" />
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row gap-4 sm:items-center sm:justify-between">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Courses</h1>
          <p className="text-muted-foreground">
            Discover and manage your course catalog • {totalCount} courses available
          </p>
        </div>
        <div className="flex items-center gap-3">
          {/* View Mode Toggle */}
          <div className="border border-input rounded-lg p-1 bg-background">
            <div className="flex">
              <Button
                variant={viewMode === 'list' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => setViewMode('list')}
                className="h-8 px-3 rounded-md"
              >
                <List className="h-4 w-4" />
              </Button>
              <Button
                variant={viewMode === 'grid' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => setViewMode('grid')}
                className="h-8 px-3 rounded-md"
              >
                <Grid className="h-4 w-4" />
              </Button>
            </div>
          </div>
          
          <CreateCourseDialog
            onCourseCreated={(_course) => {
              refreshCourses();
            }}
            trigger={
              <Button className="gap-2">
                <Plus className="h-4 w-4" />
                Create Course
              </Button>
            }
          />
        </div>
      </div>

      {/* Search and Filters */}
      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search courses, instructors, or descriptions..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button variant="outline" className="gap-2 sm:w-auto w-full">
          <Filter className="h-4 w-4" />
          Filters
        </Button>
      </div>

      {/* Selection Summary and Bulk Actions */}
      <BulkActionBar
        selectedCount={selection.selectedCount}
        totalCount={filteredCourses.length}
        onClearSelection={selection.clearSelection}
        onSelectAll={viewMode === 'grid' ? handleSelectAllGrid : undefined}
        onBulkEdit={handleBulkEdit}
        onBulkDelete={handleBulkDelete}
      />

      {/* Courses Content */}
      {filteredCourses.length === 0 ? (
        <Card className="border-dashed border-2 py-16">
          <CardContent className="text-center space-y-6">
            <div className="mx-auto h-16 w-16 rounded-full bg-muted flex items-center justify-center">
              <BookOpen className="h-8 w-8 text-muted-foreground" />
            </div>
            <div className="space-y-2">
              <h3 className="text-xl font-semibold">
                {searchTerm ? 'No courses found' : 'No courses yet'}
              </h3>
              <p className="text-muted-foreground max-w-md mx-auto">
                {searchTerm 
                  ? `No courses match "${searchTerm}". Try adjusting your search or browse all courses.`
                  : 'Get started by creating your first course to build your learning catalog.'
                }
              </p>
            </div>
            <div className="flex flex-col sm:flex-row gap-2 justify-center">
              {searchTerm ? (
                <Button variant="outline" onClick={() => setSearchTerm('')}>
                  Clear Search
                </Button>
              ) : null}
              <CreateCourseDialog
                onCourseCreated={(_course) => {
                  refreshCourses();
                }}
                trigger={
                  <Button className="gap-2">
                    <Plus className="h-4 w-4" />
                    Create Your First Course
                  </Button>
                }
              />
            </div>
          </CardContent>
        </Card>
      ) : (
        <>
          {/* Results Summary */}
          <div className="flex items-center justify-between text-sm text-muted-foreground">
            <span>
              Showing {filteredCourses.length} of {totalCount} courses
              {searchTerm && (
                <span className="ml-1">
                  for "<span className="text-foreground font-medium">{searchTerm}</span>"
                </span>
              )}
            </span>
          </div>

          {/* Course Grid/Table */}
          {viewMode === 'list' ? (
            <CourseTable 
              courses={filteredCourses}
              selectedCourses={selection.selectedIds}
              onSelectionChange={selection.handleBulkSelection}
              onView={(course) => handleViewCourse(course.id)}
              onEdit={(course) => handleEditCourse(course)}
              onDelete={(course) => handleDeleteCourse(course)}
            />
          ) : (
            /* Course Grid - Selection handled individually by each card */
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {filteredCourses.map((course) => (
                <CourseGridCard
                  key={course.id}
                  course={course}
                  isSelected={selection.isSelected(course.id)}
                  onSelectionChange={selection.handleItemSelection}
                  onView={() => handleViewCourse(course.id)}
                  onEdit={() => handleEditCourse(course)}
                  onDelete={() => handleDeleteCourse(course)}
                />
              ))}
            </div>
          )}
        </>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-center space-x-4 py-4">
          <div className="flex items-center space-x-2 text-sm text-muted-foreground">
            <span>Page {currentPage} of {totalPages}</span>
          </div>
        </div>
      )}
    </div>
  );
}