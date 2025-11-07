import {Button, Flex, Text} from "@radix-ui/themes";
import {ArrowLeft} from "lucide-react";
import React from "react";
import {useNavigate} from "react-router-dom";
import {useTranslation} from "react-i18next";

export default function BackButton() {
    const navigate = useNavigate();
    const { t } = useTranslation();

    return (
        <Flex justify="between" align="center" mb="4">
            <Flex align="center" gap="3">
                <Button
                    variant="ghost"
                    color="gray"
                    onClick={() => navigate(-1)}
                    style={{ cursor: "pointer" }}
                >
                    <ArrowLeft size={16} />
                    <Text ml="2">{t("back") || "Zurück"}</Text>
                </Button>
            </Flex>
        </Flex>
    )
}