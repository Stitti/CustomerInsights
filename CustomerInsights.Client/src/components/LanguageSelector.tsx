import React, { useState } from 'react';
import {
    Button,
    Popover,
    Flex,
    Text,
} from '@radix-ui/themes';
import { ChevronDownIcon } from 'lucide-react';

interface Language {
    name: string
    languageCode: string
}

interface Props {
    userLanguage: Language;
}

export function LanguageSelector(props: Props): any {
    console.log(props);
    const languages = [
        {name: "Deutsch", languageCode: "DE"},
        {name: "English", languageCode: "EN"}
    ];
    const [selectedCode, setSelectedCode] = useState(props.userLanguage.languageCode);
    const [open, setOpen] = useState(false);

    const selectedLang = languages?.find(l => l.languageCode === selectedCode)!;

    return (
            <Popover.Root open={open} onOpenChange={setOpen}>
                <Popover.Trigger>
                    <Button
                        variant="outline"
                        size="2"
                        style={{ display: 'flex', alignItems: 'center', gap: 4 }}
                    >
                        <img
                            src={`https://flagcdn.com/w20/${selectedLang.languageCode}.png`}
                            alt={selectedLang.name}
                            style={{ width: 20, height: 15, borderRadius: 2 }}
                        />
                        <Text size="2">{selectedLang.name}</Text>
                        <ChevronDownIcon />
                    </Button>
                </Popover.Trigger>

                <Popover.Content
                    side="bottom"
                    align="start"
                    style={{
                        padding: 8,
                        minWidth: 160,
                        background: 'white',
                        borderRadius: 4,
                        boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                    }}
                >
                    <Flex direction="column" style={{ gap: 4 }}>
                        {languages?.map(lang => (
                            <Button
                                key={lang.languageCode}
                                variant={lang.languageCode === selectedCode ? 'solid' : 'ghost'}
                                size="2"
                                style={{
                                    justifyContent: 'flex-start',
                                    gap: 8,
                                    padding: '4px 8px',
                                }}
                                onClick={() => {
                                    setSelectedCode(lang.languageCode);
                                    setOpen(false);
                                }}
                            >
                                <img
                                    src={`https://flagcdn.com/w20/${lang.languageCode}.png`}
                                    alt={lang.name}
                                    style={{ width: 20, height: 15, borderRadius: 2 }}
                                />
                                <Text>{lang.name}</Text>
                            </Button>
                        ))}
                    </Flex>
                </Popover.Content>
            </Popover.Root>
    );
}