import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useEffect, useState } from "react";
import { Button, Card, Flex, Heading, Text, TextField, SegmentedControl, Checkbox } from "@radix-ui/themes";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import PasswordField from "../components/PasswordField";
import LightRays from "../components/LightRays";
function RegisterPage() {
    const navigate = useNavigate();
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const [firstname, setFirstname] = useState("");
    const [lastname, setLastname] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [orgMode, setOrgMode] = useState("invitation");
    const [invitationCode, setInvitationCode] = useState("");
    const [organizationName, setOrganizationName] = useState("");
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    useEffect(() => {
        const code = searchParams.get("invitationCode");
        if (code) {
            setInvitationCode(code);
            setOrgMode("invitation");
        }
    }, [searchParams]);
    async function handleRegister() {
        const newErrors = {};
        if (!firstname.trim())
            newErrors.firstname = t("register_form.errors.firstname_required");
        if (!lastname.trim())
            newErrors.lastname = t("register_form.errors.lastname_required");
        if (!email.trim())
            newErrors.email = t("register_form.errors.email_required");
        if (!password)
            newErrors.password = t("register_form.errors.password_required");
        if (password && password.length < 6)
            newErrors.password = t("register_form.errors.password_length");
        if (!confirmPassword)
            newErrors.confirmPassword = t("register_form.errors.password_confirm");
        if (password && confirmPassword && password !== confirmPassword) {
            newErrors.confirmPassword = t("register_form.errors.password_matching");
        }
        if (orgMode === "invitation" && !invitationCode.trim()) {
            newErrors.invitationCode = t("register_form.errors.invitation_code_required");
        }
        if (orgMode === "neworg" && !organizationName.trim()) {
            newErrors.organizationName = t("register_form.errors.org_name_required");
        }
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }
        else {
            setErrors({});
        }
        const registerRequest /*: RegisterRequest*/ = {
            firstname,
            lastname,
            email,
            password,
            confirmPassword,
            invitationCode: orgMode === "invitation" ? invitationCode : undefined,
            organizationName: orgMode === "neworg" ? organizationName : undefined,
        };
        try {
            setIsLoading(true);
            //const result = await registerAsync(registerRequest);
            //console.log("Registration successful:", result);
            navigate("/login");
        }
        catch (error) {
            console.error("Registration failed:", error);
        }
        finally {
            setIsLoading(false);
        }
    }
    return (_jsxs("div", { style: {
            width: "100vw",
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
                    backgroundColor: "rgba(0,0,0,0.5)",
                    zIndex: 9999,
                    justifyContent: "center",
                    alignItems: "center",
                }, children: _jsx(Card, { style: { padding: "20px" }, children: _jsx(Text, { size: "3", children: t("register_form.register_loading") }) }) })), _jsxs(Card, { style: { padding: "40px", width: "35vw", minWidth: "500px", zIndex: 4 }, children: [_jsx(Heading, { style: { marginBottom: "25px" }, children: t("register_form.create_account") }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("register_form.firstname") }), _jsx(TextField.Root, { placeholder: t("register_form.firstname_placeholder"), value: firstname, onChange: (e) => setFirstname(e.target.value) }), errors.firstname && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.firstname }))] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("register_form.lastname") }), _jsx(TextField.Root, { placeholder: t("register_form.lastname_placeholder"), value: lastname, onChange: (e) => setLastname(e.target.value) }), errors.lastname && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.lastname }))] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("register_form.email") }), _jsx(TextField.Root, { placeholder: t("register_form.email_placeholder"), type: "email", value: email, onChange: (e) => setEmail(e.target.value) }), errors.email && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.email }))] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("register_form.password") }), _jsx(PasswordField, { value: password, onChange: (e) => setPassword(e.target.value), placeholder: t("register_form.password_placeholder"), showLabel: t("login_form.show_password"), hideLabel: t("login_form.hide_password") }), errors.password && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.password }))] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: t("register_form.confirm_password") }), _jsx(PasswordField, { value: confirmPassword, onChange: (e) => setConfirmPassword(e.target.value), placeholder: t("register_form.password_placeholder"), showLabel: t("login_form.show_password"), hideLabel: t("login_form.hide_password") }), errors.confirmPassword && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.confirmPassword }))] }), _jsxs(Flex, { direction: "column", gap: "2", style: { marginBottom: "20px" }, children: [_jsxs(SegmentedControl.Root, { size: "1", value: orgMode, onValueChange: (value) => setOrgMode(value), children: [_jsx(SegmentedControl.Item, { value: "invitation", children: t("register_form.invitation_code") }), _jsx(SegmentedControl.Item, { value: "neworg", children: t("register_form.new_organization") })] }), orgMode === "invitation" ? (_jsxs(Flex, { direction: "column", gap: "1", children: [_jsx(Text, { size: "2", children: "Code" }), _jsx(TextField.Root, { placeholder: t("register_form.invitation_code_placeholder"), value: invitationCode, onChange: (e) => setInvitationCode(e.target.value) }), errors.invitationCode && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.invitationCode }))] })) : (_jsxs(Flex, { direction: "column", gap: "1", children: [_jsx(Text, { size: "2", children: t("register_form.org_name") }), _jsx(TextField.Root, { placeholder: t("register_form.org_name_placeholder"), value: organizationName, onChange: (e) => setOrganizationName(e.target.value) }), errors.organizationName && (_jsx(Text, { style: { color: "red", fontSize: "0.8rem" }, children: errors.organizationName }))] })), _jsxs(Flex, { direction: "row", gap: "1", align: "center", style: { marginTop: "15px" }, children: [_jsx(Checkbox, { size: "1" }), _jsx(Text, { size: "2", children: "Signup for newsletter" })] }), _jsxs(Flex, { direction: "row", gap: "1", align: "center", style: { marginBottom: "15px" }, children: [_jsx(Checkbox, { size: "1" }), _jsx(Text, { size: "2", children: "Agree Terms and Conditions" })] })] }), _jsxs(Flex, { style: { marginTop: "25px", justifyContent: "flex-end" }, children: [_jsx(Button, { onClick: () => navigate("/login"), variant: "surface", style: { marginRight: "10px" }, children: t("register_form.signin") }), _jsx(Button, { onClick: handleRegister, variant: "solid", children: t("register_form.create_account") })] })] }), _jsx(LightRays, { raysOrigin: "top-center", raysColor: "#00ffff", raysSpeed: 1.5, lightSpread: 0.8, rayLength: 1.2, followMouse: true, mouseInfluence: 0.1, noiseAmount: 0.1, distortion: 0.05, className: "custom-rays" })] }));
}
export default RegisterPage;
