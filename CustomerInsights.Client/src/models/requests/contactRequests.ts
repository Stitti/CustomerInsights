export interface CreateContactRequest {
    firstname: string,
    lastname: string,
    email: string | null,
    phone: string | null,
    accountId: string | null,
    externalId: string | null
}

export interface UpdateContactRequest {
    firstname?: string | null,
    lastname?: string | null,
    email?: string | null,
    phone?: string | null,
    accountId?: string | null,
    externalId?: string | null
}