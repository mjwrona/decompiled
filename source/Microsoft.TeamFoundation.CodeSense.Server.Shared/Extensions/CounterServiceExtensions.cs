// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Extensions.CounterServiceExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.CodeSense.Server.Extensions
{
  public static class CounterServiceExtensions
  {
    private const string StorageGrowthCounter = "CodeLensStorageGrowth";

    public static void UpdateStorageGrowth(
      this TeamFoundationCounterService counterService,
      IVssRequestContext requestContext,
      ref long increment,
      bool forceUpdate)
    {
      long num = requestContext.GetService<IVssRegistryService>().StorageGrowthCounterUpdateThreshold(requestContext);
      if (increment < num && (!forceUpdate || increment <= 0L))
        return;
      CounterServiceExtensions.TryReserveStorageGrowthCounter(requestContext, counterService, increment);
      increment = 0L;
    }

    public static long GetStorageGrowth(
      this TeamFoundationCounterService counterService,
      IVssRequestContext requestContext)
    {
      return CounterServiceExtensions.TryReserveStorageGrowthCounter(requestContext, counterService);
    }

    public static void ResetStorageGrowthCounter(
      this TeamFoundationCounterService counterService,
      IVssRequestContext requestContext)
    {
      try
      {
        counterService.ResetCounter(requestContext, "CodeLensStorageGrowth", (string) null, new Guid(), false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1025070, TraceLevel.Error, "CodeSense", TraceLayer.Job, ex);
      }
    }

    private static long TryReserveStorageGrowthCounter(
      IVssRequestContext requestContext,
      TeamFoundationCounterService counterService,
      long delta = 0)
    {
      try
      {
        return counterService.ReserveCounterIds(requestContext, "CodeLensStorageGrowth", delta, (string) null, new Guid(), false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1025075, TraceLevel.Warning, "CodeSense", TraceLayer.Job, ex);
      }
      return 0;
    }
  }
}
