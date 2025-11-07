import React from 'react';
import {Box, Button, Flex, Text} from '@radix-ui/themes';
import ProfileButton from './ProfileButton';

export interface SidebarItem {
    key: string;
    label: string;
    icon: React.ReactNode;
}

interface SidebarProps {
    items: SidebarItem[];
    selectedKey: string;
    onSelect: (key: string) => void;
    title?: string;
}

const defaultItems: SidebarItem[] = [];

export const Sidebar: React.FC<SidebarProps> = ({
                                                    items = defaultItems,
                                                    selectedKey,
                                                    onSelect,
                                                    title = 'Event Dashboard',
                                                }) => (
    <Box
        style={{
            borderRight: '1px solid var(--gray-200)',
            background: 'var(--white)',
            position: 'sticky',
            top: 0,
            height: '100vh',
            width: 260,
            flexShrink: 0,
            display: 'flex',
            flexDirection: 'column',
        }}
    >
        {/* Header */}
        <Box p="4" style={{ borderBottom: '1px solid var(--gray-200)' }}>
            <Text asChild>
                <strong style={{ fontSize: '1.25rem' }}>{title}</strong>
            </Text>
        </Box>

        {/* Scrollbarer Bereich */}
        <Box
            style={{
                flex: 1,
                overflowY: 'auto',
            }}
        >
            {items.map(item => (
                <Flex
                    key={item.key}
                    align="center"
                    gap="2"
                    px="4"
                    py="3"
                    style={{
                        cursor: 'pointer',
                        background: selectedKey === item.key ? 'var(--gray-100)' : 'transparent',
                        color: selectedKey === item.key ? 'var(--blue-600)' : 'var(--gray-900)',
                    }}
                    onClick={() => onSelect(item.key)}
                >
                    {item.icon}
                    <Text size="2" weight={selectedKey === item.key ? 'bold' : 'medium'}>
                        {item.label}
                    </Text>
                </Flex>
            ))}
        </Box>

        {/* Fixierter Bereich unten */}
        <Box
            p="4"
            style={{
                borderTop: '1px solid var(--gray-200)',
                background: 'var(--white)',
            }}
        >
            <ProfileButton />
        </Box>
    </Box>
);

export default Sidebar;
