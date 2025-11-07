import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from "react";
import { Button, Card, Flex, Heading, Link, Text, TextField, } from "@radix-ui/themes";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import PasswordField from "../components/PasswordField";
import LightRays from "../components/LightRays";
function LoginPage() {
    const navigate = useNavigate();
    const [params] = useSearchParams();
    const returnUrl = decodeURIComponent(params.get("returnUrl") || "");
    const { t } = useTranslation();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    async function handleLogin() {
        const newErrors = {};
        if (!email.trim()) {
            newErrors.email = t("login_form.errors.email_required");
        }
        if (!password.trim()) {
            newErrors.password = t("login_form.errors.password_required");
        }
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }
        setErrors({});
        try {
            setIsLoading(true);
            //await loginAsync(email, password);
            if (returnUrl) {
                window.location.href = returnUrl;
            }
            else {
                navigate("/");
            }
        }
        catch (error) {
            console.error("Login error:", error);
            setErrors({ general: error?.response?.data?.error || error.message });
        }
        finally {
            setIsLoading(false);
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
        }, children: [isLoading && (_jsx(Flex, { style: {
                    position: "fixed",
                    top: 0,
                    left: 0,
                    width: "100vw",
                    height: "100vh",
                    backgroundColor: "rgba(0, 0, 0, 0.5)",
                    zIndex: 9999,
                    justifyContent: "center",
                    alignItems: "center",
                }, children: _jsx(Card, { style: { padding: "20px" }, children: _jsx(Heading, { size: "4", children: t("login_form.signin_loading") }) }) })), _jsxs(Card, { style: { padding: "40px", width: "35vw", minWidth: "500px", zIndex: 4 }, children: [_jsx(Heading, { style: { marginBottom: "25px" }, children: t("login_form.singin") }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("login_form.email_address") }), _jsx(TextField.Root, { value: email, onChange: (evt) => setEmail(evt.target.value), type: "email", placeholder: t("login_form.email_address_placeholder") }), errors.email && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.email }))] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "10px" }, children: [_jsxs(Flex, { style: { justifyContent: "space-between", alignItems: "center" }, children: [_jsx(Text, { size: "2", children: t("login_form.password") }), _jsx(Link, { href: "/reset-password", size: "2", children: t("login_form.forgot_password") })] }), _jsx(PasswordField, { value: password, onChange: (e) => setPassword(e.target.value), placeholder: t("login_form.password_placeholder"), showLabel: t("login_form.show_password"), hideLabel: t("login_form.hide_password") }), errors.password && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.password }))] }), errors.general && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem", marginBottom: "10px" }, children: errors.general })), _jsxs(Flex, { style: { marginTop: "25px", justifyContent: "flex-end" }, children: [_jsx(Button, { onClick: () => navigate("/register"), variant: "surface", style: { marginRight: "10px" }, children: t("login_form.create_account") }), _jsx(Button, { onClick: handleLogin, variant: "solid", children: t("login_form.signin") })] })] }), _jsx(LightRays, { raysOrigin: "top-center", raysColor: "#00ffff", raysSpeed: 1.5, lightSpread: 0.8, rayLength: 1.2, followMouse: true, mouseInfluence: 0.1, noiseAmount: 0.1, distortion: 0.05, className: "custom-rays" })] }));
}
export default LoginPage;
