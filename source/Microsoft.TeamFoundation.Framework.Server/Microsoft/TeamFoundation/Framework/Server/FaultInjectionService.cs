// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultInjectionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FaultInjectionService : IFaultInjectionService, IVssFrameworkService
  {
    private const string s_layer = "FaultInjectionService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool IsFaultInjectionEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.FaultInjectionService.InjectFault");

    public T[] GetFaults<T>(IVssRequestContext requestContext, string faultPoint, string faultType) where T : IFaultSettings
    {
      if (this.IsFaultInjectionEnabled(requestContext))
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultPoint));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(faultType, nameof (faultType));
        try
        {
          FaultDefinition[] faultDefinitionArray = this.GetFaultsFromRequest(requestContext, faultPoint, faultType);
          if (!((IEnumerable<FaultDefinition>) faultDefinitionArray).Any<FaultDefinition>())
            faultDefinitionArray = this.GetFaultsFromRegistry(requestContext, faultPoint, faultType);
          if (((IEnumerable<FaultDefinition>) faultDefinitionArray).Any<FaultDefinition>())
          {
            FaultDefinition[] faults = FaultInjectionService.FilterFaults(faultDefinitionArray);
            return FaultInjectionService.ConvertFaultSettings<T>(requestContext, faults);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1440000, TraceLevel.Info, "VisualStudio.Services.FaultInjectionService", nameof (FaultInjectionService), ex);
        }
      }
      return Array.Empty<T>();
    }

    public bool IsFaultEnabled(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType)
    {
      if (this.IsFaultInjectionEnabled(requestContext))
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultPoint));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(faultType, nameof (faultType));
        try
        {
          FaultDefinition[] faultDefinitionArray = this.GetFaultsFromRequest(requestContext, faultPoint, faultType);
          if (!((IEnumerable<FaultDefinition>) faultDefinitionArray).Any<FaultDefinition>())
            faultDefinitionArray = this.GetFaultsFromRegistry(requestContext, faultPoint, faultType);
          if (((IEnumerable<FaultDefinition>) FaultInjectionService.FilterFaults(faultDefinitionArray)).Any<FaultDefinition>())
            return true;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1440001, TraceLevel.Info, "VisualStudio.Services.FaultInjectionService", nameof (FaultInjectionService), ex);
        }
      }
      return false;
    }

    public static T[] GetFaultsRaw<T>(
      string faultConfiguration,
      string faultPoint,
      string faultType)
      where T : IFaultSettings
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultConfiguration, nameof (faultConfiguration));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultPoint));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultType, nameof (faultType));
      try
      {
        return FaultInjectionService.ConvertFaultSettings<T>((IVssRequestContext) null, FaultInjectionService.FilterFaults(new FaultDefinitionCollection(faultConfiguration).GetFaults(faultPoint, faultType)));
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1440002, TraceLevel.Info, "VisualStudio.Services.FaultInjectionService", nameof (FaultInjectionService), ex);
      }
      return Array.Empty<T>();
    }

    private FaultDefinition[] GetFaultsFromRequest(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType)
    {
      IFaultInjectionRequestSettingsService service = requestContext.GetService<IFaultInjectionRequestSettingsService>();
      return service.AreRequestScopedFaultsEnabled(requestContext) ? service.GetFaults(requestContext, faultPoint, faultType) : Array.Empty<FaultDefinition>();
    }

    private FaultDefinition[] GetFaultsFromRegistry(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IFaultInjectionRegistrySettingsService>().GetFaults(vssRequestContext, faultPoint, faultType);
    }

    private static FaultDefinition[] FilterFaults(FaultDefinition[] faults) => ((IEnumerable<FaultDefinition>) faults).Where<FaultDefinition>((Func<FaultDefinition, bool>) (f => f.ShouldExecute())).ToArray<FaultDefinition>();

    private static T[] ConvertFaultSettings<T>(
      IVssRequestContext requestContext,
      FaultDefinition[] faults)
      where T : IFaultSettings
    {
      return ((IEnumerable<FaultDefinition>) faults).Where<FaultDefinition>((Func<FaultDefinition, bool>) (f => f.Settings != null)).Select<FaultDefinition, T>((Func<FaultDefinition, T>) (f => f.Settings.ToObject<T>())).ToArray<T>();
    }
  }
}
