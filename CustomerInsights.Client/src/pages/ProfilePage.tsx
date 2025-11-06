import React from 'react';
import {Box, Flex} from "@radix-ui/themes";
import {useTranslation} from "react-i18next";
import PersonalInformation from "../components/PersonalInformation";
import PasswordReset from "../components/PasswordReset";
import ThemePicker from "../components/ThemePicker";
import Header from "../components/Header";
//import {useCurrentUser} from "../services/useUsers";

export function ProfilePage(): any {

    return (
        <>
            <Header/>
            <Flex style={{ minHeight: '100vh', background: 'var(--gray-50)' }}>
                <Box flexGrow="1">
                    <Box mx="auto" my="6" p="4">
                        <PersonalInformation/>
                        <ThemePicker/>
                        <PasswordReset />
                    </Box>
                </Box>
            </Flex>
        </>

    );
}