import React from 'react';
import ReactDOM from 'react-dom/client';
import "@radix-ui/themes/styles.css";
import App from "./App";
import './i18n';
import './css/global.css'
import {ThemeProvider} from "./theme/ThemeProvider";
import {AppToastProvider} from "./components/AppToast";
import {ActionLoaderProvider} from "./components/ActionLoaderProvider";

function Root() {
    return (
        <ThemeProvider>
            <AppToastProvider>
                <ActionLoaderProvider>
                    <App/>
                </ActionLoaderProvider>
            </AppToastProvider>
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