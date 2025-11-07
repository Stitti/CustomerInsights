import { jsx as _jsx } from "react/jsx-runtime";
import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { Theme } from "@radix-ui/themes";
import { getThemeCookie, setThemeCookie } from "./themeCookie";
const ThemeContext = createContext({ appearance: "system", setAppearance: () => { } });
export const ThemeProvider = ({ children }) => {
    const [appearance, setAppearance] = useState(() => {
        return getThemeCookie() ?? localStorage.getItem("appearance") ?? "system";
    });
    const mql = useMemo(() => window.matchMedia("(prefers-color-scheme: dark)"), []);
    const [systemDark, setSystemDark] = useState(mql.matches);
    useEffect(() => {
        const onChange = (e) => setSystemDark(e.matches);
        mql.addEventListener("change", onChange);
        return () => mql.removeEventListener("change", onChange);
    }, [mql]);
    const resolvedAppearance = appearance === "system" ? (systemDark ? "dark" : "light") : appearance;
    useEffect(() => {
        setThemeCookie(appearance);
        localStorage.setItem("appearance", appearance);
    }, [appearance]);
    return (_jsx(ThemeContext.Provider, { value: { appearance, setAppearance }, children: _jsx(Theme, { appearance: resolvedAppearance, accentColor: "blue", panelBackground: "translucent", className: "app-theme", children: children }) }));
};
export const useThemeMode = () => useContext(ThemeContext);
