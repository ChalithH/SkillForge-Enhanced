import { useMemo } from 'react';
import { useGetExchangesQuery } from '../store/api/apiSlice';
import { useAppSelector } from '../store/hooks';
import { ExchangeStatus } from '../types';

export const usePendingRequests = () => {
  const currentUser = useAppSelector((state) => state.auth.user);
  const { data: exchanges = [], isLoading, error } = useGetExchangesQuery(undefined);

  const pendingRequests = useMemo(() => {
    if (!currentUser || !exchanges.length) {
      return {
        incomingRequests: [],
        outgoingRequests: [],
        incomingCount: 0,
        outgoingCount: 0,
        totalPendingCount: 0
      };
    }

    // Incoming requests: I'm the offerer and someone wants to learn from me
    const incoming = exchanges.filter(
      exchange => 
        exchange.status === ExchangeStatus.Pending && 
        exchange.offererId === currentUser.id
    );

    // Outgoing requests: I'm the learner and waiting for someone to accept
    const outgoing = exchanges.filter(
      exchange => 
        exchange.status === ExchangeStatus.Pending && 
        exchange.learnerId === currentUser.id
    );

    return {
      incomingRequests: incoming,
      outgoingRequests: outgoing,
      incomingCount: incoming.length,
      outgoingCount: outgoing.length,
      totalPendingCount: incoming.length + outgoing.length
    };
  }, [exchanges, currentUser]);

  return {
    ...pendingRequests,
    isLoading,
    error,
    refetch: () => {
      // The query will automatically refetch based on cache invalidation
    }
  };
};