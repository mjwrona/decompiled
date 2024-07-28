// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Reference
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [JsonObject(MemberSerialization.OptIn, ItemConverterType = typeof (JsonReferenceConverter))]
  public abstract class Reference : IEquatable<Reference>
  {
    public Reference()
    {
    }

    public Reference(BlobIdentifier blobId, bool? isMissing = null)
      : this(new Blob(blobId), isMissing)
    {
    }

    public Reference(Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobId, bool? isMissing = null)
      : this(new Blob(blobId), isMissing)
    {
    }

    public Reference(Blob blob, bool? isMissing = null)
    {
      if (isMissing.HasValue)
        this.Status = isMissing.Value ? "missing" : "added";
      this.Blob = blob;
    }

    public abstract ReferenceKind Kind { get; }

    [JsonProperty(PropertyName = "blob", Required = Required.Always)]
    public Blob Blob { get; set; }

    [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
    public string Status { get; set; }

    public abstract BlobReference BlobReference { get; }

    public override bool Equals(object other) => this.Equals(other as Reference);

    public abstract bool Equals(Reference other);

    public bool ReferenceEquals(Reference other) => (object) other != null && string.Equals(this.Status, other.Status) && this.Blob == other.Blob;

    public abstract override int GetHashCode();

    public int ReferenceGetHashCode() => Microsoft.VisualStudio.Services.Content.Common.EqualityHelper.GetCombinedHashCode((object) this.Status, (object) this.Blob);

    public static bool operator ==(Reference r1, Reference r2) => (object) r1 == null ? (object) r2 == null : r1.Equals(r2);

    public static bool operator !=(Reference r1, Reference r2) => !(r1 == r2);
  }
}
