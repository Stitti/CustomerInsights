import React from 'react';
import {
    Box,
    Flex,
    Heading,
    Button,
    Card,
    Table,
    Badge,
} from '@radix-ui/themes';
import { PlusIcon, EditIcon, TrashIcon } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function ApiKeyManagementPage() {
    const navigate = useNavigate();
    const apiKeys = []
    return (
        <Flex style={{ minHeight: '100vh', background: 'var(--gray-50)' }}>
            <Box flexGrow="1" p="6" style={{ maxWidth: 1200, margin: '0 auto' }}>
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">API Key Management</Heading>
                    <Button variant="outline" size="2" style={{ gap: 4 }}>
                        <PlusIcon /> Generate Key
                    </Button>
                </Flex>

                <Card variant="surface" size="3">
                        <Table.Root>
                            <Table.Header>
                                <Table.Row>
                                    <Table.ColumnHeaderCell>Key Name</Table.ColumnHeaderCell>
                                    <Table.ColumnHeaderCell>Description</Table.ColumnHeaderCell>
                                    <Table.ColumnHeaderCell>Expires</Table.ColumnHeaderCell>
                                    <Table.ColumnHeaderCell>Created At</Table.ColumnHeaderCell>
                                    <Table.ColumnHeaderCell>Revoked</Table.ColumnHeaderCell>
                                </Table.Row>
                            </Table.Header>
                            <Table.Body>
                                {apiKeys?.map((key) => (
                                    <Table.Row
                                        key={key.id}
                                        onClick={() => navigate(`/admin-center/api-keys/${key.id}`)}
                                        style={{ cursor: 'pointer' }}
                                    >
                                        <Table.Cell>{key.name}</Table.Cell>
                                        <Table.Cell>{key.description?.slice(0, 10) ?? '—'}</Table.Cell>
                                        <Table.Cell>
                                            <Badge
                                                variant="soft"
                                                color={key.expiresAt ? 'green' : 'gray'}
                                            >
                                                {key.expiresAt ? key.expiresAt.toString().slice(0, 10) : 'Never'}
                                            </Badge>
                                        </Table.Cell>
                                        <Table.Cell>{key.createdAt.toString()}</Table.Cell>
                                        <Table.Cell>{key.isRevoked}</Table.Cell>
                                        <Table.Cell>
                                            <Flex gap="2">
                                                <Button variant="ghost" size="2" disabled>
                                                    <TrashIcon />
                                                </Button>
                                            </Flex>
                                        </Table.Cell>
                                    </Table.Row>
                                ))}
                            </Table.Body>
                        </Table.Root>
                </Card>
            </Box>
        </Flex>
    );
}