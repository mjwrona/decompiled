// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.StorageLogRecord
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage.Analytics;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public class StorageLogRecord
  {
    public StorageLogRecord()
    {
    }

    public StorageLogRecord(LogRecord logRecord)
    {
      this.VersionNumber = logRecord.VersionNumber;
      this.ResponsePacketSize = logRecord.ResponsePacketSize;
      this.RequesterIPAddress = logRecord.RequesterIPAddress;
      this.RequestedObjectKey = logRecord.RequestedObjectKey;
      this.RequestStatus = logRecord.RequestStatus;
      this.OperationType = logRecord.OperationType;
      this.ServerLatency = logRecord.ServerLatency;
      this.LastModifiedTime = logRecord.LastModifiedTime;
      this.OperationCount = logRecord.OperationCount;
      this.AuthenticationType = logRecord.AuthenticationType;
      this.EndToEndLatency = logRecord.EndToEndLatency;
      this.HttpStatusCode = logRecord.HttpStatusCode;
      this.ETagIdentifier = logRecord.ETagIdentifier;
      this.ConditionsUsed = logRecord.ConditionsUsed;
      this.ClientRequestId = logRecord.ClientRequestId;
      this.OwnerAccountName = logRecord.OwnerAccountName;
      this.ReferrerHeader = logRecord.ReferrerHeader;
      this.RequestContentLength = logRecord.RequestContentLength;
      this.RequestMD5 = logRecord.RequestMD5;
      this.ServerMD5 = logRecord.ServerMD5;
      this.RequestHeaderSize = logRecord.RequestHeaderSize;
      this.ResponseHeaderSize = logRecord.ResponseHeaderSize;
      this.RequestUrl = logRecord.RequestUrl.OriginalString;
      this.RequesterAccountName = logRecord.RequesterAccountName;
      this.RequestVersionHeader = logRecord.RequestVersionHeader;
      this.ServiceType = logRecord.ServiceType;
      this.OwnerAccountName = logRecord.OwnerAccountName;
      this.RequestIdHeader = logRecord.RequestIdHeader;
      this.RequestStartTime = logRecord.RequestStartTime;
      this.RequestPacketSize = logRecord.RequestPacketSize;
      this.UserAgentHeader = logRecord.UserAgentHeader;
    }

    public virtual string VersionNumber { get; }

    public virtual DateTimeOffset? RequestStartTime { get; }

    public virtual string OperationType { get; }

    public virtual string RequestStatus { get; }

    public virtual string HttpStatusCode { get; }

    public virtual TimeSpan? EndToEndLatency { get; }

    public virtual TimeSpan? ServerLatency { get; }

    public virtual string AuthenticationType { get; }

    public virtual string RequesterAccountName { get; }

    public virtual string OwnerAccountName { get; }

    public virtual string ServiceType { get; }

    public virtual string RequestUrl { get; }

    public virtual string RequestedObjectKey { get; }

    public virtual Guid? RequestIdHeader { get; }

    public virtual int? OperationCount { get; }

    public virtual string RequesterIPAddress { get; }

    public virtual string RequestVersionHeader { get; }

    public virtual long? RequestHeaderSize { get; }

    public virtual long? RequestPacketSize { get; }

    public virtual long? ResponseHeaderSize { get; }

    public virtual long? ResponsePacketSize { get; }

    public virtual long? RequestContentLength { get; }

    public virtual string RequestMD5 { get; }

    public virtual string ServerMD5 { get; }

    public virtual string ETagIdentifier { get; }

    public virtual DateTimeOffset? LastModifiedTime { get; }

    public virtual string ConditionsUsed { get; }

    public virtual string UserAgentHeader { get; }

    public virtual string ReferrerHeader { get; }

    public virtual string ClientRequestId { get; }

    public Guid GetHostIdFromRequestUrl()
    {
      Match match = this.RequestUrl != null ? new Regex("[db|b-]+\\w+\\/").Match(this.RequestUrl.ToString()) : throw new ArgumentException("Request url was null.");
      if (match.Success)
      {
        try
        {
          Guid result;
          if (Guid.TryParse(match.Value.Substring(2, 32), out result))
            return result;
        }
        catch
        {
        }
      }
      return Guid.Empty;
    }
  }
}
