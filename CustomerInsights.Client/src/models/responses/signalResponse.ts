import type {AccountListResponse} from "@/src/models/responses/accountListResponse.ts";

export interface SignalResponse {
    id: string,
    type: string,
    account: AccountListResponse,
    severity: string,
    createdAt: Date,
    ttlDays: number,
    accountSatisfactionIndex: number,
    threshold: number,
}