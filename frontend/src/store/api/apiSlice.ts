import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { RootState } from '../store';
import { Skill, UserSkill, User, ProfileUpdateData } from '../../types';

const baseQuery = fetchBaseQuery({
  baseUrl: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  prepareHeaders: (headers, { getState }) => {
    const token = (getState() as RootState).auth.token;
    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery,
  tagTypes: ['User', 'Skill', 'UserSkill', 'Exchange', 'Review'],
  endpoints: (builder) => ({
    // Skills endpoints
    getSkills: builder.query<Skill[], void>({
      query: () => '/skills',
      providesTags: ['Skill'],
    }),
    getSkillCategories: builder.query<string[], void>({
      query: () => '/skills/categories',
      providesTags: ['Skill'],
    }),
    searchSkills: builder.query<Skill[], string>({
      query: (query: string) => `/skills/search?query=${query}`,
      providesTags: ['Skill'],
    }),
    
    // User Skills endpoints
    getUserSkills: builder.query<UserSkill[], void>({
      query: () => '/userskills',
      providesTags: ['UserSkill'],
    }),
    addUserSkill: builder.mutation<UserSkill, Omit<UserSkill, 'id'>>({
      query: (skillData) => ({
        url: '/userskills',
        method: 'POST',
        body: skillData,
      }),
      invalidatesTags: ['UserSkill'],
      onQueryStarted: async (skillData, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          apiSlice.util.updateQueryData('getUserSkills', undefined, (draft) => {
            draft.push({ ...skillData, id: Date.now() }); // Temporary ID
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),
    updateUserSkill: builder.mutation<UserSkill, Partial<UserSkill> & { id: number }>({
      query: ({ id, ...updates }) => ({
        url: `/userskills/${id}`,
        method: 'PUT',
        body: updates,
      }),
      invalidatesTags: ['UserSkill'],
      onQueryStarted: async ({ id, ...updates }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          apiSlice.util.updateQueryData('getUserSkills', undefined, (draft) => {
            const index = draft.findIndex((skill) => skill.id === id);
            if (index !== -1) {
              draft[index] = { ...draft[index], ...updates };
            }
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),
    deleteUserSkill: builder.mutation<void, number>({
      query: (id) => ({
        url: `/userskills/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['UserSkill'],
      onQueryStarted: async (id, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          apiSlice.util.updateQueryData('getUserSkills', undefined, (draft) => {
            return draft.filter((skill) => skill.id !== id);
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),
    
    // User profile endpoints
    updateProfile: builder.mutation<User, ProfileUpdateData>({
      query: (profileData) => ({
        url: '/users/profile',
        method: 'PUT',
        body: profileData,
      }),
      invalidatesTags: ['User'],
    }),
    uploadProfileImage: builder.mutation({
      query: (formData) => ({
        url: '/users/profile/image',
        method: 'POST',
        body: formData,
      }),
      invalidatesTags: ['User'],
    }),
    
    // Exchange endpoints
    getExchanges: builder.query({
      query: (status?: string) => `/exchanges${status ? `?status=${status}` : ''}`,
      providesTags: ['Exchange'],
    }),
    createExchange: builder.mutation({
      query: (exchangeData) => ({
        url: '/exchanges',
        method: 'POST',
        body: exchangeData,
      }),
      invalidatesTags: ['Exchange'],
    }),
    updateExchangeStatus: builder.mutation({
      query: ({ id, status }) => ({
        url: `/exchanges/${id}/status`,
        method: 'PUT',
        body: { status },
      }),
      invalidatesTags: ['Exchange'],
    }),
    
    // Review endpoints
    createReview: builder.mutation({
      query: (reviewData) => ({
        url: '/reviews',
        method: 'POST',
        body: reviewData,
      }),
      invalidatesTags: ['Review', 'Exchange'],
    }),
    getUserReviews: builder.query({
      query: (userId: number) => `/reviews/user/${userId}`,
      providesTags: ['Review'],
    }),
  }),
});

export const {
  useGetSkillsQuery,
  useGetSkillCategoriesQuery,
  useSearchSkillsQuery,
  useGetUserSkillsQuery,
  useAddUserSkillMutation,
  useUpdateUserSkillMutation,
  useDeleteUserSkillMutation,
  useUpdateProfileMutation,
  useUploadProfileImageMutation,
  useGetExchangesQuery,
  useCreateExchangeMutation,
  useUpdateExchangeStatusMutation,
  useCreateReviewMutation,
  useGetUserReviewsQuery,
} = apiSlice;