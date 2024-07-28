// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceAuditEntry
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ReferenceAuditEntry
  {
    public Guid HostId { get; set; }

    public BlobIdentifier BlobIdentifier { get; set; }

    public string DomainId { get; set; }

    public IdBlobReference Reference { get; set; }

    public DateTimeOffset ReferenceTimestamp { get; set; }

    public override int GetHashCode() => EqualityHelper.GetCombinedHashCode((object) this.BlobIdentifier, (object) this.DomainId, (object) this.Reference, (object) this.HostId, (object) this.ReferenceTimestamp);

    public override bool Equals(object obj) => obj is ReferenceAuditEntry referenceAuditEntry && object.Equals((object) this.BlobIdentifier, (object) referenceAuditEntry.BlobIdentifier) && object.Equals((object) this.DomainId, (object) referenceAuditEntry.DomainId) && object.Equals((object) this.Reference, (object) referenceAuditEntry.Reference) && object.Equals((object) this.HostId, (object) referenceAuditEntry.HostId) && object.Equals((object) this.ReferenceTimestamp, (object) referenceAuditEntry.ReferenceTimestamp);
  }
}
