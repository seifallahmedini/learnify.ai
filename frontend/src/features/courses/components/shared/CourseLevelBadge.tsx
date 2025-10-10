import { Badge } from '@/shared/components/ui/badge';
import { TrendingUp, Users, Award, Zap } from 'lucide-react';
import { CourseLevel } from '../../types';
import { getCourseLevelLabel } from '../../lib';

interface CourseLevelBadgeProps {
  level: CourseLevel;
  size?: 'sm' | 'md' | 'lg';
  showIcon?: boolean;
}

export function CourseLevelBadge({ level, size = 'md', showIcon = true }: CourseLevelBadgeProps) {
  const label = getCourseLevelLabel(level);
  
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

  const getLevelConfig = (level: CourseLevel) => {
    switch (level) {
      case 1: // Beginner
        return {
          icon: Users,
          className: 'bg-green-500/10 text-green-700 border-green-200 hover:bg-green-500/20',
          gradient: 'bg-gradient-to-r from-green-50 to-emerald-50'
        };
      case 2: // Intermediate
        return {
          icon: TrendingUp,
          className: 'bg-blue-500/10 text-blue-700 border-blue-200 hover:bg-blue-500/20',
          gradient: 'bg-gradient-to-r from-blue-50 to-cyan-50'
        };
      case 3: // Advanced
        return {
          icon: Award,
          className: 'bg-orange-500/10 text-orange-700 border-orange-200 hover:bg-orange-500/20',
          gradient: 'bg-gradient-to-r from-orange-50 to-amber-50'
        };
      case 4: // Expert
        return {
          icon: Zap,
          className: 'bg-purple-500/10 text-purple-700 border-purple-200 hover:bg-purple-500/20',
          gradient: 'bg-gradient-to-r from-purple-50 to-violet-50'
        };
      default:
        return {
          icon: Users,
          className: 'bg-gray-500/10 text-gray-700 border-gray-200 hover:bg-gray-500/20',
          gradient: 'bg-gradient-to-r from-gray-50 to-slate-50'
        };
    }
  };

  const config = getLevelConfig(level);
  const Icon = config.icon;

  return (
    <Badge 
      variant="outline"
      className={`${sizeClasses[size]} font-semibold ${config.className} ${config.gradient} transition-all duration-200 flex items-center gap-1 shadow-sm hover:shadow-md border`}
    >
      {showIcon && <Icon className={`${iconSizes[size]}`} />}
      <span>{label}</span>
    </Badge>
  );
}