import { Badge } from '@/shared/components/ui/badge';

interface CourseStatusBadgeProps {
  isPublished: boolean;
  className?: string;
}

export function CourseStatusBadge({ isPublished, className }: CourseStatusBadgeProps) {
  const statusColor = isPublished 
    ? 'bg-green-100 text-green-700' 
    : 'bg-yellow-100 text-yellow-700';
  
  const statusLabel = isPublished ? 'Published' : 'Draft';

  return (
    <Badge 
      variant="secondary" 
      className={`${statusColor} ${className || ''}`}
    >
      {statusLabel}
    </Badge>
  );
}
