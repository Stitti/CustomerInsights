export interface CreateAccountRequest {
    name: string,
    externalId: string | null,
    parentAccountId: string | null,
    industry: string | null,
    country: string | null,
    classification: string | null
}

export interface UpdateAccountRequest {
    name?: string | null,
    externalId?: string | null,
    parentAccountId?: string | null,
    industry?: string | null,
    country?: string | null,
    classification?: string | null
}