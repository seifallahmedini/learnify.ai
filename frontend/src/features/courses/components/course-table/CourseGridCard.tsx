import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { 
  BookOpen, 
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
import { CourseLevelBadge, CourseRating, CourseFeatureBadge } from '../shared';
import type { CourseSummary } from '../../types';

interface CourseGridCardProps {
  course: CourseSummary;
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

export function CourseGridCard({ course, onView, onEdit, onDelete }: CourseGridCardProps) {
  return (
    <Card className="group hover:shadow-md transition-all duration-200 overflow-hidden bg-card">
      {/* Course Thumbnail */}
      <div className="aspect-video bg-gradient-to-br from-primary/10 to-primary/5 flex items-center justify-center relative overflow-hidden">
        <BookOpen className="h-12 w-12 text-primary/40" />
        <div className="absolute top-3 left-3">
          <Badge variant={course.isPublished ? 'default' : 'secondary'} className="text-xs">
            {course.isPublished ? 'Published' : 'Draft'}
          </Badge>
        </div>
        <div className="absolute top-3 right-3 flex gap-2">
          <CourseLevelBadge level={course.level} />
          <CourseFeatureBadge isFeatured={course.isFeatured} size="sm" />
        </div>
        <div className="absolute inset-0 bg-black/0 group-hover:bg-black/10 transition-all duration-200 flex items-center justify-center opacity-0 group-hover:opacity-100">
          <Button 
            size="sm" 
            variant="secondary" 
            onClick={onView}
            className="backdrop-blur-sm"
          >
            <Eye className="mr-2 h-4 w-4" />
            View Course
          </Button>
        </div>
      </div>

      <CardHeader className="space-y-3 pb-3">
        <div className="space-y-2">
          <h3 
            className="font-semibold line-clamp-2 group-hover:text-primary transition-colors cursor-pointer"
            onClick={onView}
          >
            {course.title}
          </h3>
          <p className="text-sm text-muted-foreground line-clamp-2">
            {course.shortDescription}
          </p>
        </div>
        
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">{course.instructorName}</span>
          <Badge variant="secondary" className="text-xs">
            {course.categoryName}
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="space-y-4 pt-0">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <CourseRating rating={course.averageRating || 0} size="sm" showValue />
            <span className="text-xs text-muted-foreground">({course.totalReviews})</span>
          </div>
          <div className="flex items-center gap-1 text-sm text-muted-foreground">
            <Users className="h-4 w-4" />
            <span>{course.totalStudents}</span>
          </div>
        </div>

        <div className="flex items-center justify-between pt-2 border-t">
          <div className="text-xl font-bold text-primary">
            ${course.effectivePrice}
          </div>
          
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="outline" size="sm" className="h-8 w-8 p-0">
                <MoreHorizontal className="h-4 w-4" />
                <span className="sr-only">Course actions</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={onView}>
                <Eye className="mr-2 h-4 w-4" />
                View Details
              </DropdownMenuItem>
              <DropdownMenuItem onClick={onEdit}>
                <Edit className="mr-2 h-4 w-4" />
                Edit Course
              </DropdownMenuItem>
              <DropdownMenuItem onClick={onDelete} className="text-destructive">
                <Trash2 className="mr-2 h-4 w-4" />
                Delete Course
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </CardContent>
    </Card>
  );
}