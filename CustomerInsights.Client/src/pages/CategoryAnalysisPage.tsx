import {
    Box,
    Card,
    Flex,
    Text,
    Table,
    Badge,
} from "@radix-ui/themes";
import { TrendingUp, TrendingDown, Minus } from "lucide-react";
import React from "react";
import { useNavigate } from "react-router-dom";
import {MetricsTrends} from "../components/MetricsTrends";

// -----------------------------
// Datentyp & Mockdaten
// -----------------------------
interface CategoryStats {
    id: string;
    name: string;
    share: number; // Anteil an allen Erwähnungen (%)
    sentiment: number; // Positivwert 0-100
    trend: number; // Veränderung der Erwähnungen in %
    criticalSignals: number;
    lastPeak: string;
}

const CATEGORY_DATA: CategoryStats[] = [
    {
        id: "support",
        name: "Supportqualität",
        share: 18,
        sentiment: 42,
        trend: 14,
        criticalSignals: 3,
        lastPeak: "2025-11-02",
    },
    {
        id: "delivery",
        name: "Lieferzeit",
        share: 22,
        sentiment: 35,
        trend: 12,
        criticalSignals: 5,
        lastPeak: "2025-11-04",
    },
    {
        id: "product",
        name: "Produktqualität",
        share: 15,
        sentiment: 72,
        trend: 0,
        criticalSignals: 1,
        lastPeak: "2025-10-25",
    },
    {
        id: "pricing",
        name: "Preisgestaltung",
        share: 13,
        sentiment: 50,
        trend: 8,
        criticalSignals: 2,
        lastPeak: "2025-11-05",
    },
    {
        id: "usability",
        name: "Benutzerfreundlichkeit",
        share: 10,
        sentiment: 68,
        trend: 4,
        criticalSignals: 0,
        lastPeak: "2025-11-01",
    },
];

// -----------------------------
// Hilfsfunktionen
// -----------------------------
function SentimentBadge({ value }: { value: number }) {
    if (value >= 70)
        return <Badge color="green">{value}% positiv</Badge>;
    if (value >= 45)
        return <Badge color="amber">{value}% neutral</Badge>;
    return <Badge color="red">{value}% negativ</Badge>;
}

function TrendIndicator({ trend }: { trend: number }) {
    if (trend > 0)
        return (
            <Flex align="center" gap="1" >
                <TrendingUp size={14} /> +{trend} %
            </Flex>
        );
    if (trend < 0)
        return (
            <Flex align="center" gap="1" >
                <TrendingDown size={14} /> {trend} %
            </Flex>
        );
    return (
        <Flex align="center" gap="1" >
            <Minus size={14} /> 0 %
        </Flex>
    );
}

// -----------------------------
// Hauptkomponente
// -----------------------------
export default function CategoryAnalysisPage() {
    const navigate = useNavigate();

    // KPI-Summary vorbereiten
    const totalMentions = CATEGORY_DATA.reduce((sum, c) => sum + c.share, 0);
    const topCategory = CATEGORY_DATA.sort((a, b) => b.share - a.share)[0];
    const negativeRatio = Math.round(
        (CATEGORY_DATA.filter((c) => c.sentiment < 50).length /
            CATEGORY_DATA.length) *
        100
    );

    return (
        <Box p="6" flexGrow="1">
            <MetricsTrends/>

            {/* Tabellenansicht */}
            <Card variant="surface">
                <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>Kategorie</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Erwähnungsanteil</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Sentiment</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Trend</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {CATEGORY_DATA.map((c) => (
                            <Table.Row
                                key={c.id}
                                style={{ cursor: "pointer" }}
                                onClick={() => navigate(`/categories/${c.id}`)}
                            >
                                <Table.Cell>
                                    <Text weight="medium">{c.name}</Text>
                                </Table.Cell>
                                <Table.Cell>{c.share} %</Table.Cell>
                                <Table.Cell>
                                    <SentimentBadge value={c.sentiment} />
                                </Table.Cell>
                                <Table.Cell>
                                    <TrendIndicator trend={c.trend} />
                                </Table.Cell>
                            </Table.Row>
                        ))}
                    </Table.Body>
                </Table.Root>
            </Card>
        </Box>
    );
}