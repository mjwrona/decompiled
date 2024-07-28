// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ThroughputResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Net;

namespace Microsoft.Azure.Cosmos
{
  public class ThroughputResponse : Response<ThroughputProperties>
  {
    protected ThroughputResponse()
    {
    }

    internal ThroughputResponse(
      HttpStatusCode httpStatusCode,
      Headers headers,
      ThroughputProperties throughputProperties,
      CosmosDiagnostics diagnostics,
      RequestMessage requestMessage)
    {
      this.StatusCode = httpStatusCode;
      this.Headers = headers;
      this.Resource = throughputProperties;
      this.Diagnostics = diagnostics;
      this.RequestMessage = requestMessage;
    }

    public override Headers Headers { get; }

    public override ThroughputProperties Resource { get; }

    public override HttpStatusCode StatusCode { get; }

    public override CosmosDiagnostics Diagnostics { get; }

    public override double RequestCharge
    {
      get
      {
        Headers headers = this.Headers;
        return headers == null ? 0.0 : headers.RequestCharge;
      }
    }

    public override string ActivityId => this.Headers?.ActivityId;

    public override string ETag => this.Headers?.ETag;

    public int? MinThroughput => this.Headers?.GetHeaderValue<string>("x-ms-cosmos-min-throughput") != null ? new int?(int.Parse(this.Headers.GetHeaderValue<string>("x-ms-cosmos-min-throughput"))) : new int?();

    public bool? IsReplacePending => this.Headers.GetHeaderValue<string>("x-ms-offer-replace-pending") != null ? new bool?(bool.Parse(this.Headers.GetHeaderValue<string>("x-ms-offer-replace-pending"))) : new bool?();

    internal override RequestMessage RequestMessage { get; }

    public static implicit operator ThroughputProperties(ThroughputResponse response) => response.Resource;
  }
}
