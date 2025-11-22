import React, { useEffect, useState } from "react";
import {
    Box,
    Button,
    Card,
    Flex,
    Heading,
    IconButton,
    Table,
    TextField,
    Skeleton,
} from "@radix-ui/themes";
import { SearchIcon, XIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { NIL as NIL_UUID } from "uuid";
import { getAllContacts } from "../services/contactService";
import type {ContactListResponse} from "../models/responses/contactResponse";
import {formatDateNumeric} from "../utils/dateUtils";
import CsvImportDialog from "../components/CsvImportDialog";

export default function ContactsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState("");
    const [contacts, setContacts] = useState<ContactListResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const { t } = useTranslation();

    useEffect(() => {
        let mounted = true;
        setLoading(true);

        getAllContacts()
            .then((data) => {
                if (mounted) setContacts(data ?? []);
            })
            .catch((err) => {
                console.error("Error loading contacts:", err);
                if (mounted) setContacts([]);
            })
            .finally(() => {
                if (mounted) setLoading(false);
            });

        return () => {
            mounted = false;
        };
    }, []);

    const filtered = contacts.filter((c) => {
        const q = search.toLowerCase();
        return (
            `${c.firstname} ${c.lastname}`.toLowerCase().includes(q) ||
            (c.email ?? "").toLowerCase().includes(q) ||
            (c.phone ?? "").toLowerCase().includes(q) ||
            (c.accountName ?? "").toLowerCase().includes(q)
        );
    });

    return (
        <Flex style={{ minHeight: "100vh" }}>
            <Box flexGrow="1" p="6">
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Contacts</Heading>
                    <Flex gap="2">
                        <Button
                            size="2"
                            onClick={() => navigate(NIL_UUID)}
                            style={{ cursor: "pointer" }}
                        >
                            + Add Contact
                        </Button>
                        <CsvImportDialog/>
                    </Flex>
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
                                <Table.ColumnHeaderCell>
                                    Name
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Email
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Phone
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Account
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Created At
                                </Table.ColumnHeaderCell>
                            </Table.Row>
                        </Table.Header>

                        <Table.Body>
                            {loading
                                ? // Skeleton-Placeholders während des Ladens
                                Array.from({ length: 6 }).map((_, i) => (
                                    <Table.Row key={`contact-skel-${i}`}>
                                        <Table.Cell>
                                            <Skeleton width="60%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="70%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="40%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="55%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="55%" />
                                        </Table.Cell>
                                    </Table.Row>
                                ))
                                : filtered.map((contact) => (
                                    <Table.Row
                                        key={contact.id}
                                        onClick={() => navigate(`/contacts/${contact.id}`)}
                                        style={{ cursor: "pointer" }}
                                    >
                                        <Table.Cell>
                                            {`${contact.firstname} ${contact.lastname}`}
                                        </Table.Cell>
                                        <Table.Cell>{contact.email}</Table.Cell>
                                        <Table.Cell>{contact.phone}</Table.Cell>
                                        <Table.Cell>{contact.accountName}</Table.Cell>
                                        <Table.Cell>{contact.createdAt ? formatDateNumeric.format(new Date(contact.createdAt)): ''}</Table.Cell>
                                    </Table.Row>
                                ))}
                        </Table.Body>
                    </Table.Root>
                </Card>
            </Box>
        </Flex>
    );
}