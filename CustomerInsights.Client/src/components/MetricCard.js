import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Badge, Card, Flex, Text } from "@radix-ui/themes";
export function MetricCard({ title, value, trend }) {
    return (_jsxs(Card, { style: { flex: 1, padding: "1rem" }, variant: "surface", mb: "4", children: [_jsx(Text, { size: "2", weight: "bold", children: title }), _jsxs(Flex, { style: { justifyContent: "space-between", marginTop: "1rem" }, children: [_jsx(Text, { size: "5", children: value }), _jsx(Badge, { color: "green", children: _jsxs(Text, { children: [trend, "%"] }) })] })] }));
}
