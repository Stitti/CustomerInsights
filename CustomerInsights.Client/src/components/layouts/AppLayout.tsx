import React, { useState } from 'react';
import { Flex, Box } from '@radix-ui/themes';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import Sidebar, {type SidebarItem} from "../Sidebar.tsx";
import {
    BellIcon,
    Building2Icon,
    LayoutDashboard,
    MessageSquareIcon,
    RouteIcon,
    SquareUserRoundIcon,
    TagIcon
} from "lucide-react";

const items: SidebarItem[] = [
    { key: '',     label: 'Dashboard',    icon: <LayoutDashboard /> },
    { key: 'signals',     label: 'Signals',    icon: <BellIcon/>},
    { key: 'journey', label: 'Journey',    icon: <RouteIcon /> },
    { key: 'accounts',     label: 'Accounts',    icon: <Building2Icon /> },
    { key: 'contacts',     label: 'Contacts',    icon: <SquareUserRoundIcon /> },
    { key: 'interactions',    label: 'Interactions',    icon: <MessageSquareIcon /> },
    { key: 'categories',     label: 'Categories',     icon: <TagIcon /> },
];

export default function AppLayout() {
    const location = useLocation();
    const navigate = useNavigate();

    const currentKey = location.pathname.split('/')[1] || 'overview';

    return (
        <>
            <Flex style={{ minHeight: '100vh', background: 'var(--gray-50)' }}>
                <Sidebar
                    items={items}
                    selectedKey={currentKey}
                    onSelect={key => navigate(`/${key}`)}
                    title="Event Dashboard"
                />
                <Box flexGrow="1">
                    <Outlet />
                </Box>
            </Flex>
        </>
    );
}
