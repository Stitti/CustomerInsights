export const formatDate = new Intl.DateTimeFormat(undefined, { year: 'numeric', month: 'short', day: '2-digit' });
export const formatDateNumeric = new Intl.DateTimeFormat(undefined, { year: 'numeric', month: '2-digit', day: '2-digit' });
export const formatDateCompactIntl = { format: (date) => `${date.getFullYear()}${String(date.getMonth() + 1).padStart(2, "0")}${String(date.getDate()).padStart(2, "0")}`, };
export const formatTime = new Intl.DateTimeFormat(undefined, { hour: '2-digit', minute: '2-digit', hour12: false });
