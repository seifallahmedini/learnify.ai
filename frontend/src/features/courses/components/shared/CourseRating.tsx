import { Star } from 'lucide-react';

interface CourseRatingProps {
  rating: number;
  reviewCount?: number;
  showReviewCount?: boolean;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

export function CourseRating({ 
  rating, 
  reviewCount, 
  showReviewCount = true, 
  size = 'md',
  className 
}: CourseRatingProps) {
  const sizeClasses = {
    sm: 'text-xs',
    md: 'text-sm', 
    lg: 'text-base',
  };

  const starSizeClasses = {
    sm: 'w-3 h-3',
    md: 'w-4 h-4',
    lg: 'w-5 h-5',
  };

  const getRatingColor = (rating: number): string => {
    if (rating >= 4.5) return 'text-green-600';
    if (rating >= 4.0) return 'text-yellow-600';
    if (rating >= 3.0) return 'text-orange-600';
    return 'text-red-600';
  };

  const renderStars = () => {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

    return (
      <div className="flex items-center gap-0.5">
        {/* Full stars */}
        {[...Array(fullStars)].map((_, i) => (
          <Star 
            key={`full-${i}`} 
            className={`${starSizeClasses[size]} fill-yellow-400 text-yellow-400`} 
          />
        ))}
        
        {/* Half star */}
        {hasHalfStar && (
          <div className="relative">
            <Star className={`${starSizeClasses[size]} text-gray-300`} />
            <div className="absolute inset-0 overflow-hidden w-1/2">
              <Star className={`${starSizeClasses[size]} fill-yellow-400 text-yellow-400`} />
            </div>
          </div>
        )}
        
        {/* Empty stars */}
        {[...Array(emptyStars)].map((_, i) => (
          <Star 
            key={`empty-${i}`} 
            className={`${starSizeClasses[size]} text-gray-300`} 
          />
        ))}
      </div>
    );
  };

  return (
    <div className={`flex items-center gap-2 ${sizeClasses[size]} ${className || ''}`}>
      {renderStars()}
      <span className={`font-medium ${getRatingColor(rating)}`}>
        {rating.toFixed(1)}
      </span>
      {showReviewCount && reviewCount !== undefined && (
        <span className="text-muted-foreground">
          ({reviewCount} {reviewCount === 1 ? 'review' : 'reviews'})
        </span>
      )}
    </div>
  );
}
