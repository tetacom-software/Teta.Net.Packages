using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Teta.Packages.Files.Contracts.Internal.S3
{
    /// <summary>
    /// Contains information abount S3 object metadata
    /// </summary>
    public class S3ObjectMetadataResponse : S3ObjectMainProperties, IDictionary<string, string>
    {
        /// <summary>
        /// Sha256
        /// </summary>
        public static string TetacomSha256Header => "x-tetacom-meta-checksumm-sha256";

        public S3ObjectMetadataResponse([NotNull] IDictionary<string, string> metadata)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public string ChecksumSHA256Base64
        {
            get
            {
                if (Metadata.TryGetValue(TetacomSha256Header, out var sha256))
                {
                    return sha256;
                }

                return sha256;
            }
        }

        public IDictionary<string, string> Metadata { get; protected set; }

        public string this[string key] { get => Metadata[key]; set => Metadata[key] = value; }

        public ICollection<string> Keys => Metadata.Keys;

        public ICollection<string> Values => Metadata.Values;

        public int Count => Metadata.Count;

        public bool IsReadOnly => Metadata.IsReadOnly;

        public void Add(string key, string value)
        {
            Metadata.Add(key, value);
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Metadata.Add(item);
        }

        public void Clear()
        {
            Metadata.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return Metadata.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return Metadata.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            Metadata.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Metadata.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return Metadata.Remove(key);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Metadata.Remove(item);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            return Metadata.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Metadata.GetEnumerator();
        }
    }
}
