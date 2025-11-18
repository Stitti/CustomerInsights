import api from "./api";
import type {ContactListResponse} from "@/src/models/responses/contactListResponse.ts";
import type {ContactResponse} from "@/src/models/responses/contactResponse.ts";
import type {CreateContactRequest, UpdateContactRequest} from "@/src/models/requests/contactRequests.ts";

export async function getAllContacts(): Promise<ContactListResponse[]> {
    const response = await api.get<ContactListResponse[]>("/contacts");
    return response.data;
}

export async function getContactById(id: string): Promise<ContactResponse> {
    if (!id)
        return null;

    const response = await api.get<ContactResponse>(`/contacts/${id}`);
    return response.data;
}

export async function createContact(request: CreateContactRequest): Promise<void> {
    if (!request)
        return;

    await api.post(`/contacts`, request);
}

export async function  patchContact(id: string, patch: UpdateContactRequest): Promise<void> {
    if (!id || !patch)
        return;

    await api.patch(`/contacts/${id}`, patch, {
        transformRequest: [(data) => JSON.stringify(data)]
    })
}

export async function deleteContactById(id: string): Promise<void> {
    if (!id)
        return;

    await api.delete(`/contacts/${id}`);
}