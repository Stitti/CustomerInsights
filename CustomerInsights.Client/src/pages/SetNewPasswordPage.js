import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Button, Card, Flex, Heading, Text } from "@radix-ui/themes";
import { useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import PasswordField from "../components/PasswordField";
import LightRays from "../components/LightRays";
function SetNewPasswordPage() {
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [errors, setErrors] = useState({});
    const [success, setSuccess] = useState(null);
    const [searchParams] = useSearchParams();
    const { t } = useTranslation();
    const navigate = useNavigate();
    const oobCode = searchParams.get("oobCode") || "";
    async function handleResetPassword() {
        const newErrors = {};
        if (!password.trim()) {
            newErrors.password = t("set_password_page.errors.password_required");
        }
        if (password && password.length < 6) {
            newErrors.password = t("set_password_page.errors.password_length");
        }
        if (!confirmPassword.trim()) {
            newErrors.confirmPassword = t("set_password_page.errors.password_confirm");
        }
        if (password && confirmPassword && password !== confirmPassword) {
            newErrors.confirmPassword = t("set_password_page.errors.matching");
        }
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }
        else {
            setErrors({});
        }
        try {
            //await confirmPasswordResetAsync(oobCode, password);
            setSuccess(t("set_password_page.success"));
            setTimeout(() => {
                navigate("/login");
            }, 2000);
        }
        catch (err) {
            console.error("Error resetting password:", err);
            setErrors({ general: t("set_password_page.reset_failed") });
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
        }, children: [_jsxs(Card, { style: { padding: '40px', width: '35vw', minWidth: '500px' }, children: [_jsx(Heading, { style: { marginBottom: "25px" }, children: t("set_password_page.set_password") }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("set_password_page.password") }), _jsx(PasswordField, { value: password, onChange: evt => setPassword(evt.target.value), placeholder: t("set_password_page.password_placeholder"), showLabel: t("login_form.show_password"), hideLabel: t("login_form.hide_password") }), errors.password && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.password }))] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("set_password_page.confirm_password") }), _jsx(PasswordField, { value: confirmPassword, onChange: evt => setConfirmPassword(evt.target.value), placeholder: t("set_password_page.confirm_password_placeholder"), showLabel: t("login_form.show_password"), hideLabel: t("login_form.hide_password") }), errors.confirmPassword && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.confirmPassword }))] }), errors.general && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem", marginTop: "10px" }, children: errors.general })), success && (_jsx(Text, { style: { color: "green", fontSize: "0.8rem", marginTop: "10px" }, children: success })), _jsx(Flex, { style: { marginTop: '25px', justifyContent: 'flex-end' }, children: _jsx(Button, { variant: "solid", onClick: handleResetPassword, children: t("set_password_page.set_password") }) })] }), _jsx(LightRays, { raysOrigin: "top-center", raysColor: "#00ffff", raysSpeed: 1.5, lightSpread: 0.8, rayLength: 1.2, followMouse: true, mouseInfluence: 0.1, noiseAmount: 0.1, distortion: 0.05, className: "custom-rays" })] }));
}
export default SetNewPasswordPage;
