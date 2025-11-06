import React from "react";
import { Box, Flex, IconButton } from "@radix-ui/themes";
import { useNavigate } from "react-router-dom";
import ProfileButton from "./ProfileButton";
import { ChevronLeftIcon, MoonIcon, SunIcon } from "lucide-react";

function Header() {
    const navigate = useNavigate();

    return (
        <Box
            style={{
                padding: "10px"
            }}
        >
            <Flex
                style={{
                    alignItems: "center",
                    justifyContent: "space-between"
                }}
            >
                <IconButton
                    variant="surface"
                    onClick={() => navigate(-1)}
                    style={{backgroundColor: "transparent", border: "none", cursor: "pointer"}}
                >
                    <ChevronLeftIcon />
                </IconButton>

                <Flex style={{ gap: "10px" }}>
                    <ProfileButton />
                </Flex>
            </Flex>
        </Box>
    );
}

export default Header;
