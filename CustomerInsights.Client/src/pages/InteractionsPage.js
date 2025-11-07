import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from "react";
import { Box, Button, Card, Flex, Heading, IconButton, Table, TextField } from "@radix-ui/themes";
import { SearchIcon, XIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { NIL as NIL_UUID } from 'uuid';
import { mockInteractions } from "../mock/mockInteractions";
export default function InteractionsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { t } = useTranslation();
    const interactions = mockInteractions;
    const filtered = interactions?.filter((a) => (a.title).toLowerCase().includes(search.toLowerCase()));
    return (_jsx(Flex, { style: { minHeight: '100vh' }, children: _jsxs(Box, { flexGrow: "1", p: "6", children: [_jsxs(Flex, { justify: "between", align: "center", mb: "5", children: [_jsx(Heading, { size: "5", children: "Interactions" }), _jsx(Button, { size: "2", onClick: () => navigate(NIL_UUID), style: { cursor: "pointer" }, children: "+ Add Interaction" })] }), _jsxs(Card, { variant: "surface", style: { marginTop: 16 }, children: [_jsxs(Flex, { mb: "4", gap: "3", align: "center", wrap: "wrap", children: [_jsx(TextField.Root, { placeholder: t("contact_page.search_contacts") ?? "Suchen...", value: search, onChange: (e) => setSearch(e.target.value), style: { flex: 1 }, children: _jsx(TextField.Slot, { children: _jsx(SearchIcon, { size: 16 }) }) }), _jsx(IconButton, { variant: "ghost", onClick: () => setSearch(""), children: _jsx(XIcon, { color: "grey", size: 20 }) })] }), _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: t("interaction_page.title") }), _jsx(Table.ColumnHeaderCell, { children: t("interaction_page.channel") }), _jsx(Table.ColumnHeaderCell, { children: t("interaction_page.contact_name") }), _jsx(Table.ColumnHeaderCell, { children: t("interaction_page.company_name") }), _jsx(Table.ColumnHeaderCell, { children: t("interaction_page.occurred_at") })] }) }), _jsx(Table.Body, { children: filtered.map((interaction) => (_jsxs(Table.Row, { onClick: () => navigate(`/interactions/${interaction.id}`), style: { cursor: 'pointer' }, children: [_jsx(Table.Cell, { children: interaction.title }), _jsx(Table.Cell, { children: interaction.channel }), _jsx(Table.Cell, { children: interaction.contactName }), _jsx(Table.Cell, { children: interaction.companyName }), _jsx(Table.Cell, { children: interaction.occurredAt })] }, interaction.id))) })] })] })] }) }));
}
