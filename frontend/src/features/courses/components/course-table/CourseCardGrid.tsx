import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/components/ui/card";
import { Button } from "@/shared/components/ui/button";
import { Badge } from "@/shared/components/ui/badge";
import { Avatar, AvatarFallback } from "@/shared/components/ui/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/shared/components/ui/dropdown-menu";
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
  DollarSign,
} from "lucide-react";
import { getCourseLevelLabel, getCourseLevelColor, formatCoursePrice } from "../../types";
import type { CourseSummary } from "../../types";
import { CourseFeatureBadge } from "../shared";

interface CourseCardGridProps {
  courses: CourseSummary[];
  loading?: boolean;
  totalCount: number;
  currentPage: number;
  totalPages: number;
  pageSize?: number;
  onEditCourse: (course: CourseSummary) => void;
  onDeleteCourse: (course: CourseSummary) => void;
  onViewCourse?: (course: CourseSummary) => void;
  onTogglePublish?: (course: CourseSummary) => void;
  onToggleFeature?: (course: CourseSummary) => void;
  onPageChange: (page: number) => void;
  onPageSizeChange?: (pageSize: number) => void;
  onCreateCourse?: () => void;
  onClearFilters?: () => void;
  hasFilters?: boolean;
}

export function CourseCardGrid({
  courses,
  loading,
  totalCount,
  currentPage,
  totalPages,
  onPageChange,
  onEditCourse,
  onDeleteCourse,
  onViewCourse,
  onTogglePublish,
  onToggleFeature,
}: CourseCardGridProps) {
  if (loading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {Array.from({ length: 8 }).map((_, i) => (
          <Card key={i} className="overflow-hidden">
            <div className="animate-pulse">
              <div className="w-full h-48 bg-gradient-to-r from-muted via-muted/50 to-muted"></div>
              <CardHeader className="pb-2">
                <div className="space-y-3">
                  <div className="h-5 bg-gradient-to-r from-muted via-muted/50 to-muted rounded-md"></div>
                  <div className="h-4 bg-gradient-to-r from-muted via-muted/50 to-muted rounded-md w-3/4"></div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  <div className="flex items-center gap-2">
                    <div className="h-6 w-6 bg-gradient-to-r from-muted via-muted/50 to-muted rounded-full"></div>
                    <div className="h-4 bg-gradient-to-r from-muted via-muted/50 to-muted rounded w-20"></div>
                  </div>
                  <div className="h-6 bg-gradient-to-r from-muted via-muted/50 to-muted rounded w-16"></div>
                  <div className="flex justify-between">
                    <div className="h-4 bg-gradient-to-r from-muted via-muted/50 to-muted rounded w-12"></div>
                    <div className="h-4 bg-gradient-to-r from-muted via-muted/50 to-muted rounded w-16"></div>
                  </div>
                </div>
              </CardContent>
            </div>
          </Card>
        ))}
      </div>
    );
  }

  if (courses.length === 0) {
    return (
      <Card className="border-2 border-dashed border-muted-foreground/25">
        <CardContent className="flex flex-col items-center justify-center py-16 px-6">
          <div className="text-center space-y-4">
            <div className="mx-auto h-16 w-16 bg-muted rounded-full flex items-center justify-center mb-4">
              <svg fill="none" viewBox="0 0 24 24" stroke="currentColor" className="h-8 w-8 text-muted-foreground">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.746 0 3.332.477 4.5 1.253v13C19.832 18.477 18.246 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
              </svg>
            </div>
            <div className="space-y-2">
              <h3 className="text-xl font-semibold text-foreground">No courses found</h3>
              <p className="text-sm text-muted-foreground max-w-md mx-auto leading-relaxed">
                No courses match your current filters. Try adjusting your search criteria or create a new course to get started.
              </p>
            </div>
            <Button 
              onClick={() => window.location.reload()} 
              variant="outline" 
              className="mt-4 hover:bg-primary hover:text-primary-foreground transition-colors"
            >
              Refresh Courses
            </Button>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Course Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {courses.map((course) => (
          <Card 
            key={course.id} 
            className="group hover:shadow-xl transition-all duration-300 border-border/50 hover:border-primary/30 overflow-hidden bg-card hover:bg-card/95"
          >
            <div className="relative">
              {/* Course Thumbnail */}
              <div className="relative w-full h-48 bg-gradient-to-br from-muted to-muted/70 overflow-hidden">
                {course.thumbnailUrl ? (
                  <img
                    src={course.thumbnailUrl}
                    alt={course.title}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                  />
                ) : (
                  <div className="w-full h-full bg-gradient-to-br from-primary/20 via-primary/10 to-secondary/20 flex items-center justify-center">
                    <div className="text-center">
                      <div className="w-12 h-12 bg-primary/20 rounded-full flex items-center justify-center mb-2 mx-auto">
                        <span className="text-primary text-lg font-bold">
                          {course.title.split(' ').map(word => word[0]).join('').toUpperCase().slice(0, 2)}
                        </span>
                      </div>
                      <span className="text-xs text-muted-foreground font-medium">Course Thumbnail</span>
                    </div>
                  </div>
                )}
                
                {/* Overlay Gradient */}
                <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-transparent"></div>
                
                {/* Feature Badge */}
                {course.isFeatured && (
                  <div className="absolute top-3 left-3">
                    <CourseFeatureBadge isFeatured={course.isFeatured} className="shadow-lg" />
                  </div>
                )}
                
                {/* Status Badge */}
                <div className="absolute top-3 right-3">
                  <Badge 
                    variant={course.isPublished ? "default" : "secondary"}
                    className={course.isPublished ? 
                      "bg-green-600 hover:bg-green-700 text-white shadow-lg" : 
                      "bg-muted-foreground/90 text-muted hover:bg-muted-foreground shadow-lg"
                    }
                  >
                    {course.isPublished ? "Published" : "Draft"}
                  </Badge>
                </div>

                {/* Actions Dropdown */}
                <div className="absolute bottom-3 right-3 opacity-0 group-hover:opacity-100 transition-all duration-200 transform translate-y-2 group-hover:translate-y-0">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button 
                        variant="secondary" 
                        size="sm" 
                        className="h-9 w-9 p-0 bg-background/90 hover:bg-background shadow-lg backdrop-blur-sm border border-border/50"
                      >
                        <MoreHorizontal className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-48 shadow-lg border border-border/50">
                      <DropdownMenuLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Actions</DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => onViewCourse?.(course)}
                        className="flex items-center gap-2 cursor-pointer hover:bg-primary/10 transition-colors"
                      >
                        <Eye className="h-4 w-4 text-blue-600" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem 
                        onClick={() => onEditCourse(course)}
                        className="flex items-center gap-2 cursor-pointer hover:bg-primary/10 transition-colors"
                      >
                        <Edit className="h-4 w-4 text-amber-600" />
                        Edit Course
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      {course.isPublished ? (
                        <DropdownMenuItem
                          onClick={() => onTogglePublish?.(course)}
                          className="flex items-center gap-2 cursor-pointer text-orange-600 hover:bg-orange-50 transition-colors"
                        >
                          <Pause className="h-4 w-4" />
                          Unpublish
                        </DropdownMenuItem>
                      ) : (
                        <DropdownMenuItem
                          onClick={() => onTogglePublish?.(course)}
                          className="flex items-center gap-2 cursor-pointer text-green-600 hover:bg-green-50 transition-colors"
                        >
                          <Play className="h-4 w-4" />
                          Publish
                        </DropdownMenuItem>
                      )}
                      {course.isFeatured ? (
                        <DropdownMenuItem
                          onClick={() => onToggleFeature?.(course)}
                          className="flex items-center gap-2 cursor-pointer text-orange-600 hover:bg-orange-50 transition-colors"
                        >
                          <StarOff className="h-4 w-4" />
                          Remove Feature
                        </DropdownMenuItem>
                      ) : (
                        <DropdownMenuItem
                          onClick={() => onToggleFeature?.(course)}
                          className="flex items-center gap-2 cursor-pointer text-yellow-600 hover:bg-yellow-50 transition-colors"
                        >
                          <Star className="h-4 w-4" />
                          Feature Course
                        </DropdownMenuItem>
                      )}
                      <DropdownMenuSeparator />
                      <DropdownMenuItem
                        onClick={() => onDeleteCourse(course)}
                        className="flex items-center gap-2 cursor-pointer text-red-600 hover:bg-red-50 transition-colors"
                      >
                        <Trash2 className="h-4 w-4" />
                        Delete Course
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              </div>
            </div>

            <CardHeader className="pb-3">
              {/* Course Info */}
              <div className="space-y-2">
                <CardTitle 
                  className="text-lg line-clamp-2 group-hover:text-primary transition-colors cursor-pointer font-semibold leading-tight"
                  onClick={() => onViewCourse?.(course)}
                  role="button"
                  tabIndex={0}
                  onKeyDown={(e) => e.key === 'Enter' && onViewCourse?.(course)}
                >
                  {course.title}
                </CardTitle>
                <CardDescription className="line-clamp-2 text-sm leading-relaxed">
                  {course.shortDescription}
                </CardDescription>
              </div>
            </CardHeader>

            <CardContent className="space-y-4 pt-0">
              {/* Instructor */}
              <div className="flex items-center space-x-3">
                <Avatar className="h-7 w-7 border border-border shadow-sm">
                  <AvatarFallback className="text-xs font-medium bg-gradient-to-br from-primary/10 to-primary/20 text-primary">
                    {course.instructorName.split(' ').map(n => n[0]).join('').toUpperCase()}
                  </AvatarFallback>
                </Avatar>
                <span className="text-sm text-muted-foreground font-medium">{course.instructorName}</span>
              </div>

              {/* Level Badge */}
              <div className="flex items-center justify-between">
                <Badge 
                  className={`${getCourseLevelColor(course.level)} font-medium px-3 py-1`} 
                  variant="outline"
                >
                  {getCourseLevelLabel(course.level)}
                </Badge>
                <div className="text-xs text-muted-foreground font-medium px-2 py-1 bg-muted/50 rounded-md">
                  {course.categoryName}
                </div>
              </div>

              {/* Stats */}
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-2">
                  <div className="flex items-center justify-center w-6 h-6 bg-blue-100 rounded-full">
                    <Users className="h-3.5 w-3.5 text-blue-600" />
                  </div>
                  <span className="text-sm font-semibold text-foreground">{course.totalStudents.toLocaleString()}</span>
                  <span className="text-xs text-muted-foreground">students</span>
                </div>
                <div className="flex items-center space-x-1.5">
                  <Star className="h-4 w-4 fill-amber-400 text-amber-400" />
                  <span className="text-sm font-semibold text-foreground">{course.averageRating.toFixed(1)}</span>
                </div>
              </div>

              {/* Price */}
              <div className="flex items-center justify-between pt-2 border-t border-border/50">
                <div className="flex items-center space-x-2">
                  <DollarSign className="h-4 w-4 text-green-600" />
                  <div className="flex items-center space-x-2">
                    {course.isDiscounted && course.discountPrice ? (
                      <div className="flex items-center gap-2">
                        <span className="font-bold text-lg text-green-600">
                          {formatCoursePrice(course.discountPrice)}
                        </span>
                        <span className="text-sm text-muted-foreground line-through">
                          {formatCoursePrice(course.price)}
                        </span>
                        <Badge variant="destructive" className="text-xs px-1.5 py-0.5 font-semibold">
                          Sale
                        </Badge>
                      </div>
                    ) : (
                      <span className="font-bold text-lg text-foreground">
                        {formatCoursePrice(course.price)}
                      </span>
                    )}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Enhanced Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between p-4 bg-muted/30 rounded-lg border border-border/50">
          <div className="text-sm text-muted-foreground font-medium">
            Showing <span className="font-semibold text-foreground">{((currentPage - 1) * 12) + 1}</span> to{' '}
            <span className="font-semibold text-foreground">{Math.min(currentPage * 12, totalCount)}</span> of{' '}
            <span className="font-semibold text-foreground">{totalCount}</span> courses
          </div>
          <div className="flex items-center space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => onPageChange(Math.max(1, currentPage - 1))}
              disabled={currentPage === 1}
              className="hover:bg-primary hover:text-primary-foreground transition-colors disabled:opacity-50"
            >
              Previous
            </Button>
            <div className="flex items-center space-x-1">
              <span className="text-sm font-medium px-3 py-1.5 bg-primary text-primary-foreground rounded-md">
                {currentPage}
              </span>
              <span className="text-sm text-muted-foreground">of {totalPages}</span>
            </div>
            <Button
              variant="outline"
              size="sm"
              onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
              disabled={currentPage === totalPages}
              className="hover:bg-primary hover:text-primary-foreground transition-colors disabled:opacity-50"
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}