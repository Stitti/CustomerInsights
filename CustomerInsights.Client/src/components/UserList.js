import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from 'react';
import { Table, Box, Flex, Heading, TextField, IconButton, DropdownMenu, } from '@radix-ui/themes';
import { EllipsisIcon, SearchIcon } from 'lucide-react';
import { useTranslation } from "react-i18next";
import { formatDateNumeric } from "../utils/dateUtils";
export function UserList() {
    const { t } = useTranslation();
    const [inviteOpen, setInviteOpen] = useState(false);
    const [newName, setNewName] = useState('');
    const [newEmail, setNewEmail] = useState('');
    const [newRole, setNewRole] = useState('Viewer');
    const [search, setSearch] = useState('');
    const filtered = [];
    return (_jsx(Flex, { children: _jsxs(Box, { flexGrow: "1", p: "6", children: [_jsx(Flex, { justify: "between", align: "center", mb: "5", children: _jsx(Heading, { mb: "4", children: t('sidebar.users') }) }), _jsx(Flex, { mb: "4", children: _jsx(TextField.Root, { placeholder: t('userList.searchUser'), value: search, onChange: (e) => setSearch(e.target.value), style: { flex: 1, padding: '8px 12px', borderRadius: 6, border: '1px solid var(--gray-200)' }, children: _jsx(TextField.Slot, { children: _jsx(SearchIcon, { height: "16", width: "16" }) }) }) }), _jsxs(Table.Root, { children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: t('user.name') }), _jsx(Table.ColumnHeaderCell, { children: t('user.email') }), _jsx(Table.ColumnHeaderCell, { children: t('user.role') }), _jsx(Table.ColumnHeaderCell, { children: t('user.created_at') }), _jsx(Table.ColumnHeaderCell, {})] }) }), _jsx(Table.Body, { children: filtered?.map((c) => (_jsxs(Table.Row, { style: { cursor: 'pointer' }, children: [_jsx(Table.Cell, { children: c.displayName }), _jsx(Table.Cell, { children: c.email }), _jsx(Table.Cell, { children: c.role }), _jsx(Table.Cell, { children: c.createdAt ? formatDateNumeric.format(new Date(c.createdAt)) : '—' }), _jsx(Table.Cell, { children: _jsxs(DropdownMenu.Root, { children: [_jsx(DropdownMenu.Trigger, { children: _jsx(IconButton, { variant: "ghost", children: _jsx(EllipsisIcon, {}) }) }), _jsxs(DropdownMenu.Content, { color: "gray", highContrast: true, children: [_jsx(DropdownMenu.Item, { style: { cursor: "pointer" }, children: t('userList.managePermissions') }), _jsx(DropdownMenu.Item, { style: { cursor: "pointer" }, children: t('userList.delete') })] })] }) })] }, c.uid))) })] })] }) }));
}
