// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators.SumAggregator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators
{
  internal sealed class SumAggregator : IAggregator
  {
    private double globalSum;

    private SumAggregator(double globalSum) => this.globalSum = globalSum;

    public void Aggregate(CosmosElement localSum)
    {
      if (double.IsNaN(this.globalSum))
        return;
      switch (localSum)
      {
        case CosmosUndefined _:
          this.globalSum = double.NaN;
          break;
        case CosmosNumber cosmosNumber:
          this.globalSum += Number64.ToDouble(cosmosNumber.Value);
          break;
        default:
          throw new ArgumentException("localSum must be a number.");
      }
    }

    public CosmosElement GetResult() => double.IsNaN(this.globalSum) ? (CosmosElement) CosmosUndefined.Create() : (CosmosElement) CosmosNumber64.Create((Number64) this.globalSum);

    public CosmosElement GetCosmosElementContinuationToken() => (CosmosElement) CosmosNumber64.Create((Number64) this.globalSum);

    public static TryCatch<IAggregator> TryCreate(CosmosElement requestContinuationToken)
    {
      double result;
      if (requestContinuationToken != (CosmosElement) null)
      {
        switch (requestContinuationToken)
        {
          case CosmosNumber cosmosNumber:
            result = Number64.ToDouble(cosmosNumber.Value);
            break;
          case CosmosString cosmosString:
            if (!double.TryParse(UtfAnyString.op_Implicit(cosmosString.Value), NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) CultureInfo.InvariantCulture, out result))
              return TryCatch<IAggregator>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0} continuation token: {1}", (object) nameof (SumAggregator), (object) requestContinuationToken)));
            break;
          default:
            return TryCatch<IAggregator>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0} continuation token: {1}", (object) nameof (SumAggregator), (object) requestContinuationToken)));
        }
      }
      else
        result = 0.0;
      return TryCatch<IAggregator>.FromResult((IAggregator) new SumAggregator(result));
    }
  }
}
