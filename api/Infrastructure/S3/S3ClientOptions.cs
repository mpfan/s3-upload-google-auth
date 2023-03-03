namespace API.Infrastructure.S3
{
    public class S3ClientOptions
    {
        public string ServiceUrl { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
    }
}