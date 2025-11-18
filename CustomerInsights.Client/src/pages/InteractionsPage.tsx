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
import { getAllInteractions } from "../services/interactionService";
import type {InteractionResponse} from "@/src/models/responses/interactionResponse.ts";
import {formatDateNumeric} from "../utils/dateUtils";

export default function InteractionsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState("");
    const [interactions, setInteractions] = useState<InteractionResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const { t } = useTranslation();

    useEffect(() => {
        let mounted = true;
        setLoading(true);

        getAllInteractions()
            .then((data) => {
                if (mounted) setInteractions(data ?? []);
            })
            .catch((err) => {
                console.error("Error loading interactions:", err);
                if (mounted) setInteractions([]);
            })
            .finally(() => {
                if (mounted) setLoading(false);
            });

        return () => {
            mounted = false;
        };
    }, []);

    const filtered = interactions.filter((i) =>
        i.subject?.toLowerCase()?.includes(search.toLowerCase())
    );

    return (
        <Flex style={{ minHeight: "100vh" }}>
            <Box flexGrow="1" p="6">
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Interactions</Heading>
                    <Button
                        size="2"
                        onClick={() => navigate(NIL_UUID)}
                        style={{ cursor: "pointer" }}
                    >
                        + Add Interaction
                    </Button>
                </Flex>

                <Card variant="surface" style={{ marginTop: 16 }}>
                    {/* Filter / Suche */}
                    <Flex mb="4" gap="3" align="center" wrap="wrap">
                        <TextField.Root
                            placeholder={t("interaction_page.search_interactions") ?? "Suchen..."}
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
                                    {t("interaction_page.title")}
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    {t("interaction_page.channel")}
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    {t("interaction_page.contact_name")}
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    {t("interaction_page.company_name")}
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    {t("interaction_page.occurred_at")}
                                </Table.ColumnHeaderCell>
                            </Table.Row>
                        </Table.Header>

                        <Table.Body>
                            {loading
                                ? // Skeleton-Zeilen während Ladevorgang
                                Array.from({ length: 6 }).map((_, i) => (
                                    <Table.Row key={`interaction-skel-${i}`}>
                                        <Table.Cell>
                                            <Skeleton width="60%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="40%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="50%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="45%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="30%" />
                                        </Table.Cell>
                                    </Table.Row>
                                ))
                                : filtered.map((interaction) => (
                                    <Table.Row
                                        key={interaction.id}
                                        onClick={() => navigate(`/interactions/${interaction.id}`)}
                                        style={{ cursor: "pointer" }}
                                    >
                                        <Table.Cell>{interaction.subject}</Table.Cell>
                                        <Table.Cell>{interaction.channel}</Table.Cell>
                                        <Table.Cell>{interaction.contactId}</Table.Cell>
                                        <Table.Cell>{interaction.accountId}</Table.Cell>
                                        <Table.Cell>{interaction.occurredAt ? formatDateNumeric.format(new Date(interaction.occurredAt)): ''}</Table.Cell>
                                    </Table.Row>
                                ))}
                        </Table.Body>
                    </Table.Root>
                </Card>
            </Box>
        </Flex>
    );
}