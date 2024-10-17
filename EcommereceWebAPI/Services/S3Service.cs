using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace EcommereceWebAPI.Services
{

    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            _s3Client = new AmazonS3Client(
                configuration["AWS:AccessKeyId"],
                configuration["AWS:SecretAccessKey"],
                Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
            );
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<string> GenerateUploadURLAsync()
        {
            var imageName = Guid.NewGuid().ToString(); 
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = imageName,
                Expires = DateTime.UtcNow.AddMinutes(10), 
                Verb = HttpVerb.PUT
            };

            string url = _s3Client.GetPreSignedURL(request);
            return url;
        }
    }

}
