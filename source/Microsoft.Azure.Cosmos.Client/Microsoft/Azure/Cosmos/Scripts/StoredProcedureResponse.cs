// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Scripts.StoredProcedureResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Net;

namespace Microsoft.Azure.Cosmos.Scripts
{
  public class StoredProcedureResponse : Response<StoredProcedureProperties>
  {
    protected StoredProcedureResponse()
    {
    }

    internal StoredProcedureResponse(
      HttpStatusCode httpStatusCode,
      Headers headers,
      StoredProcedureProperties storedProcedureProperties,
      CosmosDiagnostics diagnostics,
      RequestMessage requestMessage)
    {
      this.StatusCode = httpStatusCode;
      this.Headers = headers;
      this.Resource = storedProcedureProperties;
      this.Diagnostics = diagnostics;
      this.RequestMessage = requestMessage;
    }

    public override Headers Headers { get; }

    public override StoredProcedureProperties Resource { get; }

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

    public virtual string SessionToken => this.Headers?.GetHeaderValue<string>("x-ms-session-token");

    internal override RequestMessage RequestMessage { get; }

    public static implicit operator StoredProcedureProperties(StoredProcedureResponse response) => response.Resource;
  }
}
