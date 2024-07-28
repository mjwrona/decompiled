// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.DedupStoreStorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class DedupStoreStorageId : 
    IStorageId,
    IEquatable<IStorageId>,
    IEquatable<
    #nullable disable
    DedupStoreStorageId>
  {
    public DedupIdentifier SuperRootId { get; }

    public DedupIdentifier ManifestId { get; }

    public DedupStoreStorageId(DedupIdentifier contentRootId, DedupIdentifier manifestId)
    {
      this.SuperRootId = contentRootId ?? throw new ArgumentNullException(nameof (contentRootId));
      this.ManifestId = manifestId ?? throw new ArgumentNullException(nameof (manifestId));
    }

    public string ValueString => this.NonLegacyValueString;

    public string NonLegacyValueString => string.Format("dedup:{0}:{1}", (object) this.SuperRootId.ValueString, (object) this.ManifestId);

    public bool IsCacheable => true;

    public bool IsLocal => true;

    public bool? RepresentsSingleFile => new bool?(false);

    public override string ToString() => "DedupStoreStorageId(" + this.ValueString + ")";

    public bool Equals(DedupStoreStorageId other)
    {
      if (other == null)
        return false;
      return this == other || this.ValueString.Equals(other.ValueString);
    }

    public bool Equals(IStorageId other) => this.Equals(other as DedupStoreStorageId);

    public override bool Equals(object other) => this.Equals(other as DedupStoreStorageId);

    public override int GetHashCode() => this.ValueString.GetHashCode();
  }
}
