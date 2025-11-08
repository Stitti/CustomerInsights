import { Button, Card, Flex, Heading, Text, TextField } from "@radix-ui/themes";
import { useNavigate } from "react-router-dom";
import React, { useState } from "react";
import {useTranslation} from "react-i18next";
import LightRays from "../../components/LightRays";

function ResetPasswordPage() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const {t} = useTranslation();

    async function resetPassword() {
        if (!email.trim()) {
            setErrors({ email: t("reset_page.email_required") });
            return;
        }

        setErrors({});

        try {
            //await resetPasswordAsync(email);
            navigate("/login");
        } catch (error: any) {
            console.error("Error sending password reset email:", error);
            setErrors({ general: error.message || t("reset_page.sending_failed") });
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
                <Heading style={{ marginBottom: "25px" }}>{t("reset_page.reset_password")}</Heading>

                <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                    <Text size="2">{t("reset_page.email_address")}</Text>
                    <TextField.Root
                        value={email}
                        type="email"
                        onChange={evt => setEmail(evt.target.value)}
                        placeholder={t("reset_page.email_address_placeholder")}
                    />
                    {errors.email && (
                        <Text style={{ color: "red", fontSize: "0.8rem" }}>
                            {errors.email}
                        </Text>
                    )}
                    {errors.general && (
                        <Text style={{ color: "red", fontSize: "0.8rem" }}>
                            {errors.general}
                        </Text>
                    )}
                </Flex>

                <Flex style={{ marginTop: '25px', justifyContent: 'flex-end' }}>
                    <Button onClick={resetPassword} variant="solid">
                        {t("reset_page.reset_password")}
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

export default ResetPasswordPage;