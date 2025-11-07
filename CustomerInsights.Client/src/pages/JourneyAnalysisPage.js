import { jsxs as _jsxs, jsx as _jsx } from "react/jsx-runtime";
import { Box, Card, Flex, Text, Badge, Table, } from "@radix-ui/themes";
import { MoveRightIcon } from "lucide-react";
import React from "react";
import { MetricsTrends } from "../components/MetricsTrends";
import { StepNode } from "../components/StepNode";
// -----------------------------
// Datentypen & Mockdaten
// -----------------------------
const JOURNEY_DATA = [
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
function DropOffBadge({ rate }) {
    if (rate >= 40)
        return _jsxs(Badge, { color: "red", children: [rate, "%"] });
    if (rate >= 20)
        return _jsxs(Badge, { color: "amber", children: [rate, "%"] });
    return _jsxs(Badge, { color: "green", children: [rate, "%"] });
}
// -----------------------------
// Hauptkomponente
// -----------------------------
export default function JourneyAnalysisPage() {
    // Auswertung
    const totalConversion = JOURNEY_DATA[JOURNEY_DATA.length - 1].conversionRate;
    const worstStep = JOURNEY_DATA.reduce((prev, curr) => curr.dropOffRate > prev.dropOffRate ? curr : prev);
    return (_jsxs(Box, { p: "6", flexGrow: "1", children: [_jsx(MetricsTrends, {}), _jsxs(Card, { variant: "surface", mb: "6", children: [_jsx(Text, { weight: "medium", mb: "3", children: "\u00DCbersicht der Journey-Schritte" }), _jsx(Flex, { align: "center", justify: "between", style: {
                            overflowX: "auto",
                            paddingBottom: "8px",
                            gap: "16px",
                        }, children: JOURNEY_DATA.map((step, i) => (_jsxs(React.Fragment, { children: [_jsx(StepNode, { step: step }), i < JOURNEY_DATA.length - 1 && (_jsx(MoveRightIcon, {}))] }, step.id))) })] }), _jsxs(Card, { variant: "surface", children: [_jsx(Text, { weight: "medium", mb: "3", children: "Schrittweise Analyse" }), _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: "Schritt" }), _jsx(Table.ColumnHeaderCell, { children: "Konversionsrate" }), _jsx(Table.ColumnHeaderCell, { children: "Drop-off-Rate" }), _jsx(Table.ColumnHeaderCell, { children: "\u00D8 Verweildauer" })] }) }), _jsx(Table.Body, { children: JOURNEY_DATA.map((s) => (_jsxs(Table.Row, { children: [_jsx(Table.Cell, { children: s.name }), _jsxs(Table.Cell, { children: [s.conversionRate, " %"] }), _jsx(Table.Cell, { children: _jsx(DropOffBadge, { rate: s.dropOffRate }) }), _jsx(Table.Cell, { children: s.avgTime })] }, s.id))) })] })] })] }));
}
