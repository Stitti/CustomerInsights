import React, { useEffect, useState } from "react";
import {
    Box,
    Card,
    Code,
    DataList,
    Flex,
    Heading,
    IconButton,
    Table,
    Text,
    TextField,
    Skeleton,
} from "@radix-ui/themes";
import { CopyIcon } from "lucide-react";
import {useNavigate, useParams} from "react-router-dom";
import { MetricsTrends } from "../components/MetricsTrends";
import ModifiedCard from "../components/ModifiedCard";
import BackButton from "../components/BackButton";
import { getAccountById } from "../services/accountService";
import type {AccountResponse} from "@/src/models/responses/accountResponse.ts";
import Header from "../components/headers/Header";

const CLASSIFICATION_LABEL: Record<number | string, string> = {
    1: "A",
    2: "B",
    3: "C",
};

export function AccountDetailPage() {
    const { accountId } = useParams<{ accountId: string }>();
    const [account, setAccount] = useState<AccountResponse | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [copyOK, setCopyOK] = useState<boolean>(false);
    const navigate = useNavigate();

    useEffect(() => {
        let mounted = true;
        setLoading(true);
        console.log(accountId)
        getAccountById(accountId!)
            .then((data) => {
                console.log(data);
                if (mounted)
                    setAccount(data ?? null);
            })
            .catch((err) => {
                console.error("Error loading account:", err);
                if (mounted)
                    setAccount(null);
            })
            .finally(() => {
                if (mounted)
                    setLoading(false);
            });

        return () => {
            mounted = false;
        };
    }, [accountId]);

    const displayName = account?.name ?? "";
    const displayClassification =
        CLASSIFICATION_LABEL[account?.classification ?? ""] ??
        String(account?.classification ?? "");
    const displayIndustry = account?.industry ?? "";
    const displayCountry = account?.country ?? "";
    const externalId = account?.externalId ?? "";
    const contacts = account?.contacts ?? [];
    const interactions = [];
    const satisfactionIndex = account?.satisfactionState?.satisfactionIndex;

    const onCopyExternalId = async () => {
        try {
            await navigator.clipboard.writeText(externalId);
            setCopyOK(true);
            setTimeout(() => setCopyOK(false), 1500);
        } catch (e) {
            console.error("Copy failed", e);
        }
    };

    return (
        <Box flexGrow="1" p="6">
            <BackButton />
            <Flex gap="3" direction="column" wrap="wrap">
                <Header title={displayName} showTimeInterval={true} showDelete={true} showSave={true} showRefresh={true} />
                <MetricsTrends accountId={accountId} />

                <Card style={{ flex: 1, minWidth: "100%" }} variant="ghost" mb="4">
                    <Flex direction="row" gap="8" wrap="wrap">
                        <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                            <DataList.Root size="3">
                                {/* Name */}
                                <DataList.Item align="center">
                                    <DataList.Label>Name</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root
                                                style={{ width: "100%" }}
                                                disabled
                                                type="text"
                                                value={displayName}
                                            />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Classification */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Classification</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root
                                                style={{ width: "100%" }}
                                                disabled
                                                type="text"
                                                value={displayClassification}
                                            />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Industry */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Industry</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root
                                                style={{ width: "100%" }}
                                                disabled
                                                type="text"
                                                value={displayIndustry}
                                            />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Country */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Country</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root
                                                style={{ width: "100%" }}
                                                disabled
                                                type="text"
                                                value={displayCountry}
                                            />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* External ID */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">External ID</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <Flex align="center" gap="2">
                                                    <Code variant="ghost">XXXXXXXXXXXX</Code>
                                                    <IconButton size="1" aria-label="Copy value" color="gray" variant="ghost">
                                                        <CopyIcon size="16" />
                                                    </IconButton>
                                                </Flex>
                                            </Skeleton>
                                        ) : (
                                            <Flex align="center" gap="2">
                                                <Code variant="ghost">{externalId}</Code>
                                                <IconButton
                                                    size="1"
                                                    aria-label={copyOK ? "Copied" : "Copy value"}
                                                    color={copyOK ? "green" : "gray"}
                                                    variant="ghost"
                                                    onClick={onCopyExternalId}
                                                    title={copyOK ? "Copied!" : "Copy to clipboard"}
                                                >
                                                    <CopyIcon size="16" />
                                                </IconButton>
                                            </Flex>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>
                            </DataList.Root>
                        </Card>
                    </Flex>
                </Card>

                {/* Contacts */}
                <Card variant="surface" size="3" mb="6">
                    <Heading size="5" mb="3">
                        Contacts
                    </Heading>
                    <Table.Root>
                        <Table.Header>
                            <Table.Row>
                                <Table.ColumnHeaderCell>Name</Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>Email</Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>Phone</Table.ColumnHeaderCell>
                            </Table.Row>
                        </Table.Header>
                        <Table.Body>
                            {loading
                                ? Array.from({ length: 3 }).map((_, i) => (
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
                                    </Table.Row>
                                ))
                                : contacts.length > 0
                                    ? contacts.map((c) => (
                                        <Table.Row key={c.id} onClick={() => navigate(`/contacts/${c.id}`)} style={{ cursor: "pointer" }}>
                                            <Table.Cell>{`${c.firstname} ${c.lastname}`}</Table.Cell>
                                            <Table.Cell>{c.email}</Table.Cell>
                                            <Table.Cell>{c.phone}</Table.Cell>
                                        </Table.Row>
                                    ))
                                    : (
                                        <Table.Row>
                                            <Table.Cell colSpan={3}>
                                                <Text color="gray">No contacts yet.</Text>
                                            </Table.Cell>
                                        </Table.Row>
                                    )}
                        </Table.Body>
                    </Table.Root>
                </Card>

                {/* Interactions */}
                <Card variant="surface" size="3" mb="6">
                    <Heading size="5" mb="3">
                        Interactions
                    </Heading>
                    <Table.Root>
                        <Table.Header>
                            <Table.Row>
                                <Table.ColumnHeaderCell>Title</Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>Channel</Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>Occurred At</Table.ColumnHeaderCell>
                            </Table.Row>
                        </Table.Header>
                        <Table.Body>
                            {loading
                                ? Array.from({ length: 3 }).map((_, i) => (
                                    <Table.Row key={`interaction-skel-${i}`}>
                                        <Table.Cell>
                                            <Skeleton width="55%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="35%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="40%" />
                                        </Table.Cell>
                                    </Table.Row>
                                ))
                                : interactions.length > 0
                                    ? interactions.map((it) => (
                                        <Table.Row key={it.id} onClick={() => navigate(`/interactions/${it.id}`)} style={{ cursor: "pointer" }}>
                                            <Table.Cell>{it.title}</Table.Cell>
                                            <Table.Cell>{it.channel}</Table.Cell>
                                            <Table.Cell>{it.occurredAt}</Table.Cell>
                                        </Table.Row>
                                    ))
                                    : (
                                        <Table.Row>
                                            <Table.Cell colSpan={3}>
                                                <Text color="gray">No interactions yet.</Text>
                                            </Table.Cell>
                                        </Table.Row>
                                    )}
                        </Table.Body>
                    </Table.Root>
                </Card>

                <ModifiedCard />
            </Flex>
        </Box>
    );
}