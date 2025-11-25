using Minio;
using Minio.DataModel.Args;

namespace WebApi.Services
{
    public class MinioService
    {
        private readonly MinioClient? _client;
        private readonly string _bucket = "files";
        public bool Enabled => _client != null;

        public MinioService(IConfiguration cfg)
        {
            var endpoint = cfg["MINIO__ENDPOINT"];
            var access = cfg["MINIO__ACCESSKEY"];
            var secret = cfg["MINIO__SECRETKEY"];

            // Si faltan configuraciones → MinIO desactivado
            if (string.IsNullOrEmpty(endpoint) ||
                string.IsNullOrEmpty(access) ||
                string.IsNullOrEmpty(secret))
            {
                _client = null;
                return;
            }

            try
            {
                _client = (MinioClient?)new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(access, secret)
                    .WithSSL(false)
                    .Build();
            }
            catch
            {
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
            }
        }
    }
}
