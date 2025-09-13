import { Badge } from '@/shared/components/ui/badge';
import { Star } from 'lucide-react';

interface CourseFeatureBadgeProps {
  isFeatured: boolean;
  size?: 'sm' | 'md' | 'lg';
}

export function CourseFeatureBadge({ isFeatured, size = 'md' }: CourseFeatureBadgeProps) {
  const sizeClasses = {
    sm: 'text-xs px-2 py-0.5',
    md: 'text-sm px-2.5 py-0.5',
    lg: 'text-base px-3 py-1'
  };

  const iconSizes = {
    sm: 'h-3 w-3',
    md: 'h-4 w-4',
    lg: 'h-5 w-5'
  };

  if (!isFeatured) {
    return (
      <Badge 
        variant="outline"
        className={`${sizeClasses[size]} font-medium bg-gray-50 text-gray-600`}
      >
        Standard
      </Badge>
    );
  }

  return (
    <Badge 
      variant="default"
      className={`${sizeClasses[size]} font-medium bg-purple-100 text-purple-700 hover:bg-purple-200`}
    >
      <Star className={`${iconSizes[size]} mr-1 fill-current`} />
      Featured
    </Badge>
  );
}