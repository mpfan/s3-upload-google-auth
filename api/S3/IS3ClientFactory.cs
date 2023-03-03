using Amazon.S3;

namespace API.S3;

public interface IS3ClientFactory
{
    AmazonS3Client GetClient();
}