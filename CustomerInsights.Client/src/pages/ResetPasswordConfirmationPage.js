import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Card, Flex, Heading, Text } from "@radix-ui/themes";
import { CheckCircleIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import LightRays from "../components/LightRays";
function ResetPasswordConfirmationPage() {
    const { t } = useTranslation();
    return (_jsxs("div", { style: {
            width: "100%",
            height: "100vh",
            overflow: "hidden", // Kein Scrollen
            display: "flex",
            justifyContent: "center", // Horizontal zentrieren
            alignItems: "center", // Vertikal zentrieren
            margin: "-15px"
        }, children: [_jsxs(Card, { style: { padding: "40px", width: "35vw", minWidth: "500px" }, children: [_jsx(Heading, { style: { marginBottom: "25px", textAlign: "center" }, children: t("password_reset_confirmation_page.reset_requested_heading") }), _jsxs(Flex, { direction: "column", align: "center", gap: "20", children: [_jsx(CheckCircleIcon, { style: { height: "60px", width: "60px", marginBottom: "20px" } }), _jsx(Text, { style: { textAlign: "center", marginBottom: "10px" }, children: t("password_reset_confirmation_page.reset_requested_text") }), _jsx("br", {}), _jsx(Text, { style: { textAlign: "center" }, children: t("password_reset_confirmation_page.reset_requested_sub") })] })] }), _jsx(LightRays, { raysOrigin: "top-center", raysColor: "#00ffff", raysSpeed: 1.5, lightSpread: 0.8, rayLength: 1.2, followMouse: true, mouseInfluence: 0.1, noiseAmount: 0.1, distortion: 0.05, className: "custom-rays" })] }));
}
export default ResetPasswordConfirmationPage;
