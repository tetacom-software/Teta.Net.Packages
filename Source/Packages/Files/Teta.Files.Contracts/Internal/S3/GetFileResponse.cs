using System;
using System.Collections.Generic;
using System.IO;

namespace Teta.Packages.Files.Contracts.Internal.S3
{
    /// <summary>
    /// Get file response result
    /// </summary>
    public class GetS3ObjectResponse : S3ObjectMetadataResponse, IDisposable
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="metadata">Metadata collection</param>
        public GetS3ObjectResponse(IDictionary<string, string> metadata)
            : base(metadata)
        {
        }

        /// <summary>
        /// File data stream
        /// </summary>
        public Stream ObjectDataStream { get; set; }

        ~GetS3ObjectResponse()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            ObjectDataStream?.Dispose();
        }
    }
}
