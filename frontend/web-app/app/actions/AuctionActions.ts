'use server'

import { PagedResult, Auction } from "@/types";
import { getTokenWorkaround } from "./authActions";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    const res = await fetch(`http://localhost:6001/search${query}`)

    if (!res.ok) throw new Error('failed to fetch data')

    return res.json();
}

export async function UpdateAuctionTest() {
    const data = {
        make: "Mustangssss",
        mileage: 1001
    }
    const token = await getTokenWorkaround();

    console.log(token)

    const res = await fetch('http://localhost:6001/auctions/bbab4d5a-8565-48b1-9450-5ac2a5c4a654', {
        method: 'PUT',
        headers: {
            'Authorization': 'Bearer ' + token?.access_token,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    if (!res.ok) return { status: res.status, message: res.statusText, StatusCode: res.status, megaMessage: res.type }

    return res.statusText
}