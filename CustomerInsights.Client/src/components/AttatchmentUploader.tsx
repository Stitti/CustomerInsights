import * as React from 'react';
import { useDropzone } from 'react-dropzone';
import {Box, Flex, Text, Button, IconButton} from '@radix-ui/themes';
import {UploadCloudIcon, XIcon} from 'lucide-react';

export interface AttachmentUploaderProps {
    value: File[];
    onChange: (files: File[]) => void;
    maxFiles?: number;
}

export const AttachmentUploader: React.FC<AttachmentUploaderProps> = ({
                                                                          value,
                                                                          onChange,
                                                                          maxFiles = 10,
                                                                      }) => {
    const onDrop = React.useCallback(
        (acceptedFiles: File[]) => {
            if (!acceptedFiles?.length) return;
            const next = [...value, ...acceptedFiles].slice(0, maxFiles);
            onChange(next);
        },
        [value, onChange, maxFiles]
    );

    const {
        getRootProps,
        getInputProps,
        isDragActive,
        isDragReject,
        isDragAccept,
    } = useDropzone({
        onDrop,
        multiple: true,
    });

    const removeFile = (file: File) => {
        onChange(value.filter((f) => f !== file));
    };

    const borderColor = isDragReject
        ? 'var(--red-9)'
        : isDragAccept
            ? 'var(--accent-9)'
            : isDragActive
                ? 'var(--accent-8)'
                : 'var(--gray-a7)';

    const bgColor = isDragActive
        ? 'color-mix(in srgb, var(--accent-3) 55%, transparent)'
        : 'var(--color-panel)';

    return (
        <Box>
            <Box
                {...getRootProps({
                    className: 'attachment-dropzone',
                    style: {
                        borderColor,
                        backgroundColor: bgColor,
                    },
                })}
            >
                <input {...getInputProps()} />
                <Flex
                    direction={{ initial: 'column', sm: 'row' }}
                    align="center"
                    justify="between"
                    gap="3"
                >
                    <Flex align="center" gap="3">
                        <Box className="attachment-dropzone-icon">
                            <UploadCloudIcon className="attachment-dropzone-icon-svg" />
                        </Box>
                        <Flex direction="column" gap="1">
                            <Text size="2" weight="medium">
                                Dateien hierher ziehen oder klicken
                            </Text>
                            <Text size="1" color="gray">
                                Screenshots, Logs, CSV, PDFs, max. {maxFiles} Dateien
                            </Text>
                        </Flex>
                    </Flex>
                </Flex>

                {isDragReject && (
                    <Text size="1" color="red" mt="2">
                        Einige Dateien werden nicht unterstützt.
                    </Text>
                )}
            </Box>

            {value.length > 0 && (
                <Box mt="2" className="attachment-list">
                    {value.map((file) => (
                        <Flex
                            key={file.name + file.size}
                            align="center"
                            justify="between"
                            className="attachment-list-item"
                        >
                            <Box className="attachment-list-meta">
                                <Text size="1" weight="medium">
                                    {file.name}
                                </Text>
                                <Text size="1" color="gray">
                                    {Math.round(file.size / 1024)} KB
                                </Text>
                            </Box>
                            <IconButton
                                type="button"
                                size="1"
                                variant="ghost"
                                color="gray"
                                onClick={() => removeFile(file)}
                            >
                                <XIcon size={14} />
                            </IconButton>
                        </Flex>
                    ))}
                </Box>
            )}
        </Box>
    );
};