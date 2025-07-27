import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAppSelector } from '../store/hooks';

export interface SignalRNotification {
  type: string;
  message: string;
  timestamp: string;
  exchangeId?: number;
  senderId?: number;
  senderName?: string;
  skillName?: string;
  scheduledAt?: string;
  duration?: number;
  status?: string;
  previousStatus?: string;
  amount?: number;
  reason?: string;
  actorId?: number;
  actorName?: string;
}

export const useSignalR = () => {
  const { token, user } = useAppSelector((state) => state.auth);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [notifications, setNotifications] = useState<SignalRNotification[]>([]);
  const [onlineUsers, setOnlineUsers] = useState<number[]>([]);
  const notificationListeners = useRef<Array<(notification: SignalRNotification) => void>>([]);

  // Initialize connection
  useEffect(() => {
    if (!token || !user) {
      // Clean up connection if user logs out
      if (connection) {
        connection.stop();
        setConnection(null);
        setIsConnected(false);
        setNotifications([]);
        setOnlineUsers([]);
      }
      return;
    }

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/hubs/notification?access_token=${token}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff: 1s, 2s, 4s, 8s, then 30s max
          if (retryContext.previousRetryCount < 4) {
            return Math.pow(2, retryContext.previousRetryCount) * 1000;
          }
          return 30000;
        }
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Set up event handlers
    newConnection.on('ReceiveNotification', (notification: SignalRNotification) => {
      console.log('Received notification:', notification);
      
      // Add to notifications list
      setNotifications(prev => [notification, ...prev.slice(0, 49)]); // Keep last 50 notifications
      
      // Call all registered listeners
      notificationListeners.current.forEach(listener => listener(notification));
      
      // Show browser notification if permission is granted
      if (Notification.permission === 'granted') {
        const browserNotification = new Notification(notification.message, {
          icon: '/favicon.ico',
          badge: '/favicon.ico',
          timestamp: Date.now(),
          tag: notification.type, // Prevents duplicate notifications of same type
        });

        // Auto-close after 5 seconds
        setTimeout(() => {
          browserNotification.close();
        }, 5000);
      }
    });

    newConnection.on('UserOnline', (userId: number) => {
      console.log(`User ${userId} came online`);
      setOnlineUsers(prev => prev.includes(userId) ? prev : [...prev, userId]);
    });

    newConnection.on('UserOffline', (userId: number) => {
      console.log(`User ${userId} went offline`);
      setOnlineUsers(prev => prev.filter(id => id !== userId));
    });

    // Connection state handlers
    newConnection.onclose(() => {
      console.log('SignalR connection closed');
      setIsConnected(false);
    });

    newConnection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
      setIsConnected(false);
    });

    newConnection.onreconnected(() => {
      console.log('SignalR reconnected');
      setIsConnected(true);
    });

    // Start connection
    newConnection.start()
      .then(() => {
        console.log('SignalR connected successfully');
        setIsConnected(true);
        setConnection(newConnection);
      })
      .catch(err => {
        console.error('SignalR connection failed:', err);
        setIsConnected(false);
      });

    return () => {
      newConnection.stop();
    };
  }, [token, user]);

  // Helper functions
  const addNotificationListener = (listener: (notification: SignalRNotification) => void) => {
    notificationListeners.current.push(listener);
    return () => {
      notificationListeners.current = notificationListeners.current.filter(l => l !== listener);
    };
  };

  const clearNotifications = () => {
    setNotifications([]);
  };

  const markNotificationAsRead = (index: number) => {
    setNotifications(prev => prev.filter((_, i) => i !== index));
  };

  const sendNotificationToUser = async (targetUserId: number, message: string, type = 'info') => {
    if (connection && isConnected) {
      try {
        await connection.invoke('SendNotificationToUser', targetUserId, message, type);
      } catch (err) {
        console.error('Failed to send notification:', err);
      }
    }
  };

  const joinGroup = async (groupName: string) => {
    if (connection && isConnected) {
      try {
        await connection.invoke('JoinGroup', groupName);
      } catch (err) {
        console.error('Failed to join group:', err);
      }
    }
  };

  const leaveGroup = async (groupName: string) => {
    if (connection && isConnected) {
      try {
        await connection.invoke('LeaveGroup', groupName);
      } catch (err) {
        console.error('Failed to leave group:', err);
      }
    }
  };

  const isUserOnline = (userId: number) => {
    return onlineUsers.includes(userId);
  };

  const requestNotificationPermission = async () => {
    if (Notification.permission === 'default') {
      const permission = await Notification.requestPermission();
      return permission === 'granted';
    }
    return Notification.permission === 'granted';
  };

  return {
    connection,
    isConnected,
    notifications,
    onlineUsers,
    addNotificationListener,
    clearNotifications,
    markNotificationAsRead,
    sendNotificationToUser,
    joinGroup,
    leaveGroup,
    isUserOnline,
    requestNotificationPermission,
  };
};