import React, {useState} from "react";
import {Box, Button, Card, Flex, Heading, IconButton, Table, TextField} from "@radix-ui/themes";
import {SearchIcon, XIcon} from "lucide-react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router-dom";
import {NIL as NIL_UUID} from 'uuid';
import {mockAccounts} from "../mock/mockAccounts";
import type {Account} from "../models/account.ts";
import {MetricsTrends} from "../components/MetricsTrends";

export default function AccountsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { t } = useTranslation();

    const accounts = mockAccounts;

    const filtered = accounts?.filter(
        (a: Account) =>
            (a.name).toLowerCase().includes(search.toLowerCase())
    );

    return (
        <Flex style={{ minHeight: '100vh' }}>
            <Box flexGrow="1" p="6">
                <MetricsTrends/>
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Accounts</Heading>
                    <Button size="2" onClick={() => navigate(NIL_UUID)} style={{cursor: "pointer"}}>+ Add Account</Button>
                </Flex>
                <Card variant="surface" style={{ marginTop: 16 }}>
                {/* Filter / Suche */}
                <Flex mb="4" gap="3" align="center" wrap="wrap">
                    <TextField.Root
                        placeholder={t("contact_page.search_contacts") ?? "Suchen..."}
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        style={{ flex: 1 }}
                    >
                        <TextField.Slot>
                            <SearchIcon size={16} />
                        </TextField.Slot>
                    </TextField.Root>

                    <IconButton variant="ghost" onClick={() => setSearch("")}>
                        <XIcon color="grey" size={20} />
                    </IconButton>
                </Flex>

                    <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>{t("account_page.name")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("account_page.email")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("account_page.phone")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("account_page.company")}</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {
                            filtered.map((account: Account) => (
                                <Table.Row
                                    key={account.id}
                                    onClick={() => navigate(`/accounts/${account.id}`)}
                                    style={{ cursor: 'pointer' }}
                                >
                                    <Table.Cell>{account.name}</Table.Cell>
                                    <Table.Cell>{account.classification}</Table.Cell>
                                    <Table.Cell>{account.industry}</Table.Cell>
                                    <Table.Cell>{account.country}</Table.Cell>
                                </Table.Row>
                            ))
                        }
                    </Table.Body>
                </Table.Root>
                </Card>
            </Box>
        </Flex>
    )
}