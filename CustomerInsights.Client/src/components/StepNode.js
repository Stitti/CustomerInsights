import { jsxs as _jsxs, jsx as _jsx } from "react/jsx-runtime";
import { Flex, Text, Separator, Badge } from "@radix-ui/themes";
function DropOffBadge({ rate }) {
    if (rate >= 40)
        return _jsxs(Badge, { color: "red", children: [rate, "%"] });
    if (rate >= 20)
        return _jsxs(Badge, { color: "amber", children: [rate, "%"] });
    return _jsxs(Badge, { color: "green", children: [rate, "%"] });
}
export function StepNode({ step }) {
    return (_jsxs(Flex, { direction: "column", align: "start", style: {
            width: 220,
            minHeight: 132,
            padding: 12,
            borderRadius: 10,
            background: "var(--color-panel-solid)",
            boxShadow: "inset 0 0 0 1px var(--gray-a4)"
        }, children: [_jsx(Text, { weight: "medium", children: step.name }), _jsx(Separator, { my: "2", size: "2" }), _jsxs(Flex, { justify: "between", align: "center", style: { width: "100%" }, children: [_jsx(Text, { size: "2", color: "gray", children: "Konversion" }), _jsxs(Text, { size: "2", weight: "medium", children: [step.conversionRate, "%"] })] }), _jsxs(Flex, { justify: "between", align: "center", mt: "1", style: { width: "100%" }, children: [_jsx(Text, { size: "2", color: "gray", children: "Drop-off" }), _jsx(DropOffBadge, { rate: step.dropOffRate })] })] }));
}
