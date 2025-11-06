import { Avatar, DropdownMenu, IconButton } from "@radix-ui/themes";
import React from "react";
import { useNavigate } from "react-router-dom";


function ProfileButton() {
    const navigate = useNavigate();

    return (
        <DropdownMenu.Root>
            <DropdownMenu.Trigger>
                <IconButton style={{ backgroundColor: "transparent", border: "none", cursor: "pointer" }}>
                    <Avatar fallback="A" />
                </IconButton>
            </DropdownMenu.Trigger>
            <DropdownMenu.Content color="gray" highContrast>
                <DropdownMenu.Item style={{ cursor: "pointer" }}>
                    Profile
                </DropdownMenu.Item>
                <DropdownMenu.Item style={{ cursor: "pointer" }}>
                    Settings
                </DropdownMenu.Item>
                <DropdownMenu.Separator />
                <DropdownMenu.Item style={{ cursor: "pointer" }}>
                    Logout
                </DropdownMenu.Item>
            </DropdownMenu.Content>
        </DropdownMenu.Root>
    );
}

export default ProfileButton;