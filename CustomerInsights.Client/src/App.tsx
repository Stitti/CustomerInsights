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
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import ResetPasswordPage from "./pages/ResetPasswordPage";
import SetNewPasswordPage from "./pages/SetNewPasswordPage";
import ResetPasswordConfirmationPage from "./pages/ResetPasswordConfirmationPage";
import {ProfilePage} from "./pages/ProfilePage";
import {UserPage} from "./pages/UserPage";
import AdminAppLayout from "./components/AdminAppLayout";
import ApiKeyManagementPage from "./pages/ApiManagementPage";
import ApiKeyDetail from "./pages/ApiKeyDetail";

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
                    <Route path="/website" element={<AnalyticsDashboard />} />
                    <Route path="/contacts" element={<ContactsPage />} />
                    <Route path="/contacts/:contactId" element={<ContactDetailPage />} />
                    <Route path="/accounts" element={<AccountsPage />} />
                    <Route path="/accounts/:accountId" element={<AccountDetailPage />} />
                    <Route path="/interactions" element={<InteractionsPage />} />
                    <Route path="/interactions/:interactionId" element={<InteractionDetailPage />} />
                </Route>
                <Route path="/" element={
                    <AdminAppLayout/>
                }>
                    <Route path="admin/users" element={<UserPage/>}/>
                    <Route path="admin/apikeys" element={<ApiKeyManagementPage/>}/>
                    <Route path="admin/apikeys/:apiKeyId" element={<ApiKeyDetail/>}/>

                </Route>
            </Routes>
        </BrowserRouter>
  );
}

export default App
