// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsClientException
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Net;

namespace Microsoft.Cloud.Metrics.Client
{
  [Serializable]
  public class MetricsClientException : Exception
  {
    public MetricsClientException(string message)
      : this(message, (Exception) null)
    {
    }

    public MetricsClientException(string message, Exception innerException)
      : this(message, innerException, Guid.Empty, new HttpStatusCode?())
    {
    }

    public MetricsClientException(
      string message,
      Exception innerException,
      Guid traceId,
      HttpStatusCode? responseStatusCode)
      : base(message, innerException)
    {
      this.TraceId = traceId;
      this.ResponseStatusCode = responseStatusCode;
    }

    public Guid TraceId { get; private set; }

    public HttpStatusCode? ResponseStatusCode { get; private set; }
  }
}
