import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Flex, Text } from '@radix-ui/themes';
import ProfileButton from './ProfileButton';
const defaultItems = [];
export const Sidebar = ({ items = defaultItems, selectedKey, onSelect, title = 'Event Dashboard', }) => (_jsxs(Box, { style: {
        borderRight: '1px solid var(--gray-200)',
        background: 'var(--white)',
        position: 'sticky',
        top: 0,
        height: '100vh',
        width: 260,
        flexShrink: 0,
        display: 'flex',
        flexDirection: 'column',
    }, children: [_jsx(Box, { p: "4", style: { borderBottom: '1px solid var(--gray-200)' }, children: _jsx(Text, { asChild: true, children: _jsx("strong", { style: { fontSize: '1.25rem' }, children: title }) }) }), _jsx(Box, { style: {
                flex: 1,
                overflowY: 'auto',
            }, children: items.map(item => (_jsxs(Flex, { align: "center", gap: "2", px: "4", py: "3", style: {
                    cursor: 'pointer',
                    background: selectedKey === item.key ? 'var(--gray-100)' : 'transparent',
                    color: selectedKey === item.key ? 'var(--blue-600)' : 'var(--gray-900)',
                }, onClick: () => onSelect(item.key), children: [item.icon, _jsx(Text, { size: "2", weight: selectedKey === item.key ? 'bold' : 'medium', children: item.label })] }, item.key))) }), _jsx(Box, { p: "4", style: {
                borderTop: '1px solid var(--gray-200)',
                background: 'var(--white)',
            }, children: _jsx(ProfileButton, {}) })] }));
export default Sidebar;
