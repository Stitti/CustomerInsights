import { useThemeMode } from "../theme/ThemeProvider";
import { Box, Flex, Text, Card } from "@radix-ui/themes";
import { MoonIcon, SunIcon, MonitorIcon } from "lucide-react";
import {useTranslation} from "react-i18next";
import {setThemeCookie} from "../theme/themeCookie";

export default function ThemePicker() {
    const { t } = useTranslation();
    const { appearance, setAppearance } = useThemeMode();

    const options = [
        { id: "light", label: t('settings.light'), icon: SunIcon },
        { id: "dark", label: t('settings.dark'), icon: MoonIcon },
        { id: "inherit", label: t('settings.system'), icon: MonitorIcon },
    ];

    return (
        <Card variant="surface" size="3" mb="6">
        <Box>
            <Text as="label" size="3" weight="bold" style={{ display: "block", marginBottom: "1rem" }}>
                {t('settings.theme')}
            </Text>
            <Flex gap="7" justify="center" direction="row">
                {options.map(({ id, label, icon: Icon }) => (
                    <Card
                        key={id}
                        onClick={() => {
                            setAppearance(id as typeof appearance);
                            setThemeCookie(id as typeof appearance)
                        }}
                        style={{
                            cursor: "pointer",
                            padding: "1rem",
                            flex: 1,
                            border: appearance === id ? "2px solid var(--accent-9)" : "1px solid var(--gray-6)",
                            backgroundColor: "var(--gray-2)",
                            textAlign: "center",
                            transition: "border-color 0.2s ease",
                            maxWidth: "300px"
                        }}
                    >
                        <Flex direction="column" align="center" gap="2">
                            <Icon size={32} />
                            <Text size="2">{label}</Text>
                        </Flex>
                    </Card>
                ))}
            </Flex>
        </Box>
        </Card>
    );
}