import React, { useState } from 'react';
import {
    Table,
    Box,
    Flex,
    Heading,
    Text,
    Button,
    TextField,
    IconButton,
    Dialog,
    DropdownMenu,
    Select,
} from '@radix-ui/themes';
import {EllipsisIcon, PlusIcon, SearchIcon} from 'lucide-react';
import {useTranslation} from "react-i18next";
import {formatDate, formatDateNumeric} from "../utils/dateUtils";

interface User {
    id: string;
    name: string;
    email: string;
    role: 'Admin' | 'Editor' | 'Viewer';
}

export function UserList() {
    const { t } = useTranslation();


    const [inviteOpen, setInviteOpen] = useState(false);
    const [newName, setNewName] = useState('');
    const [newEmail, setNewEmail] = useState('');
    const [newRole, setNewRole] = useState<User['role']>('Viewer');
    const [search, setSearch] = useState('');


    const filtered = []

    return (
        <Flex>
            <Box flexGrow="1" p="6">
                <Flex justify="between" align="center" mb="5">
                    <Heading mb="4">{t('sidebar.users')}</Heading>
                </Flex>

                {/* Search Bar */}
                <Flex mb="4">
                    <TextField.Root placeholder={t('userList.searchUser')} value={search} onChange={(e) => setSearch(e.target.value)} style={{flex: 1, padding: '8px 12px', borderRadius: 6, border: '1px solid var(--gray-200)'}}><TextField.Slot>
                        <SearchIcon height="16" width="16" />
                    </TextField.Slot>
                    </TextField.Root>

                </Flex>
                <Table.Root>
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>{t('user.name')}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t('user.email')}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t('user.role')}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t('user.created_at')}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell></Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {filtered?.map((c) => (
                            <Table.Row key={c.uid} style={{ cursor: 'pointer' }}>
                                <Table.Cell>{c.displayName}</Table.Cell>
                                <Table.Cell>{c.email}</Table.Cell>
                                <Table.Cell>{c.role}</Table.Cell>
                                <Table.Cell>{c.createdAt ? formatDateNumeric.format(new Date(c.createdAt)) : '—'}</Table.Cell>
                                <Table.Cell>
                                    <DropdownMenu.Root>
                                        <DropdownMenu.Trigger>
                                            <IconButton variant="ghost">
                                                <EllipsisIcon/>
                                            </IconButton>
                                        </DropdownMenu.Trigger>
                                        <DropdownMenu.Content color="gray" highContrast>
                                            <DropdownMenu.Item style={{cursor: "pointer"}}>{t('userList.managePermissions')}</DropdownMenu.Item>
                                            <DropdownMenu.Item style={{cursor: "pointer"}}>{t('userList.delete')}</DropdownMenu.Item>
                                        </DropdownMenu.Content>
                                    </DropdownMenu.Root>
                                </Table.Cell>
                            </Table.Row>
                        ))}
                    </Table.Body>
                </Table.Root>
            </Box>
        </Flex>
    );
}