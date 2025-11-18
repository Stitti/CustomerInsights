import type {Interaction} from "@/src/models/interaction.ts";
import type {AccountListResponse} from "@/src/models/responses/accountResponse.ts";

export interface ContactListResponse {
    id: string,
    firstname: string,
    lastname: string,
    email: string,
    phone: string,
    accountId: string,
    accountName: string,
    createdAt: Date
}

export interface ContactResponse {
    id: string,
    firstname: string,
    lastname: string,
    email: string,
    phone: string,
    account: AccountListResponse,
    interactions: Interaction[],
}