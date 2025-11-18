import type {AccountListResponse} from "@/src/models/responses/accountResponse.ts";
import type {ContactListResponse} from "@/src/models/responses/contactResponse.ts";

export interface topChannelResponse {
    channel: number,
    channelName: string,
    interactionCount: number,
}

export interface InteractionResponse {
    id: string,
    tenantId: string,
    source: string,
    externalId: string,
    channel: number,
    occurredAt: Date,
    analyzedAt: Date,
    account: AccountListResponse,
    contact: ContactListResponse,
    threadId: string,
    subject: string,
    text: string,
    meta: string,
    textInference: TextInference,
}

export interface TextInference {
    sentiment: string,
    sentimentScore: string,
    urgency: string,
    urgencyScore: string,
    aspects: { label: string, score: number }[],
    emotions: { label: string, score: number }[],
    inferredAt: Date,
    extra: string
}