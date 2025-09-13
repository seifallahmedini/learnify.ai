import { Button } from '@/shared/components/ui/button';
import { Card, CardContent } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { 
  Users,
  Clock,
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
import { CourseLevelBadge, CourseRating } from '../shared';
import type { CourseSummary } from '../../types';

interface CourseListCardProps {
  course: CourseSummary;
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

export function CourseListCard({ course, onView, onEdit, onDelete }: CourseListCardProps) {
  return (
    <Card className="hover:shadow-sm transition-all duration-200 border-l-4 border-l-primary/20">
      <CardContent className="p-6">
        <div className="flex items-start justify-between gap-4">
          <div className="flex-1 space-y-3">
            <div className="space-y-2">
              <div className="flex items-start gap-3">
                <h3 
                  className="text-lg font-semibold line-clamp-1 hover:text-primary transition-colors cursor-pointer"
                  onClick={onView}
                >
                  {course.title}
                </h3>
                <div className="flex items-center gap-2">
                  <Badge variant={course.isPublished ? 'default' : 'secondary'}>
                    {course.isPublished ? 'Published' : 'Draft'}
                  </Badge>
                  <CourseLevelBadge level={course.level} />
                </div>
              </div>
              <p className="text-muted-foreground line-clamp-2 text-sm">
                {course.shortDescription}
              </p>
            </div>

            <div className="flex items-center gap-6 text-sm text-muted-foreground">
              <div className="flex items-center gap-1">
                <Users className="h-4 w-4" />
                <span>{course.totalStudents} students</span>
              </div>
              <div className="flex items-center gap-1">
                <Clock className="h-4 w-4" />
                <span>{course.instructorName}</span>
              </div>
              <div className="flex items-center gap-2">
                <CourseRating rating={course.averageRating || 0} size="sm" showValue />
                <span className="text-xs">({course.totalReviews} reviews)</span>
              </div>
            </div>
          </div>

          <div className="flex flex-col items-end gap-3">
            <div className="text-right">
              <div className="text-2xl font-bold text-primary">
                ${course.effectivePrice}
              </div>
              <div className="text-xs text-muted-foreground">
                {course.categoryName}
              </div>
            </div>

            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="outline" size="sm" className="h-8 w-8 p-0">
                  <MoreHorizontal className="h-4 w-4" />
                  <span className="sr-only">Open menu</span>
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
        </div>
      </CardContent>
    </Card>
  );
}