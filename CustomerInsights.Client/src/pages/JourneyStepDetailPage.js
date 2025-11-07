import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useEffect, useMemo, useState } from "react";
import { Box, Card, Flex, Text, Badge, Separator, Table, Button, TextArea, Progress } from "@radix-ui/themes";
import { useNavigate, useParams } from "react-router-dom";
import { ArrowLeft, Timer, BarChart3, Check } from "lucide-react";
import BackButton from "../components/BackButton";
const JOURNEY_DETAILS = {
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
function SentimentBadge({ s }) {
    const color = s === "Positive" ? "green" : s === "Neutral" ? "amber" : "red";
    return _jsx(Badge, { color: color, variant: "soft", children: s });
}
function DropOffBadge({ rate }) {
    if (rate >= 40)
        return _jsxs(Badge, { color: "red", children: [rate, "%"] });
    if (rate >= 20)
        return _jsxs(Badge, { color: "amber", children: [rate, "%"] });
    return _jsxs(Badge, { color: "green", children: [rate, "%"] });
}
// --------------------------------------------------
// Seite
// --------------------------------------------------
export default function JourneyStepDetailPage() {
    const { stepId } = useParams();
    const navigate = useNavigate();
    const step = useMemo(() => {
        if (!stepId)
            return undefined;
        return JOURNEY_DETAILS[stepId];
    }, [stepId]);
    // Notizen mit einfacher Persistenz
    const [notes, setNotes] = useState("");
    const [saved, setSaved] = useState(false);
    useEffect(() => {
        if (!step)
            return;
        const key = `journey_step_notes_${step.id}`;
        const stored = localStorage.getItem(key);
        if (stored)
            setNotes(stored);
    }, [step]);
    const handleSaveNotes = () => {
        if (!step)
            return;
        localStorage.setItem(`journey_step_notes_${step.id}`, notes);
        setSaved(true);
        setTimeout(() => setSaved(false), 1200);
    };
    if (!step) {
        return (_jsxs(Box, { p: "6", children: [_jsxs(Button, { variant: "ghost", onClick: () => navigate(-1), children: [_jsx(ArrowLeft, { size: 16 }), " ", _jsx(Text, { ml: "2", children: "Zur\u00FCck" })] }), _jsx(Card, { mt: "3", style: { padding: "20px" }, children: _jsx(Text, { children: "Schritt nicht gefunden." }) })] }));
    }
    // Ableitung für Header-Badge
    const priority = step.dropOffRate >= 40 ? "high" : step.dropOffRate >= 20 ? "medium" : "low";
    const priorityColor = priority === "high" ? "red" : priority === "medium" ? "amber" : "green";
    return (_jsxs(Box, { p: "6", flexGrow: "1", children: [_jsx(BackButton, {}), _jsxs(Card, { variant: "surface", size: "3", children: [_jsxs(Flex, { justify: "between", align: "center", mb: "2", children: [_jsxs(Flex, { align: "center", gap: "3", children: [_jsx(Badge, { color: priorityColor, variant: "solid", children: priority === "high" ? "Hohe Priorität" : priority === "medium" ? "Mittlere Priorität" : "Niedrige Priorität" }), _jsx(Text, { size: "4", weight: "bold", children: step.name })] }), _jsx(Text, { color: "gray", children: step.description })] }), _jsx(Separator, { my: "4", size: "4" }), _jsxs(Flex, { gap: "4", wrap: "wrap", mb: "5", children: [_jsx(Card, { variant: "surface", style: { minWidth: 220 }, children: _jsxs(Flex, { direction: "column", p: "3", gap: "1", children: [_jsx(Text, { size: "2", color: "gray", children: "Konversionsrate" }), _jsxs(Text, { size: "4", weight: "bold", children: [step.conversionRate, "%"] }), _jsx(Progress, { value: Math.min(step.conversionRate, 100) })] }) }), _jsx(Card, { variant: "surface", style: { minWidth: 220 }, children: _jsxs(Flex, { direction: "column", p: "3", gap: "1", children: [_jsx(Text, { size: "2", color: "gray", children: "Drop-off-Rate" }), _jsxs(Text, { size: "4", weight: "bold", color: priorityColor, children: [step.dropOffRate, "%"] }), _jsx(DropOffBadge, { rate: step.dropOffRate })] }) }), _jsx(Card, { variant: "surface", style: { minWidth: 220 }, children: _jsxs(Flex, { direction: "column", p: "3", gap: "1", children: [_jsx(Text, { size: "2", color: "gray", children: "\u00D8 Verweildauer" }), _jsxs(Flex, { align: "center", gap: "2", children: [_jsx(Timer, { size: 16 }), _jsx(Text, { size: "4", weight: "bold", children: step.avgTime })] })] }) }), typeof step.sessions === "number" && (_jsx(Card, { variant: "surface", style: { minWidth: 220 }, children: _jsxs(Flex, { direction: "column", p: "3", gap: "1", children: [_jsx(Text, { size: "2", color: "gray", children: "Sessions" }), _jsxs(Flex, { align: "center", gap: "2", children: [_jsx(BarChart3, { size: 16 }), _jsx(Text, { size: "4", weight: "bold", children: step.sessions.toLocaleString() })] })] }) }))] }), _jsx(Separator, { my: "4", size: "4" }), _jsx(Text, { weight: "medium", mb: "3", children: "Verkn\u00FCpfte Kundenanfragen" }), _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: "Datum" }), _jsx(Table.ColumnHeaderCell, { children: "Ticket" }), _jsx(Table.ColumnHeaderCell, { children: "Betreff" }), _jsx(Table.ColumnHeaderCell, { children: "Sentiment" })] }) }), _jsx(Table.Body, { children: step.relatedTickets.length === 0 ? (_jsx(Table.Row, { children: _jsx(Table.Cell, { colSpan: 4, children: _jsx(Text, { color: "gray", children: "Keine verkn\u00FCpften Anfragen." }) }) })) : (step.relatedTickets.map((t) => (_jsxs(Table.Row, { children: [_jsx(Table.Cell, { children: t.date }), _jsx(Table.Cell, { children: t.id }), _jsx(Table.Cell, { children: t.subject }), _jsx(Table.Cell, { children: _jsx(SentimentBadge, { s: t.sentiment }) })] }, t.id)))) })] }), _jsx(Separator, { my: "4", size: "4" }), _jsx(Text, { weight: "medium", mb: "2", children: "Empfohlene Ma\u00DFnahmen" }), step.recommendations.length ? (_jsx("ul", { style: { marginLeft: 18, color: "var(--gray-11)", fontSize: 14 }, children: step.recommendations.map((r, idx) => _jsx("li", { children: r }, idx)) })) : (_jsx(Text, { color: "gray", children: "Keine Empfehlungen hinterlegt." })), _jsx(Separator, { my: "4", size: "4" }), _jsx(Text, { weight: "medium", mb: "2", children: "Notizen" }), _jsx(TextArea, { value: notes, onChange: (e) => setNotes(e.target.value), placeholder: "Eigene Beobachtungen oder Vereinbarungen dokumentieren\u2026", style: { width: "100%", minHeight: 120, resize: "vertical" } }), _jsxs(Flex, { justify: "end", align: "center", gap: "3", mt: "2", children: [saved && (_jsxs(Flex, { align: "center", gap: "2", children: [_jsx(Check, { size: 16, color: "var(--grass-11)" }), _jsx(Text, { color: "green", children: "Gespeichert" })] })), _jsx(Button, { onClick: handleSaveNotes, variant: "soft", children: "Speichern" })] })] })] }));
}
