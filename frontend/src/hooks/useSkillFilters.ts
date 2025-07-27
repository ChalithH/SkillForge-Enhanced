import { useMemo } from 'react';
import { UserSkill } from '../types';

export function useSkillFilters(userSkills: UserSkill[]) {
  return useMemo(() => {
    // Filter out skills without skill data (defensive filtering)
    const validSkills = userSkills.filter(userSkill => userSkill.skill);
    
    // Categorize by offering vs learning
    const offeredSkills = validSkills.filter(userSkill => userSkill.isOffering);
    const learningSkills = validSkills.filter(userSkill => !userSkill.isOffering);
    
    return {
      validSkills,
      offeredSkills,
      learningSkills,
      offeredSkillsCount: offeredSkills.length,
      learningSkillsCount: learningSkills.length,
      totalValidSkillsCount: validSkills.length,
    };
  }, [userSkills]);
}