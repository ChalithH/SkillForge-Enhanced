import { useState } from 'react';

interface StarRatingProps {
  level: number;
  onChange: (level: number) => void;
  maxRating?: number;
  readonly?: boolean;
  size?: 'sm' | 'md' | 'lg';
  showLabel?: boolean;
}

const SKILL_LEVELS = ['Beginner', 'Novice', 'Intermediate', 'Advanced', 'Expert'];

export default function StarRating({ 
  level, 
  onChange, 
  maxRating = 5, 
  readonly = false,
  size = 'md',
  showLabel = true 
}: StarRatingProps) {
  const [hoveredStar, setHoveredStar] = useState<number | null>(null);
  
  const sizeClasses = {
    sm: 'w-4 h-4',
    md: 'w-6 h-6',
    lg: 'w-8 h-8'
  };
  
  const displayLevel = hoveredStar || level;
  const displayLabel = SKILL_LEVELS[displayLevel - 1] || '';
  
  return (
    <div className="flex items-center space-x-1">
      {[...Array(maxRating)].map((_, index) => {
        const starNumber = index + 1;
        const isActive = starNumber <= level;
        const isHovered = !readonly && hoveredStar !== null && starNumber <= hoveredStar;
        const shouldHighlight = isActive || isHovered;
        
        return (
          <button
            key={starNumber}
            type="button"
            onClick={() => !readonly && onChange(starNumber)}
            onMouseEnter={() => !readonly && setHoveredStar(starNumber)}
            onMouseLeave={() => !readonly && setHoveredStar(null)}
            disabled={readonly}
            className={`${sizeClasses[size]} transition-all duration-150 ${
              shouldHighlight ? 'text-yellow-400' : 'text-gray-300'
            } ${!readonly ? 'hover:scale-110 cursor-pointer' : 'cursor-default'}`}
          >
            <svg fill="currentColor" viewBox="0 0 20 20">
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
            </svg>
          </button>
        );
      })}
      {showLabel && (
        <span className="text-sm text-gray-600 ml-2">
          {displayLevel}/{maxRating} - {displayLabel}
        </span>
      )}
    </div>
  );
}