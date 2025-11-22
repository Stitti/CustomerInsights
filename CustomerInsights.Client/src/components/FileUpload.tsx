import * as React from 'react';
import { useDropzone } from 'react-dropzone';
import {
    Card,
    Flex,
    Text,
    Button,
    Box,
    Separator,
} from '@radix-ui/themes';
import { UploadCloud } from 'lucide-react';
import Papa from 'papaparse';
import * as XLSX from 'xlsx';
import FileUploadItem from "./FileUploadItem";


interface FileUploaderProps {
    onRecordsCreated?: (records: any[], context: { file: File; itemId: string }) => void;
    onParseError?: (error: unknown, context: { file: File; itemId: string }) => void;
}

function formatBytes(bytes: number | undefined | null): string {
    if (bytes === undefined || bytes === null) return '';
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    const value = bytes / Math.pow(1024, i);
    return `${value.toFixed(1)} ${sizes[i]}`;
}

function parseCsvFile(file: File): Promise<any[]> {
    return new Promise((resolve, reject) => {
        Papa.parse(file, {
            header: true,
            skipEmptyLines: true,
            complete: (results) => resolve(results.data as any[]),
            error: (err) => reject(err),
        });
    });
}

function parseXlsxFile(file: File): Promise<any[]> {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.onload = (e: ProgressEvent<FileReader>) => {
            try {
                const result = e.target?.result;

                if (!result) {
                    reject(new Error('FileReader result is null'));
                    return;
                }

                const data = new Uint8Array(result as ArrayBuffer);
                const workbook = XLSX.read(data, { type: 'array' });
                const firstSheetName = workbook.SheetNames[0];
                const worksheet = workbook.Sheets[firstSheetName];
                const json = XLSX.utils.sheet_to_json(worksheet, { defval: null });
                resolve(json as any[]);
            } catch (err) {
                reject(err);
            }
        };

        reader.onerror = (err) => reject(err);

        reader.readAsArrayBuffer(file);
    });
}

export function FileUploader({ onRecordsCreated, onParseError }: FileUploaderProps) {
    const [items, setItems] = React.useState<UploadItem[]>([]);
    const progressIntervalsRef = React.useRef<Map<string, number>>(new Map());

    const startFakeProgress = React.useCallback((id: string) => {
        const interval = window.setInterval(() => {
            setItems((prev) =>
                prev.map((it) =>
                    it.id === id
                        ? { ...it, progress: Math.min((it.progress ?? 0) + 5, 90) }
                        : it
                )
            );
        }, 250);

        progressIntervalsRef.current.set(id, interval);
    }, []);

    const stopFakeProgress = React.useCallback((id: string, finalValue = 100) => {
        const interval = progressIntervalsRef.current.get(id);
        if (interval) {
            window.clearInterval(interval);
            progressIntervalsRef.current.delete(id);
        }

        setItems((prev) =>
            prev.map((it) =>
                it.id === id ? { ...it, progress: finalValue } : it
            )
        );
    }, []);

    const processFile = React.useCallback(
        async (item: UploadItem) => {
            const { file, id } = item;

            try {
                let records: any[] = [];
                const lowerName = file.name.toLowerCase();

                if (lowerName.endsWith('.csv')) {
                    records = await parseCsvFile(file);
                } else if (lowerName.endsWith('.xlsx')) {
                    records = await parseXlsxFile(file);
                } else {
                    throw new Error('Unsupported file type');
                }

                stopFakeProgress(id, 100);

                setItems((prev) =>
                    prev.map((it) =>
                        it.id === id ? { ...it, status: 'success', progress: 100 } : it
                    )
                );

                onRecordsCreated?.(records, { file, itemId: id });
            } catch (error) {
                console.error('Fehler beim Verarbeiten der Datei:', error);
                stopFakeProgress(id, 0);

                setItems((prev) =>
                    prev.map((it) =>
                        it.id === id ? { ...it, status: 'error' } : it
                    )
                );

                onParseError?.(error, { file, itemId: id });
            }
        },
        [onRecordsCreated, onParseError, stopFakeProgress]
    );

    const onDrop = React.useCallback(
        (acceptedFiles: File[]) => {
            if (!acceptedFiles?.length) return;

            const newItems: UploadItem[] = acceptedFiles.map((file) => {
                const id = `${file.name}-${file.size}-${file.lastModified}-${Math.random()
                    .toString(36)
                    .slice(2)}`;

                return {
                    id,
                    file,
                    name: file.name,
                    sizeLabel: formatBytes(file.size),
                    status: 'uploading',
                    progress: 0,
                };
            });

            setItems((prev) => [...prev, ...newItems]);

            newItems.forEach((item) => {
                startFakeProgress(item.id);
                processFile(item);
            });
        },
        [processFile, startFakeProgress]
    );

    const {
        getRootProps,
        getInputProps,
        isDragActive,
        isDragReject,
        isDragAccept,
        fileRejections,
        open,
    } = useDropzone({
        onDrop,
        accept: {
            'text/csv': ['.csv'],
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': [
                '.xlsx',
            ],
        },
        multiple: true,
    });

    const handleRemove = (id: string) => {
        const interval = progressIntervalsRef.current.get(id);
        if (interval) {
            window.clearInterval(interval);
            progressIntervalsRef.current.delete(id);
        }
        setItems((prev) => prev.filter((item) => item.id !== id));
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

    React.useEffect(() => {
        return () => {
            progressIntervalsRef.current.forEach((interval) =>
                window.clearInterval(interval)
            );
            progressIntervalsRef.current.clear();
        };
    }, []);

    return (
        <Card
            variant="surface"
            size="3"
            className="file-upload-card"
            role="group"
            aria-label="File uploader"
        >
            <Flex direction="column" gap="4">
                <Box
                    {...getRootProps({
                        className: 'file-upload-dropzone',
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
                        gap="3"
                    >
                        <Box className="file-upload-icon-wrapper">
                            <UploadCloud className="file-upload-icon" aria-hidden />
                        </Box>

                        <Flex
                            direction="column"
                            gap="1"
                            align={{ initial: 'center', sm: 'start' }}
                        >
                            <Flex gap="1" align="baseline" wrap="wrap">
                                <Text size="2" color="gray">
                                    Click to upload
                                </Text>
                                <Text size="2" color="gray">
                                    or drag and drop
                                </Text>
                            </Flex>
                            <Text size="1" color="gray">
                                Nur .csv oder .xlsx Dateien werden akzeptiert.
                            </Text>
                        </Flex>

                        <Box
                            ml={{ initial: '0', sm: 'auto' }}
                            mt={{ initial: '3', sm: '0' }}
                        >
                            <Button
                                size="2"
                                variant="soft"
                                type="button"
                                onClick={(e) => {
                                    e.stopPropagation();
                                    open();
                                }}
                            >
                                Browse files
                            </Button>
                        </Box>
                    </Flex>

                    {(isDragReject || fileRejections.length > 0) && (
                        <Text
                            size="1"
                            color="red"
                            mt="2"
                            align={{ initial: 'center', sm: 'left' }}
                        >
                            Es sind nur Dateien mit den Endungen .csv und .xlsx erlaubt.
                        </Text>
                    )}
                </Box>

                {items.length > 0 && (
                    <Box className="file-upload-list">
                        {items.map((item, index) => (
                            <React.Fragment key={item.id}>
                                {index !== 0 && <Separator size="1" my="3" />}
                                <FileUploadItem
                                    item={item}
                                    onRemove={() => handleRemove(item.id)}
                                />
                            </React.Fragment>
                        ))}
                    </Box>
                )}
            </Flex>
        </Card>
    );
}