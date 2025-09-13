import { Badge } from '@/shared/components/ui/badge';

interface CourseStatusBadgeProps {
  isPublished: boolean;
  size?: 'sm' | 'md' | 'lg';
}

export function CourseStatusBadge({ isPublished, size = 'md' }: CourseStatusBadgeProps) {
  const sizeClasses = {
    sm: 'text-xs px-2 py-0.5',
    md: 'text-sm px-2.5 py-0.5',
    lg: 'text-base px-3 py-1'
  };

  return (
    <Badge 
      variant={isPublished ? 'default' : 'secondary'}
      className={`${sizeClasses[size]} font-medium ${
        isPublished 
          ? 'bg-green-100 text-green-700 hover:bg-green-200' 
          : 'bg-yellow-100 text-yellow-700 hover:bg-yellow-200'
      }`}
    >
      {isPublished ? 'Published' : 'Draft'}
    </Badge>
  );
}