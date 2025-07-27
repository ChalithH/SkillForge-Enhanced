import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAppSelector, useAppDispatch } from '../store/hooks';
import { logout, loadUser } from '../store/slices/authSlice';
import { useState, useEffect } from 'react';
import { Bell } from 'lucide-react';
import { NotificationBadge } from './NotificationBadge';
import { usePendingRequests } from '../hooks/usePendingRequests';

export default function Navigation() {
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const [previousCredits, setPreviousCredits] = useState<number | null>(null);
  const [creditChange, setCreditChange] = useState<'increase' | 'decrease' | null>(null);
  const { incomingCount, totalPendingCount } = usePendingRequests();

  // Track credit changes for visual feedback
  useEffect(() => {
    if (user?.timeCredits !== undefined) {
      if (previousCredits !== null && previousCredits !== user.timeCredits) {
        setCreditChange(user.timeCredits > previousCredits ? 'increase' : 'decrease');
        // Clear the change indicator after animation
        setTimeout(() => setCreditChange(null), 2000);
      }
      setPreviousCredits(user.timeCredits);
    }
  }, [user?.timeCredits, previousCredits]);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  const handleRefreshCredits = async () => {
    try {
      await dispatch(loadUser()).unwrap();
    } catch (error) {
      console.error('Failed to refresh user data:', error);
    }
  };

  const navigation = [
    { name: 'Dashboard', href: '/dashboard', current: location.pathname === '/dashboard' },
    { name: 'Profile', href: '/profile', current: location.pathname === '/profile' },
    { name: 'My Skills', href: '/skills', current: location.pathname === '/skills' },
    { name: 'My Exchanges', href: '/exchanges', current: location.pathname === '/exchanges' },
  ];

  return (
    <nav className="bg-white shadow">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex">
            <div className="flex-shrink-0 flex items-center">
              <Link to="/dashboard" className="text-xl font-semibold text-gray-900">
                SkillForge
              </Link>
            </div>
            <div className="hidden sm:ml-6 sm:flex sm:space-x-8">
              {navigation.map((item) => (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`${
                    item.current
                      ? 'border-blue-500 text-gray-900'
                      : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'
                  } inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors relative`}
                >
                  {item.name}
                  {item.name === 'My Exchanges' && incomingCount > 0 && (
                    <NotificationBadge 
                      count={incomingCount} 
                      className="ml-2 -mt-1"
                    />
                  )}
                </Link>
              ))}
            </div>
            
            {/* Mobile menu button */}
            <div className="sm:hidden flex items-center">
              <button className="text-gray-500 hover:text-gray-700 p-2">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                </svg>
              </button>
            </div>
          </div>
          <div className="flex items-center space-x-2 sm:space-x-4">
            <span className="hidden sm:block text-gray-700 text-sm">
              Welcome, {user?.name}
            </span>
            
            {/* Notification Bell */}
            <div className="relative">
              <Link
                to="/exchanges"
                className="text-gray-400 hover:text-gray-600 transition-colors p-1"
                title={`${incomingCount} pending request${incomingCount !== 1 ? 's' : ''}`}
              >
                <Bell className="h-5 w-5" />
              </Link>
              {incomingCount > 0 && (
                <NotificationBadge 
                  count={incomingCount} 
                  className="absolute -top-1 -right-1 min-w-[18px] h-[18px] text-[10px]"
                />
              )}
            </div>
            
            {/* Credit Balance with Animation */}
            <div className="flex items-center space-x-1">
              <div className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium transition-all duration-500 ${
                creditChange === 'increase' ? 'bg-green-200 text-green-900 scale-110' :
                creditChange === 'decrease' ? 'bg-red-200 text-red-900 scale-110' :
                'bg-green-100 text-green-800'
              }`}>
                <svg className="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1" />
                </svg>
                {user?.timeCredits || 0} credits
                {creditChange === 'increase' && (
                  <svg className="w-3 h-3 ml-1 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 11l5-5m0 0l5 5m-5-5v12" />
                  </svg>
                )}
                {creditChange === 'decrease' && (
                  <svg className="w-3 h-3 ml-1 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 13l-5 5m0 0l-5-5m5 5V6" />
                  </svg>
                )}
              </div>
              
              {/* Refresh Button */}
              <button
                onClick={handleRefreshCredits}
                className="p-1 text-gray-400 hover:text-gray-600 transition-colors"
                title="Refresh credit balance"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                </svg>
              </button>
            </div>
            
            <button
              onClick={handleLogout}
              className="text-gray-500 hover:text-gray-700 px-2 sm:px-3 py-2 text-sm font-medium transition-colors"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}