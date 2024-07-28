// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultInjectionServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class FaultInjectionServiceExtensions
  {
    private const string s_layer = "FaultInjectionServiceExtensions";

    public static T[] GetFaultsForTarget<T>(
      this IFaultInjectionService faultInjectionService,
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType,
      string target)
      where T : ITargetedFaultSettings
    {
      return FaultInjectionServiceExtensions.FilterFaultForTarget<T>(faultInjectionService.GetFaults<T>(requestContext, faultPoint, faultType), target);
    }

    public static T[] GetFaultsForTargetRaw<T>(
      string faultConfiguration,
      string faultPoint,
      string faultType,
      string target)
      where T : ITargetedFaultSettings
    {
      return FaultInjectionServiceExtensions.FilterFaultForTarget<T>(FaultInjectionService.GetFaultsRaw<T>(faultConfiguration, faultPoint, faultType), target);
    }

    public static void TraceFault<T>(
      this IFaultInjectionService faultInjectionService,
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType,
      T faultSettings)
      where T : IFaultSettings
    {
      string format = "{\"FaultPoint\": \"" + faultPoint + "\", \"FaultType\": \"" + faultType + "\", \"Settings\": \"" + JsonConvert.SerializeObject((object) faultSettings) + "\" }";
      requestContext.TraceAlways(1440003, TraceLevel.Warning, "VisualStudio.Services.FaultInjectionService", nameof (FaultInjectionServiceExtensions), format);
    }

    public static void TraceFaultRaw<T>(string faultPoint, string faultType, T faultSettings) where T : IFaultSettings => TeamFoundationTracingService.TraceRawAlwaysOn(1440003, TraceLevel.Warning, "VisualStudio.Services.FaultInjectionService", nameof (FaultInjectionServiceExtensions), "{\"FaultPoint\": \"" + faultPoint + "\", \"FaultType\": \"" + faultType + "\", \"Settings\": \"" + JsonConvert.SerializeObject((object) faultSettings) + "\" }");

    private static T[] FilterFaultForTarget<T>(T[] faults, string target) where T : ITargetedFaultSettings => ((IEnumerable<T>) faults).Where<T>((Func<T, bool>) (x => string.Equals(target, x.Target, StringComparison.OrdinalIgnoreCase))).ToArray<T>();
  }
}
