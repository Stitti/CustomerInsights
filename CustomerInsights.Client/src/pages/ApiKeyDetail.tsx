import React, { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
    Box,
    Flex,
    Heading,
    Button,
    Card,
    TextField,
    Badge,
} from '@radix-ui/themes';
import { ChevronLeftIcon } from 'lucide-react';

import {ApiToken} from "../models/apiToken";

export default function ApiKeyDetail() {
    const navigate = useNavigate();
    const { keyId } = useParams<{ keyId: string }>();


    const [isEditing, setIsEditing] = useState(false);
    const [form, setForm] = useState<Partial<ApiToken>>({});

    const handleChange = (field: keyof ApiToken) => (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm((prev) => ({ ...prev, [field]: e.target.value }));
    };

    return (
        <Flex style={{ minHeight: '100vh', background: 'var(--gray-50)' }}>
            <Box flexGrow="1" p="6" style={{ maxWidth: 800, margin: '0 auto' }}>
                <Flex justify="between" align="center" mb="5">
                    <Flex align="center" gap="3">
                        <Button variant="ghost" size="2" onClick={() => navigate(-1)}>
                            <ChevronLeftIcon />
                        </Button>
                        <Heading size="5">API Key: {"name"}</Heading>
                    </Flex>
                    <Flex gap="2">
                        {!isEditing ? (
                            <Button variant="outline" size="2" onClick={() => setIsEditing(true)}>
                                Edit
                            </Button>
                        ) : (
                            <Button color="green" size="2">
                                Speichern
                            </Button>
                        )}
                        <Button color="red" size="2">
                            Revoke Key
                        </Button>
                    </Flex>
                </Flex>

                <Card variant="surface" size="3" mb="5">
                    <Heading size="4" style={{ padding: '1rem 1rem 0', marginBottom: '1rem' }}>
                        Key Information
                    </Heading>
                    <Flex direction="column" gap="4" p="4">
                        <TextField.Root
                            placeholder="Name"
                            disabled={!isEditing}
                            onChange={handleChange('name')}
                        />
                        <TextField.Root
                            placeholder="Description"
                            disabled={!isEditing}
                            onChange={handleChange('description')}
                        />
                        <Flex gap="4">
                            <Box>
                                <TextField.Root
                                    type="date"
                                    disabled
                                />
                            </Box>
                            <Box>
                                <Flex align="center" gap="2">
                                    <Badge color="green" variant="soft">Expires At</Badge>
                                    <TextField.Root
                                        type="date"
                                        disabled={!isEditing}
                                        onChange={handleChange('expiresAt')}
                                    />
                                </Flex>
                            </Box>
                        </Flex>
                    </Flex>
                </Card>
            </Box>
        </Flex>
    );
}
