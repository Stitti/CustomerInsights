export const funnelMock = [
    { name: "Landing", count: 12000 },
    { name: "Signup", count: 2200 },
    { name: "Onboarding", count: 1500 },
    { name: "Purchase", count: 620 },
];
export const painPointsMock = [
    { aspect: "Lieferzeit", frequencyPct: 28, severity: 76, volume: 124, trend: "up" },
    { aspect: "Qualität", frequencyPct: 22, severity: 71, volume: 98, trend: "flat" },
    { aspect: "Preis", frequencyPct: 14, severity: 58, volume: 65, trend: "flat" },
    { aspect: "Rechnung", frequencyPct: 9, severity: 43, volume: 32, trend: "down" },
    { aspect: "Usability", frequencyPct: 18, severity: 52, volume: 54, trend: "down" },
];
export function generateHeatMock() {
    const out = [];
    for (let d = 0; d < 7; d++) {
        for (let h = 0; h < 24; h++) {
            const base = (d < 5 && h >= 9 && h <= 17) ? 6 : 2;
            const noise = Math.floor(Math.random() * 4);
            out.push({ day: d, hour: h, count: Math.max(0, base + noise) });
        }
    }
    return out;
}
export const conversionMock = Array.from({ length: 30 }, (_, i) => {
    const d = new Date();
    d.setDate(d.getDate() - (29 - i));
    const rate = 2.5 + Math.sin(i / 5) * 0.6 + (Math.random() - 0.5) * 0.3;
    return { date: d.toISOString(), ratePct: Math.max(0, Math.min(100, Math.round(rate * 10) / 10)) };
});
