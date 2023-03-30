// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsS3Options.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Mapping profile for common DTO items
// </summary>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Cryptography;
using Teta.Packages.Files.S3;
using Teta.Packages.Files.S3.Interface;

namespace Teta.Packages.Files.Tests
{
    public class S3ServiceTests
    {
        private IServiceCollection _sc;
        private IServiceProvider _sp;
        private IConfiguration _config;

        private string _testBucketName = "t";
        private string _folderToUpload = "somefolder2";

        private string _fstFileName = "asp.net-core-logo.png";

        private string _testObjectKey = "";
        private string _testObjectVersion = "";

        [OneTimeSetUp]
        public void Setup()
        {
            _sc = new ServiceCollection();

            _sc.RegisterS3FilesStorage(Configuration);

            _sc.AddSingleton<IConfiguration>(Configuration);
            _sp = _sc.BuildServiceProvider();

            var dtnow = DateTime.Now;
            _testBucketName = $"{_testBucketName}-{dtnow.ToString("yyyyddMM-HHmmss")}";
        }

        [Order(0)]
        [Test]
        public void CheckBucketExists()
        {
            var svc = _sp.GetService<IS3Service>();

            var res = svc.CheckBucketExistsAsync(_testBucketName).Result;
            Assert.IsFalse(res);
        }


        [Order(1)]
        [Test]
        public void CreteBucket()
        {
            var svc = _sp.GetService<IS3Service>();

            svc.CreateBucketWithVersioningAsync(_testBucketName).Wait();

            var res = svc.CheckBucketExistsAsync(_testBucketName).Result;
            Assert.True(res);
        }

        [Order(2)]
        [Test]
        public void CreateFolderTests()
        {
            var svc = _sp.GetService<IS3Service>();

            svc.CreateFolderAsync(_folderToUpload, _testBucketName).Wait();
        }

        [Order(3)]
        [Test]
        public async Task StoreFileTest()
        {
            var svc = _sp.GetService<IS3Service>();

            var fileToUpload = Assembly.GetExecutingAssembly().GetManifestResourceStream("Teta.Packages.Files.Tests.FilesToUpload.asp.net-core-logo.png");

            var resp = svc.StoreObjectAsync(fileToUpload, _fstFileName, _folderToUpload, _testBucketName).Result;
        }

        [Order(4)]
        [Test]
        public void StoreSameFileSecondTime()
        {
            StoreFileTest().Wait();
            StoreFileTest().Wait();
        }

        [Order(5)]
        [Test]
        public async Task StoreSameFileToAnotherFolderTest()
        {
            var svc = _sp.GetService<IS3Service>();

            var fileToUpload = Assembly.GetExecutingAssembly().GetManifestResourceStream("Teta.Packages.Files.Tests.FilesToUpload.asp.net-core-logo.png");

            var newFolder = $"{_folderToUpload}/secondfolder";
            var resp = svc.StoreObjectAsync(fileToUpload, _fstFileName, newFolder, _testBucketName).Result;

            _testObjectKey = resp.Key;
            _testObjectVersion = resp.VersionId;
        }

        [Order(6)]
        [Test]
        public void UpdateObjectMetdata()
        {
            var svc = _sp.GetService<IS3Service>();
            var objectMetdadata = svc.GetObjectMetadata(_testObjectKey, _testObjectVersion, _testBucketName).Result;

            Assert.That(objectMetdadata.Metadata.Count, Is.EqualTo(0));

            var md = new Dictionary<string, string>();

            var data = Guid.NewGuid().ToString();
            md.Add("somekey", data);
            var resp = svc.ReplaceObjectMetadataAsync(_testObjectKey, md, _testObjectVersion, _testBucketName).Result;

            objectMetdadata = svc.GetObjectMetadata(_testObjectKey, resp.VersionId, _testBucketName).Result;

            Assert.IsTrue(objectMetdadata.Metadata.ContainsKey("somekey"));
            var newData = objectMetdadata.Metadata["somekey"];
            Assert.That(newData, Is.EqualTo(data)); ;
        }

        [Order(6)]
        [Test]
        public void UpdateObjectMetdataNoVersion()
        {
            var svc = _sp.GetService<IS3Service>();
            var objectMetdadata = svc.GetObjectMetadata(_testObjectKey, bucketName: _testBucketName).Result;

            Assert.That(objectMetdadata.Metadata.Count, Is.EqualTo(0));

            var md = new Dictionary<string, string>();

            var data = Guid.NewGuid().ToString();
            md.Add("somekey", data);
            var resp = svc.ReplaceObjectMetadataAsync(_testObjectKey, md, bucketName: _testBucketName).Result;

            objectMetdadata = svc.GetObjectMetadata(_testObjectKey, bucketName: _testBucketName).Result;

            Assert.IsTrue(objectMetdadata.Metadata.ContainsKey("somekey"));
            var newData = objectMetdadata.Metadata["somekey"];
            Assert.That(newData, Is.EqualTo(data));
        }


        [Order(7)]
        [Test]
        public void DownloadloadFileTest()
        {
            var svc = _sp.GetService<IS3Service>();
            using var data = svc.GetObjectAsync(_testObjectKey, bucketName: _testBucketName).Result;

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Teta.Packages.Files.Tests.FilesToUpload.asp.net-core-logo.png");
            var originalMd5 = MD5.HashData(stream);

            var downloadedMd5 = MD5.HashData(data.ObjectDataStream);
            Assert.That(originalMd5, Is.EqualTo(downloadedMd5));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            var svc = _sp.GetService<IS3Service>();
        }

        public IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile($"./appsettings.Tests.json", optional: false);
                    _config = builder.Build();
                }

                return _config;
            }
        }
    }
}