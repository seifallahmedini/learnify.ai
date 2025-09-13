import { Badge } from '@/shared/components/ui/badge';
import { CourseLevel, getCourseLevelLabel, getCourseLevelColor } from '../../types';

interface CourseLevelBadgeProps {
  level: CourseLevel;
  size?: 'sm' | 'md' | 'lg';
}

export function CourseLevelBadge({ level, size = 'md' }: CourseLevelBadgeProps) {
  const label = getCourseLevelLabel(level);
  const colorClass = getCourseLevelColor(level);
  
  const sizeClasses = {
    sm: 'text-xs px-2 py-0.5',
    md: 'text-sm px-2.5 py-0.5',
    lg: 'text-base px-3 py-1'
  };

  return (
    <Badge 
      variant="secondary" 
      className={`${colorClass} ${sizeClasses[size]} font-medium`}
    >
      {label}
    </Badge>
  );
}