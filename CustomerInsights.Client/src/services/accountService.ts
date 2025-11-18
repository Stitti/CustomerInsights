import api from "./api";
import type {AccountListResponse, AccountResponse} from "@/src/models/responses/accountResponse.ts";
import type {CreateAccountRequest, UpdateAccountRequest} from "@/src/models/requests/accountRequests.ts";

export async function getAllAccounts(): Promise<AccountListResponse[]> {
    const response = await api.get<AccountListResponse[]>("/accounts");
    return response.data;
}

export async function getAccountById(id: string): Promise<AccountResponse> {
    if (!id)
        return null;

    const response = await api.get<AccountResponse>(`/accounts/${id}`);
    return response.data;
}

export async function createAccount(request: CreateAccountRequest): Promise<void> {
    if (!request)
        return;

    await api.post(`/accounts`, request);
}

export async function  patchAccount(id: string, patch: UpdateAccountRequest): Promise<void> {
    if (!id || !patch)
        return;

    await api.patch(`/accounts/${id}`, patch, {
        transformRequest: [(data) => JSON.stringify(data)]
    })
}

export async function deleteAccountById(id: string): Promise<void> {
    if (!id)
        return;

    await api.delete(`/accounts/${id}`);
}