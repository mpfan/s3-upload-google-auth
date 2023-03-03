namespace API.Abstractions;

public interface IFileStorageService
{
    Task<Stream> GetFile(string bucketName, string key);

    Task PutFile(string bucketName, string key, Stream stream);
    Task<IEnumerable<string>> GetFileNames(string bucketName);
}