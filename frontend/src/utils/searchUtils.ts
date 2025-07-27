import { Skill } from '../types';

/**
 * Calculate relevance score for a skill based on search term
 * Higher score = better match
 */
function calculateRelevanceScore(skill: Skill, searchTerm: string): number {
  const search = searchTerm.toLowerCase().trim();
  const name = skill.name.toLowerCase();
  const category = skill.category.toLowerCase();
  const description = skill.description.toLowerCase();
  
  let score = 0;
  
  // Exact name match gets highest score
  if (name === search) score += 100;
  
  // Name starts with search term
  else if (name.startsWith(search)) score += 80;
  
  // Name contains search term
  else if (name.includes(search)) score += 60;
  
  // Category exact match
  if (category === search) score += 50;
  
  // Category starts with search term
  else if (category.startsWith(search)) score += 40;
  
  // Category contains search term
  else if (category.includes(search)) score += 30;
  
  // Description contains search term
  if (description.includes(search)) score += 20;
  
  // Fuzzy matching for common typos/variations
  if (search.length > 3) {
    // Check if search is close to skill name (simple edit distance approximation)
    if (isCloseMatch(name, search)) score += 25;
    if (isCloseMatch(category, search)) score += 15;
  }
  
  // Boost score for exact word matches
  const searchWords = search.split(' ');
  const nameWords = name.split(' ');
  const categoryWords = category.split(' ');
  
  searchWords.forEach(searchWord => {
    if (searchWord.length > 2) { // Skip very short words
      nameWords.forEach(nameWord => {
        if (nameWord === searchWord) score += 15;
        else if (nameWord.startsWith(searchWord)) score += 10;
      });
      
      categoryWords.forEach(categoryWord => {
        if (categoryWord === searchWord) score += 10;
        else if (categoryWord.startsWith(searchWord)) score += 5;
      });
    }
  });
  
  return score;
}

/**
 * Simple fuzzy matching for close character matches
 */
function isCloseMatch(target: string, search: string): boolean {
  if (Math.abs(target.length - search.length) > 2) return false;
  
  let differences = 0;
  const maxDifferences = Math.floor(search.length / 3); // Allow ~33% differences
  
  for (let i = 0; i < Math.min(target.length, search.length); i++) {
    if (target[i] !== search[i]) {
      differences++;
      if (differences > maxDifferences) return false;
    }
  }
  
  return differences <= maxDifferences;
}

/**
 * Enhanced search function with relevance scoring and fuzzy matching
 */
export function searchSkills(skills: Skill[], searchTerm: string, maxResults: number = 10): Skill[] {
  if (!searchTerm.trim()) return [];
  
  const search = searchTerm.toLowerCase().trim();
  
  // Simple filtering first for performance
  const candidates = skills.filter(skill => 
    skill.name.toLowerCase().includes(search) ||
    skill.category.toLowerCase().includes(search) ||
    skill.description.toLowerCase().includes(search) ||
    (search.length > 3 && (
      isCloseMatch(skill.name.toLowerCase(), search) ||
      isCloseMatch(skill.category.toLowerCase(), search)
    ))
  );
  
  // Calculate relevance scores and sort
  const scoredResults = candidates
    .map(skill => ({
      skill,
      score: calculateRelevanceScore(skill, searchTerm)
    }))
    .filter(result => result.score > 0)
    .sort((a, b) => b.score - a.score)
    .slice(0, maxResults)
    .map(result => result.skill);
  
  return scoredResults;
}

/**
 * Highlight matching text in search results
 */
export function highlightSearchTerm(text: string, searchTerm: string): string {
  if (!searchTerm.trim()) return text;
  
  const search = searchTerm.trim();
  const regex = new RegExp(`(${search})`, 'gi');
  return text.replace(regex, '<mark>$1</mark>');
}

/**
 * Get search suggestions based on partial input
 */
export function getSearchSuggestions(skills: Skill[], partialTerm: string): string[] {
  if (partialTerm.length < 2) return [];
  
  const suggestions = new Set<string>();
  const search = partialTerm.toLowerCase();
  
  skills.forEach(skill => {
    // Suggest skill names that start with the search term
    if (skill.name.toLowerCase().startsWith(search)) {
      suggestions.add(skill.name);
    }
    
    // Suggest categories
    if (skill.category.toLowerCase().startsWith(search)) {
      suggestions.add(skill.category);
    }
    
    // Suggest individual words from skill names
    skill.name.split(' ').forEach(word => {
      if (word.toLowerCase().startsWith(search) && word.length > 2) {
        suggestions.add(word);
      }
    });
  });
  
  return Array.from(suggestions).slice(0, 5);
}