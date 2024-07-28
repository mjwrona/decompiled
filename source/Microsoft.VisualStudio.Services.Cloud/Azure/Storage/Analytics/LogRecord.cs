// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Analytics.LogRecord
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Storage.Analytics
{
  public class LogRecord
  {
    internal LogRecord()
    {
    }

    internal LogRecord(LogRecordStreamReader reader)
    {
      ArgumentUtility.CheckForNull<LogRecordStreamReader>(reader, nameof (reader));
      this.VersionNumber = reader.ReadString();
      ArgumentUtility.CheckStringForNullOrEmpty(this.VersionNumber, nameof (VersionNumber));
      if (!string.Equals("1.0", this.VersionNumber, StringComparison.Ordinal))
        throw new ArgumentException("A storage log version of " + this.VersionNumber + " is unsupported.");
      this.PopulateVersion1Log(reader);
    }

    private void PopulateVersion1Log(LogRecordStreamReader reader)
    {
      this.RequestStartTime = reader.ReadDateTimeOffset("o");
      this.OperationType = reader.ReadString();
      this.RequestStatus = reader.ReadString();
      this.HttpStatusCode = reader.ReadString();
      this.EndToEndLatency = reader.ReadTimeSpanInMS();
      this.ServerLatency = reader.ReadTimeSpanInMS();
      this.AuthenticationType = reader.ReadString();
      this.RequesterAccountName = reader.ReadString();
      this.OwnerAccountName = reader.ReadString();
      this.ServiceType = reader.ReadString();
      this.RequestUrl = reader.ReadUri();
      this.RequestedObjectKey = reader.ReadQuotedString();
      this.RequestIdHeader = reader.ReadGuid();
      this.OperationCount = reader.ReadInt();
      this.RequesterIPAddress = reader.ReadString();
      this.RequestVersionHeader = reader.ReadString();
      this.RequestHeaderSize = reader.ReadLong();
      this.RequestPacketSize = reader.ReadLong();
      this.ResponseHeaderSize = reader.ReadLong();
      this.ResponsePacketSize = reader.ReadLong();
      this.RequestContentLength = reader.ReadLong();
      this.RequestMD5 = reader.ReadQuotedString();
      this.ServerMD5 = reader.ReadQuotedString();
      this.ETagIdentifier = reader.ReadQuotedString();
      this.LastModifiedTime = reader.ReadDateTimeOffset("dddd, dd-MMM-yy HH:mm:ss 'GMT'");
      this.ConditionsUsed = reader.ReadQuotedString();
      this.UserAgentHeader = reader.ReadQuotedString();
      this.ReferrerHeader = reader.ReadQuotedString();
      this.ClientRequestId = reader.ReadQuotedString();
      reader.EndCurrentRecord();
    }

    public string VersionNumber { get; internal set; }

    public DateTimeOffset? RequestStartTime { get; internal set; }

    public string OperationType { get; internal set; }

    public string RequestStatus { get; internal set; }

    public string HttpStatusCode { get; internal set; }

    public TimeSpan? EndToEndLatency { get; internal set; }

    public TimeSpan? ServerLatency { get; internal set; }

    public string AuthenticationType { get; internal set; }

    public string RequesterAccountName { get; internal set; }

    public string OwnerAccountName { get; internal set; }

    public string ServiceType { get; internal set; }

    public Uri RequestUrl { get; internal set; }

    public string RequestedObjectKey { get; internal set; }

    public Guid? RequestIdHeader { get; internal set; }

    public int? OperationCount { get; internal set; }

    public string RequesterIPAddress { get; internal set; }

    public string RequestVersionHeader { get; internal set; }

    public long? RequestHeaderSize { get; internal set; }

    public long? RequestPacketSize { get; internal set; }

    public long? ResponseHeaderSize { get; internal set; }

    public long? ResponsePacketSize { get; internal set; }

    public long? RequestContentLength { get; internal set; }

    public string RequestMD5 { get; internal set; }

    public string ServerMD5 { get; internal set; }

    public string ETagIdentifier { get; internal set; }

    public DateTimeOffset? LastModifiedTime { get; internal set; }

    public string ConditionsUsed { get; internal set; }

    public string UserAgentHeader { get; internal set; }

    public string ReferrerHeader { get; internal set; }

    public string ClientRequestId { get; internal set; }
  }
}
