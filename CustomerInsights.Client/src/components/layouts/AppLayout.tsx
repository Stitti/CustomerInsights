import React, { useState } from 'react';
import { Flex, Box, Dialog, IconButton } from '@radix-ui/themes';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import Sidebar, {type SidebarItem} from "../Sidebar";
import {
    BellIcon,
    Building2Icon,
    LayoutDashboard,
    MessageSquareIcon,
    RouteIcon,
    SquareUserRoundIcon,
    TagIcon,
    BotIcon,
    XIcon
} from "lucide-react";
import ChatbotWindow from "../ChatbotWindow";
import ChatbotButton from "../ChatbotButton";

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
    const [chatOpen, setChatOpen] = useState(false);

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
                <Box flexGrow="1" style={{ position: 'relative' }}>
                    <Outlet />

                    {/* Floating Chatbot Button */}
                    {!chatOpen && (
                        <Box
                                style={{
                                position: 'fixed',
                                bottom: '24px',
                                right: '24px',
                                zIndex: 1000,
                            }}
                        >
                            <ChatbotButton onClick={() => setChatOpen(true)}/>
                        </Box>
                    )}
                </Box>
            </Flex>

            {/* Chatbot Dialog */}
            <Dialog.Root open={chatOpen} onOpenChange={setChatOpen}>
                <Dialog.Content
                    style={{
                        width: '60vw',
                        height: '80vh',
                        padding: 0,
                        overflow: 'hidden'
                    }}
                >
                    <ChatbotWindow onClose={() => setChatOpen(false)} />
                </Dialog.Content>
            </Dialog.Root>
        </>
    );
}