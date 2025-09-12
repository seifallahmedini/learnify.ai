import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Plus, List, Grid } from 'lucide-react';
import { CourseFilters } from './CourseFilters';
import { CourseTable } from './CourseTable';
import { CourseCardGrid } from './CourseCardGrid';
import { CreateCourseDialog } from '../dialogs/CreateCourseDialog';
import { DeleteCourseDialog } from '../dialogs/DeleteCourseDialog';
import { useCourseManagement } from '../../hooks';
import type { CourseSummary } from '../../types';

type ViewMode = 'list' | 'grid';

export function CoursesListPage() {
  const navigate = useNavigate();
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  
  // Edit course state
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [selectedCourse, setSelectedCourse] = useState<CourseSummary | null>(null);
  const [isLoadingCourse, setIsLoadingCourse] = useState(false);
  
  // Delete course state
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [courseToDelete, setCourseToDelete] = useState<CourseSummary | null>(null);

  const {
    courses,
    error,
    totalCount,
    currentPage,
    totalPages,
    filters,
    updateFilters,
    goToPage,
    refreshCourses,
  } = useCourseManagement();

  const clearFilters = () => {
    updateFilters({ page: 1, pageSize: filters.pageSize });
  };

  const handleTogglePublish = (course: CourseSummary) => {
    console.log('Toggle published for course:', course.id);
    refreshCourses();
  };

  const handleToggleFeature = (course: CourseSummary) => {
    console.log('Toggle featured for course:', course.id);
    refreshCourses();
  };

  const handleCreateDialogClose = (open: boolean) => {
    setShowCreateDialog(open);
  };

  const handleViewCourse = (courseId: number) => {
    navigate(`/courses/${courseId}`);
  };

  const handleEditCourse = async (course: CourseSummary) => {
    try {
      setIsLoadingCourse(true);
      // This would load the full course data
      setSelectedCourse(course);
      setShowEditDialog(true);
    } catch (error) {
      console.error('Failed to load course for editing:', error);
    } finally {
      setIsLoadingCourse(false);
    }
  };

  const handleDeleteCourseClick = async (course: CourseSummary) => {
    try {
      setIsLoadingCourse(true);
      setCourseToDelete(course);
      setShowDeleteDialog(true);
    } catch (error) {
      console.error('Failed to load course for deletion:', error);
    } finally {
      setIsLoadingCourse(false);
    }
  };

  const handleCourseDeleted = async (_deletedCourseId: number) => {
    // Refresh the courses list after deletion
    refreshCourses();
  };

  const handleDeleteDialogClose = (open: boolean) => {
    setShowDeleteDialog(open);
    if (!open) {
      setCourseToDelete(null);
    }
  };

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center h-64 space-y-4">
        <div className="text-red-600 text-center">
          <h3 className="text-lg font-medium">Error Loading Courses</h3>
          <p className="text-sm">{error}</p>
        </div>
        <Button onClick={refreshCourses} variant="outline">
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <div className="flex flex-1 flex-col gap-4 p-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Courses</h1>
          <p className="text-muted-foreground">
            Manage and organize your course catalog
          </p>
        </div>
        <div className="flex items-center gap-3">
          {/* View Mode Toggle */}
          <div className="flex border border-input rounded-md">
            <Button
              variant={viewMode === 'list' ? 'default' : 'ghost'}
              size="sm"
              onClick={() => setViewMode('list')}
              className="rounded-r-none"
            >
              <List className="h-4 w-4" />
            </Button>
            <Button
              variant={viewMode === 'grid' ? 'default' : 'ghost'}
              size="sm"
              onClick={() => setViewMode('grid')}
              className="rounded-l-none"
            >
              <Grid className="h-4 w-4" />
            </Button>
          </div>
          
          <Button onClick={() => setShowCreateDialog(true)}>
            <Plus className="mr-2 h-4 w-4" />
            Add Course
          </Button>
        </div>
      </div>

      {/* Filters */}
      <CourseFilters
        filters={filters}
        onFiltersChange={(newFilters) => updateFilters({ ...newFilters, page: 1 })}
        onClearFilters={clearFilters}
      />

      {/* Courses Content */}
      {viewMode === 'list' ? (
        <CourseTable
          courses={courses}
          totalCount={totalCount}
          currentPage={currentPage}
          totalPages={totalPages}
          onTogglePublish={handleTogglePublish}
          onToggleFeature={handleToggleFeature}
          onDeleteCourse={handleDeleteCourseClick}
          onViewCourse={(course) => handleViewCourse(course.id)}
          onEditCourse={handleEditCourse}
          onPageChange={goToPage}
        />
      ) : (
        <CourseCardGrid
          courses={courses}
          totalCount={totalCount}
          currentPage={currentPage}
          totalPages={totalPages}
          onTogglePublish={handleTogglePublish}
          onToggleFeature={handleToggleFeature}
          onDeleteCourse={handleDeleteCourseClick}
          onViewCourse={(course) => handleViewCourse(course.id)}
          onEditCourse={handleEditCourse}
          onPageChange={goToPage}
        />
      )}

      {/* Create Course Dialog */}
      <CreateCourseDialog
        open={showCreateDialog}
        onOpenChange={handleCreateDialogClose}
        onCourseCreated={(_course) => {
          setShowCreateDialog(false);
          refreshCourses();
        }}
      />

      {/* Edit Course Dialog - Placeholder */}
      {selectedCourse && showEditDialog && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg max-w-md w-full mx-4">
            <h3 className="text-lg font-semibold mb-4">Edit Course</h3>
            <p className="text-sm text-muted-foreground mb-4">
              Editing: {selectedCourse.title}
            </p>
            <div className="flex gap-2 justify-end">
              <Button variant="outline" onClick={() => setShowEditDialog(false)}>
                Cancel
              </Button>
              <Button onClick={() => {
                setShowEditDialog(false);
                refreshCourses();
              }}>
                Save Changes
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Course Dialog */}
      <DeleteCourseDialog
        course={courseToDelete}
        open={showDeleteDialog}
        onOpenChange={handleDeleteDialogClose}
        onCourseDeleted={handleCourseDeleted}
      />
      
      {/* Loading state for edit course */}
      {isLoadingCourse && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-4 rounded-lg">
            Loading course data...
          </div>
        </div>
      )}
    </div>
  );
}
