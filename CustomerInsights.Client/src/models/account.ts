export interface Account {
    id: string;
    name: string;
    classification: 'A' | 'B' | 'C' | undefined
    industry: string;
    country: string;
}