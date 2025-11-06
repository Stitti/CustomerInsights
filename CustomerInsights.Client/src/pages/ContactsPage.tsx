import React, {useState} from "react";
import {Box, Button, Flex, Heading, Table, TextField} from "@radix-ui/themes";
import {SearchIcon} from "lucide-react";
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

                {/* Search Bar */}
                <Flex mb="4">
                    <TextField.Root
                        placeholder={t("contact_page.search_contacts")}
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
            </Box>
        </Flex>
    )
}