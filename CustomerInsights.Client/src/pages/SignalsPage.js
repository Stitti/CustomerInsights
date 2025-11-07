import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Card, Flex, Table, TextField, Badge, Separator, Text, IconButton } from "@radix-ui/themes";
import { MetricsTrends } from "../components/MetricsTrends";
import { Search as SearchIcon, AlertTriangle, Activity, MessageSquareWarning, ShoppingCart, Clock3, TrendingDown, XIcon } from "lucide-react";
import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
const MOCK_SIGNALS = [
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
function TypeBadge({ type }) {
    const map = {
        KPI: { color: "crimson", icon: _jsx(Activity, { size: 14 }) },
        Emotion: { color: "plum", icon: _jsx(MessageSquareWarning, { size: 14 }) },
        Journey: { color: "indigo", icon: _jsx(ShoppingCart, { size: 14 }) },
        Support: { color: "cyan", icon: _jsx(Clock3, { size: 14 }) }
    };
    const { color, icon } = map[type];
    return (_jsx(Badge, { color: color, children: _jsxs(Flex, { align: "center", gap: "2", children: [icon, _jsx(Text, { size: "2", children: type })] }) }));
}
function PriorityBadge({ level }) {
    const map = {
        high: { color: "red", label: "High", icon: _jsx(AlertTriangle, { size: 14 }) },
        medium: { color: "amber", label: "Medium", icon: _jsx(TrendingDown, { size: 14 }) },
        low: { color: "grass", label: "Low", icon: _jsx(TrendingDown, { size: 14 }) }
    };
    const { color, label, icon } = map[level];
    return (_jsx(Badge, { color: color, children: _jsxs(Flex, { align: "center", gap: "2", children: [icon, _jsx(Text, { size: "2", children: label })] }) }));
}
export default function SignalsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState("");
    const { t } = useTranslation();
    const filtered = useMemo(() => {
        const q = search.trim().toLowerCase();
        if (!q)
            return MOCK_SIGNALS;
        return MOCK_SIGNALS.filter((s) => s.title.toLowerCase().includes(q) ||
            s.account.toLowerCase().includes(q) ||
            s.type.toLowerCase().includes(q) ||
            s.thresholdLabel.toLowerCase().includes(q));
    }, [search]);
    return (_jsxs(Box, { flexGrow: "1", p: "6", children: [_jsx(MetricsTrends, {}), _jsxs(Card, { variant: "surface", style: { marginTop: 16 }, children: [_jsxs(Flex, { mb: "4", gap: "3", align: "center", wrap: "wrap", children: [_jsx(TextField.Root, { placeholder: t("contact_page.search_contacts") ?? "Suchen...", value: search, onChange: (e) => setSearch(e.target.value), style: { flex: 1 }, children: _jsx(TextField.Slot, { children: _jsx(SearchIcon, { size: 16 }) }) }), _jsx(IconButton, { variant: "ghost", onClick: () => setSearch(""), children: _jsx(XIcon, { color: "grey", size: 20 }) })] }), _jsx(Separator, { size: "4", my: "2" }), _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { style: { width: 340 }, children: "Signal" }), _jsx(Table.ColumnHeaderCell, { children: "Account" }), _jsx(Table.ColumnHeaderCell, { children: "Type" }), _jsx(Table.ColumnHeaderCell, { children: "Threshold" }), _jsx(Table.ColumnHeaderCell, { children: "Current" }), _jsx(Table.ColumnHeaderCell, { children: "Priority" }), _jsx(Table.ColumnHeaderCell, { children: "Created" })] }) }), _jsx(Table.Body, { children: filtered.map((s) => (_jsxs(Table.Row, { onClick: () => navigate("/signals/123"), style: { cursor: "pointer" }, children: [_jsx(Table.Cell, { children: _jsx(Flex, { direction: "column", gap: "1", children: _jsx(Text, { weight: "medium", children: s.title }) }) }), _jsx(Table.Cell, { children: _jsx(Text, { children: s.account }) }), _jsx(Table.Cell, { children: _jsx(TypeBadge, { type: s.type }) }), _jsx(Table.Cell, { children: _jsx(Text, { color: "gray", children: s.thresholdLabel }) }), _jsx(Table.Cell, { children: _jsx(Text, { weight: "medium", children: s.valueLabel }) }), _jsx(Table.Cell, { children: _jsx(PriorityBadge, { level: s.priority }) }), _jsx(Table.Cell, { children: _jsx(Text, { color: "gray", children: s.createdAt }) })] }, s.id))) })] })] })] }));
}
