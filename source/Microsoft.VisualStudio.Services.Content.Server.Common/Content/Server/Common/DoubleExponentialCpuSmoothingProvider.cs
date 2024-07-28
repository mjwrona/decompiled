// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.DoubleExponentialCpuSmoothingProvider
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class DoubleExponentialCpuSmoothingProvider : ICpuPerformanceCounterProvider
  {
    private const int DefaultSamplingIntervalInMs = 150;
    private readonly PerformanceCounter cpuCounter;
    private float cpuForecast;
    private float cpuCurrEstimate;
    private float cpuPrevEstimate;
    private float bestTrendEstimate;
    private const float cpuLoadSmoothingFactor = 0.65f;
    private const float cpuTrendSmoothingFactor = 0.0f;
    private const float intervalsToEstimateInFuture = 0.0f;
    private readonly int msSampleInterval;

    public DoubleExponentialCpuSmoothingProvider()
    {
      this.msSampleInterval = 150;
      this.cpuCounter = new PerformanceCounter()
      {
        CategoryName = "Processor",
        CounterName = "% Processor Time",
        InstanceName = "_Total"
      };
      double num = (double) this.cpuCounter.NextValue();
      Task.Run((Func<Task>) (async () =>
      {
        while (true)
        {
          try
          {
            await Task.Delay(this.msSampleInterval);
            this.cpuPrevEstimate = this.cpuCurrEstimate;
            this.cpuCurrEstimate = (float) (0.64999997615814209 * (double) this.cpuCounter.NextValue() + 0.35000002384185791 * ((double) this.cpuPrevEstimate + (double) this.bestTrendEstimate));
            this.bestTrendEstimate = (float) (0.0 * ((double) this.cpuCurrEstimate - (double) this.cpuPrevEstimate) + 1.0 * (double) this.bestTrendEstimate);
            this.cpuForecast = this.cpuCurrEstimate + 0.0f * this.bestTrendEstimate;
          }
          catch
          {
          }
        }
      }));
    }

    public float GetCurrentCpuLoadForecast()
    {
      if ((double) this.cpuForecast <= 0.0)
        return 0.0f;
      return (double) this.cpuForecast >= 100.0 ? 100f : this.cpuForecast;
    }

    public int GetSampleIntervalInMs() => this.msSampleInterval;

    public float GetCpuDelta() => this.cpuForecast - this.cpuPrevEstimate;
  }
}
