// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.BlobStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class BlobStorageId : IStorageId, IEquatable<IStorageId>, IEquatable<
  #nullable disable
  BlobStorageId>
  {
    public BlobStorageId(BlobIdentifier blobId) => this.BlobId = !(blobId == (BlobIdentifier) null) ? blobId : throw new ArgumentNullException(nameof (blobId));

    public BlobIdentifier BlobId { get; }

    public string ValueString => this.BlobId.ValueString;

    public string NonLegacyValueString => "blob:" + this.BlobId.ValueString;

    public bool IsCacheable => true;

    public bool IsLocal => true;

    public bool? RepresentsSingleFile => new bool?(true);

    public override string ToString() => "BlobStorageId(" + this.ValueString + ")";

    public bool Equals(BlobStorageId other)
    {
      if (other == null)
        return false;
      return this == other || this.BlobId.Equals(other.BlobId);
    }

    public bool Equals(IStorageId other) => this.Equals(other as BlobStorageId);

    public override bool Equals(object other) => this.Equals(other as BlobStorageId);

    public override int GetHashCode() => this.BlobId.GetHashCode();
  }
}
