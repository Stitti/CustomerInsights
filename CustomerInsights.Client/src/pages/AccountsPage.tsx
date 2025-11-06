import React, {useState} from "react";
import {Box, Button, Flex, Heading, Table, TextField} from "@radix-ui/themes";
import {SearchIcon} from "lucide-react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router-dom";
import {NIL as NIL_UUID} from 'uuid';
import {mockAccounts} from "../mock/mockAccounts";
import type {Account} from "../models/account.ts";

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
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Accounts</Heading>
                    <Button size="2" onClick={() => navigate(NIL_UUID)} style={{cursor: "pointer"}}>+ Add Account</Button>
                </Flex>

                {/* Search Bar */}
                <Flex mb="4">
                    <TextField.Root
                        placeholder={t("account_page.search_accounts")}
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        style={{
                            flex: 1,
                            padding: '8px 12px',
                            borderRadius: 6,
                            border: '1px solid var(--gray-200)',
                        }}
                    />
                    <Button variant="ghost" size="2" style={{ marginLeft: 8 }}>
                        <SearchIcon />
                    </Button>
                </Flex>
                <Table.Root>
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
            </Box>
        </Flex>
    )
}