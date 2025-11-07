import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Button, Card, Flex, Text, Badge, Table, Separator, TextArea } from "@radix-ui/themes";
import { AlertTriangle, TrendingDown, Activity, MessageSquareWarning, ShoppingCart, Clock3, Check } from "lucide-react";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import BackButton from "../components/BackButton";
const MOCK_SIGNAL = {
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
function PriorityBadge({ level }) {
    const map = {
        high: { color: "red", label: "High", icon: _jsx(AlertTriangle, { size: 14 }) },
        medium: { color: "amber", label: "Medium", icon: _jsx(TrendingDown, { size: 14 }) },
        low: { color: "grass", label: "Low", icon: _jsx(TrendingDown, { size: 14 }) }
    };
    const { color, label, icon } = map[level];
    return (_jsx(Badge, { color: color, variant: "solid", children: _jsxs(Flex, { align: "center", gap: "2", children: [icon, _jsx(Text, { size: "2", children: label })] }) }));
}
function TypeBadge({ type }) {
    const map = {
        KPI: { color: "crimson", icon: _jsx(Activity, { size: 14 }) },
        Emotion: { color: "plum", icon: _jsx(MessageSquareWarning, { size: 14 }) },
        Journey: { color: "indigo", icon: _jsx(ShoppingCart, { size: 14 }) },
        Support: { color: "cyan", icon: _jsx(Clock3, { size: 14 }) }
    };
    const { color, icon } = map[type];
    return (_jsx(Badge, { color: color, variant: "soft", radius: "full", children: _jsxs(Flex, { align: "center", gap: "2", children: [icon, _jsx(Text, { size: "2", children: type })] }) }));
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
        if (savedNote)
            setNotes(savedNote);
    }, [signal.id]);
    const handleSaveNote = () => {
        localStorage.setItem(`signal_note_${signal.id}`, notes);
        setSaved(true);
        setTimeout(() => setSaved(false), 1500);
    };
    return (_jsxs(Box, { p: "6", flexGrow: "1", children: [_jsx(BackButton, {}), _jsxs(Card, { variant: "surface", size: "3", children: [_jsxs(Flex, { justify: "between", align: "center", mb: "3", children: [_jsxs(Flex, { align: "center", gap: "3", children: [_jsx(PriorityBadge, { level: signal.priority }), _jsx(Text, { size: "4", weight: "bold", children: signal.title })] }), _jsx(TypeBadge, { type: signal.type })] }), _jsxs(Text, { color: "gray", size: "2", mb: "1", children: [signal.account, " \u2022 Erst erkannt: ", signal.createdAt] }), _jsx(Separator, { size: "4", my: "4" }), _jsxs(Flex, { direction: "column", gap: "2", mb: "4", children: [_jsxs(Flex, { justify: "between", children: [_jsx(Text, { color: "gray", children: "Schwelle:" }), _jsx(Text, { weight: "medium", children: signal.threshold })] }), _jsxs(Flex, { justify: "between", children: [_jsx(Text, { color: "gray", children: "Aktueller Wert:" }), _jsx(Text, { weight: "medium", color: "red", children: signal.currentValue })] }), _jsxs(Flex, { justify: "between", children: [_jsx(Text, { color: "gray", children: "Trend:" }), _jsx(Text, { children: signal.trend })] }), _jsxs(Flex, { justify: "between", children: [_jsx(Text, { color: "gray", children: "H\u00E4ufigkeit:" }), _jsx(Text, { children: signal.occurrences })] }), _jsxs(Flex, { justify: "between", children: [_jsx(Text, { color: "gray", children: "Journey-Schritte:" }), _jsx(Text, { children: signal.journeySteps.join(", ") })] })] }), _jsx(Separator, { size: "4", my: "4" }), _jsx(Text, { weight: "medium", mb: "2", children: "Empfohlene Ma\u00DFnahmen" }), _jsx("ul", { style: {
                            marginLeft: "20px",
                            marginBottom: "12px",
                            color: "var(--gray-11)",
                            fontSize: "14px"
                        }, children: signal.recommendations.map((r, i) => (_jsx("li", { children: r }, i))) }), _jsxs(Flex, { gap: "2", mb: "4", children: [_jsx(Button, { color: "green", children: "Ma\u00DFnahme starten" }), _jsx(Button, { variant: "soft", children: "Als erledigt markieren" })] }), _jsx(Separator, { size: "4", my: "4" }), _jsx(Text, { weight: "medium", mb: "2", children: "Notizen & Beschreibung" }), _jsx(TextArea, { value: notes, onChange: (e) => setNotes(e.target.value), placeholder: "Schreibe hier deine Beobachtungen, To-Dos oder Kommentare...", style: {
                            minHeight: "120px",
                            resize: "vertical",
                            width: "100%",
                            fontSize: "14px",
                            lineHeight: "1.5"
                        } }), _jsxs(Flex, { justify: "end", mt: "2", align: "center", gap: "3", children: [saved && (_jsxs(Flex, { align: "center", gap: "2", children: [_jsx(Check, { size: 16, color: "var(--grass-11)" }), _jsx(Text, { color: "green", children: "Gespeichert" })] })), _jsx(Button, { onClick: handleSaveNote, variant: "soft", children: "Speichern" })] }), _jsx(Separator, { size: "4", my: "4" }), _jsx(Text, { weight: "medium", mb: "3", children: "Verkn\u00FCpfte Kundenanfragen" }), _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: "Datum" }), _jsx(Table.ColumnHeaderCell, { children: "Typ" }), _jsx(Table.ColumnHeaderCell, { children: "Sentiment" })] }) }), _jsx(Table.Body, { children: signal.relatedTickets.map((t) => (_jsxs(Table.Row, { children: [_jsx(Table.Cell, { children: t.date }), _jsx(Table.Cell, { children: t.type }), _jsx(Table.Cell, { children: _jsx(Badge, { color: t.sentiment === "Positive"
                                                    ? "grass"
                                                    : t.sentiment === "Neutral"
                                                        ? "amber"
                                                        : "red", variant: "soft", children: t.sentiment }) })] }, t.id))) })] })] })] }));
}
