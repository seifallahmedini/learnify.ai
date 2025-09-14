import { Badge } from '@/shared/components/ui/badge';
import { CheckCircle2, Clock } from 'lucide-react';

interface CourseStatusBadgeProps {
  isPublished: boolean;
  size?: 'sm' | 'md' | 'lg';
  variant?: 'default' | 'detailed' | 'minimal';
  showIcon?: boolean;
}

export function CourseStatusBadge({ 
  isPublished, 
  size = 'md', 
  variant = 'default',
  showIcon = true 
}: CourseStatusBadgeProps) {
  const sizeClasses = {
    sm: 'text-xs px-2 py-1 h-6',
    md: 'text-sm px-3 py-1.5 h-7',
    lg: 'text-base px-4 py-2 h-8'
  };

  const iconSizes = {
    sm: 'h-3 w-3',
    md: 'h-3.5 w-3.5',
    lg: 'h-4 w-4'
  };

  const getStatusConfig = (isPublished: boolean) => {
    if (isPublished) {
      return {
        icon: CheckCircle2,
        label: variant === 'detailed' ? 'Published' : 'Live',
        className: 'bg-emerald-500/10 text-emerald-700 border-emerald-200 hover:bg-emerald-500/20',
        gradient: 'bg-gradient-to-r from-emerald-50 to-green-50',
        dotColor: 'bg-emerald-500'
      };
    } else {
      return {
        icon: Clock,
        label: 'Draft',
        className: 'bg-amber-500/10 text-amber-700 border-amber-200 hover:bg-amber-500/20',
        gradient: 'bg-gradient-to-r from-amber-50 to-yellow-50',
        dotColor: 'bg-amber-500'
      };
    }
  };

  const config = getStatusConfig(isPublished);
  const Icon = config.icon;

  if (variant === 'minimal') {
    return (
      <div className="flex items-center space-x-1.5">
        <div className={`w-2 h-2 rounded-full ${config.dotColor} animate-pulse`} />
        <span className={`${sizeClasses[size].includes('text-xs') ? 'text-xs' : 'text-sm'} font-medium ${isPublished ? 'text-emerald-700' : 'text-amber-700'}`}>
          {config.label}
        </span>
      </div>
    );
  }

  return (
    <Badge 
      variant="outline"
      className={`${sizeClasses[size]} font-semibold ${config.className} ${config.gradient} transition-all duration-200 flex items-center gap-1.5 shadow-sm hover:shadow-md border`}
    >
      {showIcon && (
        <Icon className={`${iconSizes[size]} ${isPublished ? 'animate-pulse' : ''}`} />
      )}
      <span>{config.label}</span>
      {variant === 'detailed' && (
        <div className={`w-1.5 h-1.5 rounded-full ${config.dotColor} ${isPublished ? 'animate-pulse' : ''}`} />
      )}
    </Badge>
  );
}