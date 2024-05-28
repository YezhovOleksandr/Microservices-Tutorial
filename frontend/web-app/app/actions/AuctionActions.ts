'use server'

import { PagedResult, Auction, Bid } from "@/types";
import { getTokenWorkaround } from "./authActions";
import { fetchWrapper } from "@/app/lib/fetchWrapper";
import { FieldValues } from "react-hook-form";
import { revalidatePath } from "next/cache";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    console.log(query)
    return await fetchWrapper.get(`search${query}`)
}

export async function updateAuctionTest() {
    const data = {
        make: "Mustangssss",
        mileage: 1001
    }
    const token = await getTokenWorkaround();

    return await fetchWrapper.put('auctions/bbab4d5a-8565-48b1-9450-5ac2a5c4a654', data)
}

export async function createAuction(data: FieldValues) {
    return await fetchWrapper.post('auctions', data)
}

export async function getDetailedViewData(id: string): Promise<Auction> {
    return await fetchWrapper.get(`auctions/${id}`)
}

export async function updateAction(data: FieldValues, id: string) {
    const res = await fetchWrapper.put(`auctions/${id}`, data);
    revalidatePath(`/auctions/${id}`);
    return res;
}

export async function deleteAuction(id: string) {
    return await fetchWrapper.del(`auctions/${id}`);
}

export async function getBidsForAuction(id: string): Promise<Bid[]> {
    return await fetchWrapper.get(`bids/${id}`)
}

export async function placeBidForAuction(auctionId: string, amount:number) {
    return await fetchWrapper.post(`bids?auctionId=${auctionId}&amount=${amount}`, {})
}