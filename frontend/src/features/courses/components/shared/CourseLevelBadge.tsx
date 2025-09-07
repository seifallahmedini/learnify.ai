import { Badge } from '@/shared/components/ui/badge';
import type { CourseLevel } from '../../types';
import { getCourseLevelLabel, getCourseLevelColor } from '../../types';

interface CourseLevelBadgeProps {
  level: CourseLevel;
  className?: string;
}

export function CourseLevelBadge({ level, className }: CourseLevelBadgeProps) {
  const label = getCourseLevelLabel(level);
  const colorClass = getCourseLevelColor(level);

  return (
    <Badge 
      variant="secondary" 
      className={`${colorClass} ${className || ''}`}
    >
      {label}
    </Badge>
  );
}
