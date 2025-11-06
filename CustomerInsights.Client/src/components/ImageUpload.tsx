import React, { useRef, useState, useCallback } from 'react';
import { Box, Button, Text, IconButton, Flex} from '@radix-ui/themes';
import { XIcon } from 'lucide-react';
import {useTranslation} from "react-i18next";

interface ImageUploadProps {
    allowMultiple?: boolean;
}

export function ImageUpload({ allowMultiple = false }: ImageUploadProps) {
    const { t } = useTranslation();
    const [previews, setPreviews] = useState<string[]>([]);
    const [dragActive, setDragActive] = useState(false);
    const inputRef = useRef<HTMLInputElement>(null);

    const onFile = useCallback(
        (file: File) => {
            const url = URL.createObjectURL(file);
            setPreviews(prev => {
                if (!allowMultiple && prev[0]) URL.revokeObjectURL(prev[0]);
                if (allowMultiple) return [...prev, url];
                return [url];
            });
        },
        [allowMultiple]
    );

    const handleDragEnter = (e: React.DragEvent) => {
        e.preventDefault();
        setDragActive(true);
    };
    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
        setDragActive(true);
    };
    const handleDragLeave = (e: React.DragEvent) => {
        e.preventDefault();
        setDragActive(false);
    };
    const handleDrop = (e: React.DragEvent) => {
        e.preventDefault();
        setDragActive(false);
        const files = e.dataTransfer.files;
        if (files && files.length) {
            if (allowMultiple) {
                Array.from(files).forEach(onFile);
            } else {
                onFile(files[0]);
            }
            e.dataTransfer.clearData();
        }
    };

    const handleBrowseClick = () => inputRef.current?.click();
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (files && files.length) {
            if (allowMultiple) {
                Array.from(files).forEach(onFile);
            } else {
                onFile(files[0]);
            }
        }
    };

    const removeImage = (idx: number) => {
        setPreviews(prev => {
            const url = prev[idx];
            URL.revokeObjectURL(url);
            return prev.filter((_, i) => i !== idx);
        });
        if (inputRef.current) inputRef.current.value = '';
    };

    return (
        <Box
            style={{
                border: '2px dashed',
                padding: '2rem',
                borderRadius: '8px',
                textAlign: 'center',
                backgroundColor: dragActive ? 'lightgrey' : 'transparent',
                position: 'relative',
                borderStyle: "dashed"
            }}
            onDragEnter={handleDragEnter}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onDrop={handleDrop}
        >
            <input
                ref={inputRef}
                type="file"
                accept="image/*"
                multiple={allowMultiple}
                style={{ display: 'none' }}
                onChange={handleFileChange}
            />

            {allowMultiple == true || previews.length === 0 ? (
                <>
                    <Flex direction="column">
                        <Text size="3" style={{ marginBottom: '1rem' }}>
                            Drag & drop image here
                        </Text>
                        <Button size="3" variant="outline" onClick={handleBrowseClick}>
                            Browse {allowMultiple ? 'Files' : 'File'}
                        </Button>
                    </Flex>
                </>
            ) : (<></>)}
            <Box
                style={{
                    display: 'flex',
                    gap: '1rem',
                    flexWrap: 'wrap',
                    marginTop: '1rem',
                    justifyContent: 'center',
                }}
            >
                {previews.map((src, idx) => (
                    <Box
                        key={idx}
                        style={{ position: 'relative', display: 'inline-block' }}
                    >
                        <img
                            src={src}
                            alt={`preview-${idx}`}
                            style={{
                                width: '200px',
                                height: '200px',
                                objectFit: 'cover',
                                borderRadius: '4px',
                            }}
                        />
                        <IconButton
                            variant="solid"
                            style={{ position: 'absolute', top: '5px', right: '5px' }}
                            onClick={() => removeImage(idx)}
                        >
                            <XIcon />
                        </IconButton>
                    </Box>
                ))}
            </Box>
        </Box>
    );
}
