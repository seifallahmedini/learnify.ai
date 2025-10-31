import { Badge } from '@/shared/components/ui/badge';
import { Lock, Unlock } from 'lucide-react';

interface LessonAccessBadgeProps {
  isFree: boolean;
  size?: 'sm' | 'md' | 'lg';
  variant?: 'default' | 'detailed' | 'minimal';
  showIcon?: boolean;
}

export function LessonAccessBadge({ 
  isFree, 
  size = 'md', 
  variant = 'default',
  showIcon = true 
}: LessonAccessBadgeProps) {
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

  const getAccessConfig = (isFree: boolean) => {
    if (isFree) {
      return {
        icon: Unlock,
        label: 'Free',
        className: 'bg-blue-500/10 text-blue-700 border-blue-200 hover:bg-blue-500/20',
        gradient: 'bg-gradient-to-r from-blue-50 to-cyan-50',
        dotColor: 'bg-blue-500'
      };
    } else {
      return {
        icon: Lock,
        label: 'Premium',
        className: 'bg-purple-500/10 text-purple-700 border-purple-200 hover:bg-purple-500/20',
        gradient: 'bg-gradient-to-r from-purple-50 to-pink-50',
        dotColor: 'bg-purple-500'
      };
    }
  };

  const config = getAccessConfig(isFree);
  const Icon = config.icon;

  if (variant === 'minimal') {
    return (
      <div className="flex items-center space-x-1.5">
        <div className={`w-2 h-2 rounded-full ${config.dotColor}`} />
        <span className={`${sizeClasses[size].includes('text-xs') ? 'text-xs' : 'text-sm'} font-medium ${isFree ? 'text-blue-700' : 'text-purple-700'}`}>
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
        <Icon className={iconSizes[size]} />
      )}
      <span>{config.label}</span>
      {variant === 'detailed' && (
        <div className={`w-1.5 h-1.5 rounded-full ${config.dotColor}`} />
      )}
    </Badge>
  );
}

