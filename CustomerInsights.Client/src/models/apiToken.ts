export interface ApiToken {
    id: string;
    name: string;
    description: string;
    expiresAt: Date;
    isRevoked: boolean;
    createdAt: Date;
    modifiedAt: Date;
}
