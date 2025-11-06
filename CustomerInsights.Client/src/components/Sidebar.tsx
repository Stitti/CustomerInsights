import React from 'react';
import {Box, Flex, Text} from '@radix-ui/themes';

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
        }}
    >
        <Box p="4" mb="4">
            <Text asChild>
                <strong style={{ fontSize: '1.25rem' }}>{title}</strong>
            </Text>
        </Box>

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
);

export default Sidebar;