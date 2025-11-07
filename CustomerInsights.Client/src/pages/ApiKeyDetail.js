import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Box, Flex, Heading, Button, Card, TextField, Badge, } from '@radix-ui/themes';
import { ChevronLeftIcon } from 'lucide-react';
export default function ApiKeyDetail() {
    const navigate = useNavigate();
    const { keyId } = useParams();
    const [isEditing, setIsEditing] = useState(false);
    const [form, setForm] = useState({});
    const handleChange = (field) => (e) => {
        setForm((prev) => ({ ...prev, [field]: e.target.value }));
    };
    return (_jsx(Flex, { style: { minHeight: '100vh', background: 'var(--gray-50)' }, children: _jsxs(Box, { flexGrow: "1", p: "6", style: { maxWidth: 800, margin: '0 auto' }, children: [_jsxs(Flex, { justify: "between", align: "center", mb: "5", children: [_jsxs(Flex, { align: "center", gap: "3", children: [_jsx(Button, { variant: "ghost", size: "2", onClick: () => navigate(-1), children: _jsx(ChevronLeftIcon, {}) }), _jsxs(Heading, { size: "5", children: ["API Key: ", "name"] })] }), _jsxs(Flex, { gap: "2", children: [!isEditing ? (_jsx(Button, { variant: "outline", size: "2", onClick: () => setIsEditing(true), children: "Edit" })) : (_jsx(Button, { color: "green", size: "2", children: "Speichern" })), _jsx(Button, { color: "red", size: "2", children: "Revoke Key" })] })] }), _jsxs(Card, { variant: "surface", size: "3", mb: "5", children: [_jsx(Heading, { size: "4", style: { padding: '1rem 1rem 0', marginBottom: '1rem' }, children: "Key Information" }), _jsxs(Flex, { direction: "column", gap: "4", p: "4", children: [_jsx(TextField.Root, { placeholder: "Name", disabled: !isEditing, onChange: handleChange('name') }), _jsx(TextField.Root, { placeholder: "Description", disabled: !isEditing, onChange: handleChange('description') }), _jsxs(Flex, { gap: "4", children: [_jsx(Box, { children: _jsx(TextField.Root, { type: "date", disabled: true }) }), _jsx(Box, { children: _jsxs(Flex, { align: "center", gap: "2", children: [_jsx(Badge, { color: "green", variant: "soft", children: "Expires At" }), _jsx(TextField.Root, { type: "date", disabled: !isEditing, onChange: handleChange('expiresAt') })] }) })] })] })] })] }) }));
}
