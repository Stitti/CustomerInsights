type UploadStatus = 'uploading' | 'success' | 'error';

interface UploadItem {
    id: string;
    file: File;
    name: string;
    sizeLabel: string;
    status: UploadStatus;
    progress: number;
}