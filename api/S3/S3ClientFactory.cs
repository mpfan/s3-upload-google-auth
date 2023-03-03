using Amazon.S3;
using Microsoft.Extensions.Options;

namespace API.S3;

public class S3ClientFactory : IS3ClientFactory
    {
        private readonly S3ClientOptions _options;

        public S3ClientFactory(IOptions<S3ClientOptions> options)
        {
            _options = options.Value;
        }

        public AmazonS3Client GetClient() 
        {
            var config = new AmazonS3Config
            {
                ServiceURL = _options.ServiceUrl
            };

            return new AmazonS3Client(_options.AccessKey, _options.AccessSecret, config);
        }
    }