// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryClient.QueryResponseCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryClient
{
  internal readonly struct QueryResponseCore
  {
    private static readonly IReadOnlyList<CosmosElement> EmptyList = (IReadOnlyList<CosmosElement>) new List<CosmosElement>().AsReadOnly();
    internal static readonly string EmptyGuidString = Guid.Empty.ToString();

    private QueryResponseCore(
      IReadOnlyList<CosmosElement> result,
      bool isSuccess,
      HttpStatusCode statusCode,
      double requestCharge,
      string activityId,
      long responseLengthBytes,
      string disallowContinuationTokenMessage,
      string continuationToken,
      CosmosException cosmosException,
      SubStatusCodes? subStatusCode,
      CosmosQueryExecutionInfo cosmosQueryExecutionInfo = null)
    {
      this.IsSuccess = isSuccess;
      this.CosmosElements = result;
      this.StatusCode = statusCode;
      this.ActivityId = activityId;
      this.ResponseLengthBytes = responseLengthBytes;
      this.RequestCharge = requestCharge;
      this.DisallowContinuationTokenMessage = disallowContinuationTokenMessage;
      this.ContinuationToken = continuationToken;
      this.CosmosException = cosmosException;
      this.SubStatusCode = subStatusCode;
      this.CosmosQueryExecutionInfo = cosmosQueryExecutionInfo;
    }

    internal IReadOnlyList<CosmosElement> CosmosElements { get; }

    internal CosmosException CosmosException { get; }

    internal SubStatusCodes? SubStatusCode { get; }

    internal HttpStatusCode StatusCode { get; }

    internal string DisallowContinuationTokenMessage { get; }

    internal string ContinuationToken { get; }

    internal double RequestCharge { get; }

    internal string ActivityId { get; }

    internal long ResponseLengthBytes { get; }

    internal CosmosQueryExecutionInfo CosmosQueryExecutionInfo { get; }

    internal bool IsSuccess { get; }

    internal static QueryResponseCore CreateSuccess(
      IReadOnlyList<CosmosElement> result,
      double requestCharge,
      string activityId,
      long responseLengthBytes,
      string disallowContinuationTokenMessage,
      string continuationToken,
      CosmosQueryExecutionInfo cosmosQueryExecutionInfo = null)
    {
      return new QueryResponseCore(result, true, HttpStatusCode.OK, requestCharge, activityId, responseLengthBytes, disallowContinuationTokenMessage, continuationToken, (CosmosException) null, new SubStatusCodes?(), cosmosQueryExecutionInfo);
    }

    internal static QueryResponseCore CreateFailure(
      HttpStatusCode statusCode,
      SubStatusCodes? subStatusCodes,
      CosmosException cosmosException,
      double requestCharge,
      string activityId)
    {
      return new QueryResponseCore(QueryResponseCore.EmptyList, false, statusCode, requestCharge, activityId, 0L, (string) null, (string) null, cosmosException, subStatusCodes);
    }
  }
}
