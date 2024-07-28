// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.PointOperationStatisticsTraceDatum
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal sealed class PointOperationStatisticsTraceDatum : TraceDatum
  {
    public PointOperationStatisticsTraceDatum(
      string activityId,
      HttpStatusCode statusCode,
      SubStatusCodes subStatusCode,
      DateTime responseTimeUtc,
      double requestCharge,
      string errorMessage,
      HttpMethod method,
      string requestUri,
      string requestSessionToken,
      string responseSessionToken,
      string beLatencyInMs)
    {
      this.ActivityId = activityId;
      this.StatusCode = statusCode;
      this.SubStatusCode = subStatusCode;
      this.RequestCharge = requestCharge;
      this.ErrorMessage = errorMessage;
      this.Method = method;
      this.RequestUri = requestUri;
      this.RequestSessionToken = requestSessionToken;
      this.ResponseSessionToken = responseSessionToken;
      this.ResponseTimeUtc = responseTimeUtc;
      this.BELatencyInMs = beLatencyInMs;
    }

    public string ActivityId { get; }

    public HttpStatusCode StatusCode { get; }

    public SubStatusCodes SubStatusCode { get; }

    public DateTime ResponseTimeUtc { get; }

    public double RequestCharge { get; }

    public string ErrorMessage { get; }

    public HttpMethod Method { get; }

    public string RequestUri { get; }

    public string RequestSessionToken { get; }

    public string ResponseSessionToken { get; }

    public string BELatencyInMs { get; }

    internal override void Accept(ITraceDatumVisitor traceDatumVisitor) => traceDatumVisitor.Visit(this);
  }
}
