import { useState } from 'react';
import { Plus, Search, Filter, Download } from 'lucide-react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Badge } from '@/shared/components/ui/badge';
import { useCourseManagement } from '../../hooks';
import { CourseTable } from './CourseTable';
import { CourseFilters } from './CourseFilters';
import { CreateCourseDialog } from '../dialogs/CreateCourseDialog';
import { DeleteCourseDialog } from '../dialogs/DeleteCourseDialog';
import type { CourseFilterRequest, CourseSummary, Course } from '../../types';

export function CoursesListPage() {
  const [selectedCourse, setSelectedCourse] = useState<CourseSummary | null>(null);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [quickSearch, setQuickSearch] = useState('');
  const [showFilters, setShowFilters] = useState(false);

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

  // const { formatPrice, formatDuration } = useCourseUtils();

  const handleSearch = (searchTerm: string) => {
    setQuickSearch(searchTerm);
    updateFilters({ ...filters, searchTerm, page: 1 });
  };

  const handleFilterChange = (newFilters: CourseFilterRequest) => {
    updateFilters({ ...newFilters, page: 1 });
    setShowFilters(false);
  };

  const handleEditCourse = (course: CourseSummary) => {
    setSelectedCourse(course);
    // Navigate to edit page or open edit dialog
    console.log('Edit course:', course);
  };

  const handleDeleteCourse = (course: CourseSummary) => {
    setSelectedCourse(course);
    setShowDeleteDialog(true);
  };

  const handleCourseCreated = (_newCourse: Course) => {
    setShowCreateDialog(false);
    refreshCourses();
  };

  const handleCourseDeleted = () => {
    setShowDeleteDialog(false);
    setSelectedCourse(null);
    refreshCourses();
  };

  // Table-specific handlers
  const handleTogglePublished = (course: CourseSummary) => {
    // This would call the API to toggle published status
    console.log('Toggle published for course:', course.id);
    refreshCourses();
  };

  const handleToggleFeatured = (course: CourseSummary) => {
    // This would call the API to toggle featured status
    console.log('Toggle featured for course:', course.id);
    refreshCourses();
  };

  const handleExportCourses = async () => {
    try {
      // This would be implemented with the courses API
      console.log('Export courses with filters:', filters);
    } catch (error) {
      console.error('Failed to export courses:', error);
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
        <div className="flex gap-2">
          <Button 
            variant="outline" 
            onClick={handleExportCourses}
            className="flex items-center gap-2"
          >
            <Download className="h-4 w-4" />
            Export
          </Button>
          <Button onClick={() => setShowCreateDialog(true)}>
            <Plus className="mr-2 h-4 w-4" />
            Add Course
          </Button>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-lg border shadow-sm p-6">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Search courses by title, description, or instructor..."
              value={quickSearch}
              onChange={(e) => handleSearch(e.target.value)}
              className="pl-10"
            />
          </div>
          <Button
            variant="outline"
            onClick={() => setShowFilters(!showFilters)}
            className="flex items-center gap-2"
          >
            <Filter className="h-4 w-4" />
            Filters
            {Object.keys(filters).filter(key => 
              key !== 'page' && key !== 'pageSize' && filters[key as keyof CourseFilterRequest]
            ).length > 0 && (
              <Badge variant="secondary" className="ml-1 h-5 px-1 text-xs">
                {Object.keys(filters).filter(key => 
                  key !== 'page' && key !== 'pageSize' && filters[key as keyof CourseFilterRequest]
                ).length}
              </Badge>
            )}
          </Button>
        </div>

        {/* Filters Panel */}
        {showFilters && (
          <div className="mt-6 pt-6 border-t">
            <CourseFilters
              filters={filters}
              onFiltersChange={handleFilterChange}
              onClearFilters={() => handleFilterChange({})}
            />
          </div>
        )}
      </div>

      {/* Course Table */}
      <CourseTable
        courses={courses}
        currentPage={currentPage}
        totalPages={totalPages}
        totalCount={totalCount}
        onPageChange={goToPage}
        onEditCourse={handleEditCourse}
        onDeleteCourse={handleDeleteCourse}
        onTogglePublish={handleTogglePublished}
        onToggleFeature={handleToggleFeatured}
      />

      {/* Dialogs */}
      <CreateCourseDialog
        open={showCreateDialog}
        onOpenChange={setShowCreateDialog}
        onCourseCreated={handleCourseCreated}
      />

      <DeleteCourseDialog
        course={selectedCourse}
        open={showDeleteDialog}
        onOpenChange={setShowDeleteDialog}
        onCourseDeleted={handleCourseDeleted}
      />
    </div>
  );
}
