// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.LogHistogram
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  [Serializable]
  public class LogHistogram : Histogram
  {
    [JsonProperty]
    private readonly double logBase;
    [JsonProperty]
    private readonly double maxValue;

    public LogHistogram(double logBase, double maxValue)
      : base(LogHistogram.GetBucketCount(logBase, maxValue))
    {
      this.logBase = logBase;
      this.maxValue = maxValue;
    }

    private static int GetBucketCount(double logBase, double maxValue)
    {
      if (logBase < 2.0)
        throw new ArgumentException("Logarithm base is less than 2.0.");
      return logBase <= maxValue ? (int) Math.Ceiling(Math.Log(maxValue, logBase)) : throw new ArgumentException("Histogram maximum value is less than base.");
    }

    protected override int GetBucketIndex(double value) => base.GetBucketIndex(value < 1.0 ? 0.0 : Math.Ceiling(Math.Log(value, this.logBase)));

    protected override Tuple<double, double> GetBucketRange(int bucketIndex)
    {
      double num1 = Math.Pow(this.logBase, (double) bucketIndex);
      double num2 = Math.Pow(this.logBase, (double) (bucketIndex + 1));
      if (bucketIndex == 0)
        num1 = double.MinValue;
      else if (bucketIndex == this.counts.Length - 1)
        num2 = double.MaxValue;
      return Tuple.Create<double, double>(num1, num2);
    }
  }
}
