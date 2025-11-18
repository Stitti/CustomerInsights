"use client";

import * as React from "react";
import * as ToastPrimitive from "@radix-ui/react-toast";
import { Cross2Icon } from "@radix-ui/react-icons";

type ToastOptions = {
    title?: React.ReactNode;
    description?: React.ReactNode;
};

type ToastContextValue = {
    showToast: (options: ToastOptions) => void;
};

const ToastContext = React.createContext<ToastContextValue | undefined>(
    undefined
);

export const useToast = () => {
    const ctx = React.useContext(ToastContext);
    if (!ctx) {
        throw new Error("useToast must be used within AppToastProvider");
    }
    return ctx;
};

export const AppToastProvider: React.FC<{ children: React.ReactNode }> = ({
                                                                              children,
                                                                          }) => {
    const [open, setOpen] = React.useState(false);
    const [toastOptions, setToastOptions] = React.useState<ToastOptions>({});

    const showToast = React.useCallback((options: ToastOptions) => {
        setToastOptions(options);
        setOpen(false);
        requestAnimationFrame(() => setOpen(true));
    }, []);

    return (
        <ToastContext.Provider value={{ showToast }}>
            <ToastPrimitive.Provider duration={2500} swipeDirection="right">
                {children}

                <ToastPrimitive.Root
                    className="AppToastRoot"
                    open={open}
                    onOpenChange={setOpen}
                    type="foreground"
                >
                    <div className="AppToastBody">
                        {toastOptions.title && (
                            <ToastPrimitive.Title className="AppToastTitle">
                                {toastOptions.title}
                            </ToastPrimitive.Title>
                        )}
                        {toastOptions.description && (
                            <ToastPrimitive.Description className="AppToastDescription">
                                {toastOptions.description}
                            </ToastPrimitive.Description>
                        )}
                    </div>

                    <div className="AppToastActions">
                        <ToastPrimitive.Close
                            className="AppToastClose"
                            aria-label="Toast schließen"
                        >
                            <Cross2Icon />
                        </ToastPrimitive.Close>
                    </div>
                </ToastPrimitive.Root>

                <ToastPrimitive.Viewport className="AppToastViewport" />
            </ToastPrimitive.Provider>
        </ToastContext.Provider>
    );
};