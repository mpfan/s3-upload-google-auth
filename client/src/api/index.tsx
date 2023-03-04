export async function getFiles(): Promise<string[]> {
    const res = await fetch("/api/files", {
        method: "GET",
        credentials: 'include'
    });

    if(res.status !== 200) {
        throw new Error(`Request failed with code: ${res.status}`);
    }

    const files = await res.json();

    return files;
}

export async function uploadFile(file: File): Promise<string> {
    if (!file) {
        throw new Error("Invalid file");
    }

    const formData = new FormData();
    formData.append("file", file);

    const res = await fetch("/api/files", {
        method: "POST",
        body: formData,
        credentials: "include"
    });

    if(res.status !== 200) {
        throw new Error(`Request failed with code: ${res.status}`);
    }

    const uploadedFile = await res.json();

    return uploadedFile;
}