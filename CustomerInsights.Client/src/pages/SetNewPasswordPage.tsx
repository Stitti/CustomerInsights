import { Button, Card, Flex, Heading, Text } from "@radix-ui/themes";
import React, { useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import {useTranslation} from "react-i18next";
import PasswordField from "../components/PasswordField";
import LightRays from "../components/LightRays";

function SetNewPasswordPage() {
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [success, setSuccess] = useState<string | null>(null);
    const [searchParams] = useSearchParams();
    const {t} = useTranslation();
    const navigate = useNavigate();
    const oobCode = searchParams.get("oobCode") || "";

    async function handleResetPassword() {
        const newErrors: { [key: string]: string } = {};
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
        } else {
            setErrors({});
        }

        try {
            //await confirmPasswordResetAsync(oobCode, password);
            setSuccess(t("set_password_page.success"));
            setTimeout(() => {
                navigate("/login");
            }, 2000);
        } catch (err: any) {
            console.error("Error resetting password:", err);
            setErrors({ general: t("set_password_page.reset_failed") });
        }
    }

    return (
        <div
            style={{
                width: "100%",
                height: "100vh",
                overflow: "hidden",       // Kein Scrollen
                display: "flex",
                justifyContent: "center", // Horizontal zentrieren
                alignItems: "center",     // Vertikal zentrieren
                margin: "-15px"
            }}
        >
            <Card style={{ padding: '40px', width: '35vw', minWidth: '500px' }}>
                <Heading style={{ marginBottom: "25px" }}>{t("set_password_page.set_password")}</Heading>

                <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                    <Text size="2">{t("set_password_page.password")}</Text>
                    <PasswordField
                        value={password}
                        onChange={evt => setPassword(evt.target.value)}
                        placeholder={t("set_password_page.password_placeholder")}
                        showLabel={t("login_form.show_password")}
                        hideLabel={t("login_form.hide_password")}
                    />
                    {errors.password && (
                        <Text style={{ color: "red", fontSize: "0.8rem" }}>
                            {errors.password}
                        </Text>
                    )}
                </Flex>

                <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                    <Text size="2">{t("set_password_page.confirm_password")}</Text>
                    <PasswordField
                        value={confirmPassword}
                        onChange={evt => setConfirmPassword(evt.target.value)}
                        placeholder={t("set_password_page.confirm_password_placeholder")}
                        showLabel={t("login_form.show_password")}
                        hideLabel={t("login_form.hide_password")}
                    />
                    {errors.confirmPassword && (
                        <Text style={{ color: "red", fontSize: "0.8rem" }}>
                            {errors.confirmPassword}
                        </Text>
                    )}
                </Flex>

                {errors.general && (
                    <Text style={{ color: "red", fontSize: "0.8rem", marginTop: "10px" }}>
                        {errors.general}
                    </Text>
                )}
                {success && (
                    <Text style={{ color: "green", fontSize: "0.8rem", marginTop: "10px" }}>
                        {success}
                    </Text>
                )}

                <Flex style={{ marginTop: '25px', justifyContent: 'flex-end' }}>
                    <Button variant="solid" onClick={handleResetPassword}>
                        {t("set_password_page.set_password")}
                    </Button>
                </Flex>
            </Card>
            <LightRays
                raysOrigin="top-center"
                raysColor="#00ffff"
                raysSpeed={1.5}
                lightSpread={0.8}
                rayLength={1.2}
                followMouse={true}
                mouseInfluence={0.1}
                noiseAmount={0.1}
                distortion={0.05}
                className="custom-rays"
            />
        </div>
    );
}

export default SetNewPasswordPage;
