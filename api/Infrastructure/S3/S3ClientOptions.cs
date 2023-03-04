namespace API.Infrastructure.S3
{
    public class S3ClientOptions
    {
        public string ServiceUrl { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string AccessSecret { get; set; } = string.Empty;
    }
}