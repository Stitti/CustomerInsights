import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Box, Flex } from "@radix-ui/themes";
import { useTranslation } from "react-i18next";
import { UserList } from "../components/UserList";
import { InvitationList } from "../components/InvitationList";
export function UserPage() {
    const { t } = useTranslation();
    return (_jsx(_Fragment, { children: _jsx(Box, { flexGrow: "1", children: _jsx(Box, { mx: "auto", my: "6", p: "4", children: _jsxs(Flex, { gap: "4", direction: "column", children: [_jsx(UserList, {}), _jsx(InvitationList, {})] }) }) }) }));
}
