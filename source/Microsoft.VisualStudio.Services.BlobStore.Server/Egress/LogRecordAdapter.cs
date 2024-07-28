// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.LogRecordAdapter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage.Analytics;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public class LogRecordAdapter
  {
    public LogRecordAdapter()
    {
    }

    public LogRecordAdapter(LogRecord logRecord)
    {
      this.VersionNumber = logRecord.VersionNumber;
      this.ResponsePacketSize = logRecord.ResponsePacketSize;
      this.RequesterIPAddress = logRecord.RequesterIPAddress;
      this.RequestedObjectKey = logRecord.RequestedObjectKey;
      this.RequestStatus = logRecord.RequestStatus;
      this.OperationType = logRecord.OperationType;
    }

    public virtual string VersionNumber { get; }

    public virtual long? ResponsePacketSize { get; }

    public virtual string RequesterIPAddress { get; }

    public virtual string RequestedObjectKey { get; }

    public virtual string RequestStatus { get; }

    public virtual string OperationType { get; }
  }
}
