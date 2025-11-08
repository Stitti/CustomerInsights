import React from 'react'
import {Dashboard} from "./pages/Dashboard";
import AppLayout from "./components/AppLayout";
import {BrowserRouter, Route, Routes} from 'react-router-dom';
import ContactsPage from "./pages/ContactsPage";
import AccountsPage from "./pages/AccountsPage";
import InteractionsPage from "./pages/InteractionsPage";
import {InteractionDetailPage} from "./pages/InteractionDetailPage";
import {AccountDetailPage} from "./pages/AccountDetailPage";
import {ContactDetailPage} from "./pages/ContactDetailPage";
import AnalyticsDashboard from "./components/AnalyticsDashboard";
import LoginPage from "./pages/auth/LoginPage";
import RegisterPage from "./pages/auth/RegisterPage";
import ResetPasswordPage from "./pages/auth/ResetPasswordPage";
import SetNewPasswordPage from "./pages/auth/SetNewPasswordPage";
import ResetPasswordConfirmationPage from "./pages/auth/ResetPasswordConfirmationPage";
import {ProfilePage} from "./pages/ProfilePage";
import {UserPage} from "./pages/admin/UserPage";
import AdminAppLayout from "./components/AdminAppLayout";
import ApiKeyManagementPage from "./pages/admin/ApiManagementPage";
import ApiKeyDetail from "./pages/admin/ApiKeyDetail";
import SignalsPage from "./pages/SignalsPage";
import SignalDetailPage from "./pages/SignalDetailPage";
import CategoryAnalysisPage from "./pages/CategoryAnalysisPage";
import JourneyAnalysisPage from "./pages/JourneyAnalysisPage";
import JourneyStepDetailPage from "./pages/JourneyStepDetailPage";
import IntegrationsPage from "@/src/pages/admin/IntegrationsPage";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<LoginPage/>}/>
                <Route path="/reset-password" element={<ResetPasswordPage/>}/>
                <Route path="/set-password" element={<SetNewPasswordPage/>}/>
                <Route path="/register" element={<RegisterPage/>}/>
                <Route path="/reset-confirmation" element={<ResetPasswordConfirmationPage/>}/>
                <Route path="/profile" element={<ProfilePage />} />

                <Route
                  path="/"
                  element={
                      <AppLayout/>
                  }
                >
                    <Route index path="/" element={<Dashboard />} />
                    <Route path="/signals" element={<SignalsPage />} />
                    <Route path="/signals/:signalId" element={<SignalDetailPage />} />
                    <Route path="/journey" element={<JourneyAnalysisPage />} />
                    <Route path="/journey/:stepId" element={<JourneyStepDetailPage />} />
                    <Route path="/website" element={<AnalyticsDashboard />} />
                    <Route path="/contacts" element={<ContactsPage />} />
                    <Route path="/contacts/:contactId" element={<ContactDetailPage />} />
                    <Route path="/accounts" element={<AccountsPage />} />
                    <Route path="/accounts/:accountId" element={<AccountDetailPage />} />
                    <Route path="/interactions" element={<InteractionsPage />} />
                    <Route path="/interactions/:interactionId" element={<InteractionDetailPage />} />
                    <Route path="/categories" element={<CategoryAnalysisPage/>}/>
                </Route>
                <Route path="/" element={
                    <AdminAppLayout/>
                }>
                    <Route path="admin/users" element={<UserPage/>}/>
                    <Route path="admin/integrations" element={<IntegrationsPage/>}/>
                    <Route path="admin/apikeys" element={<ApiKeyManagementPage/>}/>
                    <Route path="admin/apikeys/:apiKeyId" element={<ApiKeyDetail/>}/>
                </Route>
            </Routes>
        </BrowserRouter>
  );
}

export default App
