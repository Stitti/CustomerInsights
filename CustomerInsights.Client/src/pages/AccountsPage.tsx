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
import { MetricsTrends } from "../components/MetricsTrends";
import { getAllAccounts } from "../services/accountService";
import type {AccountListResponse} from "../models/responses/accountResponse";
import {formatDateNumeric} from "../utils/dateUtils";

export default function AccountsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState("");
    const [accounts, setAccounts] = useState<AccountListResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const { t } = useTranslation();

    useEffect(() => {
        let mounted = true;
        setLoading(true);

        getAllAccounts()
            .then((data) => {
                if (mounted) {
                    setAccounts(data ?? []);
                }
            })
            .catch((err) => {
                console.error("Error loading accounts:", err);
            })
            .finally(() => {
                if (mounted) setLoading(false);
            });

        return () => {
            mounted = false;
        };
    }, []);

    const filtered = accounts?.filter((a: AccountListResponse) =>
        a.name.toLowerCase().includes(search.toLowerCase())
    );

    return (
        <Flex style={{ minHeight: "100vh" }}>
            <Box flexGrow="1" p="6">
                <MetricsTrends />
                <Flex justify="between" align="center" mb="5">
                    <Heading size="5">Accounts</Heading>
                    <Button
                        size="2"
                        onClick={() => navigate(NIL_UUID)}
                        style={{ cursor: "pointer" }}
                    >
                        + Add Account
                    </Button>
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
                                    Classification
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Industry
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Country
                                </Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>
                                    Created At
                                </Table.ColumnHeaderCell>
                            </Table.Row>
                        </Table.Header>

                        <Table.Body>
                            {loading
                                ? // Skeleton Rows anzeigen
                                Array.from({ length: 5 }).map((_, idx) => (
                                    <Table.Row key={`skeleton-${idx}`}>
                                        <Table.Cell>
                                            <Skeleton width="80%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="70%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="50%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="60%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="60%" />
                                        </Table.Cell>
                                    </Table.Row>
                                ))
                                : filtered.map((account: AccountListResponse) => (
                                    <Table.Row
                                        key={account.id}
                                        onClick={() => navigate(`/accounts/${account.id}`)}
                                        style={{ cursor: "pointer" }}
                                    >
                                        <Table.Cell>{account.name}</Table.Cell>
                                        <Table.Cell>{account.classification}</Table.Cell>
                                        <Table.Cell>{account.industry}</Table.Cell>
                                        <Table.Cell>{account.country}</Table.Cell>
                                        <Table.Cell>{account.createdAt ? formatDateNumeric.format(new Date(account.createdAt)): ''}</Table.Cell>
                                    </Table.Row>
                                ))}
                        </Table.Body>
                    </Table.Root>
                </Card>
            </Box>
        </Flex>
    );
}