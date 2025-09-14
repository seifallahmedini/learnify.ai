import { Badge } from '@/shared/components/ui/badge';
import { Crown } from 'lucide-react';

interface CourseFeatureBadgeProps {
  isFeatured: boolean;
  size?: 'sm' | 'md' | 'lg';
  variant?: 'default' | 'minimal';
}

export function CourseFeatureBadge({ isFeatured, size = 'md', variant = 'default' }: CourseFeatureBadgeProps) {
  const sizeClasses = {
    sm: 'text-xs px-2 py-1 h-6',
    md: 'text-xs px-2.5 py-1 h-6',
    lg: 'text-sm px-3 py-1.5 h-7'
  };

  const iconSizes = {
    sm: 'h-3 w-3',
    md: 'h-3 w-3',
    lg: 'h-3.5 w-3.5'
  };

  if (!isFeatured) {
    return variant === 'minimal' ? null : (
      <Badge 
        variant="outline"
        className={`${sizeClasses[size]} font-medium border-gray-200 bg-gray-50/50 text-gray-600 hover:bg-gray-100/80 transition-colors`}
      >
        Standard
      </Badge>
    );
  }

  return (
    <Badge 
      className={`${sizeClasses[size]} font-semibold bg-gradient-to-r from-purple-500 to-purple-600 hover:from-purple-600 hover:to-purple-700 text-white border-0 shadow-sm hover:shadow-md transition-all duration-200 flex items-center gap-1`}
    >
      <Crown className={`${iconSizes[size]} fill-current drop-shadow-sm`} />
      <span className="drop-shadow-sm">Featured</span>
    </Badge>
  );
}