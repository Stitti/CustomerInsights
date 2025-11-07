import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from "react";
import { Box, Button, Card, Flex, Heading, IconButton, Table, TextField } from "@radix-ui/themes";
import { SearchIcon, XIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { NIL as NIL_UUID } from 'uuid';
import { mockAccounts } from "../mock/mockAccounts";
import { MetricsTrends } from "../components/MetricsTrends";
export default function AccountsPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { t } = useTranslation();
    const accounts = mockAccounts;
    const filtered = accounts?.filter((a) => (a.name).toLowerCase().includes(search.toLowerCase()));
    return (_jsx(Flex, { style: { minHeight: '100vh' }, children: _jsxs(Box, { flexGrow: "1", p: "6", children: [_jsx(MetricsTrends, {}), _jsxs(Flex, { justify: "between", align: "center", mb: "5", children: [_jsx(Heading, { size: "5", children: "Accounts" }), _jsx(Button, { size: "2", onClick: () => navigate(NIL_UUID), style: { cursor: "pointer" }, children: "+ Add Account" })] }), _jsxs(Card, { variant: "surface", style: { marginTop: 16 }, children: [_jsxs(Flex, { mb: "4", gap: "3", align: "center", wrap: "wrap", children: [_jsx(TextField.Root, { placeholder: t("contact_page.search_contacts") ?? "Suchen...", value: search, onChange: (e) => setSearch(e.target.value), style: { flex: 1 }, children: _jsx(TextField.Slot, { children: _jsx(SearchIcon, { size: 16 }) }) }), _jsx(IconButton, { variant: "ghost", onClick: () => setSearch(""), children: _jsx(XIcon, { color: "grey", size: 20 }) })] }), _jsxs(Table.Root, { variant: "surface", size: "2", children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: t("account_page.name") }), _jsx(Table.ColumnHeaderCell, { children: t("account_page.email") }), _jsx(Table.ColumnHeaderCell, { children: t("account_page.phone") }), _jsx(Table.ColumnHeaderCell, { children: t("account_page.company") })] }) }), _jsx(Table.Body, { children: filtered.map((account) => (_jsxs(Table.Row, { onClick: () => navigate(`/accounts/${account.id}`), style: { cursor: 'pointer' }, children: [_jsx(Table.Cell, { children: account.name }), _jsx(Table.Cell, { children: account.classification }), _jsx(Table.Cell, { children: account.industry }), _jsx(Table.Cell, { children: account.country })] }, account.id))) })] })] })] }) }));
}
