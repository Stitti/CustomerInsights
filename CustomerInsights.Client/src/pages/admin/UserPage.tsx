import React from 'react';
import {Box, Flex, Separator} from "@radix-ui/themes";
import {useTranslation} from "react-i18next";
import {UserList} from "../../components/lists/UserList.tsx";
import {InvitationList} from "../../components/lists/InvitationList.tsx";

export function UserPage(): any {
    const { t } = useTranslation();

    return (
        <>
            <Box flexGrow="1">
                <Box mx="auto" my="6" p="4">
                    <Flex gap="4" direction="column">
                        <UserList/>
                        <InvitationList/>
                    </Flex>
                </Box>
            </Box>
        </>

    );
}