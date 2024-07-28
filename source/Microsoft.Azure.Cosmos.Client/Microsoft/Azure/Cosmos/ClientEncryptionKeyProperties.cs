// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientEncryptionKeyProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos
{
  public class ClientEncryptionKeyProperties : IEquatable<ClientEncryptionKeyProperties>
  {
    public ClientEncryptionKeyProperties(
      string id,
      string encryptionAlgorithm,
      byte[] wrappedDataEncryptionKey,
      EncryptionKeyWrapMetadata encryptionKeyWrapMetadata)
    {
      this.Id = !string.IsNullOrEmpty(id) ? id : throw new ArgumentNullException(nameof (id));
      this.EncryptionAlgorithm = !string.IsNullOrEmpty(encryptionAlgorithm) ? encryptionAlgorithm : throw new ArgumentNullException(nameof (encryptionAlgorithm));
      this.WrappedDataEncryptionKey = wrappedDataEncryptionKey ?? throw new ArgumentNullException(nameof (wrappedDataEncryptionKey));
      this.EncryptionKeyWrapMetadata = encryptionKeyWrapMetadata ?? throw new ArgumentNullException(nameof (encryptionKeyWrapMetadata));
    }

    protected ClientEncryptionKeyProperties()
    {
    }

    internal ClientEncryptionKeyProperties(ClientEncryptionKeyProperties source)
    {
      this.CreatedTime = source.CreatedTime;
      this.ETag = source.ETag;
      this.Id = source.Id;
      this.EncryptionAlgorithm = source.EncryptionAlgorithm;
      this.EncryptionKeyWrapMetadata = new EncryptionKeyWrapMetadata(source.EncryptionKeyWrapMetadata);
      this.LastModified = source.LastModified;
      this.ResourceId = source.ResourceId;
      this.SelfLink = source.SelfLink;
      if (source.WrappedDataEncryptionKey != null)
      {
        this.WrappedDataEncryptionKey = new byte[source.WrappedDataEncryptionKey.Length];
        source.WrappedDataEncryptionKey.CopyTo((Array) this.WrappedDataEncryptionKey, 0);
      }
      this.AdditionalProperties = source.AdditionalProperties;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; internal set; }

    [JsonProperty(PropertyName = "encryptionAlgorithm", NullValueHandling = NullValueHandling.Ignore)]
    public string EncryptionAlgorithm { get; internal set; }

    [JsonProperty(PropertyName = "wrappedDataEncryptionKey", NullValueHandling = NullValueHandling.Ignore)]
    public byte[] WrappedDataEncryptionKey { get; internal set; }

    [JsonProperty(PropertyName = "keyWrapMetadata", NullValueHandling = NullValueHandling.Ignore)]
    public EncryptionKeyWrapMetadata EncryptionKeyWrapMetadata { get; internal set; }

    [JsonConverter(typeof (UnixDateTimeConverter))]
    [JsonProperty(PropertyName = "_cts", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? CreatedTime { get; internal set; }

    [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
    public string ETag { get; internal set; }

    [JsonConverter(typeof (UnixDateTimeConverter))]
    [JsonProperty(PropertyName = "_ts", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastModified { get; internal set; }

    [JsonProperty(PropertyName = "_self", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string SelfLink { get; internal set; }

    [JsonProperty(PropertyName = "_rid", NullValueHandling = NullValueHandling.Ignore)]
    internal string ResourceId { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as ClientEncryptionKeyProperties);

    public bool Equals(ClientEncryptionKeyProperties other)
    {
      if (other != null && this.Id == other.Id && this.EncryptionAlgorithm == other.EncryptionAlgorithm && ClientEncryptionKeyProperties.Equals(this.WrappedDataEncryptionKey, other.WrappedDataEncryptionKey) && EqualityComparer<EncryptionKeyWrapMetadata>.Default.Equals(this.EncryptionKeyWrapMetadata, other.EncryptionKeyWrapMetadata) && this.AdditionalProperties.EqualsTo(other.AdditionalProperties))
      {
        DateTime? createdTime = this.CreatedTime;
        DateTime? nullable = other.CreatedTime;
        if ((createdTime.HasValue == nullable.HasValue ? (createdTime.HasValue ? (createdTime.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.ETag == other.ETag)
        {
          nullable = this.LastModified;
          DateTime? lastModified = other.LastModified;
          if ((nullable.HasValue == lastModified.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == lastModified.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.SelfLink == other.SelfLink)
            return this.ResourceId == other.ResourceId;
        }
      }
      return false;
    }

    public override int GetHashCode() => (((((((-1673632966 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.EncryptionAlgorithm)) * -1521134295 + EqualityComparer<EncryptionKeyWrapMetadata>.Default.GetHashCode(this.EncryptionKeyWrapMetadata)) * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(this.CreatedTime)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ETag)) * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(this.LastModified)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.SelfLink)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ResourceId);

    private static bool Equals(byte[] x, byte[] y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && ((IEnumerable<byte>) x).SequenceEqual<byte>((IEnumerable<byte>) y);
    }
  }
}
