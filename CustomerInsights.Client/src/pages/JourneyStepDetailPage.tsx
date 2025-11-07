import React, { useEffect, useMemo, useState } from "react";
import {
    Box,
    Card,
    Flex,
    Text,
    Badge,
    Separator,
    Table,
    Button,
    TextArea,
    Callout,
    Progress
} from "@radix-ui/themes";
import { useNavigate, useParams } from "react-router-dom";
import {
    ArrowLeft,
    AlertTriangle,
    Timer,
    BarChart3,
    Check
} from "lucide-react";
import { MetricsTrends } from "../components/MetricsTrends";
import BackButton from "../components/BackButton";

// --------------------------------------------------
// Datentypen & Mockdaten (kannst du durch API ersetzen)
// --------------------------------------------------
interface JourneyStepDetail {
    id: string;
    name: string;
    description: string;
    conversionRate: number;
    dropOffRate: number;
    avgTime: string;
    sessions?: number; // optional: Anzahl Sessions/Visits im Step
    issues: { label: string; weight: number }[]; // weight: relativer Anteil 0–100
    relatedTickets: { id: string; date: string; subject: string; sentiment: "Positive" | "Neutral" | "Negative" }[];
    recommendations: string[];
}

const JOURNEY_DETAILS: Record<string, JourneyStepDetail> = {
    landing: {
        id: "landing",
        name: "Landing Page",
        description: "Einstieg über Kampagnen, Direktzugriffe und organische Suche.",
        conversionRate: 97,
        dropOffRate: 3,
        avgTime: "00:08",
        sessions: 12000,
        issues: [],
        relatedTickets: [],
        recommendations: ["A/B-Test für Hero-Sektion prüfen", "Core Web Vitals beobachten"]
    },
    product: {
        id: "product",
        name: "Produktseite",
        description: "Nutzer informieren sich über Produkteigenschaften und Preise.",
        conversionRate: 84,
        dropOffRate: 16,
        avgTime: "00:34",
        sessions: 9300,
        issues: [
            { label: "Ladezeit hoch", weight: 55 },
            { label: "Unklare CTAs", weight: 30 },
            { label: "Bilder zu groß", weight: 15 }
        ],
        relatedTickets: [
            { id: "T-8141", date: "2025-10-22", subject: "Produktseite lädt langsam", sentiment: "Negative" },
            { id: "T-8193", date: "2025-10-28", subject: "Finde Größe nicht", sentiment: "Neutral" }
        ],
        recommendations: [
            "Bilder komprimieren und Lazy-Loading aktivieren",
            "CTA-Platzierung und Wortlaut testen",
            "Above-the-fold Inhalte priorisieren"
        ]
    },
    cart: {
        id: "cart",
        name: "Warenkorb",
        description: "Überblick vor dem Checkout mit Versand-, Steuer- und Rabattinformationen.",
        conversionRate: 72,
        dropOffRate: 28,
        avgTime: "00:57",
        sessions: 6800,
        issues: [
            { label: "Versandkosten unklar", weight: 60 },
            { label: "Ablenkung durch Cross-Selling", weight: 25 },
            { label: "Fehlende Trust-Signale", weight: 15 }
        ],
        relatedTickets: [
            { id: "T-8260", date: "2025-10-29", subject: "Versandkosten erst im letzten Schritt", sentiment: "Negative" }
        ],
        recommendations: [
            "Gesamtkosten früh darstellen",
            "Cross-Selling reduzieren oder nachrangig platzieren",
            "Trust-Badges und Rückgaberegeln sichtbar platzieren"
        ]
    },
    checkout: {
        id: "checkout",
        name: "Checkout",
        description: "Adress-, Versand- und Zahlungsdaten werden erfasst.",
        conversionRate: 54,
        dropOffRate: 46,
        avgTime: "01:25",
        sessions: 4900,
        issues: [
            { label: "Fehler bei Zahlungsart", weight: 50 },
            { label: "Formularabbrüche", weight: 35 },
            { label: "Validierungsprobleme", weight: 15 }
        ],
        relatedTickets: [
            { id: "T-8321", date: "2025-10-30", subject: "Zahlung wird abgelehnt", sentiment: "Negative" },
            { id: "T-8294", date: "2025-10-27", subject: "Formular verliert Eingaben", sentiment: "Negative" }
        ],
        recommendations: [
            "Fallback-Payment aktivieren",
            "Autosave und Fehlerhinweise im Formular",
            "3-D-Secure Fehlerquoten analysieren"
        ]
    },
    confirmation: {
        id: "confirmation",
        name: "Bestellung abgeschlossen",
        description: "Bestellbestätigung und Nächste-Schritte.",
        conversionRate: 100,
        dropOffRate: 0,
        avgTime: "—",
        sessions: 2600,
        issues: [],
        relatedTickets: [],
        recommendations: ["Transaktions-E-Mails testen", "Dankeseite für Cross-Sells nutzen"]
    }
};

// --------------------------------------------------
// Hilfs-UI
// --------------------------------------------------
function SentimentBadge({ s }: { s: "Positive" | "Neutral" | "Negative" }) {
    const color = s === "Positive" ? "green" : s === "Neutral" ? "amber" : "red";
    return <Badge color={color} variant="soft">{s}</Badge>;
}

function DropOffBadge({ rate }: { rate: number }) {
    if (rate >= 40) return <Badge color="red">{rate}%</Badge>;
    if (rate >= 20) return <Badge color="amber">{rate}%</Badge>;
    return <Badge color="green">{rate}%</Badge>;
}

// --------------------------------------------------
// Seite
// --------------------------------------------------
export default function JourneyStepDetailPage() {
    const { stepId } = useParams<{ stepId: string }>();
    const navigate = useNavigate();

    const step = useMemo(() => {
        if (!stepId) return undefined;
        return JOURNEY_DETAILS[stepId];
    }, [stepId]);

    // Notizen mit einfacher Persistenz
    const [notes, setNotes] = useState("");
    const [saved, setSaved] = useState(false);
    useEffect(() => {
        if (!step) return;
        const key = `journey_step_notes_${step.id}`;
        const stored = localStorage.getItem(key);
        if (stored) setNotes(stored);
    }, [step]);
    const handleSaveNotes = () => {
        if (!step) return;
        localStorage.setItem(`journey_step_notes_${step.id}`, notes);
        setSaved(true);
        setTimeout(() => setSaved(false), 1200);
    };

    if (!step) {
        return (
            <Box p="6">
                <Button variant="ghost" onClick={() => navigate(-1)}>
                    <ArrowLeft size={16} /> <Text ml="2">Zurück</Text>
                </Button>
                <Card mt="3" style={{padding: "20px"}}>
                    <Text>Schritt nicht gefunden.</Text>
                </Card>
            </Box>
        );
    }

    // Ableitung für Header-Badge
    const priority: "high" | "medium" | "low" =
        step.dropOffRate >= 40 ? "high" : step.dropOffRate >= 20 ? "medium" : "low";
    const priorityColor = priority === "high" ? "red" : priority === "medium" ? "amber" : "green";

    return (
        <Box p="6" flexGrow="1">
            <BackButton/>

            <Card variant="surface" size="3">
                <Flex justify="between" align="center" mb="2">
                    <Flex align="center" gap="3">
                        <Badge color={priorityColor} variant="solid">
                            {priority === "high" ? "Hohe Priorität" : priority === "medium" ? "Mittlere Priorität" : "Niedrige Priorität"}
                        </Badge>
                        <Text size="4" weight="bold">{step.name}</Text>
                    </Flex>
                    <Text color="gray">{step.description}</Text>
                </Flex>

                <Separator my="4" size="4" />

                {/* KPI-Kacheln */}
                <Flex gap="4" wrap="wrap" mb="5">
                    <Card variant="surface" style={{ minWidth: 220 }}>
                        <Flex direction="column" p="3" gap="1">
                            <Text size="2" color="gray">Konversionsrate</Text>
                            <Text size="4" weight="bold">{step.conversionRate}%</Text>
                            <Progress value={Math.min(step.conversionRate, 100)} />
                        </Flex>
                    </Card>
                    <Card variant="surface" style={{ minWidth: 220 }}>
                        <Flex direction="column" p="3" gap="1">
                            <Text size="2" color="gray">Drop-off-Rate</Text>
                            <Text size="4" weight="bold" color={priorityColor}>{step.dropOffRate}%</Text>
                            <DropOffBadge rate={step.dropOffRate} />
                        </Flex>
                    </Card>
                    <Card variant="surface" style={{ minWidth: 220 }}>
                        <Flex direction="column" p="3" gap="1">
                            <Text size="2" color="gray">Ø Verweildauer</Text>
                            <Flex align="center" gap="2">
                                <Timer size={16} />
                                <Text size="4" weight="bold">{step.avgTime}</Text>
                            </Flex>
                        </Flex>
                    </Card>
                    {typeof step.sessions === "number" && (
                        <Card variant="surface" style={{ minWidth: 220 }}>
                            <Flex direction="column" p="3" gap="1">
                                <Text size="2" color="gray">Sessions</Text>
                                <Flex align="center" gap="2">
                                    <BarChart3 size={16} />
                                    <Text size="4" weight="bold">{step.sessions.toLocaleString()}</Text>
                                </Flex>
                            </Flex>
                        </Card>
                    )}
                </Flex>

                <Separator my="4" size="4" />

                {/* Beispiel-Tickets */}
                <Text weight="medium" mb="3">Verknüpfte Kundenanfragen</Text>
                <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>Datum</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Ticket</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Betreff</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Sentiment</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {step.relatedTickets.length === 0 ? (
                            <Table.Row>
                                <Table.Cell colSpan={4}>
                                    <Text color="gray">Keine verknüpften Anfragen.</Text>
                                </Table.Cell>
                            </Table.Row>
                        ) : (
                            step.relatedTickets.map((t) => (
                                <Table.Row key={t.id}>
                                    <Table.Cell>{t.date}</Table.Cell>
                                    <Table.Cell>{t.id}</Table.Cell>
                                    <Table.Cell>{t.subject}</Table.Cell>
                                    <Table.Cell><SentimentBadge s={t.sentiment} /></Table.Cell>
                                </Table.Row>
                            ))
                        )}
                    </Table.Body>
                </Table.Root>

                <Separator my="4" size="4" />

                {/* Empfehlungen */}
                <Text weight="medium" mb="2">Empfohlene Maßnahmen</Text>
                {step.recommendations.length ? (
                    <ul style={{ marginLeft: 18, color: "var(--gray-11)", fontSize: 14 }}>
                        {step.recommendations.map((r, idx) => <li key={idx}>{r}</li>)}
                    </ul>
                ) : (
                    <Text color="gray">Keine Empfehlungen hinterlegt.</Text>
                )}

                <Separator my="4" size="4" />

                {/* Notizen */}
                <Text weight="medium" mb="2">Notizen</Text>
                <TextArea
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                    placeholder="Eigene Beobachtungen oder Vereinbarungen dokumentieren…"
                    style={{ width: "100%", minHeight: 120, resize: "vertical" }}
                />
                <Flex justify="end" align="center" gap="3" mt="2">
                    {saved && (
                        <Flex align="center" gap="2">
                            <Check size={16} color="var(--grass-11)" />
                            <Text color="green">Gespeichert</Text>
                        </Flex>
                    )}
                    <Button onClick={handleSaveNotes} variant="soft">Speichern</Button>
                </Flex>
            </Card>
        </Box>
    );
}