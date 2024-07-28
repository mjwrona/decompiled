// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ContainerResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Net;

namespace Microsoft.Azure.Cosmos
{
  public class ContainerResponse : Response<ContainerProperties>
  {
    protected ContainerResponse()
    {
    }

    internal ContainerResponse(
      HttpStatusCode httpStatusCode,
      Headers headers,
      ContainerProperties containerProperties,
      Container container,
      CosmosDiagnostics diagnostics,
      RequestMessage requestMessage)
    {
      this.StatusCode = httpStatusCode;
      this.Headers = headers;
      this.Resource = containerProperties;
      this.Container = container;
      this.Diagnostics = diagnostics;
      this.RequestMessage = requestMessage;
    }

    public virtual Container Container { get; private set; }

    public override Headers Headers { get; }

    public override ContainerProperties Resource { get; }

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

    internal override RequestMessage RequestMessage { get; }

    public static implicit operator Container(ContainerResponse response) => response.Container;
  }
}
