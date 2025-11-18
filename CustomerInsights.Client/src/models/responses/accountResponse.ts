import type {ContactListResponse} from "@/src/models/responses/contactResponse.ts";

export interface AccountListResponse {
    id: string,
    name: string
    industry: string,
    country: string,
    classification: number,
    parentAccountId: string,
    parentAccountName: string,
    createdAt: Date,
}

export interface AccountResponse {
    id: string,
    externalId: string,
    name: string,
    parentAccount: AccountListResponse,
    industry: string,
    country: string,
    classification: number,
    createdAt: Date,
    contacts: ContactListResponse[],
    satisfactionState: SatisfactionState,
}

export interface SatisfactionState {
    satisfactionIndex: number
}