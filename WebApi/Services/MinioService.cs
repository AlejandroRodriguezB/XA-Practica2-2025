using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Security.AccessControl;

namespace WebApi.Services
{
    public class MinioService
    {
        private readonly ILogger<MinioService> _logger;
        public readonly MinioClient? _client;
        private readonly string _bucket = "files";
        private bool _bucketEnsured = false;
        private const string ObjectName = "logo.png";

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
                _bucketEnsured = await _client.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(_bucket));

                if (!_bucketEnsured)
                    await _client.MakeBucketAsync(
                        new MakeBucketArgs().WithBucket(_bucket));
            }
            catch
            {
                _logger.LogError("Error ensuring MinIO bucket exists");
            }
        }

        public async Task PutObjectAsync(Stream? stream, IFormFile file)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("MinIO client is not initialized.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "Stream cannot be null.");
            }

            if (!_bucketEnsured)
            {
                await TryEnsureBucketAsync();
            }

            await _client.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(ObjectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(file.ContentType)
            );
        }

        public async Task<FileContentResult?> GetObjectAsync()
        {
            if (_client == null)
                return null;

            try
            {
                using var ms = new MemoryStream();

                await _client.GetObjectAsync(
                    new GetObjectArgs()
                        .WithBucket(_bucket)
                        .WithObject(ObjectName)
                        .WithCallbackStream(s => s.CopyTo(ms))
                );

                var data = ms.ToArray();

                return new FileContentResult(data, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting object {Object} from bucket {Bucket}", ObjectName, _bucket);
                return null;
            }
        }
    }
}
