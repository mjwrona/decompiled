// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.CpuThrottleHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class CpuThrottleHelper
  {
    internal const string DefaultCpuThrottlingDisableRegistryPath = "/Configuration/CpuThrottleHelper/DisableCpuThrottling";
    private const bool DefaultCpuThrottlingDisable = false;
    public static bool ThrottlingEnabled = true;
    public static readonly CpuThrottleHelper Instance = new CpuThrottleHelper(CpuPerformanceCounterProvider<DoubleExponentialCpuSmoothingProvider>.Instance, (IRandomProbabilityGenerator) RandomProbabilityGenerator.Instance);
    public static long WaitingOnYield = 0;
    public const int DefaultThrottlingUpperCpuThreshold = 87;
    public const int DefaultThrottlingLowerCpuThreshold = 82;
    public const int DefaultThrottlingUpperCpuThresholdForLowWorkLoads = 95;
    public const int DefaultWaiterThreshold = 30;
    public const int DefaultWaiterMaxWaitTime = 120;
    private readonly ICpuPerformanceCounterProvider performanceCounterProvider;
    private readonly IRandomProbabilityGenerator randomProbabilityGenerator;

    internal CpuThrottleHelper(
      ICpuPerformanceCounterProvider performanceCounterProvider,
      IRandomProbabilityGenerator randomProbabilityGenerator)
    {
      this.randomProbabilityGenerator = randomProbabilityGenerator;
      this.performanceCounterProvider = performanceCounterProvider;
    }

    public async Task<int> Yield(int threshold, CancellationToken cancellationToken)
    {
      int totalDelayInSeconds = 0;
      if (threshold == 100)
        return 0;
      cancellationToken.ThrowIfCancellationRequested();
      int delay;
      long val1;
      ConfiguredTaskAwaitable configuredTaskAwaitable;
      while ((val1 = Interlocked.Read(ref CpuThrottleHelper.WaitingOnYield)) > 30L)
      {
        delay = (int) Math.Min(val1, 120L);
        configuredTaskAwaitable = Task.Delay(TimeSpan.FromSeconds((double) delay), cancellationToken).ConfigureAwait(false);
        await configuredTaskAwaitable;
        totalDelayInSeconds += delay;
      }
      Interlocked.Increment(ref CpuThrottleHelper.WaitingOnYield);
      try
      {
        while (this.IsThrottledWithInfo(threshold, threshold - 10).IsThrottled)
        {
          delay = 10 + ThreadLocalRandom.Generator.Next(0, 10);
          configuredTaskAwaitable = Task.Delay(TimeSpan.FromSeconds((double) delay), cancellationToken).ConfigureAwait(false);
          await configuredTaskAwaitable;
          totalDelayInSeconds += delay;
        }
      }
      finally
      {
        Interlocked.Decrement(ref CpuThrottleHelper.WaitingOnYield);
      }
      return totalDelayInSeconds;
    }

    public bool IsThrottled(
      IVssRequestContext requestContext,
      string registryRootPath,
      string controllerLabel,
      int defaultCpuMaxThreshold = -1,
      int defaultCpuMinThreshold = -1)
    {
      return this.IsThrottledWithInfo(requestContext, registryRootPath, controllerLabel, defaultCpuMaxThreshold, defaultCpuMinThreshold).IsThrottled;
    }

    public ThrottleInfo IsThrottledWithInfo(
      IVssRequestContext requestContext,
      string registryRootPath,
      string controllerLabel,
      int defaultCpuMaxThreshold = -1,
      int defaultCpuMinThreshold = -1)
    {
      if (defaultCpuMaxThreshold < 0)
        defaultCpuMaxThreshold = 87;
      if (defaultCpuMinThreshold < 0)
        defaultCpuMinThreshold = 82;
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return new ThrottleInfo(false, (string) null, new TimeSpan?());
      return CpuThrottleHelper.GetCpuThrottlingDisabled(requestContext) ? new ThrottleInfo(false, (string) null, new TimeSpan?()) : this.IsThrottledWithInfo(ThrottleHelper.GetThrottlingParameter<int>(requestContext, registryRootPath, ThrottleHelper.RegistryKey.CpuPercentageUpperThreshold, controllerLabel, defaultCpuMaxThreshold), ThrottleHelper.GetThrottlingParameter<int>(requestContext, registryRootPath, ThrottleHelper.RegistryKey.CpuPercentageLowerThreshold, controllerLabel, defaultCpuMinThreshold));
    }

    private static bool GetCpuThrottlingDisabled(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/CpuThrottleHelper/DisableCpuThrottling", true, false);

    private ThrottleInfo IsThrottledWithInfo(int maxThreshold, int minThreshold)
    {
      if (!CpuThrottleHelper.ThrottlingEnabled)
        return new ThrottleInfo(false, (string) null, new TimeSpan?());
      if (maxThreshold <= 0 || maxThreshold > 100)
      {
        maxThreshold = 87;
        minThreshold = 82;
      }
      if (minThreshold <= 0 || minThreshold >= maxThreshold)
        minThreshold = maxThreshold - 5;
      float currentCpuLoadForecast = this.performanceCounterProvider.GetCurrentCpuLoadForecast();
      if ((double) currentCpuLoadForecast >= (double) maxThreshold)
      {
        float cpuDelta = this.performanceCounterProvider.GetCpuDelta();
        int sampleIntervalInMs = this.performanceCounterProvider.GetSampleIntervalInMs();
        int num1 = (int) ((double) currentCpuLoadForecast + 1.0) - maxThreshold;
        int num2;
        if ((double) cpuDelta > 0.0)
        {
          int num3 = (int) ((1.0 + (double) cpuDelta) * (double) num1) + 1;
          num2 = sampleIntervalInMs * num3;
        }
        else
          num2 = sampleIntervalInMs * num1;
        return new ThrottleInfo(true, string.Format(Resources.ForecastedCpuOverMaxThreshold, (object) currentCpuLoadForecast, (object) maxThreshold), new TimeSpan?(TimeSpan.FromMilliseconds((double) num2)));
      }
      if ((double) currentCpuLoadForecast <= (double) minThreshold)
        return new ThrottleInfo(false, (string) null, new TimeSpan?());
      float num = (currentCpuLoadForecast - (float) minThreshold) / (float) (maxThreshold - minThreshold);
      return (double) num > this.randomProbabilityGenerator.GetNextValue() ? new ThrottleInfo(true, string.Format(Resources.ForecastedCpuOverMaxThreshold, (object) currentCpuLoadForecast, (object) minThreshold, (object) (int) ((double) num * 100.0)), new TimeSpan?(TimeSpan.FromMilliseconds((double) this.performanceCounterProvider.GetSampleIntervalInMs()))) : new ThrottleInfo(false, (string) null, new TimeSpan?());
    }
  }
}
