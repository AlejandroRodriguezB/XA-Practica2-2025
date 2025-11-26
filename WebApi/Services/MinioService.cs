using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace WebApi.Services
{
    public class MinioService
    {
        private readonly ILogger<MinioService> _logger;
        private readonly MinioClient? _client;
        private readonly string _bucket = "files";
        public bool Enabled => _client != null;

        public MinioService(ILogger<MinioService> logger, IConfiguration cfg)
        {
            _logger = logger;

            var endpoint = cfg["MINIO:ENDPOINT"];
            var access = cfg["MINIO:ACCESSKEY"];
            var secret = cfg["MINIO:SECRETKEY"];

            // Si faltan configuraciones → MinIO desactivado
            if (string.IsNullOrEmpty(endpoint) ||
                string.IsNullOrEmpty(access) ||
                string.IsNullOrEmpty(secret))
            {
                _client = null;
                _logger.LogError("There's missing MinIO configuration values");
                return;
            }

            try
            {
                _client = (MinioClient)new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(access, secret)
                    .WithSSL(false)
                    .Build();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error initializing MinIO client");
                _client = null;
            }
        }

        public async Task TryEnsureBucketAsync()
        {
            if (!Enabled || _client == null) return;

            try
            {
                bool exists = await _client.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(_bucket));

                if (!exists)
                    await _client.MakeBucketAsync(
                        new MakeBucketArgs().WithBucket(_bucket));
            }
            catch
            {
                _logger.LogError("Error ensuring MinIO bucket exists");
            }
        }
    }
}
