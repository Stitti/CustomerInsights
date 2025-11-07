import { jsx as _jsx } from "react/jsx-runtime";
import React from "react";
import { TextField, IconButton } from "@radix-ui/themes";
import { Eye, EyeOff } from "lucide-react";
const PasswordField = React.forwardRef(({ showLabel = "Passwort anzeigen", hideLabel = "Passwort verbergen", autoComplete = "current-password", ...props }, _ref) => {
    const [visible, setVisible] = React.useState(false);
    return (_jsx(TextField.Root, { ...props, type: visible ? "text" : "password", autoComplete: autoComplete, children: _jsx(TextField.Slot, { side: "right", children: _jsx(IconButton, { size: "1", variant: "ghost", type: "button", "aria-label": visible ? hideLabel : showLabel, title: visible ? hideLabel : showLabel, "aria-pressed": visible, onMouseDown: (e) => e.preventDefault(), onClick: () => setVisible((v) => !v), children: visible ? _jsx(EyeOff, { size: 16 }) : _jsx(Eye, { size: 16 }) }) }) }));
});
PasswordField.displayName = "PasswordField";
export default PasswordField;
