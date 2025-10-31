import { Badge } from '@/shared/components/ui/badge';
import { CheckCircle2, XCircle } from 'lucide-react';

interface QuizStatusBadgeProps {
  isActive: boolean;
  size?: 'sm' | 'default';
}

export function QuizStatusBadge({ isActive, size = 'default' }: QuizStatusBadgeProps) {
  const sizeClasses = size === 'sm' ? 'text-xs h-5' : 'text-xs';
  
  if (isActive) {
    return (
      <Badge variant="default" className={`${sizeClasses} bg-green-100 text-green-800 hover:bg-green-200 border-green-200`}>
        <CheckCircle2 className="h-3 w-3 mr-1" />
        Active
      </Badge>
    );
  }
  
  return (
    <Badge variant="secondary" className={`${sizeClasses} bg-gray-100 text-gray-800 hover:bg-gray-200`}>
      <XCircle className="h-3 w-3 mr-1" />
      Inactive
    </Badge>
  );
}

