// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.HistogramBucket`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  internal class HistogramBucket<T> where T : struct
  {
    private readonly double minBoundary;
    private readonly double maxBoundary;

    internal double MinBoundary => this.minBoundary;

    internal double MaxBoundary => this.maxBoundary;

    internal HistogramStatistics<T> Statistics { get; private set; }

    public HistogramBucket(
      double minBoundary,
      double maxBoundary,
      IMeter meter,
      HistogramConfiguration configuration)
    {
      this.minBoundary = minBoundary;
      this.maxBoundary = maxBoundary;
      this.Statistics = new HistogramStatistics<T>(meter, configuration);
    }

    internal bool IsCorrectBucket(T measurement)
    {
      int num1;
      if (this.MinBoundary != double.NegativeInfinity)
      {
        T a = measurement;
        // ISSUE: reference to a compiler-generated field
        if (HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (T), typeof (HistogramBucket<T>)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        T b = HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__0, (object) this.MinBoundary);
        num1 = GenericNumericUtility<T>.Compare(a, b);
      }
      else
        num1 = -1;
      int num2;
      if (this.MaxBoundary != double.PositiveInfinity)
      {
        T a = measurement;
        // ISSUE: reference to a compiler-generated field
        if (HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (T), typeof (HistogramBucket<T>)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        T b = HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__1.Target((CallSite) HistogramBucket<T>.\u003C\u003Eo__11.\u003C\u003Ep__1, (object) this.MaxBoundary);
        num2 = GenericNumericUtility<T>.Compare(a, b);
      }
      else
        num2 = 1;
      int num3 = num2;
      return num1 > 0 && num3 <= 0;
    }
  }
}
