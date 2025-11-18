import React, { useEffect, useState } from "react";
import {
    Box,
    Card,
    Code,
    DataList,
    Flex,
    Heading,
    IconButton,
    Link,
    Table,
    Text,
    TextField,
    Skeleton,
} from "@radix-ui/themes";
import { CopyIcon, MailIcon, PhoneIcon } from "lucide-react";
import ModifiedCard from "../components/ModifiedCard";
import LookupField from "../components/LookupField";
import {useNavigate, useParams} from "react-router-dom";
import type { Account } from "../models/account";
import {deleteContactById, getContactById} from "../services/contactService";
import type {ContactResponse} from "@/src/models/responses/contactResponse.ts";
import Header from "../components/headers/Header";
import { useToast } from "../components/AppToast";
import {useActionLoader} from "../components/ActionLoaderProvider";

type ContactAccount = Pick<Account, "id" | "name" | "industry" | "classification"> & {
    createdAt?: string;
    parentAccountId?: string | null;
    parentAccountName?: string | null;
};

const CHANNEL_LABEL: Record<number, string> = {
    1: "Email",
    2: "Ticket",
    3: "Call",
    4: "Meeting",
    5: "Chat",
    10: "Survey",
    11: "Review",
    12: "Social",
    20: "Visit Report",
    21: "CRM Note",
    22: "NPS Survey",
    99: "Other"
};

export function ContactDetailPage() {
    const { contactId } = useParams<{ contactId: string }>();
    const [contact, setContact] = useState<ContactResponse | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [copied, setCopied] = useState(false);
    const navigate = useNavigate();
    const { showToast } = useToast();
    const { withLoader } = useActionLoader();

    useEffect(() => {
        let mounted = true;
        setLoading(true);

        getContactById(contactId!)
            .then((data) => {
                if (mounted)
                    setContact(data ?? null);
            })
            .catch((err) => {
                console.error("Error loading contact:", err);
                if (mounted)
                    setContact(null);
            })
            .finally(() => {
                if (mounted)
                    setLoading(false);
            });

        return () => {
            mounted = false;
        };
    }, [contactId]);

    const firstname = contact?.firstname ?? "";
    const lastname = contact?.lastname ?? "";
    const email = contact?.email ?? "";
    const phone = contact?.phone ?? "";
    const externalId = "";
    const account = contact?.account ?? null;
    const interactions = contact?.interactions ?? [];

    const onCopyExternalId = async () => {
        try {
            await navigator.clipboard.writeText(externalId);
            setCopied(true);
            setTimeout(() => setCopied(false), 1500);
        } catch (e) {
            console.error("Copy failed", e);
        }
    };

    const handleDelete = async () => {
        if (!contactId)
            return;

        try {
            await withLoader("Kontakt wird gelöscht...", async () => {
                await deleteContactById(contactId!);
                navigate(-1);
            });

            showToast({
                title: "Kontakt gelöscht",
                description: `${firstname} ${lastname} wurde erfolgreich gelöscht.`,
            });

            navigate(-1);
        } catch (e) {
            console.error("Delete failed", e);

            showToast({
                title: "Fehler beim Löschen",
                description: "Der Kontakt konnte nicht gelöscht werden.",
            });
        }
    };

    return (
        <Box flexGrow="1" p="6">
            <Header title={`${firstname} ${lastname}`} showRefresh={true} showSave={true} showDelete={true} onDeleteClick={handleDelete}/>
            <Flex gap="3" direction="column" wrap="wrap">
                <Card style={{ flex: 1, minWidth: "100%" }} variant="ghost" mb="4">
                    <Flex direction="row" gap="8" wrap="wrap">
                        <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                            <DataList.Root size="3">
                                {/* Firstname */}
                                <DataList.Item align="center">
                                    <DataList.Label>Firstname</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root style={{ width: "100%" }} disabled type="text" value={firstname} />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Lastname */}
                                <DataList.Item align="center">
                                    <DataList.Label>Lastname</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root style={{ width: "100%" }} disabled type="text" value={lastname} />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Email */}
                                <DataList.Item align="center">
                                    <DataList.Label>E-Mail</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root style={{ width: "100%" }} disabled type="text" value={email}>
                                                {email && (
                                                    <TextField.Slot side="right">
                                                        <Link href={`mailto:${email}`} target="_blank">
                                                            <IconButton size="1" variant="ghost">
                                                                <MailIcon height="14" width="14" />
                                                            </IconButton>
                                                        </Link>
                                                    </TextField.Slot>
                                                )}
                                            </TextField.Root>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Telephone */}
                                <DataList.Item align="center">
                                    <DataList.Label>Telephone</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <TextField.Root style={{ width: "100%" }} disabled value=" " />
                                            </Skeleton>
                                        ) : (
                                            <TextField.Root style={{ width: "100%" }} disabled type="text" value={phone}>
                                                {phone && (
                                                    <TextField.Slot side="right">
                                                        <Link href={`tel:${phone}`}>
                                                            <IconButton size="1" variant="ghost">
                                                                <PhoneIcon height="14" width="14" />
                                                            </IconButton>
                                                        </Link>
                                                    </TextField.Slot>
                                                )}
                                            </TextField.Root>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Company */}
                                <DataList.Item align="center">
                                    <DataList.Label>Company</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        {loading ? (
                                            <Skeleton>
                                                <Text> </Text>
                                            </Skeleton>
                                        ) : account ? (
                                            <LookupField
                                                title={account.name}
                                                description={`${account.industry || "—"}`}
                                                targetUrl={`/accounts/${account.id}`}
                                                iconUrl=""
                                                iconFallback={account.name?.slice(0, 2).toUpperCase() || "AC"}
                                            />
                                        ) : (
                                            <Text color="gray">—</Text>
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
                                                <Code variant="ghost">{externalId || "—"}</Code>
                                                {externalId && (
                                                    <IconButton
                                                        size="1"
                                                        aria-label={copied ? "Copied" : "Copy value"}
                                                        color={copied ? "green" : "gray"}
                                                        variant="ghost"
                                                        onClick={onCopyExternalId}
                                                        title={copied ? "Copied!" : "Copy to clipboard"}
                                                    >
                                                        <CopyIcon size="16" />
                                                    </IconButton>
                                                )}
                                            </Flex>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>
                            </DataList.Root>
                        </Card>
                    </Flex>
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
                                ? Array.from({ length: 6 }).map((_, i) => (
                                    <Table.Row key={`interaction-skel-${i}`}>
                                        <Table.Cell>
                                            <Skeleton width="60%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="40%" />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Skeleton width="45%" />
                                        </Table.Cell>
                                    </Table.Row>
                                ))
                                : interactions.length > 0
                                    ? interactions.map((it) => (
                                        <Table.Row key={it.id} onClick={() => navigate(`/interactions/${it.id}`)} style={{ cursor: "pointer" }}>
                                            <Table.Cell>{it.title ?? "—"}</Table.Cell>
                                            <Table.Cell>
                                                {CHANNEL_LABEL[(it as any).channel as number] ??
                                                    String((it as any).channel ?? "—")}
                                            </Table.Cell>
                                            <Table.Cell>{(it as any).occurredAt ?? "—"}</Table.Cell>
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