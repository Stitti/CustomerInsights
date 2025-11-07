import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import React from "react";
import { Box, Card, Flex, Text } from "@radix-ui/themes";
import FunnelChart from "./FunnelChart";
import PainPointMatrix from "./PainPointMatrix";
import ConversionArea from "./ConversionArea";
import { funnelMock, painPointsMock, generateHeatMock, conversionMock } from "../mock/mockAnalytics";
export default function AnalyticsDashboard() {
    const heatData = React.useMemo(() => generateHeatMock(), []);
    return (_jsxs(Box, { mx: "auto", my: "6", p: "4", children: [_jsxs(Flex, { gap: "4", wrap: "wrap", children: [_jsxs(Card, { variant: "surface", style: { flex: 1, minWidth: 380 }, children: [_jsx(Text, { size: "5", mb: "3", children: "Funnel" }), _jsx(FunnelChart, { steps: funnelMock })] }), _jsxs(Card, { variant: "surface", style: { flex: 1, minWidth: 380 }, children: [_jsx(Text, { size: "5", mb: "3", children: "Pain Points" }), _jsx(PainPointMatrix, { data: painPointsMock })] })] }), _jsx(Flex, { gap: "4", wrap: "wrap", mt: "4", children: _jsxs(Card, { variant: "surface", style: { flex: 1, minWidth: 420 }, children: [_jsx(Text, { size: "5", mb: "3", children: "Conversion Rate (30 Tage)" }), _jsx(ConversionArea, { data: conversionMock })] }) })] }));
}
