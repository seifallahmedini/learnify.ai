import { Button } from '@/shared/components/ui/button';
import { Badge } from '@/shared/components/ui/badge';
import { Checkbox } from '@/shared/components/ui/checkbox';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/shared/components/ui/table';
import { 
  Users,
  MoreHorizontal,
  Eye,
  Edit,
  Trash2
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/shared/components/ui/dropdown-menu';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/shared/components/ui/tooltip';
import { CourseLevelBadge, CourseRating, CourseFeatureBadge } from '../shared';
import type { CourseSummary } from '../../types';

interface CourseTableProps {
  courses: CourseSummary[];
  selectedCourses?: number[];
  onSelectionChange?: (selectedIds: number[]) => void;
  onView: (course: CourseSummary) => void;
  onEdit: (course: CourseSummary) => void;
  onDelete: (course: CourseSummary) => void;
}

export function CourseTable({ 
  courses, 
  selectedCourses = [], 
  onSelectionChange,
  onView, 
  onEdit, 
  onDelete 
}: CourseTableProps) {
  // Handle individual row selection
  const handleRowSelection = (courseId: number, checked: boolean) => {
    if (!onSelectionChange) return;
    
    const newSelection = checked
      ? [...selectedCourses, courseId]
      : selectedCourses.filter(id => id !== courseId);
    
    onSelectionChange(newSelection);
  };

  // Handle select all functionality
  const handleSelectAll = (checked: boolean) => {
    if (!onSelectionChange) return;
    
    const newSelection = checked ? courses.map(course => course.id) : [];
    onSelectionChange(newSelection);
  };

  const isAllSelected = selectedCourses.length === courses.length && courses.length > 0;
  const isIndeterminate = selectedCourses.length > 0 && selectedCourses.length < courses.length;
  return (
    <TooltipProvider>
      <div className="rounded-md border">
        <Table>
        <TableHeader>
          <TableRow>
            {onSelectionChange && (
              <TableHead className="w-[50px]">
                <Checkbox
                  checked={isAllSelected}
                  onCheckedChange={handleSelectAll}
                  aria-label="Select all courses"
                  className="data-[state=indeterminate]:bg-primary data-[state=indeterminate]:text-primary-foreground"
                  {...(isIndeterminate && { 'data-state': 'indeterminate' })}
                />
              </TableHead>
            )}
            <TableHead className="w-[300px]">Course</TableHead>
            <TableHead>Instructor</TableHead>
            <TableHead className="w-[140px]">Category</TableHead>
            <TableHead className="w-[130px]">Level</TableHead>
            <TableHead className="text-center">Featured</TableHead>
            <TableHead className="text-center">Students</TableHead>
            <TableHead className="text-center">Rating</TableHead>
            <TableHead>Status</TableHead>
            <TableHead className="text-right">Price</TableHead>
            <TableHead className="w-[70px]"></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {courses.map((course) => (
            <TableRow key={course.id} className="hover:bg-muted/50">
              {onSelectionChange && (
                <TableCell className="py-4">
                  <Checkbox
                    checked={selectedCourses.includes(course.id)}
                    onCheckedChange={(checked) => handleRowSelection(course.id, checked as boolean)}
                    aria-label={`Select course ${course.title}`}
                  />
                </TableCell>
              )}
              <TableCell className="py-4">
                <div className="space-y-1 max-w-[300px]">
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <div className="cursor-help">
                        <h3 
                          className="font-medium truncate hover:text-primary transition-colors cursor-pointer"
                          onClick={() => onView(course)}
                        >
                          {course.title}
                        </h3>
                        <p className="text-sm text-muted-foreground truncate">
                          {course.shortDescription}
                        </p>
                      </div>
                    </TooltipTrigger>
                    <TooltipContent className="max-w-sm p-4 bg-background border shadow-lg">
                      <div className="space-y-3">
                        {/* Course Title Section */}
                        <div className="space-y-1">
                          <div className="flex items-center justify-between">
                            <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                              Course Title
                            </span>
                            <div className="flex items-center gap-1">
                              <CourseLevelBadge level={course.level} />
                              {course.isFeatured && (
                                <CourseFeatureBadge isFeatured={course.isFeatured} size="sm" />
                              )}
                            </div>
                          </div>
                          <h4 className="font-semibold text-foreground leading-tight">
                            {course.title}
                          </h4>
                        </div>
                        
                        {/* Description Section */}
                        <div className="space-y-1">
                          <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                            Description
                          </span>
                          <p className="text-sm text-muted-foreground leading-relaxed">
                            {course.shortDescription}
                          </p>
                        </div>
                        
                        {/* Course Details */}
                        <div className="pt-2 border-t border-border">
                          <div className="grid grid-cols-2 gap-3 text-xs">
                            <div className="space-y-1">
                              <span className="text-muted-foreground">Instructor</span>
                              <p className="font-medium text-foreground">{course.instructorName}</p>
                            </div>
                            <div className="space-y-1">
                              <span className="text-muted-foreground">Category</span>
                              <p className="font-medium text-foreground">{course.categoryName}</p>
                            </div>
                            <div className="space-y-1">
                              <span className="text-muted-foreground">Students</span>
                              <p className="font-medium text-foreground">{course.totalStudents}</p>
                            </div>
                            <div className="space-y-1">
                              <span className="text-muted-foreground">Price</span>
                              {course.discountPrice && course.discountPrice < course.price ? (
                                <div className="space-y-1">
                                  <div className="flex items-center gap-2">
                                    <span className="text-xs text-muted-foreground line-through">
                                      ${course.price.toFixed(2)}
                                    </span>
                                    <span className="inline-flex items-center rounded-md bg-destructive/10 px-1.5 py-0.5 text-xs font-medium text-destructive">
                                      -{Math.round(((course.price - course.discountPrice) / course.price) * 100)}%
                                    </span>
                                  </div>
                                  <p className="font-semibold text-primary">${course.discountPrice.toFixed(2)}</p>
                                </div>
                              ) : (
                                <p className="font-semibold text-primary">
                                  {course.price === 0 ? 'Free' : `$${course.price.toFixed(2)}`}
                                </p>
                              )}
                            </div>
                          </div>
                        </div>
                      </div>
                    </TooltipContent>
                  </Tooltip>
                </div>
              </TableCell>
              <TableCell>
                <span className="text-sm">{course.instructorName}</span>
              </TableCell>
              <TableCell>
                <Badge variant="secondary" className="text-xs">
                  {course.categoryName}
                </Badge>
              </TableCell>
              <TableCell>
                <CourseLevelBadge level={course.level} />
              </TableCell>
              <TableCell className="text-center">
                <CourseFeatureBadge isFeatured={course.isFeatured} size="sm" />
              </TableCell>
              <TableCell className="text-center">
                <div className="flex items-center justify-center gap-1">
                  <Users className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm">{course.totalStudents}</span>
                </div>
              </TableCell>
              <TableCell className="text-center">
                <div className="flex items-center justify-center gap-1">
                  <CourseRating rating={course.averageRating || 0} size="sm" />
                  <span className="text-xs text-muted-foreground">
                    ({course.totalReviews})
                  </span>
                </div>
              </TableCell>
              <TableCell>
                <Badge variant={course.isPublished ? 'default' : 'secondary'}>
                  {course.isPublished ? 'Published' : 'Draft'}
                </Badge>
              </TableCell>
              <TableCell className="text-right">
                <div className="flex flex-col items-end space-y-0.5 min-w-[100px]">
                  {course.discountPrice && course.discountPrice < course.price ? (
                    <>
                      <div className="flex items-center gap-1.5">
                        <span className="text-xs text-muted-foreground/70 line-through">
                          ${course.price.toFixed(2)}
                        </span>
                        <div className="relative">
                          <span className="inline-flex items-center rounded-full bg-gradient-to-r from-red-50 to-red-100 px-1.5 py-0.5 text-xs font-bold text-red-700 ring-1 ring-red-200 hover:scale-105 transition-transform duration-200">
                            -{Math.round(((course.price - course.discountPrice) / course.price) * 100)}%
                          </span>
                          <div className="absolute -top-0.5 -right-0.5 h-1.5 w-1.5 bg-red-500 rounded-full animate-pulse"></div>
                        </div>
                      </div>
                      <div className="relative group">
                        <span className="font-bold text-lg text-primary group-hover:text-primary/80 transition-colors duration-200">
                          ${course.discountPrice.toFixed(2)}
                        </span>
                        <div className="absolute -bottom-0.5 left-0 w-0 h-0.5 bg-primary group-hover:w-full transition-all duration-300"></div>
                      </div>
                      <span className="text-xs text-green-600 font-medium bg-green-50 px-1.5 py-0.5 rounded-full">
                        Save ${(course.price - course.discountPrice).toFixed(2)}
                      </span>
                    </>
                  ) : (
                    <div className="relative group">
                      {course.price === 0 ? (
                        <div className="flex flex-col items-end space-y-0.5">
                          <span className="font-bold text-lg bg-gradient-to-r from-green-600 to-emerald-600 bg-clip-text text-transparent group-hover:from-green-500 group-hover:to-emerald-500 transition-all duration-300">
                            FREE
                          </span>
                          <span className="text-xs text-green-600 font-medium bg-green-50 px-1.5 py-0.5 rounded-full">
                            No cost
                          </span>
                        </div>
                      ) : (
                        <>
                          <span className="font-bold text-lg text-primary group-hover:text-primary/80 transition-colors duration-200">
                            ${course.price.toFixed(2)}
                          </span>
                          <div className="absolute -bottom-0.5 left-0 w-0 h-0.5 bg-primary group-hover:w-full transition-all duration-300"></div>
                        </>
                      )}
                    </div>
                  )}
                </div>
              </TableCell>
              <TableCell>
                <DropdownMenu modal={false}>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                      <MoreHorizontal className="h-4 w-4" />
                      <span className="sr-only">Open menu</span>
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem onClick={() => onView(course)}>
                      <Eye className="mr-2 h-4 w-4" />
                      View Details
                    </DropdownMenuItem>
                    <DropdownMenuItem 
                      onClick={(e) => {
                        e.preventDefault();
                        onEdit(course);
                      }}
                    >
                      <Edit className="mr-2 h-4 w-4" />
                      Edit Course
                    </DropdownMenuItem>
                    <DropdownMenuItem 
                      onClick={() => onDelete(course)} 
                      className="text-destructive"
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
    </TooltipProvider>
  );
}