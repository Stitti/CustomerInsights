import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Flex } from "@radix-ui/themes";
import { MetricCard } from "./MetricCard";
export function MetricsTrends() {
    return (_jsxs(Flex, { gap: "4", direction: "row", wrap: "wrap", children: [_jsx(MetricCard, { title: "Satisfaction Index", value: "78", trend: 3 }), _jsx(MetricCard, { title: "Interactions", value: "1432", trend: -9 }), _jsx(MetricCard, { title: "Model Confidence", value: "82%", trend: 4 }), _jsx(MetricCard, { title: "High Urgency Interactions", value: "212", trend: 13 })] }));
}
