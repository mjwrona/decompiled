// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.CodeLensExtension
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public static class CodeLensExtension
  {
    public static ExportLifetimeContext<T> CreateLifeTimeExport<T>(IVssRequestContext requestContext)
    {
      IDisposableReadOnlyList<T> serviceCollection = CodeLensExtension.GetServiceCollection<T>(requestContext);
      return new ExportLifetimeContext<T>(serviceCollection.FirstOrDefault<T>(), new Action(((IDisposable) serviceCollection).Dispose));
    }

    public static void LogKPI(
      this IVssRequestContext requestContext,
      string kpiArea,
      string kpiName)
    {
      requestContext.LogKPI(CodeLensExtension.GetHostId(requestContext), kpiArea, kpiName);
    }

    public static void LogKPI(
      this IVssRequestContext requestContext,
      Guid hostId,
      string kpiArea,
      string kpiName)
    {
      try
      {
        CodeLensExtension.GetCodeLensKpiLoggerService(requestContext).LogKPI(requestContext, hostId, kpiArea, kpiName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1025911, "CodeSense", TraceLayer.KpiLogger, ex);
      }
    }

    public static void PublishCI(
      this IVssRequestContext requestContext,
      CodeLensCILevel level,
      string area,
      string feature,
      Dictionary<string, object> data)
    {
      try
      {
        CodeLensExtension.GetCodeLensCiLoggerService(requestContext).PublishCI(requestContext, level, area, feature, data);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1025912, "CodeSense", TraceLayer.CiLogger, ex);
      }
    }

    public static void PublishLayerCakeCI(
      this IVssRequestContext requestContext,
      CodeLensCILevel level,
      string area,
      string feature,
      Dictionary<string, object> data,
      TimeSpan timeElapsed)
    {
      try
      {
        ICodeLensCILoggerService lensCiLoggerService = CodeLensExtension.GetCodeLensCiLoggerService(requestContext);
        LayerCakeCISettings settings = lensCiLoggerService is ICodeLensLayerCakeSettings ? (lensCiLoggerService as ICodeLensLayerCakeSettings).Settings : (LayerCakeCISettings) null;
        if (!CodeLensExtension.ShouldPublishLayerCakeCI(requestContext, settings, timeElapsed))
          return;
        data.Add(CodeLensCIProperty.TimeElapsedInMilliseconds, (object) ((int) timeElapsed.TotalMilliseconds).ToString());
        lensCiLoggerService.PublishCI(requestContext, CodeLensCILevel.Important, CodeLensCIArea.CodeLensService, CodeLensCIFeature.LayerCakeAnalysis, data);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1025912, "CodeSense", TraceLayer.CiLogger, ex);
      }
    }

    internal static bool ShouldPublishLayerCakeCI(
      IVssRequestContext requestContext,
      LayerCakeCISettings layerCakeCISettings,
      TimeSpan timeElapsed)
    {
      if (layerCakeCISettings == null)
      {
        requestContext.Trace(1025913, TraceLevel.Info, "CodeSense", TraceLayer.CiLogger, "LayerCakeCISettings is null. Cannot log LayerCakeCI.");
        return false;
      }
      if (!layerCakeCISettings.IsLayerCakeSamplingEnabled)
      {
        requestContext.Trace(1025913, TraceLevel.Info, "CodeSense", TraceLayer.CiLogger, "LayerCake logging is disabled.");
        return false;
      }
      if (!(timeElapsed < layerCakeCISettings.TimeElapsedThresholdForLayerCakeCI))
        return CodeLensExtension.ShouldPublish(requestContext.ActivityId.GetHashCode(), Math.Max(1, layerCakeCISettings.ProbabilityInverse));
      requestContext.Trace(1025913, TraceLevel.Info, "CodeSense", TraceLayer.CiLogger, "TimeElapsed ({0} ms) is lesser than the threshold ({1} ms).", (object) timeElapsed.TotalMilliseconds, (object) layerCakeCISettings.TimeElapsedThresholdForLayerCakeCI.TotalMilliseconds);
      return false;
    }

    private static bool ShouldPublish(int seed, int probabilityInverse) => new Random(seed).Next(1, probabilityInverse) == 1;

    private static Guid GetHostId(IVssRequestContext requestContext) => requestContext != null && requestContext.ServiceHost != null ? requestContext.ServiceHost.InstanceId : Guid.Empty;

    private static ICodeLensKpiLoggerService GetCodeLensKpiLoggerService(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (vssRequestContext == null ? requestContext : vssRequestContext.Elevate()).GetService<ICodeLensKpiLoggerService>();
    }

    private static ICodeLensCILoggerService GetCodeLensCiLoggerService(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ICodeLensCILoggerService>();
    }

    private static IDisposableReadOnlyList<T> GetServiceCollection<T>(
      IVssRequestContext requestContext)
    {
      IDisposableReadOnlyList<T> extensions = requestContext.GetExtensions<T>();
      return extensions != null && extensions.Any<T>() ? extensions : throw new Exception(string.Format("Could not find any export for type {0}", (object) typeof (T)));
    }
  }
}
