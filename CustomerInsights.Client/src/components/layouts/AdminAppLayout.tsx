import React from 'react';
import { Flex, Box } from '@radix-ui/themes';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import Sidebar, { SidebarItem } from '../Sidebar.tsx';
import {
    UserIcon,
    BuildingIcon,
    GlobeLockIcon,
    MessageCircleQuestionMarkIcon, ServerIcon,
} from 'lucide-react';
import Header from "../headers/Header.tsx";

const items: SidebarItem[] = [
    { key: 'users',    label: 'Users',    icon: <UserIcon /> },
    { key: 'organization', label: 'Organization',   icon: <BuildingIcon /> },
    { key: 'integrations', label: 'Integrations', icon: <ServerIcon/>},
    { key: 'apikeys',   label: 'API Keys',     icon: <GlobeLockIcon /> },
    { key: 'support',    label: 'Support',      icon: <MessageCircleQuestionMarkIcon /> },
];

export default function AdminAppLayout() {
    const location = useLocation();
    const navigate = useNavigate();

    const currentKey = location.pathname.split('/')[1] || 'overview';

    return (
        <>
            <Header/>
            <Flex style={{ minHeight: '100vh', background: 'var(--gray-50)' }}>
                <Sidebar
                    items={items}
                    selectedKey={currentKey}
                    onSelect={key => navigate(`admin/${key}`)}
                    title="Event Dashboard"
                />

                <Box flexGrow="1">
                    <Outlet />
                </Box>
            </Flex>
        </>
    );
}
