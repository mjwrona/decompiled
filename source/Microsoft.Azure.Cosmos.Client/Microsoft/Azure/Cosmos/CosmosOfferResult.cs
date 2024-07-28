// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosOfferResult
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosOfferResult
  {
    public CosmosOfferResult(int? throughput)
    {
      this.Throughput = throughput;
      this.StatusCode = throughput.HasValue ? HttpStatusCode.OK : HttpStatusCode.NotFound;
    }

    public CosmosOfferResult(HttpStatusCode statusCode, CosmosException cosmosRequestException)
    {
      this.StatusCode = statusCode;
      this.CosmosException = cosmosRequestException;
    }

    public CosmosException CosmosException { get; }

    public HttpStatusCode StatusCode { get; }

    public int? Throughput { get; }
  }
}
