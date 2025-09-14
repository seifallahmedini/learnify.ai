import { Star, StarHalf } from 'lucide-react';

interface CourseRatingProps {
  rating: number;
  maxRating?: number;
  size?: 'sm' | 'md' | 'lg';
  showValue?: boolean;
  variant?: 'default' | 'compact' | 'detailed';
}

export function CourseRating({ 
  rating, 
  maxRating = 5, 
  size = 'md', 
  showValue = false,
  variant = 'default'
}: CourseRatingProps) {
  const sizeClasses = {
    sm: 'h-3.5 w-3.5',
    md: 'h-4 w-4',
    lg: 'h-5 w-5'
  };

  const textSizes = {
    sm: 'text-xs',
    md: 'text-sm',
    lg: 'text-base'
  };

  const spacing = {
    sm: 'space-x-0.5',
    md: 'space-x-1',
    lg: 'space-x-1.5'
  };

  const getRatingCategory = (rating: number) => {
    if (rating >= 4.5) return { label: 'Excellent', color: 'text-green-600' };
    if (rating >= 4.0) return { label: 'Very Good', color: 'text-blue-600' };
    if (rating >= 3.5) return { label: 'Good', color: 'text-yellow-600' };
    if (rating >= 3.0) return { label: 'Fair', color: 'text-orange-600' };
    return { label: 'Poor', color: 'text-red-600' };
  };

  const stars = Array.from({ length: maxRating }, (_, index) => {
    const starValue = index + 1;
    const isFilled = starValue <= rating;
    const isHalf = starValue > rating && starValue - 0.5 <= rating;
    
    return (
      <div key={index} className="relative">
        {isHalf ? (
          <div className="relative">
            <Star className={`${sizeClasses[size]} fill-gray-200 text-gray-200`} />
            <StarHalf className={`${sizeClasses[size]} absolute inset-0 fill-amber-400 text-amber-400`} />
          </div>
        ) : (
          <Star
            className={`${sizeClasses[size]} transition-colors duration-200 ${
              isFilled 
                ? 'fill-amber-400 text-amber-400 drop-shadow-sm' 
                : 'fill-gray-200 text-gray-200 hover:fill-gray-300'
            }`}
          />
        )}
      </div>
    );
  });

  if (variant === 'compact') {
    return (
      <div className="flex items-center space-x-1">
        <Star className={`${sizeClasses[size]} fill-amber-400 text-amber-400`} />
        <span className={`${textSizes[size]} font-medium text-gray-700`}>
          {rating.toFixed(1)}
        </span>
      </div>
    );
  }

  if (variant === 'detailed') {
    const category = getRatingCategory(rating);
    return (
      <div className="flex items-center space-x-2">
        <div className={`flex items-center ${spacing[size]}`}>
          {stars}
        </div>
        <div className="flex items-center space-x-1">
          <span className={`${textSizes[size]} font-semibold text-gray-800`}>
            {rating.toFixed(1)}
          </span>
          <span className={`${textSizes[size]} ${category.color} font-medium`}>
            {category.label}
          </span>
        </div>
      </div>
    );
  }

  return (
    <div className="flex items-center space-x-1.5">
      <div className={`flex items-center ${spacing[size]}`}>
        {stars}
      </div>
      {showValue && (
        <span className={`${textSizes[size]} font-medium text-gray-700 ml-1`}>
          {rating.toFixed(1)}
        </span>
      )}
    </div>
  );
}