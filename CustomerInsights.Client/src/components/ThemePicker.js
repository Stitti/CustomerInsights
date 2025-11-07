import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useThemeMode } from "../theme/ThemeProvider";
import { Box, Flex, Text, Card } from "@radix-ui/themes";
import { MoonIcon, SunIcon, MonitorIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import { setThemeCookie } from "../theme/themeCookie";
export default function ThemePicker() {
    const { t } = useTranslation();
    const { appearance, setAppearance } = useThemeMode();
    const options = [
        { id: "light", label: t('settings.light'), icon: SunIcon },
        { id: "dark", label: t('settings.dark'), icon: MoonIcon },
        { id: "inherit", label: t('settings.system'), icon: MonitorIcon },
    ];
    return (_jsx(Card, { variant: "surface", size: "3", mb: "6", children: _jsxs(Box, { children: [_jsx(Text, { as: "label", size: "3", weight: "bold", style: { display: "block", marginBottom: "1rem" }, children: t('settings.theme') }), _jsx(Flex, { gap: "7", justify: "center", direction: "row", children: options.map(({ id, label, icon: Icon }) => (_jsx(Card, { onClick: () => {
                            setAppearance(id);
                            setThemeCookie(id);
                        }, style: {
                            cursor: "pointer",
                            padding: "1rem",
                            flex: 1,
                            border: appearance === id ? "2px solid var(--accent-9)" : "1px solid var(--gray-6)",
                            backgroundColor: "var(--gray-2)",
                            textAlign: "center",
                            transition: "border-color 0.2s ease",
                            maxWidth: "300px"
                        }, children: _jsxs(Flex, { direction: "column", align: "center", gap: "2", children: [_jsx(Icon, { size: 32 }), _jsx(Text, { size: "2", children: label })] }) }, id))) })] }) }));
}
