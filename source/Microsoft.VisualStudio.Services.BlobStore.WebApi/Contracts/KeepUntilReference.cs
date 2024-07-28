// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.KeepUntilReference
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
  public class KeepUntilReference : Reference, IEquatable<KeepUntilReference>
  {
    [JsonConstructor]
    public KeepUntilReference()
    {
    }

    public KeepUntilReference(DateTime date, BlobIdentifier b)
      : this(new KeepUntilBlobReference(date), b)
    {
    }

    public KeepUntilReference(
      KeepUntilBlobReference reference,
      BlobIdentifier blobId,
      bool? isMissing = null)
      : this(reference, new Blob(blobId), isMissing)
    {
    }

    public KeepUntilReference(
      KeepUntilBlobReference reference,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobId,
      bool? isMissing = null)
      : this(reference, new Blob(blobId), isMissing)
    {
    }

    public KeepUntilReference(KeepUntilBlobReference reference, Blob blob, bool? isMissing = null)
      : base(blob, isMissing)
    {
      this.KeepUntil = reference.KeepUntil;
    }

    public override ReferenceKind Kind => ReferenceKind.KeepUntilReference;

    [JsonProperty(PropertyName = "keepUntil", Required = Required.Always)]
    [JsonConverter(typeof (KeepUntilDateTimeConverter))]
    public DateTime KeepUntil { get; set; }

    public override BlobReference BlobReference => new BlobReference(this.KeepUntilBlobReference);

    public KeepUntilBlobReference KeepUntilBlobReference => new KeepUntilBlobReference(this.KeepUntil);

    public override bool Equals(Reference other) => this.Equals(other as KeepUntilReference);

    public bool Equals(KeepUntilReference other) => other != null && this.ReferenceEquals((Reference) other) && this.KeepUntil == other.KeepUntil;

    public override int GetHashCode() => Microsoft.VisualStudio.Services.Content.Common.EqualityHelper.GetCombinedHashCode((object) this.ReferenceGetHashCode(), (object) this.KeepUntil);
  }
}
