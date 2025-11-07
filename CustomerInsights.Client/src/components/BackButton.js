import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Button, Flex, Text } from "@radix-ui/themes";
import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
export default function BackButton() {
    const navigate = useNavigate();
    const { t } = useTranslation();
    return (_jsx(Flex, { justify: "between", align: "center", mb: "4", children: _jsx(Flex, { align: "center", gap: "3", children: _jsxs(Button, { variant: "ghost", color: "gray", onClick: () => navigate(-1), style: { cursor: "pointer" }, children: [_jsx(ArrowLeft, { size: 16 }), _jsx(Text, { ml: "2", children: t("back") || "Zurück" })] }) }) }));
}
