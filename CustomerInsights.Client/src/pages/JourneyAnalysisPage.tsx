import {
    Box,
    Card,
    Flex,
    Text,
    Badge,
    Separator,
    Table,
} from "@radix-ui/themes";
import {TrendingDown, AlertTriangle, MoveRightIcon} from "lucide-react";
import React from "react";
import {MetricsTrends} from "../components/MetricsTrends";
import {type JourneyStep, StepNode} from "../components/StepNode";

// -----------------------------
// Datentypen & Mockdaten
// -----------------------------

const JOURNEY_DATA: JourneyStep[] = [
    {
        id: "landing",
        name: "Landing Page",
        conversionRate: 97,
        dropOffRate: 3,
        avgTime: "00:08",
    },
    {
        id: "product",
        name: "Produktseite",
        conversionRate: 84,
        dropOffRate: 16,
        avgTime: "00:34",
    },
    {
        id: "cart",
        name: "Warenkorb",
        conversionRate: 72,
        dropOffRate: 28,
        avgTime: "00:57",
    },
    {
        id: "checkout",
        name: "Checkout",
        conversionRate: 54,
        dropOffRate: 46,
        avgTime: "01:25",
    },
    {
        id: "confirmation",
        name: "Bestellung abgeschlossen",
        conversionRate: 100,
        dropOffRate: 0,
        avgTime: "—",
    },
];

// -----------------------------
// Hilfsfunktionen
// -----------------------------
function DropOffBadge({ rate }: { rate: number }) {
    if (rate >= 40) return <Badge color="red">{rate}%</Badge>;
    if (rate >= 20) return <Badge color="amber">{rate}%</Badge>;
    return <Badge color="green">{rate}%</Badge>;
}

// -----------------------------
// Hauptkomponente
// -----------------------------
export default function JourneyAnalysisPage() {
    // Auswertung
    const totalConversion = JOURNEY_DATA[JOURNEY_DATA.length - 1].conversionRate;
    const worstStep = JOURNEY_DATA.reduce((prev, curr) =>
        curr.dropOffRate > prev.dropOffRate ? curr : prev
    );

    return (
        <Box p="6" flexGrow="1">
            <MetricsTrends/>

            {/* Journey Map */}
            <Card variant="surface" mb="6">
                <Text weight="medium" mb="3">
                    Übersicht der Journey-Schritte
                </Text>

                <Flex
                    align="center"
                    justify="between"
                    style={{
                        overflowX: "auto",
                        paddingBottom: "8px",
                        gap: "16px",
                    }}
                >
                    {JOURNEY_DATA.map((step, i) => (
                        <React.Fragment key={step.id}>
                            <StepNode step={step} />
                            {i < JOURNEY_DATA.length - 1 && (
                                <MoveRightIcon/>
                            )}
                        </React.Fragment>
                    ))}
                </Flex>
            </Card>

            {/* Tabelle mit Details */}
            <Card variant="surface">
                <Text weight="medium" mb="3">
                    Schrittweise Analyse
                </Text>
                <Table.Root variant="surface" size="2">
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>Schritt</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Konversionsrate</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Drop-off-Rate</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Ø Verweildauer</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        {JOURNEY_DATA.map((s) => (
                            <Table.Row key={s.id}>
                                <Table.Cell>{s.name}</Table.Cell>
                                <Table.Cell>{s.conversionRate} %</Table.Cell>
                                <Table.Cell>
                                    <DropOffBadge rate={s.dropOffRate} />
                                </Table.Cell>
                                <Table.Cell>{s.avgTime}</Table.Cell>
                            </Table.Row>
                        ))}
                    </Table.Body>
                </Table.Root>
            </Card>
        </Box>
    );
}