// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.IdReference
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [JsonObject(MemberSerialization.OptIn)]
  public class IdReference : Reference, IEquatable<IdReference>
  {
    public IdReference()
    {
    }

    public IdReference(
      IdBlobReference blobReference,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobId,
      bool? isMissing = null)
      : this(blobReference, new Blob(blobId), isMissing)
    {
    }

    public IdReference(IdBlobReference blobReference, BlobIdentifier blobId, bool? isMissing = null)
      : this(blobReference, new Blob(blobId), isMissing)
    {
    }

    public IdReference(IdBlobReference blobReference, Blob blob, bool? isMissing = null)
      : base(blob, isMissing)
    {
      this.Id = blobReference.Name;
      this.Scope = blobReference.Scope;
    }

    public override ReferenceKind Kind => ReferenceKind.IdReference;

    [JsonProperty(PropertyName = "id", Required = Required.Always)]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "scope", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string Scope { get; set; }

    public override BlobReference BlobReference => new BlobReference(this.Id, this.Scope);

    public override bool Equals(Reference other) => this.Equals(other as IdReference);

    public bool Equals(IdReference other) => other != null && this.ReferenceEquals((Reference) other) && this.Id == other.Id && this.Scope == other.Scope;

    public override int GetHashCode() => Microsoft.VisualStudio.Services.Content.Common.EqualityHelper.GetCombinedHashCode((object) this.ReferenceGetHashCode(), (object) this.Id, (object) this.Scope);
  }
}
