// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.OptimisticDirectExecutionQuery.OptimisticDirectExecutionContinuationToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.OptimisticDirectExecutionQuery
{
  internal sealed class OptimisticDirectExecutionContinuationToken : IPartitionedToken
  {
    private const string OptimisticDirectExecutionToken = "OptimisticDirectExecutionToken";

    public OptimisticDirectExecutionContinuationToken(ParallelContinuationToken token) => this.Token = token;

    public ParallelContinuationToken Token { get; }

    public Microsoft.Azure.Documents.Routing.Range<string> Range => this.Token.Range;

    public static CosmosElement ToCosmosElement(
      OptimisticDirectExecutionContinuationToken continuationToken)
    {
      CosmosElement cosmosElement = ParallelContinuationToken.ToCosmosElement(continuationToken.Token);
      return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        ["OptimisticDirectExecutionToken"] = cosmosElement
      });
    }

    public static TryCatch<OptimisticDirectExecutionContinuationToken> TryCreateFromCosmosElement(
      CosmosElement cosmosElement)
    {
      CosmosObject cosmosObject = cosmosElement as CosmosObject;
      if ((CosmosElement) cosmosObject == (CosmosElement) null)
        return TryCatch<OptimisticDirectExecutionContinuationToken>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Malformed Continuation Token"));
      TryCatch<ParallelContinuationToken> fromCosmosElement = ParallelContinuationToken.TryCreateFromCosmosElement(cosmosObject["OptimisticDirectExecutionToken"]);
      return !fromCosmosElement.Succeeded ? TryCatch<OptimisticDirectExecutionContinuationToken>.FromException(fromCosmosElement.Exception) : TryCatch<OptimisticDirectExecutionContinuationToken>.FromResult(new OptimisticDirectExecutionContinuationToken(fromCosmosElement.Result));
    }
  }
}
