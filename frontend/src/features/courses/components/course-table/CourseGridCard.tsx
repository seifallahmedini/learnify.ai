import React from 'react';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Checkbox } from '@/shared/components/ui/checkbox';
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
  isSelected?: boolean;
  onSelectionChange?: (courseId: number, checked: boolean) => void;
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

export function CourseGridCard({ 
  course, 
  isSelected = false, 
  onSelectionChange,
  onView, 
  onEdit, 
  onDelete 
}: CourseGridCardProps) {
  const handleCardClick = (e: React.MouseEvent) => {
    // Prevent selection when clicking on interactive elements
    if ((e.target as HTMLElement).closest('button, [role="button"], a, input')) {
      return;
    }
    
    if (onSelectionChange) {
      onSelectionChange(course.id, !isSelected);
    }
  };

  return (
    <Card 
      className={`group relative cursor-pointer transition-all duration-300 ease-out overflow-hidden ${
        isSelected 
          ? 'ring-2 ring-primary shadow-lg scale-[1.02] bg-primary/5 border-primary/20' 
          : 'hover:shadow-lg hover:scale-[1.01] hover:-translate-y-1 bg-card border-border'
      }`}
      onClick={handleCardClick}
    >
      {/* Selection Overlay */}
      {isSelected && (
        <div className="absolute inset-0 bg-primary/5 pointer-events-none z-10" />
      )}

      {/* Course Thumbnail */}
      <div className="aspect-video bg-gradient-to-br from-primary/10 via-primary/5 to-accent/10 flex items-center justify-center relative overflow-hidden">
        {/* Background Pattern */}
        <div className="absolute inset-0 opacity-5">
          <div className="absolute top-4 left-4 w-8 h-8 border border-primary/20 rounded-full" />
          <div className="absolute bottom-6 right-6 w-12 h-12 border border-primary/15 rounded-lg rotate-12" />
          <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-16 h-16 border border-primary/10 rounded-full" />
        </div>

        <BookOpen className="h-12 w-12 text-primary/40 relative z-5" />
        
        {/* Top Left: Selection & Status */}
        <div className="absolute top-3 left-3 flex items-center gap-2 z-20">
          {onSelectionChange && (
            <div className={`transition-all duration-200 ${
              isSelected 
                ? 'bg-primary text-primary-foreground shadow-md scale-110' 
                : 'bg-background/95 hover:bg-background border'
            } backdrop-blur-sm rounded-md p-1.5 shadow-sm`}>
              <Checkbox
                checked={isSelected}
                onCheckedChange={(checked) => onSelectionChange(course.id, checked as boolean)}
                aria-label={`Select course ${course.title}`}
                className={`h-4 w-4 border-2 ${
                  isSelected 
                    ? 'border-primary-foreground data-[state=checked]:bg-primary-foreground data-[state=checked]:text-primary' 
                    : 'border-muted-foreground/50 hover:border-primary'
                }`}
                onClick={(e) => e.stopPropagation()}
              />
            </div>
          )}
          <Badge 
            variant={course.isPublished ? 'default' : 'secondary'} 
            className={`text-xs font-medium shadow-sm transition-all duration-200 ${
              course.isPublished 
                ? 'bg-emerald-500 hover:bg-emerald-600 text-white' 
                : 'bg-amber-100 text-amber-800 hover:bg-amber-200'
            }`}
          >
            {course.isPublished ? 'Live' : 'Draft'}
          </Badge>
        </div>

        {/* Top Right: Badges */}
        <div className="absolute top-3 right-3 flex gap-1.5 z-20">
          <CourseLevelBadge level={course.level} size="sm" />
          {course.isFeatured && <CourseFeatureBadge isFeatured={course.isFeatured} size="sm" />}
        </div>
        {/* Hover Overlay with Action */}
        <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-all duration-300 flex items-end justify-center pb-4 z-15">
          <Button 
            size="sm" 
            variant="secondary" 
            onClick={(e) => {
              e.stopPropagation();
              onView();
            }}
            className="backdrop-blur-md bg-background/90 hover:bg-background border shadow-lg transition-all duration-200 hover:scale-105"
          >
            <Eye className="mr-2 h-4 w-4" />
            View Course
          </Button>
        </div>
      </div>

      <CardHeader className="space-y-3 pb-3 relative z-10">
        <div className="space-y-2">
          <h3 
            className="font-semibold text-base line-clamp-2 group-hover:text-primary transition-colors cursor-pointer leading-tight"
            onClick={(e) => {
              e.stopPropagation();
              onView();
            }}
          >
            {course.title}
          </h3>
          <p className="text-sm text-muted-foreground line-clamp-2 leading-relaxed">
            {course.shortDescription}
          </p>
        </div>
        
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground font-medium truncate mr-2">{course.instructorName}</span>
          <Badge variant="outline" className="text-xs font-medium bg-muted/50 hover:bg-muted shrink-0">
            {course.categoryName}
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="space-y-4 pt-0 relative z-10">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <CourseRating rating={course.averageRating || 0} size="sm" showValue />
            <span className="text-xs text-muted-foreground font-medium">
              ({course.totalReviews} {course.totalReviews === 1 ? 'review' : 'reviews'})
            </span>
          </div>
          <div className="flex items-center gap-1.5 text-sm text-muted-foreground bg-muted/30 px-2 py-1 rounded-full">
            <Users className="h-3.5 w-3.5" />
            <span className="font-medium">{course.totalStudents}</span>
          </div>
        </div>

        <div className="flex items-center justify-between pt-3 border-t border-border/50">
          <div className="flex flex-col">
            <span className="text-xs text-muted-foreground font-medium">Price</span>
            <div className="text-xl font-bold text-primary">
              ${course.effectivePrice}
            </div>
          </div>
          
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button 
                variant="outline" 
                size="sm" 
                className={`h-9 w-9 p-0 transition-all duration-200 hover:scale-105 ${
                  isSelected ? 'border-primary/50 bg-primary/5' : 'hover:bg-muted'
                }`}
                onClick={(e) => e.stopPropagation()}
              >
                <MoreHorizontal className="h-4 w-4" />
                <span className="sr-only">Course actions</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-48">
              <DropdownMenuItem 
                onClick={(e) => {
                  e.stopPropagation();
                  onView();
                }}
                className="cursor-pointer"
              >
                <Eye className="mr-2 h-4 w-4" />
                View Details
              </DropdownMenuItem>
              <DropdownMenuItem 
                onClick={(e) => {
                  e.stopPropagation();
                  onEdit();
                }}
                className="cursor-pointer"
              >
                <Edit className="mr-2 h-4 w-4" />
                Edit Course
              </DropdownMenuItem>
              <DropdownMenuItem 
                onClick={(e) => {
                  e.stopPropagation();
                  onDelete();
                }} 
                className="text-destructive cursor-pointer hover:bg-destructive/10"
              >
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