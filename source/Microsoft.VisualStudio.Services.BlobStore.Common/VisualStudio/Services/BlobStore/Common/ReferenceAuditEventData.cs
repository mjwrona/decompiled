// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ReferenceAuditEventData
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [DataContract]
  [Serializable]
  public class ReferenceAuditEventData
  {
    private const int SerializationOverhead = 64;
    [IgnoreDataMember]
    public const string EventType = "Microsoft.VisualStudio.Services.BlobStore.ReferenceAuditEntry";

    [IgnoreDataMember]
    public int EstimatedSerializedSize => this.BlobId.Length + this.ReferenceId.Length + this.ReferenceTimestamp.Length + 64;

    [DataMember(Name = "blobId")]
    public string BlobId { get; set; }

    [DataMember(Name = "domainId")]
    public string DomainId { get; set; }

    [DataMember(Name = "refTimestamp")]
    public string ReferenceTimestamp { get; set; }

    [DataMember(Name = "referenceId")]
    public string ReferenceId { get; set; }
  }
}
