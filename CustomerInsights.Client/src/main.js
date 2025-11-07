import { jsx as _jsx } from "react/jsx-runtime";
import React from 'react';
import ReactDOM from 'react-dom/client';
import "@radix-ui/themes/styles.css";
import App from "./App";
import './i18n';
import { ThemeProvider } from "./theme/ThemeProvider";
function Root() {
    return (_jsx(ThemeProvider, { children: _jsx(App, {}) }));
}
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(_jsx(React.StrictMode, { children: _jsx(Root, {}) }));
