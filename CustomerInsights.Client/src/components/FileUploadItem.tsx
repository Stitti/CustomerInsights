import {Box, Button, Flex, IconButton, Progress, Text} from "@radix-ui/themes";
import {FileText, X} from "lucide-react";
import * as React from "react";

interface FileUploadItemProps {
    item: UploadItem;
    onRemove: () => void;
}

export default function FileUploadItem({ item, onRemove }: FileUploadItemProps) {
    return (
        <Flex align="start" gap="3">
            <Box className="file-upload-item-icon">
                <FileText className="file-upload-file-icon" aria-hidden />
            </Box>

            <Flex direction="column" gap="1" flexGrow="1">
                <Flex
                    justify="between"
                    align={{ initial: 'start', sm: 'center' }}
                    gap="2"
                >
                    <Flex direction="column" gap="1">
                        <Text size="2" weight="medium">
                            {item.name}
                        </Text>
                        <Text size="1" color="gray">
                            {item.sizeLabel}
                        </Text>
                    </Flex>

                    <Flex
                        align={{ initial: 'start', sm: 'center' }}
                        justify="end"
                        gap="3"
                        style={{ minWidth: 0 }}
                    >
                        <Box minWidth={{ initial: 'auto', sm: '160px' }}>
                            {item.status === 'uploading' && (
                                <Flex direction="column" gap="1" align="end">
                                    <Text size="1" color="gray">
                                        Verarbeite…
                                    </Text>
                                    <Flex align="center" gap="2">
                                        <Box style={{ width: 120 }}>
                                            <Progress
                                                value={item.progress ?? 0}
                                                size="1"
                                                radius="full"
                                            />
                                        </Box>
                                        <Text size="1" color="gray">
                                            {(item.progress ?? 0) + '%'}
                                        </Text>
                                    </Flex>
                                </Flex>
                            )}

                            {item.status === 'success' && (
                                <Flex direction="column" gap="1" align="end">
                                    <Text size="1" color="green">
                                        Datensätze erstellt
                                    </Text>
                                </Flex>
                            )}

                            {item.status === 'error' && (
                                <Flex direction="column" gap="1" align="end">
                                    <Text size="1" color="red">
                                        Fehler beim Import
                                    </Text>
                                </Flex>
                            )}
                        </Box>

                        <IconButton
                            size="1"
                            variant="ghost"
                            color="gray"
                            onClick={onRemove}
                            aria-label={`Remove ${item.name}`}
                        >
                            <X size={14} />
                        </IconButton>
                    </Flex>
                </Flex>
            </Flex>
        </Flex>
    );
}