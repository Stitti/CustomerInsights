import React, { useState } from "react";
import {
    Button,
    Card,
    Flex,
    Heading,
    Link,
    Text,
    TextField,
} from "@radix-ui/themes";
import {useNavigate, useSearchParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import PasswordField from "../../components/PasswordField";
import LightRays from "../../components/LightRays";

function LoginPage() {
    const navigate = useNavigate();
    const [params] = useSearchParams();
    const returnUrl = decodeURIComponent(params.get("returnUrl") || "");
    const {t} = useTranslation();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [isLoading, setIsLoading] = useState(false);

    async function handleLogin() {
        const newErrors: { [key: string]: string } = {};

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
            } else {
                navigate("/");
            }
        } catch (error: any) {
            console.error("Login error:", error);
            setErrors({ general: error?.response?.data?.error || error.message });
        } finally {
            setIsLoading(false);
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
    {/* Ladescreen-Overlay */
    }
    {
        isLoading && (
            <Flex
                style={{
                    position: "fixed",
                    top: 0,
                    left: 0,
                    width: "100vw",
                    height: "100vh",
                    backgroundColor: "rgba(0, 0, 0, 0.5)",
                    zIndex: 9999,
                        justifyContent: "center",
                        alignItems: "center",
                    }}
                >
                    <Card style={{ padding: "20px" }}>
                        <Heading size="4">{t("login_form.signin_loading")}</Heading>
                    </Card>
                </Flex>
            )}
            <Card style={{ padding: "40px", width: "35vw", minWidth: "500px", zIndex: 4 }}>
                <Heading style={{ marginBottom: "25px" }}>{t("login_form.singin")}</Heading>

                <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                    <Text size="2">{t("login_form.email_address")}</Text>
                    <TextField.Root
                        value={email}
                        onChange={(evt) => setEmail(evt.target.value)}
                        type="email"
                        placeholder={t("login_form.email_address_placeholder")}
                    />
                    {errors.email && (
                        <Text style={{ color: "red", fontSize: "0.8rem" }}>
                            {errors.email}
                        </Text>
                    )}
                </Flex>

                <Flex direction="column" gap="1" style={{ marginBottom: "10px" }}>
                    <Flex style={{ justifyContent: "space-between", alignItems: "center" }}>
                        <Text size="2">{t("login_form.password")}</Text>
                        <Link href="/reset-password" size="2">
                            {t("login_form.forgot_password")}
                        </Link>
                    </Flex>
                    <PasswordField
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder={t("login_form.password_placeholder")}
                        showLabel={t("login_form.show_password")}
                        hideLabel={t("login_form.hide_password")}
                    />
                    {errors.password && (
                        <Text style={{ color: "red", fontSize: "0.8rem" }}>
                            {errors.password}
                        </Text>
                    )}
                </Flex>

                {errors.general && (
                    <Text
                        style={{ color: "red", fontSize: "0.8rem", marginBottom: "10px" }}
                    >
                        {errors.general}
                    </Text>
                )}

                <Flex style={{ marginTop: "25px", justifyContent: "flex-end" }}>
                    <Button
                        onClick={() => navigate("/register")}
                        variant="surface"
                        style={{ marginRight: "10px" }}
                    >
                        {t("login_form.create_account")}
                    </Button>
                    <Button onClick={handleLogin} variant="solid">
                        {t("login_form.signin")}
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

export default LoginPage;
