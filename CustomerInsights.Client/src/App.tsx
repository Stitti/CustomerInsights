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

function App() {
  return (
      <BrowserRouter>
        <Routes>
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
        </Routes>
      </BrowserRouter>
  );
}

export default App
