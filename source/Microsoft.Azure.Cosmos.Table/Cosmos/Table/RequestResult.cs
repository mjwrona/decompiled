// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RequestResult
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class RequestResult
  {
    private volatile Exception exception;

    public int HttpStatusCode { get; set; }

    public string HttpStatusMessage { get; internal set; }

    public string ServiceRequestID { get; internal set; }

    public string ContentMd5 { get; internal set; }

    public string Etag { get; internal set; }

    public string RequestDate { get; internal set; }

    public StorageLocation TargetLocation { get; internal set; }

    public StorageExtendedErrorInformation ExtendedErrorInformation { get; internal set; }

    public string ErrorCode { get; internal set; }

    public bool IsRequestServerEncrypted { get; internal set; }

    public Exception Exception
    {
      get => this.exception;
      set => this.exception = value;
    }

    public DateTimeOffset StartTime { get; internal set; }

    public DateTimeOffset EndTime { get; internal set; }

    public double? RequestCharge { get; internal set; }
  }
}
