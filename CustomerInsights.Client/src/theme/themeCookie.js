const THEME_COOKIE = "ui_theme";
export function getThemeCookie() {
    const m = document.cookie.match(new RegExp(`(?:^|; )${THEME_COOKIE}=([^;]*)`));
    if (!m)
        return null;
    const v = decodeURIComponent(m[1]);
    return v === "light" || v === "dark" || v === "system" ? v : null;
}
export function setThemeCookie(value) {
    // host-only, für alle Ports gültig
    document.cookie = `${THEME_COOKIE}=${encodeURIComponent(value)}; Max-Age=${60 * 60 * 24 * 365}; Path=/; SameSite=Lax`;
}
