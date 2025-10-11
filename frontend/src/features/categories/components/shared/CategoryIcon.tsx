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
  iconUrl?: string;
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

export function CategoryIcon({ iconUrl, className }: CategoryIconProps) {
  // If iconUrl is provided, try to render as image, fallback to icon
  if (iconUrl) {
    return (
      <div 
        className={cn(
          "rounded-lg flex items-center justify-center bg-muted border",
          className
        )}
      >
        <img 
          src={iconUrl} 
          alt="Category icon" 
          className="h-4 w-4 object-contain"
          onError={(e) => {
            // Fallback to folder icon if image fails to load
            const target = e.target as HTMLImageElement;
            target.style.display = 'none';
            target.nextElementSibling?.classList.remove('hidden');
          }}
        />
        <Folder className="h-4 w-4 text-muted-foreground hidden" />
      </div>
    );
  }

  // Default folder icon
  return (
    <div 
      className={cn(
        "rounded-lg flex items-center justify-center bg-muted border",
        className
      )}
    >
      <Folder className="h-4 w-4 text-muted-foreground" />
    </div>
  );
}