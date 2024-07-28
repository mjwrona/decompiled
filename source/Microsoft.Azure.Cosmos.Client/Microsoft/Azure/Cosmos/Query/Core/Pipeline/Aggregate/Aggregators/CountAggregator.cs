// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators.CountAggregator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators
{
  internal sealed class CountAggregator : IAggregator
  {
    private long globalCount;

    private CountAggregator(long initialCount) => this.globalCount = initialCount >= 0L ? initialCount : throw new ArgumentOutOfRangeException(nameof (initialCount));

    public void Aggregate(CosmosElement localCount)
    {
      if (!(localCount is CosmosNumber cosmosNumber))
        throw new ArgumentException("localCount must be a number.");
      this.globalCount += Number64.ToLong(cosmosNumber.Value);
    }

    public CosmosElement GetResult() => (CosmosElement) CosmosNumber64.Create((Number64) this.globalCount);

    public string GetContinuationToken() => this.globalCount.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public static TryCatch<IAggregator> TryCreate(CosmosElement continuationToken)
    {
      long initialCount;
      if (continuationToken != (CosmosElement) null)
      {
        if (!(continuationToken is CosmosNumber cosmosNumber))
          return TryCatch<IAggregator>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid count continuation token: \"{0}\".", (object) continuationToken)));
        initialCount = Number64.ToLong(cosmosNumber.Value);
      }
      else
        initialCount = 0L;
      return TryCatch<IAggregator>.FromResult((IAggregator) new CountAggregator(initialCount));
    }

    public CosmosElement GetCosmosElementContinuationToken() => (CosmosElement) CosmosNumber64.Create((Number64) this.globalCount);
  }
}
