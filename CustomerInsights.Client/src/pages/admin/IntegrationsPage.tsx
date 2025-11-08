import {Box, Button, Card, Flex, Heading, IconButton, Table, TextField} from "@radix-ui/themes";
import {PlusIcon, SearchIcon, XIcon} from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

export default function IntegrationsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { t } = useTranslation();

    const filtered = [];

    return (
        <Flex style={{ minHeight: '100vh' }}>
            <Box flexGrow="1" p="6">
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

                        <IconButton variant="surface" onClick={() => setSearch("")}>
                            <PlusIcon size={20} />
                        </IconButton>
                    </Flex>
                    <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>{t("contact_page.name")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("contact_page.email")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("contact_page.phone")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("contact_page.company")}</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {
                            filtered.map((contact) => (
                                <Table.Row
                                    key={contact.id}
                                    onClick={() => navigate(`/contacts/${contact.id}`)}
                                    style={{ cursor: 'pointer' }}
                                >
                                    <Table.Cell>{contact.firstname + " " + contact.lastname}</Table.Cell>
                                    <Table.Cell>{contact.email}</Table.Cell>
                                    <Table.Cell>{contact.phone}</Table.Cell>
                                    <Table.Cell>{contact.company}</Table.Cell>
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