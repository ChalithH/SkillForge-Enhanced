import { useRef, useEffect } from 'react';
import { highlightSearchTerm } from '../../utils/searchUtils';

interface EnhancedSkillResult {
  skill: {
    id: number;
    name: string;
    category: string;
    description: string;
  };
  isAlreadyAdded: boolean;
  isTeaching: boolean;
  isLearning: boolean;
  contextMessage: string | null;
}

interface SkillSearchInputProps {
  searchTerm: string;
  onSearchChange: (term: string) => void;
  searchResults: EnhancedSkillResult[];
  onSkillSelect: (skill: EnhancedSkillResult['skill']) => void;
  isLoading: boolean;
  placeholder?: string;
  showDropdown: boolean;
  onDropdownChange: (show: boolean) => void;
}

export default function SkillSearchInput({
  searchTerm,
  onSearchChange,
  searchResults,
  onSkillSelect,
  isLoading,
  placeholder = "Type to search skills...",
  showDropdown,
  onDropdownChange
}: SkillSearchInputProps) {
  const searchInputRef = useRef<HTMLInputElement>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Focus search input when component mounts
  useEffect(() => {
    const timer = setTimeout(() => {
      searchInputRef.current?.focus();
    }, 100);
    return () => clearTimeout(timer);
  }, []);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        onDropdownChange(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [onDropdownChange]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    onSearchChange(value);
    onDropdownChange(true);
  };

  const handleSkillClick = (skill: EnhancedSkillResult['skill']) => {
    onSkillSelect(skill);
    onDropdownChange(false);
  };

  return (
    <div className="relative" ref={dropdownRef}>
      <label htmlFor="skill-search" className="block text-sm font-medium text-gray-700 mb-1">
        Search for a skill
      </label>
      <input
        ref={searchInputRef}
        type="text"
        id="skill-search"
        value={searchTerm}
        onChange={handleInputChange}
        onFocus={() => onDropdownChange(true)}
        placeholder={placeholder}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        required
      />
      
      {/* Dropdown */}
      {showDropdown && searchTerm && (
        <div className="absolute z-10 mt-1 w-full bg-white shadow-lg max-h-60 rounded-md py-1 text-base ring-1 ring-black ring-opacity-5 overflow-auto">
          {isLoading ? (
            <div className="px-4 py-2 text-sm text-gray-500">Loading skills...</div>
          ) : searchResults.length > 0 ? (
            searchResults.map((result) => (
              <button
                key={result.skill.id}
                type="button"
                onClick={() => handleSkillClick(result.skill)}
                className="w-full text-left px-4 py-2 text-sm text-gray-900 hover:bg-blue-50 focus:outline-none focus:bg-blue-50"
              >
                <div className="flex items-center justify-between">
                  <div className="flex-1">
                    <div 
                      className="font-medium"
                      dangerouslySetInnerHTML={{ 
                        __html: highlightSearchTerm(result.skill.name, searchTerm) 
                      }}
                    />
                    <div 
                      className="text-xs text-gray-500"
                      dangerouslySetInnerHTML={{ 
                        __html: highlightSearchTerm(result.skill.category, searchTerm) 
                      }}
                    />
                  </div>
                  {result.contextMessage && (
                    <div className="ml-2">
                      <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        {result.contextMessage}
                      </span>
                    </div>
                  )}
                </div>
              </button>
            ))
          ) : (
            <div className="px-4 py-2 text-sm text-gray-500">No skills found</div>
          )}
        </div>
      )}
    </div>
  );
}