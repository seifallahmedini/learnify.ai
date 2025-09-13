import { Badge } from '@/shared/components/ui/badge';
import { Star } from 'lucide-react';

interface CourseFeatureBadgeProps {
  isFeatured: boolean;
  className?: string;
}

export function CourseFeatureBadge({ isFeatured, className }: CourseFeatureBadgeProps) {
  if (!isFeatured) return null;

  return (
    <Badge 
      className={`
        relative overflow-hidden
        bg-gradient-to-r from-violet-500 via-purple-500 to-indigo-600 
        hover:from-violet-600 hover:via-purple-600 hover:to-indigo-700
        text-white border-0 shadow-md hover:shadow-lg
        transition-all duration-300 ease-out
        px-3 py-1.5 rounded-full
        font-semibold text-xs
        before:absolute before:inset-0 
        before:bg-gradient-to-r before:from-white/20 before:via-transparent before:to-transparent
        before:opacity-0 hover:before:opacity-100
        before:transition-opacity before:duration-300
        transform hover:scale-105
        ${className || ''}
      `}
    >
      <Star className="w-3 h-3 mr-1.5 fill-current drop-shadow-sm animate-pulse" />
      <span className="relative z-10 tracking-wide">Featured</span>
    </Badge>
  );
}
