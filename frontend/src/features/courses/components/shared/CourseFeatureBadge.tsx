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
      variant="secondary" 
      className={`bg-purple-100 text-purple-700 ${className || ''}`}
    >
      <Star className="w-3 h-3 mr-1 fill-current" />
      Featured
    </Badge>
  );
}
