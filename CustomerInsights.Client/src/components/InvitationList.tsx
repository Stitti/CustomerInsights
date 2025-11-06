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
    Select,
} from '@radix-ui/themes';
import {PlusIcon, SearchIcon, TrashIcon} from 'lucide-react';
import {useTranslation} from "react-i18next";

interface UserInvitation {
    email: string;
    role: 'admin' | 'member';
}

export function InvitationList() {
    const { t } = useTranslation();


    const [inviteOpen, setInviteOpen] = useState(false);
    const [newName, setNewName] = useState('');
    const [newEmail, setNewEmail] = useState('');
    const [newRole, setNewRole] = useState<UserInvitation['role']>('member');
    const [search, setSearch] = useState('');
    const filtered = []
    return (
        <Flex>
            <Box flexGrow="1" p="6">
                <Flex justify="between" align="center" mb="5">
                    <Heading mb="4">{t('sidebar.users')}</Heading>
                    <Dialog.Root>
                        <Dialog.Trigger>
                            <Button>
                                <PlusIcon /> {t('userList.inviteUser')}
                            </Button>
                        </Dialog.Trigger>

                        <Dialog.Content maxWidth="450px">
                            <Dialog.Title>Invite User</Dialog.Title>
                            <Dialog.Description size="2" mb="4">
                                {t('userList.inviteHint')}
                            </Dialog.Description>

                            <Text as="div" size="2" mb="1" weight="bold">
                                {t('user.email')}
                            </Text>
                            <TextField.Root placeholder="email address" value={newEmail} onChange={(e) => setNewEmail(e.target.value)} />

                            <Text as="div" size="2" mb="1" weight="bold">
                                {t('user.role')}
                            </Text>
                            <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                                <Select.Root defaultValue="member" value={newRole} onValueChange={(e) => setNewRole(e as "admin" | "member")}>
                                    <Select.Trigger />
                                    <Select.Content>
                                        <Select.Item value="member">Member</Select.Item>
                                        <Select.Item value="admin">Admin</Select.Item>
                                    </Select.Content>
                                </Select.Root>
                            </Flex>

                            <Flex gap="3" mt="4" justify="between">
                                <Dialog.Close>
                                    <Button variant="soft" color="gray">
                                        {t('general.cancel')}
                                    </Button>
                                </Dialog.Close>
                                <Button>{t('general.submit')}</Button>
                            </Flex>
                        </Dialog.Content>
                    </Dialog.Root>
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
                            <Table.ColumnHeaderCell>{t('user.email')}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t('user.role')}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell></Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {filtered?.map((c) => (
                            <Table.Row key={c.invitationCode} style={{ cursor: 'pointer' }}>
                                <Table.Cell>{c.email}</Table.Cell>
                                <Table.Cell>{c.role}</Table.Cell>
                                <Table.Cell>
                                    <IconButton variant="ghost">
                                        <TrashIcon />
                                    </IconButton>
                                </Table.Cell>
                            </Table.Row>
                        ))}
                    </Table.Body>
                </Table.Root>
            </Box>
        </Flex>
    );
}