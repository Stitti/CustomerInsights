import { jsxs as _jsxs, jsx as _jsx } from "react/jsx-runtime";
import { Box, Card, Flex, Text, Table, Badge, } from "@radix-ui/themes";
import { TrendingUp, TrendingDown, Minus } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { MetricsTrends } from "../components/MetricsTrends";
const CATEGORY_DATA = [
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
function SentimentBadge({ value }) {
    if (value >= 70)
        return _jsxs(Badge, { color: "green", children: [value, "% positiv"] });
    if (value >= 45)
        return _jsxs(Badge, { color: "amber", children: [value, "% neutral"] });
    return _jsxs(Badge, { color: "red", children: [value, "% negativ"] });
}
function TrendIndicator({ trend }) {
    if (trend > 0)
        return (_jsxs(Flex, { align: "center", gap: "1", children: [_jsx(TrendingUp, { size: 14 }), " +", trend, " %"] }));
    if (trend < 0)
        return (_jsxs(Flex, { align: "center", gap: "1", children: [_jsx(TrendingDown, { size: 14 }), " ", trend, " %"] }));
    return (_jsxs(Flex, { align: "center", gap: "1", children: [_jsx(Minus, { size: 14 }), " 0 %"] }));
}
// -----------------------------
// Hauptkomponente
// -----------------------------
export default function CategoryAnalysisPage() {
    const navigate = useNavigate();
    // KPI-Summary vorbereiten
    const totalMentions = CATEGORY_DATA.reduce((sum, c) => sum + c.share, 0);
    const topCategory = CATEGORY_DATA.sort((a, b) => b.share - a.share)[0];
    const negativeRatio = Math.round((CATEGORY_DATA.filter((c) => c.sentiment < 50).length /
        CATEGORY_DATA.length) *
        100);
    return (_jsxs(Box, { p: "6", flexGrow: "1", children: [_jsx(MetricsTrends, {}), _jsx(Card, { variant: "surface", children: _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: "Kategorie" }), _jsx(Table.ColumnHeaderCell, { children: "Erw\u00E4hnungsanteil" }), _jsx(Table.ColumnHeaderCell, { children: "Sentiment" }), _jsx(Table.ColumnHeaderCell, { children: "Trend" })] }) }), _jsx(Table.Body, { children: CATEGORY_DATA.map((c) => (_jsxs(Table.Row, { style: { cursor: "pointer" }, onClick: () => navigate(`/categories/${c.id}`), children: [_jsx(Table.Cell, { children: _jsx(Text, { weight: "medium", children: c.name }) }), _jsxs(Table.Cell, { children: [c.share, " %"] }), _jsx(Table.Cell, { children: _jsx(SentimentBadge, { value: c.sentiment }) }), _jsx(Table.Cell, { children: _jsx(TrendIndicator, { trend: c.trend }) })] }, c.id))) })] }) })] }));
}
