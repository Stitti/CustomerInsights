import {Avatar, Button, DropdownMenu, Flex, IconButton, Strong, Text} from "@radix-ui/themes";
import React from "react";
import { useNavigate } from "react-router-dom";
import {CaretDownIcon, CaretSortIcon, CaretUpIcon} from "@radix-ui/react-icons";


function ProfileButton() {
    const navigate = useNavigate();

    function handleLogout() {
        navigate("/login");
    }

    return (
        <DropdownMenu.Root >
            <DropdownMenu.Trigger>
                <Flex direction="row" gap="3" style={{cursor: "pointer"}}>
                    <Flex direction="column" style={{justifyContent: "center"}}>
                        <Avatar fallback="M" style={{width: "35px", height: "35px"}} />
                    </Flex>
                    <Flex direction="column" style={{justifyContent: "center"}}>
                        <Text size="2"><Strong>Max Mustermann</Strong></Text>
                        <Text size="1">max.mustermann@test-gmbh.com</Text>
                    </Flex>
                    <Flex direction="column" style={{justifyContent: "center"}}>
                        <CaretUpIcon width="20" height="20" />
                        <CaretDownIcon width="20" height="20"/>
                    </Flex>
                </Flex>
            </DropdownMenu.Trigger>
            <DropdownMenu.Content color="gray" highContrast style={{width: "269px"}}>
                <DropdownMenu.Item style={{ cursor: "pointer" }} onClick={() => navigate("/profile")}>
                    Profile
                </DropdownMenu.Item>
                <DropdownMenu.Item style={{ cursor: "pointer" }} onClick={() => navigate("/admin/users")}>
                    Settings
                </DropdownMenu.Item>
                <DropdownMenu.Separator />
                <DropdownMenu.Item style={{ cursor: "pointer" }} onClick={() => handleLogout()}>
                    Logout
                </DropdownMenu.Item>
            </DropdownMenu.Content>
        </DropdownMenu.Root>
    );
}

export default ProfileButton;