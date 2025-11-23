import React, { useState, useRef, useEffect } from 'react';
import { Theme, Box, Flex, TextField, Button, ScrollArea, Card, Text, Avatar, IconButton } from '@radix-ui/themes';
import {SendHorizonalIcon, SendIcon, XIcon} from 'lucide-react';

interface Message {
    id: string;
    text: string;
    sender: 'user' | 'bot';
    timestamp: Date;
}

interface ChatbotWindowProps {
    onClose?: () => void;
}

export function ChatbotWindow({ onClose }: ChatbotWindowProps) {
    const [messages, setMessages] = useState<Message[]>([]);
    const [inputValue, setInputValue] = useState('');
    const [isTyping, setIsTyping] = useState(false);
    const scrollRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [messages, isTyping]);

    const handleSend = () => {
        if (!inputValue.trim()) return;

        const userMessage: Message = {
            id: Date.now().toString(),
            text: inputValue,
            sender: 'user',
            timestamp: new Date()
        };

        setMessages(prev => [...prev, userMessage]);
        setInputValue('');
        setIsTyping(true);

        setTimeout(() => {
            const botMessage: Message = {
                id: (Date.now() + 1).toString(),
                text: getBotResponse(inputValue),
                sender: 'bot',
                timestamp: new Date()
            };
            setMessages(prev => [...prev, botMessage]);
            setIsTyping(false);
        }, 1000);
    };

    const getBotResponse = (input: string): string => {
        return 'Bot-Antwort kommt hier...';
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    return (
        <Flex direction="column" style={{ height: '100%', width: '100%', background: 'var(--gray-1)' }}>
            {/* Header */}
            <Box p="4" style={{ borderBottom: '1px solid var(--gray-6)', background: 'var(--white)' }}>
                <Flex align="center" justify="between">
                    <Flex align="center" gap="3">
                        <Avatar size="3" fallback="🤖" color="blue" />
                        <Box>
                            <Text size="4" weight="bold">AI Assistant</Text>
                        </Box>
                    </Flex>
                    {onClose && (
                        <IconButton
                            variant="ghost"
                            color="gray"
                            onClick={onClose}
                            style={{ cursor: 'pointer' }}
                        >
                            <XIcon size={20} />
                        </IconButton>
                    )}
                </Flex>
            </Box>

            {/* Messages */}
            <ScrollArea style={{ flex: 1, padding: '16px', width: 'auto' }} ref={scrollRef}>
                {messages.length === 0 ? (
                    <Flex
                        direction="column"
                        align="center"
                        justify="center"
                        gap="3"
                        style={{ height: '100%', textAlign: 'center' }}
                    >
                        <Box style={{ fontSize: '48px' }}>🤖</Box>
                        <Text size="3" weight="bold">Willkommen beim AI Assistant</Text>
                        <Text size="2" color="gray">Stelle mir eine Frage oder starte eine Unterhaltung</Text>
                    </Flex>
                ) : (
                    <Flex direction="column" gap="3">
                        {messages.map(msg => (
                            <Flex
                                key={msg.id}
                                justify={msg.sender === 'user' ? 'end' : 'start'}
                                gap="2"
                            >
                                {msg.sender === 'bot' && (
                                    <Avatar size="2" fallback="🤖" color="blue" style={{ flexShrink: 0 }} />
                                )}
                                <Box
                                    p="3"
                                    style={{
                                        maxWidth: '70%',
                                        borderRadius: '12px',
                                        background: msg.sender === 'user' ? 'var(--accent-9)' : 'var(--gray-4)',
                                        color: msg.sender === 'user' ? 'white' : 'var(--gray-12)'
                                    }}
                                >
                                    <Text size="2">{msg.text}</Text>
                                </Box>
                                {msg.sender === 'user' && (
                                    <Avatar size="2" fallback="U" color="green" style={{ flexShrink: 0 }} />
                                )}
                            </Flex>
                        ))}
                        {isTyping && (
                            <Flex align="center" gap="2">
                                <Avatar size="2" fallback="🤖" color="blue" />
                                <Box p="3" style={{ borderRadius: '12px', background: 'var(--gray-4)' }}>
                                    <Text size="2" color="gray">Tippt...</Text>
                                </Box>
                            </Flex>
                        )}
                    </Flex>
                )}
            </ScrollArea>

            {/* Input */}
            <Box p="3" style={{ borderTop: '1px solid var(--gray-6)', background: 'var(--white)' }}>
                <Flex gap="2">
                    <TextField.Root
                        placeholder="Schreib eine Nachricht..."
                        value={inputValue}
                        onChange={e => setInputValue(e.target.value)}
                        onKeyPress={handleKeyPress}
                        style={{ flex: 1 }}
                        size="3"
                    />
                    <IconButton variant="soft" onClick={handleSend} disabled={!inputValue.trim()} size="3">
                        <SendHorizonalIcon size={23} />
                    </IconButton>
                </Flex>
            </Box>
        </Flex>
    );
}

export default ChatbotWindow;