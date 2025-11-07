import React, {useState} from "react";
import {Box, Button, Card, Flex, Heading, IconButton, Table, TextField} from "@radix-ui/themes";
import {SearchIcon, XIcon} from "lucide-react";
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
                </Card>
            </Box>
        </Flex>
    )
}