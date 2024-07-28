// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.Aggregation.AverageAggregator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents.Query.Aggregation
{
  internal sealed class AverageAggregator : IAggregator
  {
    private AverageAggregator.AverageInfo globalAverage = new AverageAggregator.AverageInfo(new double?(0.0), 0L);

    public void Aggregate(object localAverage)
    {
      // ISSUE: reference to a compiler-generated field
      if (AverageAggregator.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AverageAggregator.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, JObject>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (JObject), typeof (AverageAggregator)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.globalAverage += AverageAggregator.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) AverageAggregator.\u003C\u003Eo__1.\u003C\u003Ep__0, localAverage).ToObject<AverageAggregator.AverageInfo>();
    }

    public object GetResult() => this.globalAverage.GetAverage();

    private sealed class AverageInfo
    {
      public AverageInfo(double? sum, long count)
      {
        this.Sum = sum;
        this.Count = count;
      }

      [JsonProperty("sum")]
      public double? Sum { get; }

      [JsonProperty("count")]
      public long Count { get; }

      public static AverageAggregator.AverageInfo operator +(
        AverageAggregator.AverageInfo info1,
        AverageAggregator.AverageInfo info2)
      {
        if (info1 == null || info2 == null)
          return (AverageAggregator.AverageInfo) null;
        double? sum1 = info1.Sum;
        if (sum1.HasValue)
        {
          sum1 = info2.Sum;
          if (sum1.HasValue)
          {
            sum1 = info1.Sum;
            double? sum2 = info2.Sum;
            return new AverageAggregator.AverageInfo(sum1.HasValue & sum2.HasValue ? new double?(sum1.GetValueOrDefault() + sum2.GetValueOrDefault()) : new double?(), info1.Count + info2.Count);
          }
        }
        sum1 = new double?();
        return new AverageAggregator.AverageInfo(sum1, info1.Count + info2.Count);
      }

      public object GetAverage()
      {
        if (!this.Sum.HasValue || this.Count <= 0L)
          return (object) Undefined.Value;
        double? sum = this.Sum;
        double count = (double) this.Count;
        return (object) (sum.HasValue ? new double?(sum.GetValueOrDefault() / count) : new double?());
      }
    }
  }
}
