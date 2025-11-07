import React, {useState} from "react";
import {Box, Button, Card, Flex, Heading, IconButton, Table, TextField} from "@radix-ui/themes";
import {SearchIcon, XIcon} from "lucide-react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router-dom";
import {NIL as NIL_UUID} from 'uuid';
import { mockContacts } from "../mock/mockContacts";
import type {Contact} from "../models/contact";

export default function ContactsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { t } = useTranslation();

    const contacts = mockContacts;

    const filtered = contacts?.filter(
        (c: Contact) =>
            (c.firstname + " " + c.lastname).toLowerCase().includes(search.toLowerCase()) ||
            c.email.toLowerCase().includes(search.toLowerCase()) ||
            c.phone.includes(search.toLowerCase())
    );

    return (
        <Flex style={{ minHeight: '100vh' }}>
            <Box flexGrow="1" p="6">
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Contacts</Heading>
                    <Button size="2" onClick={() => navigate(NIL_UUID)} style={{cursor: "pointer"}}>+ Add Contact</Button>
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
                            <Table.ColumnHeaderCell>{t("contact_page.name")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("contact_page.email")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("contact_page.phone")}</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>{t("contact_page.company")}</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {
                            filtered.map((contact: Contact) => (
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