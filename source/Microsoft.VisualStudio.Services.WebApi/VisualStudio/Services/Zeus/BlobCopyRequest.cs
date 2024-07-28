// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Zeus.BlobCopyRequest
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Zeus
{
  [DataContract]
  public class BlobCopyRequest
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RequestId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid JobId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SourceStorageAccount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TargetStorageAccount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Containers { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ContainersCopied { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BlobCopyRequestStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StatusMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? QueuedTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? EndTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool CopyOnlyGuids { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BlobCopyRequest\r\n[\r\n    RequestId:                {0}\r\n    JobId:                 {1}    \r\n    Containers:            {2}\r\n    ContainersCopied:      {3}\r\n    QueuedTime:            {4}\r\n    StartTime:             {5}\r\n    EndTime:               {6}\r\n    Status:                {7}\r\n    StatusMessage:         {8}\r\n    CopyOnlyGuids:         {9}\r\n]", (object) this.RequestId, (object) this.JobId, (object) this.Containers, (object) this.ContainersCopied, (object) this.QueuedTime, (object) this.StartTime, (object) this.EndTime, (object) this.Status, (object) this.StatusMessage, (object) this.CopyOnlyGuids);
  }
}
