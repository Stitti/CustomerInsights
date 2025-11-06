import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import { Theme } from "@radix-ui/themes";
import { getThemeCookie, setThemeCookie, AppearanceMode } from "./themeCookie";

type Ctx = {
    appearance: AppearanceMode;
    setAppearance: (mode: AppearanceMode) => void;
};

const ThemeContext = createContext<Ctx>({ appearance: "system", setAppearance: () => {} });

export const ThemeProvider = ({ children }: { children: React.ReactNode }) => {
    const [appearance, setAppearance] = useState<AppearanceMode>(() => {
        return getThemeCookie() ?? (localStorage.getItem("appearance") as AppearanceMode) ?? "system";
    });

    const mql = useMemo(() => window.matchMedia("(prefers-color-scheme: dark)"), []);
    const [systemDark, setSystemDark] = useState<boolean>(mql.matches);
    useEffect(() => {
        const onChange = (e: MediaQueryListEvent) => setSystemDark(e.matches);
        mql.addEventListener("change", onChange);
        return () => mql.removeEventListener("change", onChange);
    }, [mql]);

    const resolvedAppearance: "light" | "dark" =
        appearance === "system" ? (systemDark ? "dark" : "light") : appearance;

    useEffect(() => {
        setThemeCookie(appearance);
        localStorage.setItem("appearance", appearance);
    }, [appearance]);

    return (
        <ThemeContext.Provider value={{ appearance, setAppearance }}>
            <Theme
                appearance={resolvedAppearance}
                accentColor="blue"
                panelBackground="translucent"
                className="app-theme"
            >
                {children}
            </Theme>
        </ThemeContext.Provider>
    );
};

export const useThemeMode = () => useContext(ThemeContext);