import React, {useState} from "react";
import {Box, Button, Flex, Heading, Table, TextField} from "@radix-ui/themes";
import {SearchIcon} from "lucide-react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router-dom";
import {NIL as NIL_UUID} from 'uuid';
import {mockInteractions} from "../mock/mockInteractions";
import type {Interaction} from "../models/interaction";

export default function InteractionsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { t } = useTranslation();

    const interactions = mockInteractions;

    const filtered = interactions?.filter(
        (a: Interaction) =>
            (a.title).toLowerCase().includes(search.toLowerCase())
    );

    return (
        <Flex style={{ minHeight: '100vh' }}>
            <Box flexGrow="1" p="6">
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Interactions</Heading>
                    <Button size="2" onClick={() => navigate(NIL_UUID)} style={{cursor: "pointer"}}>+ Add Interaction</Button>
                </Flex>

                {/* Search Bar */}
                <Flex mb="4">
                    <TextField.Root
                        placeholder={t("interaction_page.search_interactions")}
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
                            <Table.ColumnHeaderCell>{t("interaction_page.title")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("interaction_page.channel")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("interaction_page.contact_name")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("interaction_page.company_name")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("interaction_page.occurred_at")}</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {
                            filtered.map((interaction: Interaction) => (
                                <Table.Row
                                    key={interaction.id}
                                    onClick={() => navigate(`/interactions/${interaction.id}`)}
                                    style={{ cursor: 'pointer' }}
                                >
                                    <Table.Cell>{interaction.title}</Table.Cell>
                                    <Table.Cell>{interaction.channel}</Table.Cell>
                                    <Table.Cell>{interaction.contactName}</Table.Cell>
                                    <Table.Cell>{interaction.companyName}</Table.Cell>
                                    <Table.Cell>{interaction.occurredAt}</Table.Cell>
                                </Table.Row>
                            ))
                        }
                    </Table.Body>
                </Table.Root>
            </Box>
        </Flex>
    )
}