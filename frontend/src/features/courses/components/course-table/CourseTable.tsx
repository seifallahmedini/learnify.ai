import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/shared/components/ui/table';
import { Button } from '@/shared/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar';
import { Badge } from '@/shared/components/ui/badge';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/shared/components/ui/dropdown-menu';
import {
  MoreHorizontal,
  Edit,
  Trash2,
  Eye,
  Star,
  StarOff,
  Play,
  Pause,
  Users,
} from 'lucide-react';
import { useCourseUtils } from '../../hooks';
import {
  CourseLevelBadge,
  CourseStatusBadge,
  CourseFeatureBadge,
  CourseRating,
} from '../shared';
import type { CourseSummary } from '../../types';

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
  const { 
    formatPrice, 
    getCourseInitials,
  } = useCourseUtils();

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

  const renderPagination = () => {
    return (
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => currentPage > 1 && onPageChange(currentPage - 1)}
          disabled={currentPage <= 1}
        >
          Previous
        </Button>
        
        <span className="text-sm">
          Page {currentPage} of {totalPages}
        </span>
        
        <Button
          variant="outline"
          size="sm"
          onClick={() => currentPage < totalPages && onPageChange(currentPage + 1)}
          disabled={currentPage >= totalPages}
        >
          Next
        </Button>
      </div>
    );
  };

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
      {/* Bulk Actions */}
      {selectedCourses.length > 0 && (
        <div className="flex items-center gap-2 p-3 bg-muted rounded-lg">
          <span className="text-sm font-medium">
            {selectedCourses.length} course{selectedCourses.length > 1 ? 's' : ''} selected
          </span>
          <div className="flex gap-2 ml-auto">
            <Button size="sm" variant="outline">
              Bulk Edit
            </Button>
            <Button size="sm" variant="outline">
              Bulk Publish
            </Button>
            <Button size="sm" variant="outline">
              Bulk Feature
            </Button>
            <Button size="sm" variant="destructive">
              Bulk Delete
            </Button>
          </div>
        </div>
      )}

      {/* Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-12">
                <input
                  type="checkbox"
                  checked={isAllSelected}
                  onChange={(e) => handleSelectAll(e.target.checked)}
                  className="rounded border border-gray-300"
                />
              </TableHead>
              <TableHead>Course</TableHead>
              <TableHead>Instructor</TableHead>
              <TableHead>Level</TableHead>
              <TableHead>Price</TableHead>
              <TableHead>Students</TableHead>
              <TableHead>Rating</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="w-12">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {courses.map((course) => (
              <TableRow key={course.id}>
                <TableCell>
                  <div>Select course</div>
                  <input
                    type="checkbox"
                    checked={selectedCourses.includes(course.id)}
                    onChange={(e) => handleSelectCourse(course.id, e.target.checked)}
                    className="rounded border border-gray-300"
                  />
                </TableCell>
                
                <TableCell>
                  <div className="flex items-center gap-3">
                    <Avatar className="h-12 w-12">
                      <AvatarImage src={course.thumbnailUrl} alt={course.title} />
                      <AvatarFallback className="text-sm font-medium">
                        {getCourseInitials(course.title)}
                      </AvatarFallback>
                    </Avatar>
                    <div className="space-y-1">
                      <div className="font-medium line-clamp-1">{course.title}</div>
                      <div className="text-sm text-muted-foreground line-clamp-1">
                        {course.shortDescription}
                      </div>
                      <div className="flex items-center gap-2">
                        <Badge variant="outline" className="text-xs">
                          {course.categoryName}
                        </Badge>
                        <CourseFeatureBadge isFeatured={course.isFeatured} />
                      </div>
                    </div>
                  </div>
                </TableCell>
                
                <TableCell>
                  <div className="font-medium">{course.instructorName}</div>
                </TableCell>
                
                <TableCell>
                  <CourseLevelBadge level={course.level} />
                </TableCell>
                
                <TableCell>
                  <div className="space-y-1">
                    <div className="font-medium">
                      {course.isDiscounted ? (
                        <div className="space-y-1">
                          <div className="text-red-600 font-semibold">
                            {formatPrice(course.effectivePrice)}
                          </div>
                          <div className="text-sm text-muted-foreground line-through">
                            {formatPrice(course.price)}
                          </div>
                        </div>
                      ) : (
                        <div className="text-gray-900">
                          {formatPrice(course.price)}
                        </div>
                      )}
                    </div>
                  </div>
                </TableCell>
                
                <TableCell>
                  <div className="flex items-center gap-1 text-sm">
                    <Users className="h-4 w-4" />
                    {course.totalStudents.toLocaleString()}
                  </div>
                </TableCell>
                
                <TableCell>
                  <CourseRating 
                    rating={course.averageRating} 
                    showReviewCount={false}
                    size="sm"
                  />
                </TableCell>
                
                <TableCell>
                  <div className="space-y-1">
                    <CourseStatusBadge isPublished={course.isPublished} />
                  </div>
                </TableCell>
                
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <MoreHorizontal className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuLabel>Actions</DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      
                      {onViewCourse && (
                        <DropdownMenuItem onClick={() => onViewCourse(course)}>
                          <Eye className="mr-2 h-4 w-4" />
                          View Details
                        </DropdownMenuItem>
                      )}
                      
                      <DropdownMenuItem onClick={() => onEditCourse(course)}>
                        <Edit className="mr-2 h-4 w-4" />
                        Edit Course
                      </DropdownMenuItem>
                      
                      <DropdownMenuSeparator />
                      
                      {onTogglePublish && (
                        <DropdownMenuItem onClick={() => onTogglePublish(course)}>
                          {course.isPublished ? (
                            <>
                              <Pause className="mr-2 h-4 w-4" />
                              Unpublish
                            </>
                          ) : (
                            <>
                              <Play className="mr-2 h-4 w-4" />
                              Publish
                            </>
                          )}
                        </DropdownMenuItem>
                      )}
                      
                      {onToggleFeature && (
                        <DropdownMenuItem onClick={() => onToggleFeature(course)}>
                          {course.isFeatured ? (
                            <>
                              <StarOff className="mr-2 h-4 w-4" />
                              Unfeature
                            </>
                          ) : (
                            <>
                              <Star className="mr-2 h-4 w-4" />
                              Feature
                            </>
                          )}
                        </DropdownMenuItem>
                      )}
                      
                      <DropdownMenuSeparator />
                      
                      <DropdownMenuItem 
                        onClick={() => onDeleteCourse(course)}
                        className="text-red-600"
                      >
                        <Trash2 className="mr-2 h-4 w-4" />
                        Delete Course
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      <div className="flex items-center justify-between">
        <div className="text-sm text-muted-foreground">
          Showing {((currentPage - 1) * 10) + 1} to {Math.min(currentPage * 10, totalCount)} of {totalCount} courses
        </div>
        <div>
          {renderPagination()}
        </div>
      </div>
    </div>
  );
}
