export interface Interaction {
    id: string;
    title: string;
    channel: string;
    contactName: string;
    companyName: string;
    occurredAt: string; // ISO-Datum als string, z. B. "2025-10-30T10:30:00Z"
}