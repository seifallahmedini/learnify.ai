import { Star } from 'lucide-react';

interface CourseRatingProps {
  rating: number;
  maxRating?: number;
  size?: 'sm' | 'md' | 'lg';
  showValue?: boolean;
}

export function CourseRating({ 
  rating, 
  maxRating = 5, 
  size = 'md', 
  showValue = false 
}: CourseRatingProps) {
  const sizeClasses = {
    sm: 'h-3 w-3',
    md: 'h-4 w-4',
    lg: 'h-5 w-5'
  };

  const textSizes = {
    sm: 'text-xs',
    md: 'text-sm',
    lg: 'text-base'
  };

  const stars = Array.from({ length: maxRating }, (_, index) => {
    const starValue = index + 1;
    const isFilled = starValue <= rating;
    const isPartial = starValue > rating && starValue - 1 < rating;
    
    return (
      <Star
        key={index}
        className={`${sizeClasses[size]} ${
          isFilled 
            ? 'fill-yellow-400 text-yellow-400' 
            : isPartial 
            ? 'fill-yellow-200 text-yellow-400'
            : 'fill-gray-200 text-gray-200'
        }`}
      />
    );
  });

  return (
    <div className="flex items-center space-x-1">
      <div className="flex items-center">
        {stars}
      </div>
      {showValue && (
        <span className={`${textSizes[size]} text-gray-600 ml-1`}>
          ({rating.toFixed(1)})
        </span>
      )}
    </div>
  );
}