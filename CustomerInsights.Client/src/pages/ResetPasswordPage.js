import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Button, Card, Flex, Heading, Text, TextField } from "@radix-ui/themes";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import LightRays from "../components/LightRays";
function ResetPasswordPage() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [errors, setErrors] = useState({});
    const { t } = useTranslation();
    async function resetPassword() {
        if (!email.trim()) {
            setErrors({ email: t("reset_page.email_required") });
            return;
        }
        setErrors({});
        try {
            //await resetPasswordAsync(email);
            navigate("/login");
        }
        catch (error) {
            console.error("Error sending password reset email:", error);
            setErrors({ general: error.message || t("reset_page.sending_failed") });
        }
    }
    return (_jsxs("div", { style: {
            width: "100%",
            height: "100vh",
            overflow: "hidden", // Kein Scrollen
            display: "flex",
            justifyContent: "center", // Horizontal zentrieren
            alignItems: "center", // Vertikal zentrieren
            margin: "-15px"
        }, children: [_jsxs(Card, { style: { padding: '40px', width: '35vw', minWidth: '500px' }, children: [_jsx(Heading, { style: { marginBottom: "25px" }, children: t("reset_page.reset_password") }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("reset_page.email_address") }), _jsx(TextField.Root, { value: email, type: "email", onChange: evt => setEmail(evt.target.value), placeholder: t("reset_page.email_address_placeholder") }), errors.email && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.email })), errors.general && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.general }))] }), _jsx(Flex, { style: { marginTop: '25px', justifyContent: 'flex-end' }, children: _jsx(Button, { onClick: resetPassword, variant: "solid", children: t("reset_page.reset_password") }) })] }), _jsx(LightRays, { raysOrigin: "top-center", raysColor: "#00ffff", raysSpeed: 1.5, lightSpread: 0.8, rayLength: 1.2, followMouse: true, mouseInfluence: 0.1, noiseAmount: 0.1, distortion: 0.05, className: "custom-rays" })] }));
}
export default ResetPasswordPage;
