import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { useRef, useState, useCallback } from 'react';
import { Box, Button, Text, IconButton, Flex } from '@radix-ui/themes';
import { XIcon } from 'lucide-react';
import { useTranslation } from "react-i18next";
export function ImageUpload({ allowMultiple = false }) {
    const { t } = useTranslation();
    const [previews, setPreviews] = useState([]);
    const [dragActive, setDragActive] = useState(false);
    const inputRef = useRef(null);
    const onFile = useCallback((file) => {
        const url = URL.createObjectURL(file);
        setPreviews(prev => {
            if (!allowMultiple && prev[0])
                URL.revokeObjectURL(prev[0]);
            if (allowMultiple)
                return [...prev, url];
            return [url];
        });
    }, [allowMultiple]);
    const handleDragEnter = (e) => {
        e.preventDefault();
        setDragActive(true);
    };
    const handleDragOver = (e) => {
        e.preventDefault();
        setDragActive(true);
    };
    const handleDragLeave = (e) => {
        e.preventDefault();
        setDragActive(false);
    };
    const handleDrop = (e) => {
        e.preventDefault();
        setDragActive(false);
        const files = e.dataTransfer.files;
        if (files && files.length) {
            if (allowMultiple) {
                Array.from(files).forEach(onFile);
            }
            else {
                onFile(files[0]);
            }
            e.dataTransfer.clearData();
        }
    };
    const handleBrowseClick = () => inputRef.current?.click();
    const handleFileChange = (e) => {
        const files = e.target.files;
        if (files && files.length) {
            if (allowMultiple) {
                Array.from(files).forEach(onFile);
            }
            else {
                onFile(files[0]);
            }
        }
    };
    const removeImage = (idx) => {
        setPreviews(prev => {
            const url = prev[idx];
            URL.revokeObjectURL(url);
            return prev.filter((_, i) => i !== idx);
        });
        if (inputRef.current)
            inputRef.current.value = '';
    };
    return (_jsxs(Box, { style: {
            border: '2px dashed',
            padding: '2rem',
            borderRadius: '8px',
            textAlign: 'center',
            backgroundColor: dragActive ? 'lightgrey' : 'transparent',
            position: 'relative',
            borderStyle: "dashed"
        }, onDragEnter: handleDragEnter, onDragOver: handleDragOver, onDragLeave: handleDragLeave, onDrop: handleDrop, children: [_jsx("input", { ref: inputRef, type: "file", accept: "image/*", multiple: allowMultiple, style: { display: 'none' }, onChange: handleFileChange }), allowMultiple == true || previews.length === 0 ? (_jsx(_Fragment, { children: _jsxs(Flex, { direction: "column", children: [_jsx(Text, { size: "3", style: { marginBottom: '1rem' }, children: "Drag & drop image here" }), _jsxs(Button, { size: "3", variant: "outline", onClick: handleBrowseClick, children: ["Browse ", allowMultiple ? 'Files' : 'File'] })] }) })) : (_jsx(_Fragment, {})), _jsx(Box, { style: {
                    display: 'flex',
                    gap: '1rem',
                    flexWrap: 'wrap',
                    marginTop: '1rem',
                    justifyContent: 'center',
                }, children: previews.map((src, idx) => (_jsxs(Box, { style: { position: 'relative', display: 'inline-block' }, children: [_jsx("img", { src: src, alt: `preview-${idx}`, style: {
                                width: '200px',
                                height: '200px',
                                objectFit: 'cover',
                                borderRadius: '4px',
                            } }), _jsx(IconButton, { variant: "solid", style: { position: 'absolute', top: '5px', right: '5px' }, onClick: () => removeImage(idx), children: _jsx(XIcon, {}) })] }, idx))) })] }));
}
