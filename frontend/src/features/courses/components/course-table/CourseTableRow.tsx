import { TableCell, TableRow } from "@/shared/components/ui/table"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/shared/components/ui/dropdown-menu"
import { Button } from "@/shared/components/ui/button"
import { Badge } from "@/shared/components/ui/badge"
import { 
  MoreHorizontal, 
  Eye, 
  Edit, 
  Play, 
  Pause, 
  Star, 
  StarOff, 
  Trash2, 
  Users 
} from "lucide-react"
import type { CourseSummary } from "../../types"
import { CourseLevelBadge, CourseStatusBadge, CourseRating, CourseFeatureBadge } from "../shared"
import { useCourseUtils } from "../../hooks"

interface CourseTableRowProps {
  course: CourseSummary
  selectedCourses: number[]
  onSelectCourse: (courseId: number, checked: boolean) => void
  onEditCourse: (course: CourseSummary) => void
  onDeleteCourse: (course: CourseSummary) => void
  onViewCourse?: (course: CourseSummary) => void
  onTogglePublish?: (course: CourseSummary) => void
  onToggleFeature?: (course: CourseSummary) => void
}

export function CourseTableRow({ 
  course, 
  selectedCourses,
  onSelectCourse,
  onEditCourse, 
  onDeleteCourse, 
  onViewCourse, 
  onTogglePublish, 
  onToggleFeature 
}: CourseTableRowProps) {
  const { formatPrice } = useCourseUtils()
  const isSelected = selectedCourses.includes(course.id)

  return (
    <TableRow 
      className={`
        group hover:bg-muted/50 transition-all duration-200 border-b border-border/40
        ${isSelected ? 'bg-primary/5 border-primary/20' : ''}
      `}
    >
      <TableCell className="w-10 py-2">
        <div className="flex items-center justify-center">
          <label className="relative inline-flex items-center cursor-pointer">
            <input
              type="checkbox"
              checked={isSelected}
              onChange={(e) => onSelectCourse(course.id, e.target.checked)}
              className="sr-only peer"
              aria-label={`Select ${course.title}`}
            />
            <div className={`
              relative w-4 h-4 rounded border-2 transition-all duration-200
              ${isSelected 
                ? 'bg-primary border-primary' 
                : 'border-input hover:border-primary/50 peer-focus:ring-2 peer-focus:ring-primary/20'
              }
            `}>
              {isSelected && (
                <svg className="absolute inset-0 w-3 h-3 text-primary-foreground m-auto" fill="currentColor" viewBox="0 0 16 16">
                  <path d="M13.854 3.646a.5.5 0 0 1 0 .708l-7 7a.5.5 0 0 1-.708 0l-3.5-3.5a.5.5 0 1 1 .708-.708L6.5 10.293l6.646-6.647a.5.5 0 0 1 .708 0z"/>
                </svg>
              )}
            </div>
          </label>
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <div className="space-y-2 min-w-0 flex-1 max-w-xs">
          <div className="flex items-center gap-2">
            <div 
              className="font-semibold text-base text-foreground hover:text-primary transition-colors cursor-pointer group-hover:text-primary flex-1 min-w-0"
              onClick={() => onViewCourse?.(course)}
              role="button"
              tabIndex={0}
              onKeyDown={(e) => e.key === 'Enter' && onViewCourse?.(course)}
              title={course.title}
            >
              <span className="block truncate max-w-[200px]">
                {course.title}
              </span>
            </div>
            <CourseFeatureBadge isFeatured={course.isFeatured} />
          </div>
          <p 
            className="text-xs text-muted-foreground leading-relaxed"
            title={course.shortDescription}
          >
            <span className="block truncate max-w-[250px]">
              {course.shortDescription}
            </span>
          </p>
          <div className="flex items-center gap-1">
            <Badge 
              variant="secondary" 
              className="text-xs px-2 py-0 font-medium bg-primary/10 text-primary hover:bg-primary/20 transition-colors border border-primary/20 truncate max-w-[120px]"
              title={course.categoryName}
            >
              {course.categoryName}
            </Badge>
          </div>
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <div className="flex items-center">
          <div className="font-semibold text-sm text-foreground">{course.instructorName}</div>
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <CourseLevelBadge level={course.level} />
      </TableCell>
      
      <TableCell className="py-2">
        <div className="space-y-1">
          {course.isDiscounted ? (
            <div className="space-y-1">
              <div className="flex items-center gap-1">
                <span className="text-base font-bold text-green-600">
                  {formatPrice(course.effectivePrice)}
                </span>
                <Badge variant="destructive" className="text-xs px-1 py-0 font-semibold">
                  Sale
                </Badge>
              </div>
              <div className="text-xs text-muted-foreground line-through">
                {formatPrice(course.price)}
              </div>
            </div>
          ) : (
            <div className="text-base font-bold text-foreground">
              {formatPrice(course.price)}
            </div>
          )}
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <div className="flex items-center gap-2">
          <div className="flex items-center justify-center w-6 h-6 bg-gradient-to-r from-blue-100 to-blue-200 rounded-full">
            <Users className="h-3 w-3 text-blue-600" />
          </div>
          <div className="space-y-0">
            <div className="font-bold text-sm text-foreground">{course.totalStudents.toLocaleString()}</div>
            <div className="text-xs text-muted-foreground">enrolled</div>
          </div>
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <div className="flex items-center gap-1">
          <CourseRating 
            rating={course.averageRating} 
            showReviewCount={false}
            size="sm"
          />
          <div className="flex flex-col">
            <span className="text-sm font-semibold text-foreground">{course.averageRating}</span>
            <span className="text-xs text-muted-foreground">rating</span>
          </div>
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <div className="space-y-1">
          <CourseStatusBadge isPublished={course.isPublished} />
          {course.isPublished && (
            <div className="flex items-center gap-1">
              <div className="w-1.5 h-1.5 bg-green-500 rounded-full animate-pulse"></div>
              <span className="text-xs text-green-600 font-medium">Live</span>
            </div>
          )}
        </div>
      </TableCell>
      
      <TableCell className="py-2">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-6 w-6 p-0 hover:bg-muted transition-colors">
              <MoreHorizontal className="h-4 w-4" />
              <span className="sr-only">Open menu</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-44">
            <DropdownMenuLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">
              Actions
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            
            {onViewCourse && (
              <DropdownMenuItem 
                onClick={() => onViewCourse(course)}
                className="flex items-center gap-2 cursor-pointer text-sm"
              >
                <Eye className="h-3 w-3 text-blue-600" />
                <span>View</span>
              </DropdownMenuItem>
            )}
            
            <DropdownMenuItem 
              onClick={() => onEditCourse(course)}
              className="flex items-center gap-2 cursor-pointer text-sm"
            >
              <Edit className="h-3 w-3 text-amber-600" />
              <span>Edit</span>
            </DropdownMenuItem>
            
            <DropdownMenuSeparator />
            
            {onTogglePublish && (
              <DropdownMenuItem 
                onClick={() => onTogglePublish(course)}
                className="flex items-center gap-2 cursor-pointer text-sm"
              >
                {course.isPublished ? (
                  <>
                    <Pause className="h-3 w-3 text-orange-600" />
                    <span>Unpublish</span>
                  </>
                ) : (
                  <>
                    <Play className="h-3 w-3 text-green-600" />
                    <span>Publish</span>
                  </>
                )}
              </DropdownMenuItem>
            )}
            
            {onToggleFeature && (
              <DropdownMenuItem 
                onClick={() => onToggleFeature(course)}
                className="flex items-center gap-2 cursor-pointer text-sm"
              >
                {course.isFeatured ? (
                  <>
                    <StarOff className="h-3 w-3 text-gray-600" />
                    <span>Unfeature</span>
                  </>
                ) : (
                  <>
                    <Star className="h-3 w-3 text-yellow-600" />
                    <span>Feature</span>
                  </>
                )}
              </DropdownMenuItem>
            )}
            
            <DropdownMenuSeparator />
            
            <DropdownMenuItem 
              onClick={() => onDeleteCourse(course)}
              className="flex items-center gap-2 cursor-pointer text-sm text-red-600 focus:text-red-600 focus:bg-red-50"
            >
              <Trash2 className="h-3 w-3" />
              <span>Delete</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </TableCell>
    </TableRow>
  )
}
