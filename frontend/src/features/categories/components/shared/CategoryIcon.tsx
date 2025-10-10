import { cn } from '@/shared/lib/utils';
import { 
  BookOpen, 
  Code, 
  Palette, 
  Calculator, 
  Briefcase,
  Heart,
  Music,
  Camera,
  Globe,
  Gamepad2,
  Folder
} from 'lucide-react';

interface CategoryIconProps {
  icon?: string;
  color?: string;
  className?: string;
}

const iconMap = {
  book: BookOpen,
  code: Code,
  palette: Palette,
  calculator: Calculator,
  briefcase: Briefcase,
  heart: Heart,
  music: Music,
  camera: Camera,
  globe: Globe,
  gamepad: Gamepad2,
  folder: Folder,
};

export function CategoryIcon({ icon, color, className }: CategoryIconProps) {
  const IconComponent = icon && iconMap[icon as keyof typeof iconMap] ? 
    iconMap[icon as keyof typeof iconMap] : 
    Folder;

  return (
    <div 
      className={cn(
        "rounded-lg flex items-center justify-center",
        className
      )}
      style={{ 
        backgroundColor: color ? `${color}20` : '#f1f5f9',
        border: `1px solid ${color ? `${color}40` : '#e2e8f0'}`
      }}
    >
      <IconComponent 
        className="h-4 w-4" 
        style={{ color: color || '#64748b' }}
      />
    </div>
  );
}