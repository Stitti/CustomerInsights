import * as React from 'react';
import {Dialog, Button, Flex, Box, Text, Link, IconButton} from '@radix-ui/themes';
import { FileUploader } from './FileUpload';
import {UploadIcon} from "lucide-react";

export default function CsvImportDialog() {
    const [open, setOpen] = React.useState(false);
    const [pendingRecords, setPendingRecords] = React.useState<any[]>([]);

    const handleRecordsCreated = React.useCallback((records: any[], { file }: { file: File; itemId: string }) => {
        setPendingRecords((prev) => [...prev, ...records]);
        console.log('Neue Records aus Datei:', file.name, records);
    }, []);

    const handleParseError = React.useCallback((error: unknown, { file }: { file: File; itemId: string }) => {
        console.error('Fehler beim Parsen der Datei:', file.name, error);
    }, []);

    const handleCreateDatasets = async () => {
        if (!pendingRecords.length)
            return;

        try {
            await fetch('/api/datasets/import', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ records: pendingRecords }),
            });

            setPendingRecords([]);
            setOpen(false);
        } catch (error) {
            console.error('Fehler beim Anlegen der Datensätze:', error);
        }
    };

    const handleOpenChange = (nextOpen: boolean) => {
        setOpen(nextOpen);
        if (!nextOpen) {
            setPendingRecords([]);
        }
    };

    return (
        <Dialog.Root open={open} onOpenChange={handleOpenChange}>
            <Dialog.Trigger>
                <IconButton>
                    <UploadIcon size="20"/>
                </IconButton>
            </Dialog.Trigger>

            <Dialog.Content maxWidth="720px">
                <Dialog.Title>CSV/XLSX importieren</Dialog.Title>
                <Dialog.Description size="2" mb="4">
                    <Text>Lade eine oder mehrere CSV/XLSX-Dateien hoch. Die Datensätze werden erst beim Klick auf „Datensätze anlegen“ erstellt.</Text>
                    <br/>
                    <Link href="">Template herunterladen</Link>
                </Dialog.Description>

                <Box mb="4">
                    <FileUploader
                        onRecordsCreated={handleRecordsCreated}
                        onParseError={handleParseError}
                    />
                </Box>

                <Flex justify="between" align="center" mt="3">
                    <Text size="1" color="gray">
                        {pendingRecords.length
                            ? `${pendingRecords.length} Datensätze bereit zum Anlegen`
                            : 'Noch keine Datensätze vorbereitet'}
                    </Text>

                    <Flex gap="2">
                        <Dialog.Close>
                            <Button variant="surface">
                                Abbrechen
                            </Button>
                        </Dialog.Close>

                        <Button
                            onClick={handleCreateDatasets}
                            disabled={pendingRecords.length === 0}
                        >
                            Datensätze anlegen
                        </Button>
                    </Flex>
                </Flex>
            </Dialog.Content>
        </Dialog.Root>
    );
}