'use client'

import { useAuctionStore } from '@/hooks/useAuctionStore';
import { useBidStore } from '@/hooks/useBidStore';
import { Auction, AuctionFinished, Bid } from '@/types';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { User } from 'next-auth';
import React, { ReactNode, useEffect, useState } from 'react'
import toast from 'react-hot-toast';
import AuctionCreatedToast from '../components/AuctionCreatedToast';
import { getDetailedViewData } from '../actions/AuctionActions';
import AuctionFinishedToast from '../components/AuctionFinishedToast';

type Props = {
    children: ReactNode | null
    user: User | null
}
export default function SignalRProvider({ children, user }: Props) {
    const [ connection, setConnection ] = useState<HubConnection | null>(null);
    const setCurrenctPrice = useAuctionStore(state => state.setCurrentPrice);

    const addBid = useBidStore(state => state.addBid);
    const apiUrl = process.env.NODE_ENV === 'production' ? 'https://api.carsties.com/notifications' : process.env.NEXT_PUBLIC_NOTIFY_URL

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(apiUrl!)
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, [ apiUrl ]);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected to notification hub');

                    connection.on('BidPlaced', (bid: Bid) => {
                        console.log('Bid placed event received');
                        if (bid.bidStatus.includes('Accepted')) {
                            setCurrenctPrice(bid.auctionId, bid.amount);
                        }
                        console.log(bid)
                        addBid(bid);
                    });
                    connection.on('AuctionCreated', (auction: Auction) => {
                        if (user?.username !== auction.seller) {
                            return toast(<AuctionCreatedToast auction={auction} />, { duration: 10000 })
                        }
                    })
                    connection.on('AuctionFinished', (auctionFinished: AuctionFinished) => {
                        const auction = getDetailedViewData(auctionFinished.auctionId);
                        return toast.promise(auction, {
                            loading: 'Loading',
                            success: (auction) => <AuctionFinishedToast
                                finishedAuction={auctionFinished}
                                auction={auction} />,
                            error: (err) => 'Auction Finished!'
                        }, { success: { duration: 10000, icon: null } })
                    })
                }).catch(error => toast.error(error));
        }

        return () => {
            connection?.stop();
        }
    }, [ connection, setCurrenctPrice, addBid, user?.username ]);
    //console.log(children);
    return (
        children
    )
}
