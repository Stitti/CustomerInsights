import React from 'react';
import ReactDOM from 'react-dom/client';
import "@radix-ui/themes/styles.css";
import App from "./App";
import './i18n';
import {ThemeProvider} from "./theme/ThemeProvider";

function Root() {
    return (
        <ThemeProvider>
            <App/>
        </ThemeProvider>
    );
}

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);

root.render(
    <React.StrictMode>
        <Root />
    </React.StrictMode>
);