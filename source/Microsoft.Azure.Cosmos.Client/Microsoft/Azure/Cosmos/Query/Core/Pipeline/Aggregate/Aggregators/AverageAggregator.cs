// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators.AverageAggregator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators
{
  internal sealed class AverageAggregator : IAggregator
  {
    private AverageAggregator.AverageInfo globalAverage;

    private AverageAggregator(AverageAggregator.AverageInfo globalAverage) => this.globalAverage = globalAverage;

    public void Aggregate(CosmosElement localAverage)
    {
      TryCatch<AverageAggregator.AverageInfo> fromCosmosElement = AverageAggregator.AverageInfo.TryCreateFromCosmosElement(localAverage);
      if (!fromCosmosElement.Succeeded)
        throw fromCosmosElement.Exception;
      this.globalAverage += fromCosmosElement.Result;
    }

    public CosmosElement GetResult() => this.globalAverage.GetAverage();

    public static TryCatch<IAggregator> TryCreate(CosmosElement continuationToken)
    {
      AverageAggregator.AverageInfo globalAverage;
      if (continuationToken != (CosmosElement) null)
      {
        TryCatch<AverageAggregator.AverageInfo> fromCosmosElement = AverageAggregator.AverageInfo.TryCreateFromCosmosElement(continuationToken);
        if (!fromCosmosElement.Succeeded)
          return TryCatch<IAggregator>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid continuation token: {0}", (object) continuationToken), fromCosmosElement.Exception));
        globalAverage = fromCosmosElement.Result;
      }
      else
        globalAverage = new AverageAggregator.AverageInfo(new double?(0.0), 0L);
      return TryCatch<IAggregator>.FromResult((IAggregator) new AverageAggregator(globalAverage));
    }

    public CosmosElement GetCosmosElementContinuationToken() => AverageAggregator.AverageInfo.ToCosmosElement(this.globalAverage);

    private readonly struct AverageInfo
    {
      private const string SumName = "sum";
      private const string CountName = "count";

      public AverageInfo(double? sum, long count)
      {
        this.Sum = sum;
        this.Count = count;
      }

      public static CosmosElement ToCosmosElement(AverageAggregator.AverageInfo averageInfo)
      {
        Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>();
        if (averageInfo.Sum.HasValue)
          dictionary.Add("sum", (CosmosElement) CosmosNumber64.Create((Number64) averageInfo.Sum.Value));
        dictionary.Add("count", (CosmosElement) CosmosNumber64.Create((Number64) averageInfo.Count));
        return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) dictionary);
      }

      public static TryCatch<AverageAggregator.AverageInfo> TryCreateFromCosmosElement(
        CosmosElement cosmosElement)
      {
        if (cosmosElement == (CosmosElement) null)
          throw new ArgumentNullException("cosmosElement must not be null.");
        if (!(cosmosElement is CosmosObject cosmosObject))
          return TryCatch<AverageAggregator.AverageInfo>.FromException((Exception) new ArgumentException("cosmosElement must be an object."));
        CosmosElement cosmosElement1;
        double? sum;
        if (cosmosObject.TryGetValue("sum", out cosmosElement1))
        {
          if (!(cosmosElement1 is CosmosNumber cosmosNumber))
            return TryCatch<AverageAggregator.AverageInfo>.FromException((Exception) new ArgumentException("value for the sum field was not a number"));
          sum = new double?(Number64.ToDouble(cosmosNumber.Value));
        }
        else
          sum = new double?();
        CosmosNumber typedCosmosElement;
        if (!cosmosObject.TryGetValue<CosmosNumber>("count", out typedCosmosElement))
          return TryCatch<AverageAggregator.AverageInfo>.FromException((Exception) new ArgumentException("value for the count field was not a number"));
        long count = Number64.ToLong(typedCosmosElement.Value);
        return TryCatch<AverageAggregator.AverageInfo>.FromResult(new AverageAggregator.AverageInfo(sum, count));
      }

      public double? Sum { get; }

      public long Count { get; }

      public static AverageAggregator.AverageInfo operator +(
        AverageAggregator.AverageInfo info1,
        AverageAggregator.AverageInfo info2)
      {
        if (!info1.Sum.HasValue || !info2.Sum.HasValue)
          return new AverageAggregator.AverageInfo(new double?(), info1.Count + info2.Count);
        double? sum1 = info1.Sum;
        double? sum2 = info2.Sum;
        return new AverageAggregator.AverageInfo(sum1.HasValue & sum2.HasValue ? new double?(sum1.GetValueOrDefault() + sum2.GetValueOrDefault()) : new double?(), info1.Count + info2.Count);
      }

      public CosmosElement GetAverage() => !this.Sum.HasValue || this.Count <= 0L ? (CosmosElement) CosmosUndefined.Create() : (CosmosElement) CosmosNumber64.Create((Number64) (this.Sum.Value / (double) this.Count));

      public override string ToString() => string.Format("{{\r\n                    {0}\r\n                    \"{1}\" : {2}\r\n                }}", this.Sum.HasValue ? (object) string.Format("\"{0}\" : {1},", (object) "sum", (object) this.Sum.Value) : (object) string.Empty, (object) "count", (object) this.Count);
    }
  }
}
