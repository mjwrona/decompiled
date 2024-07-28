// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [DataContract]
  public class Blob : IEquatable<Blob>
  {
    public Blob()
    {
    }

    public Blob(string blobId) => this.Id = blobId;

    public Blob(BlobIdentifier blobId)
      : this(blobId.ValueString)
    {
    }

    public Blob(Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdWithBlocks)
      : this(blobIdWithBlocks.BlobId)
    {
      this.BlockHashes = blobIdWithBlocks.BlockHashes.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>((Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>) (blockHash => blockHash.HashString)).ToList<string>();
    }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "url")]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "blockHashes")]
    public List<string> BlockHashes { get; set; }

    public BlobIdentifier ToBlobIdentifier() => BlobIdentifier.Deserialize(this.Id);

    public Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks ToBlobIdentifierWithBlocks()
    {
      if (!this.HasBlockHashes())
        throw new ArgumentException("This blob has no block hashes.");
      return new Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks(BlobIdentifier.Deserialize(this.Id), this.BlockHashes.Select<string, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>((Func<string, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) (bh => new Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash(bh))));
    }

    public bool HasBlockHashes() => this.BlockHashes != null && this.BlockHashes.Any<string>();

    public bool Equals(Blob other)
    {
      if ((object) other == null || !(this.Id == other.Id) || this.HasBlockHashes() != other.HasBlockHashes())
        return false;
      return !this.HasBlockHashes() || this.BlockHashes == other.BlockHashes || this.BlockHashes.SequenceEqual<string>((IEnumerable<string>) other.BlockHashes);
    }

    public override bool Equals(object other) => this.Equals(other as Blob);

    public static bool operator ==(Blob r1, Blob r2) => (object) r1 == null ? (object) r2 == null : r1.Equals(r2);

    public static bool operator !=(Blob r1, Blob r2) => !(r1 == r2);

    public override int GetHashCode()
    {
      string id = this.Id;
      int hashCode = id != null ? id.GetHashCode() : 23;
      foreach (string str in (IEnumerable<string>) this.BlockHashes ?? Enumerable.Empty<string>())
        hashCode = hashCode * 397 ^ str.GetHashCode();
      return hashCode;
    }

    public override string ToString() => JsonSerializer.Serialize<Blob>(this);
  }
}
