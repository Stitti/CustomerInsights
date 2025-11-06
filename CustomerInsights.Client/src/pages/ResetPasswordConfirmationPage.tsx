import { Card, Flex, Heading, Text } from "@radix-ui/themes";
import React from "react";
import { CheckCircleIcon } from "lucide-react";
import {useTranslation} from "react-i18next";
import LightRays from "../components/LightRays";

function ResetPasswordConfirmationPage() {
    const {t} = useTranslation();

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
            <Card style={{ padding: "40px", width: "35vw", minWidth: "500px" }}>
                <Heading style={{ marginBottom: "25px", textAlign: "center" }}>
                    {t("password_reset_confirmation_page.reset_requested_heading")}
                </Heading>
                <Flex direction="column" align="center" gap="20">
                    <CheckCircleIcon style={{ height: "60px", width: "60px", marginBottom: "20px" }} />
                    <Text style={{ textAlign: "center", marginBottom: "10px" }}>
                        {t("password_reset_confirmation_page.reset_requested_text")}
                    </Text>
                    <br/>
                    <Text style={{ textAlign: "center" }}>
                        {t("password_reset_confirmation_page.reset_requested_sub")}
                    </Text>
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

export default ResetPasswordConfirmationPage;
