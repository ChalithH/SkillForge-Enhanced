import { useEffect } from 'react';
import { useAppDispatch } from '../store/hooks';
import { loadUser } from '../store/slices/authSlice';

export const AppInitializer: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const dispatch = useAppDispatch();

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      // Try to load user data if token exists
      dispatch(loadUser());
    }
  }, [dispatch]);

  return <>{children}</>;
};