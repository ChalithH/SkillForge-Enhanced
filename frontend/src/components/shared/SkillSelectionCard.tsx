import { Skill } from '../../types';

interface SkillSelectionCardProps {
  skill: Skill;
  onClear: () => void;
}

export default function SkillSelectionCard({ skill, onClear }: SkillSelectionCardProps) {
  return (
    <div className="p-3 bg-blue-50 rounded-md border border-blue-200">
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <h4 className="font-medium text-blue-900">{skill.name}</h4>
          <p className="text-sm text-blue-700 mt-1">{skill.description}</p>
          <span className="inline-block mt-2 px-2 py-1 text-xs font-medium bg-blue-100 text-blue-800 rounded-full">
            {skill.category}
          </span>
        </div>
        <button
          type="button"
          onClick={onClear}
          className="ml-2 text-blue-400 hover:text-blue-600 transition-colors"
          aria-label="Clear selection"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </div>
  );
}