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
            onMouseDown={(e) => e.preventDefault()}
            onClick={() => setVisible((v) => !v)}
            style={{
              position: "relative",
              width: 20,
              height: 20,
              display: "inline-flex",
              alignItems: "center",
              justifyContent: "center",
              overflow: "hidden",
            }}
          >
            {/* Eye (sichtbar, wenn Passwort gezeigt wird) */}
            <Eye
              size={16}
              style={{
                position: "absolute",
                transition:
                  "opacity 180ms ease, transform 180ms ease, rotate 180ms ease",
                opacity: visible ? 1 : 0,
                transform: visible ? "scale(1) rotate(0deg)" : "scale(0.85) rotate(-15deg)",
              }}
            />

            {/* EyeOff (sichtbar, wenn Passwort verborgen wird) */}
            <EyeOff
              size={16}
              style={{
                position: "absolute",
                transition:
                  "opacity 180ms ease, transform 180ms ease, rotate 180ms ease",
                opacity: visible ? 0 : 1,
                transform: visible ? "scale(0.85) rotate(15deg)" : "scale(1) rotate(0deg)",
              }}
            />
          </IconButton>
        </TextField.Slot>
      </TextField.Root>
    );
  }
);

PasswordField.displayName = "PasswordField";
export default PasswordField;