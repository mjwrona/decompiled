// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WatermarkExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class WatermarkExtensions
  {
    public static bool TryGetWatermark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      WatermarkKind kind)
    {
      Watermark returnWatermark = new Watermark(kind);
      return registryService.TryGetWatermark(requestContext, kind, out returnWatermark);
    }

    public static void SetWatermark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      WatermarkKind kind,
      int changesetId)
    {
      Watermark returnWatermark = new Watermark(kind);
      Watermark newWatermark = !registryService.TryGetWatermark(requestContext, kind, out returnWatermark) || returnWatermark.ChangesetId != changesetId ? new Watermark(kind, changesetId) : new Watermark(kind, changesetId, returnWatermark.RetryCount + 1);
      registryService.SetWatermark(requestContext, newWatermark);
    }

    public static void SetInitialWatermarks(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      int latestChangesetId)
    {
      requestContext.Trace(1023520, TraceLayer.TfsCheckinHandler, "Setting the initial watermarks");
      registryService.SetWatermark(requestContext, WatermarkKind.High, latestChangesetId - 1);
      registryService.SetWatermark(requestContext, WatermarkKind.Low, latestChangesetId);
    }

    public static bool TryGetWatermark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      WatermarkKind watermarkKind,
      out Watermark returnWatermark)
    {
      bool watermark = false;
      returnWatermark = (Watermark) null;
      if (watermarkKind != WatermarkKind.Invalid)
      {
        returnWatermark = new Watermark(watermarkKind);
        using (new CodeSenseTraceWatch(requestContext, 1025600, TraceLayer.ExternalFramework, "Get watermark {0}", new object[1]
        {
          (object) returnWatermark.RegistryKey
        }))
        {
          returnWatermark = Watermark.FromString(registryService.GetOrDefault<string>(requestContext, returnWatermark.RegistryKey, string.Empty));
          watermark = returnWatermark != null && returnWatermark.Kind != 0;
          if (watermark)
            requestContext.Trace(1025600, TraceLayer.Job, "Read {0} watermark as {1}", (object) returnWatermark.Description, (object) returnWatermark.ChangesetId);
        }
      }
      return watermark;
    }

    public static void UpdateWatermarkIfHigh(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      int currentChangesetId)
    {
      Watermark returnWatermark = new Watermark(WatermarkKind.High);
      if (registryService.TryGetWatermark(requestContext, WatermarkKind.High, out returnWatermark))
      {
        if (returnWatermark.ChangesetId >= currentChangesetId)
          return;
        registryService.SetWatermark(requestContext, WatermarkKind.High, currentChangesetId);
      }
      else
        requestContext.Trace(1025595, "CodeSense", TraceLayer.ExternalFramework, (object) "UpdateWatermarkIfHigh : Failed to get the current watermark");
    }

    public static void UpdateWatermarkIfLow(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      int currentChangesetId)
    {
      Watermark returnWatermark = new Watermark(WatermarkKind.Low);
      if (registryService.TryGetWatermark(requestContext, WatermarkKind.Low, out returnWatermark))
      {
        if (returnWatermark.ChangesetId <= currentChangesetId)
          return;
        registryService.SetWatermark(requestContext, WatermarkKind.Low, currentChangesetId);
      }
      else
        requestContext.Trace(1025605, "CodeSense", TraceLayer.ExternalFramework, (object) "UpdateWatermarkIfLow : Failed to get the current watermark");
    }

    internal static void SetWatermark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Watermark newWatermark)
    {
      ArgumentUtility.CheckForNull<Watermark>(newWatermark, nameof (newWatermark));
      if (newWatermark.Kind == WatermarkKind.Invalid)
        throw new ArgumentException("Invalid watermark kind");
      using (new CodeSenseTraceWatch(requestContext, 1025610, TraceLayer.ExternalFramework, "Set {0} watermark to {1}", new object[2]
      {
        (object) newWatermark.Description,
        (object) newWatermark.ChangesetId
      }))
        registryService.SetValue<string>(requestContext, newWatermark.RegistryKey, newWatermark.ToString());
    }
  }
}
