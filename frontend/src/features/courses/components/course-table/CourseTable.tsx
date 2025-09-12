import { useState } from 'react';
import {
  Table,
  TableBody,
  TableHead,
  TableHeader,
  TableRow,
} from '@/shared/components/ui/table';
import { Button } from '@/shared/components/ui/button';
import {
  Edit,
  Star,
  Play,
  Trash2,
} from 'lucide-react';
import type { CourseSummary } from '../../types';
import { CourseTableRow } from './CourseTableRow';

interface CourseTableProps {
  courses: CourseSummary[];
  currentPage: number;
  totalPages: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onEditCourse: (course: CourseSummary) => void;
  onDeleteCourse: (course: CourseSummary) => void;
  onViewCourse?: (course: CourseSummary) => void;
  onTogglePublish?: (course: CourseSummary) => void;
  onToggleFeature?: (course: CourseSummary) => void;
}

export function CourseTable({
  courses,
  currentPage,
  totalPages,
  totalCount,
  onPageChange,
  onEditCourse,
  onDeleteCourse,
  onViewCourse,
  onTogglePublish,
  onToggleFeature,
}: CourseTableProps) {
  const [selectedCourses, setSelectedCourses] = useState<number[]>([]);

  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      setSelectedCourses(courses.map(course => course.id));
    } else {
      setSelectedCourses([]);
    }
  };

  const handleSelectCourse = (courseId: number, checked: boolean) => {
    if (checked) {
      setSelectedCourses(prev => [...prev, courseId]);
    } else {
      setSelectedCourses(prev => prev.filter(id => id !== courseId));
    }
  };

  const isAllSelected = courses.length > 0 && selectedCourses.length === courses.length;

  if (courses.length === 0) {
    return (
      <div className="border rounded-lg p-8">
        <div className="text-center space-y-2">
          <h3 className="text-lg font-medium">No courses found</h3>
          <p className="text-muted-foreground">
            No courses match your current filters. Try adjusting your search criteria.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Enhanced Bulk Actions */}
      {selectedCourses.length > 0 && (
        <div className="flex items-center justify-between p-4 bg-primary/5 border border-primary/20 rounded-lg border-l-4 border-l-primary">
          <div className="flex items-center gap-3">
            <div className="flex items-center justify-center w-8 h-8 bg-primary/10 rounded-full">
              <span className="text-sm font-semibold text-primary">{selectedCourses.length}</span>
            </div>
            <span className="text-sm font-medium">
              {selectedCourses.length} course{selectedCourses.length > 1 ? 's' : ''} selected
            </span>
          </div>
          <div className="flex gap-2">
            <Button size="sm" variant="outline" className="h-8">
              <Edit className="h-3 w-3 mr-1" />
              Edit
            </Button>
            <Button size="sm" variant="outline" className="h-8">
              <Play className="h-3 w-3 mr-1" />
              Publish
            </Button>
            <Button size="sm" variant="outline" className="h-8">
              <Star className="h-3 w-3 mr-1" />
              Feature
            </Button>
            <Button size="sm" variant="destructive" className="h-8">
              <Trash2 className="h-3 w-3 mr-1" />
              Delete
            </Button>
          </div>
        </div>
      )}

      {/* Enhanced Table */}
      <div className="border rounded-lg overflow-hidden bg-card shadow-sm">
        <Table>
          <TableHeader>
            <TableRow className="bg-muted/50">
              <TableHead className="w-12">
                <div className="flex items-center justify-center">
                  <input
                    type="checkbox"
                    checked={isAllSelected}
                    onChange={(e) => handleSelectAll(e.target.checked)}
                    className="rounded border border-input w-4 h-4 accent-primary"
                    aria-label="Select all courses"
                  />
                </div>
              </TableHead>
              <TableHead className="font-semibold">Course</TableHead>
              <TableHead className="font-semibold">Instructor</TableHead>
              <TableHead className="font-semibold">Level</TableHead>
              <TableHead className="font-semibold">Price</TableHead>
              <TableHead className="font-semibold">Students</TableHead>
              <TableHead className="font-semibold">Rating</TableHead>
              <TableHead className="font-semibold">Status</TableHead>
              <TableHead className="w-12 font-semibold">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {courses.map((course) => (
              <CourseTableRow
                key={course.id}
                course={course}
                selectedCourses={selectedCourses}
                onSelectCourse={handleSelectCourse}
                onEditCourse={onEditCourse}
                onDeleteCourse={onDeleteCourse}
                onViewCourse={onViewCourse}
                onTogglePublish={onTogglePublish}
                onToggleFeature={onToggleFeature}
              />
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Enhanced Pagination */}
      <div className="flex items-center justify-between py-4 px-2">
        <div className="text-sm text-muted-foreground bg-muted/30 px-3 py-1.5 rounded-md border">
          Showing <span className="font-medium text-foreground">{((currentPage - 1) * 10) + 1}</span> to{' '}
          <span className="font-medium text-foreground">{Math.min(currentPage * 10, totalCount)}</span> of{' '}
          <span className="font-medium text-foreground">{totalCount}</span> courses
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => currentPage > 1 && onPageChange(currentPage - 1)}
            disabled={currentPage <= 1}
            className="h-9"
          >
            Previous
          </Button>
          
          <div className="flex items-center gap-1">
            <span className="text-sm text-muted-foreground">Page</span>
            <div className="bg-primary text-primary-foreground px-2 py-1 rounded text-sm font-medium min-w-8 text-center">
              {currentPage}
            </div>
            <span className="text-sm text-muted-foreground">of {totalPages}</span>
          </div>
          
          <Button
            variant="outline"
            size="sm"
            onClick={() => currentPage < totalPages && onPageChange(currentPage + 1)}
            disabled={currentPage >= totalPages}
            className="h-9"
          >
            Next
          </Button>
        </div>
      </div>
    </div>
  );
}
