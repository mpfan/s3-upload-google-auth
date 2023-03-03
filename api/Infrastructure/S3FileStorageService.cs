using Amazon.S3;
using Amazon.S3.Model;
using API.Abstractions;
using API.Infrastructure.S3;

namespace API.Infrastructure;

public class S3FileStorageService : IFileStorageService
{
    private readonly AmazonS3Client _client;

    public S3FileStorageService(IS3ClientFactory s3ClientFactory)
    {
        _client = s3ClientFactory.GetClient();
    }

    public async Task<Stream> GetFile(string bucketName, string key)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var getObjectResponse = await _client.GetObjectAsync(getObjectRequest);

        return getObjectResponse.ResponseStream;
    }

    public async Task<IEnumerable<string>> GetFileNames(string bucketName)
    {
        var files = new List<string>();

        var listObjectRequest = new ListObjectsRequest
        {
            BucketName = bucketName

        };

        ListObjectsResponse listObjectResponse;

        do
        {
            listObjectResponse = await _client.ListObjectsAsync(listObjectRequest);

            foreach (var obj in listObjectResponse.S3Objects)
            {
                files.Add(obj.Key);
            }

        } while (listObjectResponse.IsTruncated);

        return files;
    }

    public async Task PutFile(string bucketName, string key, Stream stream)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
        };

        await _client.PutObjectAsync(putObjectRequest);
    }
}