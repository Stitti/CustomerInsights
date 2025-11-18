"use client";

import React, { createContext, useContext, useState, useCallback } from "react";
import {Text} from "@radix-ui/themes";

type LoaderContextValue = {
    showLoader: (label?: string) => void;
    hideLoader: () => void;
    withLoader: <T>(label: string, fn: () => Promise<T>) => Promise<T>;
};

const LoaderContext = createContext<LoaderContextValue | undefined>(undefined);

export const useActionLoader = () => {
    const ctx = useContext(LoaderContext);
    if (!ctx) throw new Error("useActionLoader must be used inside <ActionLoaderProvider>");
    return ctx;
};

export const ActionLoaderProvider = ({ children }: { children: React.ReactNode }) => {
    const [open, setOpen] = useState(false);
    const [label, setLabel] = useState<string | undefined>(undefined);

    const showLoader = useCallback((lbl?: string) => {
        setLabel(lbl);
        setOpen(true);
    }, []);

    const hideLoader = useCallback(() => {
        setOpen(false);
        setLabel(undefined);
    }, []);

    const withLoader = useCallback(async <T,>(lbl: string, fn: () => Promise<T>) => {
        showLoader(lbl);
        try {
            return await fn();
        } finally {
            hideLoader();
        }
    }, [showLoader, hideLoader]);

    return (
        <LoaderContext.Provider value={{ showLoader, hideLoader, withLoader }}>
            {children}

            {/* Loader UI */}
            {open && (
                <div className="action-loader-backdrop">
                    <div className="action-loader-card">
                        <div className="action-loader-spinner">
                            <span className="action-loader-dot" />
                        </div>

                        {label && <Text className="action-loader-label">{label}</Text>}
                    </div>
                </div>
            )}
        </LoaderContext.Provider>
    );
};