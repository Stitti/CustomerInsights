import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from 'react';
import { Button, Popover, Flex, Text, } from '@radix-ui/themes';
import { ChevronDownIcon } from 'lucide-react';
export function LanguageSelector(props) {
    console.log(props);
    const languages = [
        { name: "Deutsch", languageCode: "DE" },
        { name: "English", languageCode: "EN" }
    ];
    const [selectedCode, setSelectedCode] = useState(props.userLanguage.languageCode);
    const [open, setOpen] = useState(false);
    const selectedLang = languages?.find(l => l.languageCode === selectedCode);
    return (_jsxs(Popover.Root, { open: open, onOpenChange: setOpen, children: [_jsx(Popover.Trigger, { children: _jsxs(Button, { variant: "outline", size: "2", style: { display: 'flex', alignItems: 'center', gap: 4 }, children: [_jsx("img", { src: `https://flagcdn.com/w20/${selectedLang.languageCode}.png`, alt: selectedLang.name, style: { width: 20, height: 15, borderRadius: 2 } }), _jsx(Text, { size: "2", children: selectedLang.name }), _jsx(ChevronDownIcon, {})] }) }), _jsx(Popover.Content, { side: "bottom", align: "start", style: {
                    padding: 8,
                    minWidth: 160,
                    background: 'white',
                    borderRadius: 4,
                    boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                }, children: _jsx(Flex, { direction: "column", style: { gap: 4 }, children: languages?.map(lang => (_jsxs(Button, { variant: lang.languageCode === selectedCode ? 'solid' : 'ghost', size: "2", style: {
                            justifyContent: 'flex-start',
                            gap: 8,
                            padding: '4px 8px',
                        }, onClick: () => {
                            setSelectedCode(lang.languageCode);
                            setOpen(false);
                        }, children: [_jsx("img", { src: `https://flagcdn.com/w20/${lang.languageCode}.png`, alt: lang.name, style: { width: 20, height: 15, borderRadius: 2 } }), _jsx(Text, { children: lang.name })] }, lang.languageCode))) }) })] }));
}
