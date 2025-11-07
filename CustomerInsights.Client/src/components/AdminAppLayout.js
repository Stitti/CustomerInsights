import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Flex, Box } from '@radix-ui/themes';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import Sidebar from './Sidebar';
import { UserIcon, BuildingIcon, GlobeLockIcon, MessageCircleQuestionMarkIcon, } from 'lucide-react';
import Header from "./Header";
const items = [
    { key: 'users', label: 'Users', icon: _jsx(UserIcon, {}) },
    { key: 'organization', label: 'Organization', icon: _jsx(BuildingIcon, {}) },
    { key: 'apikeys', label: 'API Keys', icon: _jsx(GlobeLockIcon, {}) },
    { key: 'support', label: 'Support', icon: _jsx(MessageCircleQuestionMarkIcon, {}) },
];
export default function AdminAppLayout() {
    const location = useLocation();
    const navigate = useNavigate();
    const currentKey = location.pathname.split('/')[1] || 'overview';
    return (_jsxs(_Fragment, { children: [_jsx(Header, {}), _jsxs(Flex, { style: { minHeight: '100vh', background: 'var(--gray-50)' }, children: [_jsx(Sidebar, { items: items, selectedKey: currentKey, onSelect: key => navigate(`admin/${key}`), title: "Event Dashboard" }), _jsx(Box, { flexGrow: "1", children: _jsx(Outlet, {}) })] })] }));
}
