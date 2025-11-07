import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Button, Card, Flex, Text } from "@radix-ui/themes";
import PasswordField from "./PasswordField";
export default function PasswordReset() {
    return (_jsx(Card, { variant: "surface", size: "3", mb: "6", children: _jsxs(Flex, { direction: "column", gap: "4", p: "4", children: [_jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: "Current Password" }), _jsx(PasswordField
                        //value={password}
                        //onChange={evt => setPassword(evt.target.value)}
                        //placeholder={t("set_password_page.password_placeholder")}
                        //showLabel={t("login_form.show_password")}
                        //hideLabel={t("login_form.hide_password")}
                        , {})] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: "New Password" }), _jsx(PasswordField
                        //value={password}
                        //onChange={evt => setPassword(evt.target.value)}
                        //placeholder={t("set_password_page.password_placeholder")}
                        //showLabel={t("login_form.show_password")}
                        //hideLabel={t("login_form.hide_password")}
                        , {})] }), _jsxs(Flex, { direction: "column", gap: "1", style: { marginBottom: "15px" }, children: [_jsx(Text, { size: "2", children: "Confirm new Password" }), _jsx(PasswordField
                        //value={password}
                        //onChange={evt => setPassword(evt.target.value)}
                        //placeholder={t("set_password_page.password_placeholder")}
                        //showLabel={t("login_form.show_password")}
                        //hideLabel={t("login_form.hide_password")}
                        , {})] }), _jsx(Flex, { style: { "justifyContent": "end" }, children: _jsx(Button, { children: "Save" }) })] }) }));
}
