import {
    Box,
    Button,
    Card,
    Flex,
    Text,
    Badge,
    Table,
    Separator,
    TextArea
} from "@radix-ui/themes";
import {
    AlertTriangle,
    TrendingDown,
    ArrowLeft,
    Activity,
    MessageSquareWarning,
    ShoppingCart,
    Clock3,
    Check
} from "lucide-react";
import { useNavigate, useParams } from "react-router-dom";
import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import BackButton from "../components/BackButton";
import Header from "../components/headers/Header";
import LookupField from "../components/LookupField";

// -----------------------------
// Types & Mock Signal Data
// -----------------------------
type Priority = "high" | "medium" | "low";
type SignalType = "KPI" | "Emotion" | "Journey" | "Support";

interface SignalDetail {
    id: string;
    title: string;
    account: string;
    type: SignalType;
    priority: Priority;
    threshold: string;
    currentValue: string;
    trend: string;
    createdAt: string;
    occurrences: string;
    journeySteps: string[];
    recommendations: string[];
    relatedTickets: {
        id: string;
        date: string;
        type: string;
        sentiment: "Positive" | "Neutral" | "Negative";
    }[];
}

const MOCK_SIGNAL: SignalDetail = {
    id: "sig_001",
    title: "CSI unter Schwellenwert",
    account: "Müller Consulting GmbH",
    type: "KPI",
    priority: "high",
    threshold: "< 70",
    currentValue: "64 ↓ (-12 vs. Vormonat)",
    trend: "Abwärtstrend seit 3 Wochen",
    createdAt: "2025-10-30 09:30",
    occurrences: "3x in den letzten 60 Tagen",
    journeySteps: ["Checkout", "Billing"],
    recommendations: [
        "Kundenbetreuer informieren",
        "Ticket #4391 prüfen (Support Team)",
        "Feedbackformular zur Zufriedenheit senden"
    ],
    relatedTickets: [
        { id: "4391", date: "2025-10-29", type: "Rechnungskorrektur", sentiment: "Negative" },
        { id: "4352", date: "2025-10-25", type: "Lieferzeitprobleme", sentiment: "Neutral" }
    ]
};

// -----------------------------
// Helper Components
// -----------------------------
function PriorityBadge({ level }: { level: Priority }) {
    const map: Record<
        Priority,
        { color: any; label: string; icon: React.ReactNode }
    > = {
        high: { color: "red", label: "High", icon: <AlertTriangle size={14} /> },
        medium: { color: "amber", label: "Medium", icon: <TrendingDown size={14} /> },
        low: { color: "grass", label: "Low", icon: <TrendingDown size={14} /> }
    };
    const { color, label, icon } = map[level];
    return (
        <Badge color={color} variant="solid">
            <Flex align="center" gap="2">
                {icon}
                <Text size="2">{label}</Text>
            </Flex>
        </Badge>
    );
}

function TypeBadge({ type }: { type: SignalType }) {
    const map: Record<SignalType, { color: any; icon: React.ReactNode }> = {
        KPI: { color: "crimson", icon: <Activity size={14} /> },
        Emotion: { color: "plum", icon: <MessageSquareWarning size={14} /> },
        Journey: { color: "indigo", icon: <ShoppingCart size={14} /> },
        Support: { color: "cyan", icon: <Clock3 size={14} /> }
    };
    const { color, icon } = map[type];
    return (
        <Badge color={color} variant="soft" radius="full">
            <Flex align="center" gap="2">
                {icon}
                <Text size="2">{type}</Text>
            </Flex>
        </Badge>
    );
}

// -----------------------------
// Main Component
// -----------------------------
export default function SignalDetailPage() {
    const navigate = useNavigate();
    const { id } = useParams();
    const { t } = useTranslation();
    const signal = MOCK_SIGNAL; // später via fetch(id) oder useLoaderData

    // -----------------------------
    // Notes State & Persistence
    // -----------------------------
    const [notes, setNotes] = useState("");
    const [saved, setSaved] = useState(false);

    // Load existing note (z. B. aus localStorage)
    useEffect(() => {
        const savedNote = localStorage.getItem(`signal_note_${signal.id}`);
        if (savedNote) setNotes(savedNote);
    }, [signal.id]);

    const handleSaveNote = () => {
        localStorage.setItem(`signal_note_${signal.id}`, notes);
        setSaved(true);
        setTimeout(() => setSaved(false), 1500);
    };

    return (
        <Box p="6" flexGrow="1">
            <Header title={signal.title} showSave={true} showRefresh={true}/>
            <Card variant="surface" size="3">
                <Flex justify="between" align="center" mb="3">
                    <Flex align="center" gap="3">
                        <PriorityBadge level={signal.priority} />
                        <Text size="4" weight="bold">
                            {signal.title}
                        </Text>
                    </Flex>
                    <TypeBadge type={signal.type} />
                </Flex>

                <Text color="gray" size="2" mb="1">
                    <LookupField title={signal.account} description={""} targetUrl={""} iconUrl={""} iconFallback={""}/>
                     • Erst erkannt: {signal.createdAt}
                </Text>

                <Separator size="4" my="4" />

                {/* KPI Details */}
                <Flex direction="column" gap="2" mb="4">
                    <Flex justify="between">
                        <Text color="gray">Schwelle:</Text>
                        <Text weight="medium">{signal.threshold}</Text>
                    </Flex>
                    <Flex justify="between">
                        <Text color="gray">Aktueller Wert:</Text>
                        <Text weight="medium" color="red">
                            {signal.currentValue}
                        </Text>
                    </Flex>
                    <Flex justify="between">
                        <Text color="gray">Trend:</Text>
                        <Text>{signal.trend}</Text>
                    </Flex>
                    <Flex justify="between">
                        <Text color="gray">Häufigkeit:</Text>
                        <Text>{signal.occurrences}</Text>
                    </Flex>
                    <Flex justify="between">
                        <Text color="gray">Journey-Schritte:</Text>
                        <Text>{signal.journeySteps.join(", ")}</Text>
                    </Flex>
                </Flex>

                <Separator size="4" my="4" />

                {/* Recommendations */}
                <Text weight="medium" mb="2">Empfohlene Maßnahmen</Text>
                <ul
                    style={{
                        marginLeft: "20px",
                        marginBottom: "12px",
                        color: "var(--gray-11)",
                        fontSize: "14px"
                    }}
                >
                    {signal.recommendations.map((r, i) => (
                        <li key={i}>{r}</li>
                    ))}
                </ul>
                <Flex gap="2" mb="4">
                    <Button color="green">Maßnahme starten</Button>
                    <Button variant="soft">Als erledigt markieren</Button>
                </Flex>

                <Separator size="4" my="4" />

                {/* Notes Section */}
                <Text weight="medium" mb="2">
                    Notizen & Beschreibung
                </Text>
                <TextArea
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                    placeholder="Schreibe hier deine Beobachtungen, To-Dos oder Kommentare..."
                    style={{
                        minHeight: "120px",
                        resize: "vertical",
                        width: "100%",
                        fontSize: "14px",
                        lineHeight: "1.5"
                    }}
                />

                <Separator size="4" my="4" />

                {/* Related Tickets */}
                <Text weight="medium" mb="3">
                    Verknüpfte Kundenanfragen
                </Text>
                <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>Datum</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Typ</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Sentiment</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {signal.relatedTickets.map((t) => (
                            <Table.Row key={t.id} onClick={() => navigate(`/interactions/${t.id}`)} style={{ cursor: "pointer" }}>
                                <Table.Cell>{t.date}</Table.Cell>
                                <Table.Cell>{t.type}</Table.Cell>
                                <Table.Cell>
                                    <Badge
                                        color={
                                            t.sentiment === "Positive"
                                                ? "grass"
                                                : t.sentiment === "Neutral"
                                                    ? "amber"
                                                    : "red"
                                        }
                                        variant="soft"
                                    >
                                        {t.sentiment}
                                    </Badge>
                                </Table.Cell>
                            </Table.Row>
                        ))}
                    </Table.Body>
                </Table.Root>
            </Card>
        </Box>
    );
}