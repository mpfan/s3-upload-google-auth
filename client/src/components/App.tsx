import React, { useEffect, useState } from "react";
import { getFiles, uploadFile } from "../api";

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [files, setFiles] = useState<string[]>([]);
  const [file, setFile] = useState<File>();

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { files } = e.target;

    if (!files) {
      return;
    }
    
    setFile(files[0]);
  }

  const handleUpload = (e: React.FormEvent<HTMLButtonElement>) => {
    if(!file) {
      return;
    }
    
    uploadFile(file).then(file => {
      setFiles([file, ...files]);
      setFile(undefined);
    });
  }

  useEffect(() => {
    getFiles().then(files => {
      setFiles(files);
      setIsAuthenticated(true);
    });
  }, []);

  return (
    <>
      <div className="header">
        <p className="header-title">s3-upload-google-auth</p>
      </div>
      <div className="app">
        {isAuthenticated ? (
          <div className="file-uploader-container">
            <input type="file" name="file" onChange={handleFileChange} />
            <button onClick={handleUpload}>Upload</button>
            <div className="file-uploader-files-container">
              {files.map((file, index) => {
                return (
                  <a key={index} href={`/api/files/${file}`} download>{file}</a>
                );

              })}
            </div>
          </div>
        ) : (<p>You are not currently logged in. <a href="/api/login">Log in here.</a></p>)}
      </div>
    </>
  );
}

export default App;
