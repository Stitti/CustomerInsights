import {
    Box,
    Button,
    Card,
    Flex,
    Table,
    TextField,
    Badge,
    Separator,
    Text, IconButton
} from "@radix-ui/themes";
import { MetricsTrends } from "../components/MetricsTrends";
import {
    Search as SearchIcon,
    AlertTriangle,
    Activity,
    MessageSquareWarning,
    ShoppingCart,
    Clock3,
    TrendingDown, XIcon
} from "lucide-react";
import React, { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

type Priority = "high" | "medium" | "low";
type SignalType = "KPI" | "Emotion" | "Journey" | "Support";

type Signal = {
    id: string;
    title: string;
    account: string;
    type: SignalType;
    thresholdLabel: string; // z.B. "CSI < 70", "Open > 72h"
    valueLabel: string;     // z.B. "64", "95h", "+18%"
    priority: Priority;
    createdAt: string;      // ISO oder kurzer Text
};

const MOCK_SIGNALS: Signal[] = [
    {
        id: "sig_001",
        title: "CSI unter Schwellenwert",
        account: "Müller Consulting GmbH",
        type: "KPI",
        thresholdLabel: "CSI < 70",
        valueLabel: "64",
        priority: "high",
        createdAt: "2025-10-30 09:30"
    },
    {
        id: "sig_002",
        title: "Ticket offen zu lange",
        account: "Müller Consulting GmbH",
        type: "Support",
        thresholdLabel: "Offen > 72h",
        valueLabel: "95h",
        priority: "medium",
        createdAt: "2025-10-30 08:15"
    },
    {
        id: "sig_003",
        title: "Frustration erkannt",
        account: "Anna Müller",
        type: "Emotion",
        thresholdLabel: "—",
        valueLabel: "negativ",
        priority: "high",
        createdAt: "2025-10-30 07:40"
    },
    {
        id: "sig_004",
        title: "Checkout-Drop gestiegen",
        account: "Webshop – DE",
        type: "Journey",
        thresholdLabel: "Δ Dropoff < 5%",
        valueLabel: "+18%",
        priority: "medium",
        createdAt: "2025-10-30 06:10"
    },
    {
        id: "sig_005",
        title: "Antwortquote sinkt",
        account: "SaaS Enterprise",
        type: "KPI",
        thresholdLabel: "Resp. Rate > 35%",
        valueLabel: "28%",
        priority: "low",
        createdAt: "2025-10-29 17:22"
    }
];

function TypeBadge({ type }: { type: SignalType }) {
    const map: Record<SignalType, { color: any; icon: React.ReactNode }> = {
        KPI: { color: "crimson", icon: <Activity size={14} /> },
        Emotion: { color: "plum", icon: <MessageSquareWarning size={14} /> },
        Journey: { color: "indigo", icon: <ShoppingCart size={14} /> },
        Support: { color: "cyan", icon: <Clock3 size={14} /> }
    };
    const { color, icon } = map[type];
    return (
        <Badge color={color} >
            <Flex align="center" gap="2">
                {icon}
                <Text size="2">{type}</Text>
            </Flex>
        </Badge>
    );
}

function PriorityBadge({ level }: { level: Priority }) {
    const map: Record<Priority, { color: any; label: string; icon: React.ReactNode }> = {
        high: { color: "red", label: "High", icon: <AlertTriangle size={14} /> },
        medium: { color: "amber", label: "Medium", icon: <TrendingDown size={14} /> },
        low: { color: "grass", label: "Low", icon: <TrendingDown size={14} /> }
    };
    const { color, label, icon } = map[level];
    return (
        <Badge color={color}>
            <Flex align="center" gap="2">
                {icon}
                <Text size="2">{label}</Text>
            </Flex>
        </Badge>
    );
}

export default function SignalsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState("");
    const { t } = useTranslation();

    const filtered = useMemo(() => {
        const q = search.trim().toLowerCase();
        if (!q) return MOCK_SIGNALS;
        return MOCK_SIGNALS.filter(
            (s) =>
                s.title.toLowerCase().includes(q) ||
                s.account.toLowerCase().includes(q) ||
                s.type.toLowerCase().includes(q) ||
                s.thresholdLabel.toLowerCase().includes(q)
        );
    }, [search]);

    return (
        <Box flexGrow="1" p="6">
            <MetricsTrends />

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

                <Separator size="4" my="2" />

                {/* Tabelle */}
                <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell style={{ width: 340 }}>Signal</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Account</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Type</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Threshold</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Current</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Priority</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Created</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>

                    <Table.Body>
                        {filtered.map((s) => (
                            <Table.Row key={s.id} onClick={() => navigate("/signals/123")} style={{ cursor: "pointer" }}>
                                <Table.Cell>
                                    <Flex direction="column" gap="1">
                                        <Text weight="medium">{s.title}</Text>
                                    </Flex>
                                </Table.Cell>

                                <Table.Cell>
                                    <Text>{s.account}</Text>
                                </Table.Cell>

                                <Table.Cell>
                                    <TypeBadge type={s.type} />
                                </Table.Cell>

                                <Table.Cell>
                                    <Text color="gray">{s.thresholdLabel}</Text>
                                </Table.Cell>

                                <Table.Cell>
                                    <Text weight="medium">{s.valueLabel}</Text>
                                </Table.Cell>

                                <Table.Cell>
                                    <PriorityBadge level={s.priority} />
                                </Table.Cell>

                                <Table.Cell>
                                    <Text color="gray">{s.createdAt}</Text>
                                </Table.Cell>
                            </Table.Row>
                        ))}
                    </Table.Body>
                </Table.Root>
            </Card>
        </Box>
    );
}
