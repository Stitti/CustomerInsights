import React from "react";
import {
    Box,
    Badge,
    Card,
    Code,
    DataList,
    Flex,
    IconButton,
    Text,
    Skeleton,
} from "@radix-ui/themes";
import { CopyIcon } from "lucide-react";
import ModifiedCard from "../components/ModifiedCard";
import LookupField from "../components/LookupField";
import TagEditor from "../components/TagEditor";
import Header from "../components/headers/Header";
import { useParams, useNavigate } from "react-router-dom";
import { getInteractionById, deleteInteractionById } from "../services/interactionService";
import type { InteractionResponse } from "@/src/models/responses/interactionResponse";
import {useActionLoader} from "../components/ActionLoaderProvider";

const CHANNEL_LABEL: Record<number, string> = {
    1: "Phone",
    2: "Gmail",
    3: "Phone",
    4: "Zendesk",
    5: "App",
    10: "Gmail",
    12: "Zendesk",
    20: "Web",
    21: "App",
};

function formatDate(d?: string | Date | null) {
    if (!d) return "—";
    const date = typeof d === "string" ? new Date(d) : d;
    if (isNaN(date.getTime())) return "—";
    return date.toLocaleString(undefined, {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
    });
}

export function InteractionDetailPage() {
    const { interactionId } = useParams<{ interactionId: string }>();
    const navigate = useNavigate();
    const { withLoader } = useActionLoader();

    const [loading, setLoading] = React.useState(true);
    const [interaction, setInteraction] = React.useState<InteractionResponse | null>(null);
    const [copyOK, setCopyOK] = React.useState(false);

    const [emotions, setEmotions] = React.useState<string[]>([]);
    const [aspects, setAspects] = React.useState<string[]>([]);

    const fetchData = React.useCallback(async () => {
        if (!interactionId) return;
        setLoading(true);
        try {
            const data = await getInteractionById(interactionId);
            setInteraction(data ?? null);

            const emo = data?.textInference?.emotions?.map(e => e.label).filter(Boolean) ?? [];
            const asp = data?.textInference?.aspects?.map(a => a.label).filter(Boolean) ?? [];
            setEmotions(emo);
            setAspects(asp);
        } catch (e) {
            console.error("Failed to load interaction:", e);
            setInteraction(null);
            setEmotions([]);
            setAspects([]);
        } finally {
            setLoading(false);
        }
    }, [interactionId]);

    React.useEffect(() => {
        fetchData();
    }, [fetchData]);

    const title = interaction?.subject ?? (interaction as any)?.title ?? "";
    const occurredAtStr = formatDate(interaction?.occurredAt);
    const analyzed = !!interaction?.analyzedAt;

    const onCopyExternalId = async () => {
        const ext = interaction?.externalId;
        if (!ext) return;
        try {
            await navigator.clipboard.writeText(ext);
            setCopyOK(true);
            setTimeout(() => setCopyOK(false), 1400);
        } catch (e) {
            console.error("Copy failed", e);
        }
    };

    const handleRefresh = async () => {
        await withLoader("Account wird gelöscht...", async () => {
            await fetchData();
        });
    };

    const handleDelete = async () => {
        if (!interaction?.id)
            return;
        try {
            await withLoader("Account wird gelöscht...", async () => {
                await deleteInteractionById(interaction.id);
                navigate(-1);
            });
        } catch (e) {
            console.error("Delete failed", e);
        }
    };

    return (
        <Box flexGrow="1" p="6">
            <Header
                title={loading ? "…" : (title || "Interaction")}
                showSave={true}
                showDelete={true}
                showRefresh={true}
                onDeleteClick={handleDelete}
                onRefreshClick={handleRefresh}
            />

            <Flex gap="9" direction="column" wrap="wrap">
                <Card style={{ flex: 1, minWidth: "100%" }} variant="ghost" mb="4">
                    <Flex direction="row" gap="8" wrap="wrap">
                        {/* Linke Spalte */}
                        <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                            <DataList.Root size="3">
                                {/* Title */}
                                <DataList.Item align="center">
                                    <DataList.Label>Title</DataList.Label>
                                    <DataList.Value>
                                        {loading ? <Skeleton><Text> </Text></Skeleton> : (title || "—")}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Contact (eingebettet) */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Contact</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Skeleton><Text> </Text></Skeleton>
                                        ) : interaction?.contact ? (
                                            <LookupField
                                                title={`${interaction.contact.firstname} ${interaction.contact.lastname}`}
                                                description={interaction.contact.email ?? ""}
                                                targetUrl={`/contacts/${interaction.contact.id}`}
                                                iconUrl=""
                                                iconFallback={`${interaction.contact.firstname?.[0] ?? ""}${interaction.contact.lastname?.[0] ?? ""}`.toUpperCase() || "CO"}
                                            />
                                        ) : (
                                            <Text color="gray">—</Text>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Account (eingebettet) */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Account</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Skeleton><Text> </Text></Skeleton>
                                        ) : interaction?.account ? (
                                            <LookupField
                                                title={interaction.account.name}
                                                description={interaction.account.industry || "—"}
                                                targetUrl={`/accounts/${interaction.account.id}`}
                                                iconUrl=""
                                                iconFallback={interaction.account.name?.slice(0, 2).toUpperCase() || "AC"}
                                            />
                                        ) : (
                                            <Text color="gray">—</Text>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* External ID */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">External ID</DataList.Label>
                                    <DataList.Value>
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
                                                <Code variant="ghost">{interaction?.externalId ?? "—"}</Code>
                                                {interaction?.externalId && (
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
                                                )}
                                            </Flex>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Channel */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Channel</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Skeleton><Text> </Text></Skeleton>
                                        ) : (
                                            CHANNEL_LABEL[interaction?.channel ?? 0] ?? String(interaction?.channel ?? "—")
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Thread */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Thread</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Skeleton><Text> </Text></Skeleton>
                                        ) : interaction?.threadId ? (
                                            <Code variant="ghost">{interaction.threadId}</Code>
                                        ) : (
                                            <Text color="gray">—</Text>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Occurred At */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Occurred At</DataList.Label>
                                    <DataList.Value>
                                        {loading ? <Skeleton><Text> </Text></Skeleton> : occurredAtStr}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Text */}
                                <DataList.Item>
                                    <DataList.Label minWidth="88px">Text</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <>
                                                <Skeleton><Text> </Text></Skeleton>
                                                <Skeleton><Text> </Text></Skeleton>
                                                <Skeleton><Text> </Text></Skeleton>
                                            </>
                                        ) : (
                                            <Text wrap="pretty">{interaction?.text ?? "—"}</Text>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>
                            </DataList.Root>
                        </Card>

                        {/* Rechte Spalte */}
                        <Card style={{ flex: 1, maxWidth: "20vw", minWidth: 280 }} variant="surface" size="3" mb="6">
                            <DataList.Root>
                                {/* Status */}
                                <DataList.Item align="center">
                                    <DataList.Label minWidth="88px">Status</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Skeleton><Badge>Analyzing</Badge></Skeleton>
                                        ) : (
                                            <Badge color={analyzed ? "jade" : "amber"} variant="soft" radius="full">
                                                {analyzed ? "Analyzed" : "Pending"}
                                            </Badge>
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Sentiment */}
                                <DataList.Item>
                                    <DataList.Label>Sentiment</DataList.Label>
                                    <DataList.Value>
                                        {loading ? <Skeleton><Text> </Text></Skeleton> : (interaction?.textInference?.sentiment ?? "—")}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Urgency */}
                                <DataList.Item>
                                    <DataList.Label>Urgency</DataList.Label>
                                    <DataList.Value>
                                        {loading ? <Skeleton><Text> </Text></Skeleton> : (interaction?.textInference?.urgency ?? "—")}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Emotions (TagEditor) */}
                                <DataList.Item>
                                    <DataList.Label>Emotions</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Flex gap="2" wrap="wrap">
                                                {Array.from({ length: 3 }).map((_, i) => (
                                                    <Skeleton key={`emo-${i}`}><Badge variant="solid"> </Badge></Skeleton>
                                                ))}
                                            </Flex>
                                        ) : (
                                            <TagEditor
                                                value={emotions}
                                                onChange={setEmotions}
                                                placeholder="z.B. Ärger, Sorge…"
                                                suggestions={[
                                                    "Frustration",
                                                    "Enttäuschung",
                                                    "Verwirrung",
                                                    "Sorge",
                                                    "Verärgerung",
                                                    "Ungeduld",
                                                ]}
                                                ariaLabel="Emotion hinzufügen"
                                                badgeColor="indigo"
                                            />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>

                                {/* Aspects (TagEditor) */}
                                <DataList.Item>
                                    <DataList.Label>Aspects</DataList.Label>
                                    <DataList.Value>
                                        {loading ? (
                                            <Flex gap="2" wrap="wrap">
                                                {Array.from({ length: 4 }).map((_, i) => (
                                                    <Skeleton key={`asp-${i}`}><Badge variant="solid"> </Badge></Skeleton>
                                                ))}
                                            </Flex>
                                        ) : (
                                            <TagEditor
                                                value={aspects}
                                                onChange={setAspects}
                                                placeholder="z.B. Versand, Rechnung…"
                                                suggestions={["Lieferzeit", "Preis", "Support", "Qualität", "Website", "Rechnung"]}
                                                ariaLabel="Aspekt hinzufügen"
                                                badgeColor="indigo"
                                            />
                                        )}
                                    </DataList.Value>
                                </DataList.Item>
                            </DataList.Root>
                        </Card>
                    </Flex>
                </Card>

                <ModifiedCard />
            </Flex>
        </Box>
    );
}