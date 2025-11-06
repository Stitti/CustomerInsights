import React from "react";
import { TextField, IconButton } from "@radix-ui/themes";
import { Eye, EyeOff } from "lucide-react";

type PasswordFieldProps = Omit<
    React.ComponentPropsWithoutRef<typeof TextField.Root>,
    "type" | "children"
> & {
    showLabel?: string;
    hideLabel?: string;
};

const PasswordField = React.forwardRef<HTMLInputElement, PasswordFieldProps>(
    (
        {
            showLabel = "Passwort anzeigen",
            hideLabel = "Passwort verbergen",
            autoComplete = "current-password",
            ...props
        },
        _ref
    ) => {
        const [visible, setVisible] = React.useState(false);

        return (
            <TextField.Root
                {...props}
                type={visible ? "text" : "password"}
                autoComplete={autoComplete}
            >
                <TextField.Slot side="right">
                    <IconButton
                        size="1"
                        variant="ghost"
                        type="button"
                        aria-label={visible ? hideLabel : showLabel}
                        title={visible ? hideLabel : showLabel}
                        aria-pressed={visible}
                        onMouseDown={(e) => e.preventDefault()} // verhindert Fokusverlust
                        onClick={() => setVisible((v) => !v)}
                    >
                        {visible ? <EyeOff size={16} /> : <Eye size={16} />}
                    </IconButton>
                </TextField.Slot>
            </TextField.Root>
        );
    }
);

PasswordField.displayName = "PasswordField";
export default PasswordField;
