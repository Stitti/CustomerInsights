import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Flex, Box } from '@radix-ui/themes';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import Sidebar from "./Sidebar";
import { BellIcon, Building2Icon, LayoutDashboard, MessageSquareIcon, RouteIcon, SquareUserRoundIcon, TagIcon } from "lucide-react";
const items = [
    { key: '', label: 'Dashboard', icon: _jsx(LayoutDashboard, {}) },
    { key: 'signals', label: 'Signals', icon: _jsx(BellIcon, {}) },
    { key: 'journey', label: 'Journey', icon: _jsx(RouteIcon, {}) },
    { key: 'accounts', label: 'Accounts', icon: _jsx(Building2Icon, {}) },
    { key: 'contacts', label: 'Contacts', icon: _jsx(SquareUserRoundIcon, {}) },
    { key: 'interactions', label: 'Interactions', icon: _jsx(MessageSquareIcon, {}) },
    { key: 'categories', label: 'Categories', icon: _jsx(TagIcon, {}) },
];
export default function AppLayout() {
    const location = useLocation();
    const navigate = useNavigate();
    const currentKey = location.pathname.split('/')[1] || 'overview';
    return (_jsx(_Fragment, { children: _jsxs(Flex, { style: { minHeight: '100vh', background: 'var(--gray-50)' }, children: [_jsx(Sidebar, { items: items, selectedKey: currentKey, onSelect: key => navigate(`/${key}`), title: "Event Dashboard" }), _jsx(Box, { flexGrow: "1", children: _jsx(Outlet, {}) })] }) }));
}
