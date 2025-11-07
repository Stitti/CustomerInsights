import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Box, Flex } from "@radix-ui/themes";
import PersonalInformation from "../components/PersonalInformation";
import PasswordReset from "../components/PasswordReset";
import ThemePicker from "../components/ThemePicker";
import Header from "../components/Header";
//import {useCurrentUser} from "../services/useUsers";
export function ProfilePage() {
    return (_jsxs(_Fragment, { children: [_jsx(Header, {}), _jsx(Flex, { style: { minHeight: '100vh', background: 'var(--gray-50)' }, children: _jsx(Box, { flexGrow: "1", children: _jsxs(Box, { mx: "auto", my: "6", p: "4", children: [_jsx(PersonalInformation, {}), _jsx(ThemePicker, {}), _jsx(PasswordReset, {})] }) }) })] }));
}
