import React, { useEffect, useState } from "react";
import {
    Button,
    Card,
    Flex,
    Heading,
    Text,
    TextField,
    SegmentedControl, Checkbox
} from "@radix-ui/themes";
import { useNavigate, useSearchParams } from "react-router-dom";
import {useTranslation} from "react-i18next";
import PasswordField from "../../components/PasswordField";
import LightRays from "../../components/LightRays";

function RegisterPage() {
    const navigate = useNavigate();
    const {t} = useTranslation();
    const [searchParams] = useSearchParams();
    const [firstname, setFirstname] = useState("");
    const [lastname, setLastname] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [orgMode, setOrgMode] = useState<"invitation" | "neworg">("invitation");
    const [invitationCode, setInvitationCode] = useState("");
    const [organizationName, setOrganizationName] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        const code = searchParams.get("invitationCode");
        if (code) {
            setInvitationCode(code);
            setOrgMode("invitation");
        }
    }, [searchParams]);

    async function handleRegister() {
        const newErrors: { [key: string]: string } = {};

        if (!firstname.trim()) newErrors.firstname = t("register_form.errors.firstname_required");
        if (!lastname.trim()) newErrors.lastname = t("register_form.errors.lastname_required");
        if (!email.trim()) newErrors.email = t("register_form.errors.email_required");
        if (!password) newErrors.password = t("register_form.errors.password_required");
        if (password && password.length < 6) newErrors.password = t("register_form.errors.password_length");
        if (!confirmPassword) newErrors.confirmPassword = t("register_form.errors.password_confirm");
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
        } else {
            setErrors({});
        }

        const registerRequest/*: RegisterRequest*/ = {
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
        } catch (error) {
            console.error("Registration failed:", error);
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <div
            style={{
                width: "100vw",
                height: "100vh",
                overflow: "hidden",       // Kein Scrollen
                display: "flex",
                justifyContent: "center", // Horizontal zentrieren
                alignItems: "center",     // Vertikal zentrieren
                margin: "-15px"
            }}
        >
            {/* Ladescreen-Overlay */}
            {isLoading && (
                <Flex
                    style={{
                        position: "fixed",
                        top: 0,
                        left: 0,
                        width: "100vw",
                        height: "100vh",
                        backgroundColor: "rgba(0,0,0,0.5)",
                        zIndex: 9999,
                        justifyContent: "center",
                        alignItems: "center",
                    }}
                >
                    <Card style={{padding: "20px"}}>
                        <Text size="3">{t("register_form.register_loading")}</Text>
                    </Card>
                </Flex>
            )}

            <Card style={{padding: "40px", width: "35vw", minWidth: "500px", zIndex: 4}}>
                <Heading style={{marginBottom: "25px"}}>{t("register_form.create_account")}</Heading>

                {/* Vorname */}
                <Flex direction="column" gap="1" style={{marginBottom: "15px"}}>
                    <Text size="2">{t("register_form.firstname")}</Text>
                    <TextField.Root
                        placeholder={t("register_form.firstname_placeholder")}
                        value={firstname}
                        onChange={(e) => setFirstname(e.target.value)}
                    />
                    {errors.firstname && (
                        <Text style={{color: "red", fontSize: "0.8rem"}}>
                            {errors.firstname}
                        </Text>
                    )}
                </Flex>

                {/* Nachname */}
                <Flex direction="column" gap="1" style={{marginBottom: "15px"}}>
                    <Text size="2">{t("register_form.lastname")}</Text>
                    <TextField.Root
                        placeholder={t("register_form.lastname_placeholder")}
                        value={lastname}
                        onChange={(e) => setLastname(e.target.value)}
                    />
                    {errors.lastname && (
                        <Text style={{color: "red", fontSize: "0.8rem"}}>
                            {errors.lastname}
                        </Text>
                    )}
                </Flex>

                {/* Email */}
                <Flex direction="column" gap="1" style={{marginBottom: "15px"}}>
                    <Text size="2">{t("register_form.email")}</Text>
                    <TextField.Root
                        placeholder={t("register_form.email_placeholder")}
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    {errors.email && (
                        <Text style={{color: "red", fontSize: "0.8rem"}}>
                            {errors.email}
                        </Text>
                    )}
                </Flex>

                {/* Passwort */}
                <Flex direction="column" gap="1" style={{marginBottom: "15px"}}>
                    <Text size="2">{t("register_form.password")}</Text>
                    <PasswordField
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder={t("register_form.password_placeholder")}
                        showLabel={t("login_form.show_password")}
                        hideLabel={t("login_form.hide_password")}
                    />
                    {errors.password && (
                        <Text style={{color: "red", fontSize: "0.8rem"}}>
                            {errors.password}
                        </Text>
                    )}
                </Flex>

                {/* Passwort bestätigen */}
                <Flex direction="column" gap="1" style={{marginBottom: "15px"}}>
                    <Text size="2">{t("register_form.confirm_password")}</Text>
                    <PasswordField
                        value={confirmPassword}
                        onChange={(e: { target: { value: React.SetStateAction<string>; }; }) => setConfirmPassword(e.target.value)}
                        placeholder={t("register_form.password_placeholder")}
                        showLabel={t("login_form.show_password")}
                        hideLabel={t("login_form.hide_password")}
                    />
                    {errors.confirmPassword && (
                        <Text style={{color: "red", fontSize: "0.8rem"}}>
                            {errors.confirmPassword}
                        </Text>
                    )}
                </Flex>

                {/* SegmentedControl für Einladungscode oder neue Orga */}
                <Flex direction="column" gap="2" style={{marginBottom: "20px"}}>
                    <SegmentedControl.Root
                        size="1"
                        value={orgMode}
                        onValueChange={(value) => setOrgMode(value as "invitation" | "neworg")}
                    >
                        <SegmentedControl.Item value="invitation">
                            {t("register_form.invitation_code")}
                        </SegmentedControl.Item>
                        <SegmentedControl.Item value="neworg">
                            {t("register_form.new_organization")}
                        </SegmentedControl.Item>
                    </SegmentedControl.Root>

                    {orgMode === "invitation" ? (
                        <Flex direction="column" gap="1">
                            <Text size="2">Code</Text>
                            <TextField.Root
                                placeholder={t("register_form.invitation_code_placeholder")}
                                value={invitationCode}
                                onChange={(e) => setInvitationCode(e.target.value)}
                            />
                            {errors.invitationCode && (
                                <Text style={{color: "red", fontSize: "0.8rem"}}>
                                    {errors.invitationCode}
                                </Text>
                            )}
                        </Flex>
                    ) : (
                        <Flex direction="column" gap="1">
                            <Text size="2">{t("register_form.org_name")}</Text>
                            <TextField.Root
                                placeholder={t("register_form.org_name_placeholder")}
                                value={organizationName}
                                onChange={(e) => setOrganizationName(e.target.value)}
                            />
                            {errors.organizationName && (
                                <Text style={{color: "red", fontSize: "0.8rem"}}>
                                    {errors.organizationName}
                                </Text>
                            )}
                        </Flex>
                    )}

                    <Flex direction="row" gap="1" align="center" style={{ marginTop: "15px" }}>
                        <Checkbox size="1" />
                        <Text size="2">Signup for newsletter</Text>
                    </Flex>

                    <Flex direction="row" gap="1" align="center" style={{ marginBottom: "15px" }}>
                        <Checkbox size="1" />
                        <Text size="2">Agree Terms and Conditions</Text>
                    </Flex>

                </Flex>

                {/* Buttons */}
                <Flex style={{marginTop: "25px", justifyContent: "flex-end"}}>
                    <Button
                        onClick={() => navigate("/login")}
                        variant="surface"
                        style={{marginRight: "10px"}}
                    >
                        {t("register_form.signin")}
                    </Button>
                    <Button onClick={handleRegister} variant="solid">
                        {t("register_form.create_account")}
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

export default RegisterPage;
