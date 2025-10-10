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
  Search
} from 'lucide-react';
import { CreateCourseDialog, EditCourseDialog } from '../dialogs';
import { CourseGridCard } from './CourseGridCard';
import { CourseTable } from './CourseTable';
import { BulkActionBar } from '../shared';
import { useCourseManagement, useSelectionManager } from '../../hooks';
import { coursesApi } from '../../services';
import type { CourseSummary, Course } from '../../types';

type ViewMode = 'list' | 'grid';

export function CoursesListPage() {
  const navigate = useNavigate();
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  
  // Edit course state
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [editingCourse, setEditingCourse] = useState<Course | null>(null);

  const {
    courses,
    isLoading,
    isSearching,
    error,
    totalCount,
    currentPage,
    totalPages,
    searchTerm,
    refreshCourses,
    handleSearchChange,
    clearSearch
  } = useCourseManagement();

  // Selection management (use courses directly since they're already filtered by API)
  const selection = useSelectionManager({
    items: courses,
    getItemId: (course: CourseSummary) => course.id,
  });

  const handleViewCourse = (courseId: number) => {
    navigate(`/courses/${courseId}`);
  };

  const handleEditCourse = async (course: CourseSummary) => {
    try {
      console.log('handleEditCourse called with:', course.title);
      
      // Fetch the complete course details from the API
      console.log('Fetching complete course details for ID:', course.id);
      const fullCourseData = await coursesApi.getCourseById(course.id);
      
      console.log('Retrieved full course data:', fullCourseData);
      console.log('Setting editing course and showing dialog');
      setEditingCourse(fullCourseData);
      setShowEditDialog(true);
      
    } catch (error) {
      console.error('Failed to fetch course details:', error);
      // Fallback to using the summary data if API call fails
      const fallbackCourse: Course = {
        ...course,
        description: course.shortDescription, // Use shortDescription as fallback
        thumbnailUrl: course.thumbnailUrl || '',
        language: 'English', // Default fallback
        categoryId: 1, // Default fallback - should ideally come from API
        instructorId: 1, // Default fallback - should ideally come from API
        maxStudents: undefined, // Not available in summary
        prerequisites: '', // Default empty string
        learningObjectives: '', // Default empty string
        updatedAt: course.createdAt, // Use createdAt as fallback
      };
      console.log('Using fallback course data due to API error');
      setEditingCourse(fallbackCourse);
      setShowEditDialog(true);
    }
  };

  const handleEditDialogClose = (open: boolean) => {
    setShowEditDialog(open);
    if (!open) {
      setEditingCourse(null);
    }
  };

  const handleCourseUpdate = async (updatedCourse: Course) => {
    try {
      console.log('Updating course with data:', updatedCourse);
      
      // Call the API service to update the course
      const result = await coursesApi.updateCourse(updatedCourse.id, {
        title: updatedCourse.title,
        shortDescription: updatedCourse.shortDescription,
        description: updatedCourse.description,
        price: updatedCourse.price,
        discountPrice: updatedCourse.discountPrice,
        level: updatedCourse.level,
        durationHours: updatedCourse.durationHours,
        maxStudents: updatedCourse.maxStudents,
        prerequisites: updatedCourse.prerequisites,
        learningObjectives: updatedCourse.learningObjectives,
        isPublished: updatedCourse.isPublished,
        isFeatured: updatedCourse.isFeatured,
      });
      
      console.log('Course updated successfully:', result);
      
      // Refresh the courses list to show the updated data
      await refreshCourses();
      
      // Close the dialog
      setShowEditDialog(false);
      setEditingCourse(null);
      
    } catch (error) {
      console.error('Failed to update course:', error);
      // The error will be handled by the EditCourseDialog's form hook
      throw error;
    }
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
            onChange={(e) => handleSearchChange(e.target.value)}
            className="pl-10 pr-10"
          />
          {/* Search loading indicator - doesn't interfere with typing */}
          {isSearching && (
            <div className="absolute right-3 top-1/2 -translate-y-1/2">
              <div className="h-4 w-4 animate-spin rounded-full border-2 border-primary border-t-transparent" />
            </div>
          )}
        </div>
      </div>

      {/* Selection Summary and Bulk Actions */}
      <BulkActionBar
        selectedCount={selection.selectedCount}
        totalCount={courses.length}
        onClearSelection={selection.clearSelection}
        onSelectAll={viewMode === 'grid' ? handleSelectAllGrid : undefined}
        onBulkEdit={handleBulkEdit}
        onBulkDelete={handleBulkDelete}
      />

      {/* Courses Content */}
      {courses.length === 0 ? (
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
                <Button variant="outline" onClick={clearSearch}>
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
              Showing {courses.length} of {totalCount} courses
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
              courses={courses}
              selectedCourses={selection.selectedIds}
              onSelectionChange={selection.handleBulkSelection}
              onView={(course) => handleViewCourse(course.id)}
              onEdit={(course) => handleEditCourse(course)}
              onDelete={(course) => handleDeleteCourse(course)}
            />
          ) : (
            /* Course Grid - Selection handled individually by each card */
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {courses.map((course: CourseSummary) => (
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
      
      {/* Edit Course Dialog */}
      {editingCourse && (
        <EditCourseDialog
          course={editingCourse}
          open={showEditDialog}
          onOpenChange={handleEditDialogClose}
          onCourseUpdated={handleCourseUpdate}
        />
      )}
    </div>
  );
}