# s3-upload-google-auth
This is a solution to a challenge that involves implementing an application that allows an authenticated user to upload, list, and download files from Amazon S3. This solution uses AmazonS3Client in the api but the storage service used is minio which is S3 compatible.

To run the application create a .env with the following environment variables:
```
DOTNET_CLIENT_ID=<value required>
DOTNET_CLIENT_SECRET=<value required>
DOTNET_CALLBACK_PATH=<value required>
DOTNET_CLIENT_URL=<value required>
DOTNET_MINIO_SERVICE_URL=<value required>
DOTNET_MINIO_BUCKET_NAME=<value required>
DOTNET_MINIO_ACCESS_KEY=<value required>
DOTNET_MINIO_SECRET_KEY=<value required>
MINIO_ROOT_USER=<value required>
MINIO_ROOT_PASSWORD=<value required>
```

The bucket and access key are created by going to the [minio admin page](http://localhost:9001)

To run the application:

`docker compose up`

and navigate to http://localhost:3000