import { useState, useEffect } from 'react';
import { useAppSelector, useAppDispatch } from '../store/hooks';
import { useUpdateProfileMutation } from '../store/api/apiSlice';
import { updateUserProfile } from '../store/slices/authSlice';
import Navigation from '../components/Navigation';

export default function Profile() {
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const [updateProfile, { isLoading: isUpdating }] = useUpdateProfileMutation();

  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    bio: '',
    profileImageUrl: '',
  });

  // Initialize form data when user data loads
  useEffect(() => {
    if (user) {
      setFormData({
        name: user.name || '',
        email: user.email || '',
        bio: user.bio || '',
        profileImageUrl: user.profileImageUrl || '',
      });
    }
  }, [user]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const updatedUser = await updateProfile(formData).unwrap();
      dispatch(updateUserProfile(updatedUser));
      setIsEditing(false);
    } catch (error) {
      console.error('Failed to update profile:', error);
    }
  };

  const handleCancel = () => {
    // Reset form data to original user data
    if (user) {
      setFormData({
        name: user.name || '',
        email: user.email || '',
        bio: user.bio || '',
        profileImageUrl: user.profileImageUrl || '',
      });
    }
    setIsEditing(false);
  };

  if (!user) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Loading Profile...</h2>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navigation />
      <div className="max-w-3xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
        <div className="bg-white shadow rounded-lg">
          {/* Header */}
          <div className="px-6 py-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h1 className="text-2xl font-bold text-gray-900">Profile</h1>
              {!isEditing && (
                <button
                  onClick={() => setIsEditing(true)}
                  className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  Edit Profile
                </button>
              )}
            </div>
          </div>

          {/* Profile Content */}
          <div className="px-6 py-6">
            {isEditing ? (
              /* Edit Form */
              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="grid grid-cols-1 gap-6">
                  {/* Profile Image */}
                  <div className="flex items-center space-x-6">
                    <div className="flex-shrink-0">
                      <img
                        className="h-20 w-20 rounded-full object-cover"
                        src={formData.profileImageUrl || '/api/placeholder/80/80'}
                        alt="Profile"
                        onError={(e) => {
                          const target = e.target as HTMLImageElement;
                          target.src = '/api/placeholder/80/80';
                        }}
                      />
                    </div>
                    <div className="flex-1">
                      <label htmlFor="profileImageUrl" className="block text-sm font-medium text-gray-700">
                        Profile Image URL
                      </label>
                      <input
                        type="url"
                        id="profileImageUrl"
                        name="profileImageUrl"
                        value={formData.profileImageUrl}
                        onChange={handleChange}
                        className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                        placeholder="https://example.com/profile.jpg" // Add proper default later
                      />
                    </div>
                  </div>

                  {/* Name */}
                  <div>
                    <label htmlFor="name" className="block text-sm font-medium text-gray-700">
                      Full Name
                    </label>
                    <input
                      type="text"
                      id="name"
                      name="name"
                      value={formData.name}
                      onChange={handleChange}
                      required
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    />
                  </div>

                  {/* Email */}
                  <div>
                    <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                      Email Address
                    </label>
                    <input
                      type="email"
                      id="email"
                      name="email"
                      value={formData.email}
                      onChange={handleChange}
                      required
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    />
                  </div>

                  {/* Bio */}
                  <div>
                    <label htmlFor="bio" className="block text-sm font-medium text-gray-700">
                      Bio
                    </label>
                    <textarea
                      id="bio"
                      name="bio"
                      rows={4}
                      value={formData.bio}
                      onChange={handleChange}
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                      placeholder="Tell us about yourself and your skills..."
                    />
                  </div>
                </div>

                {/* Form Actions */}
                <div className="flex justify-end space-x-3">
                  <button
                    type="button"
                    onClick={handleCancel}
                    className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={isUpdating}
                    className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {isUpdating ? 'Saving...' : 'Save Changes'}
                  </button>
                </div>
              </form>
            ) : (
              /* Display Mode */
              <div className="space-y-6">
                {/* Profile Image and Basic Info */}
                <div className="flex items-start space-x-6">
                  <div className="flex-shrink-0">
                    <img
                      className="h-24 w-24 rounded-full object-cover"
                      src={user.profileImageUrl || '/api/placeholder/96/96'}
                      alt="Profile"
                      onError={(e) => {
                        const target = e.target as HTMLImageElement;
                        target.src = '/api/placeholder/96/96';
                      }}
                    />
                  </div>
                  <div className="flex-1">
                    <h2 className="text-2xl font-bold text-gray-900">{user.name}</h2>
                    <p className="text-sm text-gray-500">{user.email}</p>
                    <div className="mt-2">
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                        {user.timeCredits} time credits
                      </span>
                    </div>
                  </div>
                </div>

                {/* Bio */}
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-2">About</h3>
                  <p className="text-gray-700 whitespace-pre-wrap">
                    {user.bio || 'No bio provided yet. Click "Edit Profile" to add your bio.'}
                  </p>
                </div>

                {/* Profile Stats */}
                <div className="grid grid-cols-3 gap-4 pt-6 border-t border-gray-200">
                  <div className="text-center">
                    <div className="text-2xl font-bold text-gray-900">0</div>
                    <div className="text-sm text-gray-500">Skills Offered</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-gray-900">0</div>
                    <div className="text-sm text-gray-500">Exchanges Completed</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-gray-900">0</div>
                    <div className="text-sm text-gray-500">Reviews Received</div>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}