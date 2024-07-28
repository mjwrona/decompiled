// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StandardFaultInjection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class StandardFaultInjection
  {
    private static readonly Random s_random = new Random();

    internal static void InjectDelay(DelayFaultSettings[] faults, string faultPoint)
    {
      double random = StandardFaultInjection.s_random.NextDouble();
      if (faults == null)
        return;
      DelayFaultSettings faultSettings = ((IEnumerable<DelayFaultSettings>) faults).Where<DelayFaultSettings>((Func<DelayFaultSettings, bool>) (o => o.Probability >= random)).OrderBy<DelayFaultSettings, int>((Func<DelayFaultSettings, int>) (f => f.DelayMs)).FirstOrDefault<DelayFaultSettings>();
      if (faultSettings == null)
        return;
      FaultInjectionServiceExtensions.TraceFaultRaw<DelayFaultSettings>(faultPoint, "Delay", faultSettings);
      Thread.Sleep(TimeSpan.FromMilliseconds((double) faultSettings.DelayMs));
    }

    internal static void InjectException(ExceptionFaultSettings[] faults, string faultPoint)
    {
      double random = StandardFaultInjection.s_random.NextDouble();
      if (faults != null && ((IEnumerable<ExceptionFaultSettings>) faults).Any<ExceptionFaultSettings>((Func<ExceptionFaultSettings, bool>) (o => o.Probability >= random)))
      {
        FaultInjectionServiceExtensions.TraceFaultRaw<ExceptionFaultSettings>(faultPoint, "Exception", ((IEnumerable<ExceptionFaultSettings>) faults).First<ExceptionFaultSettings>());
        throw new FaultInjectionException();
      }
    }

    public static void InjectDelayRaw(string faultConfiguration, string faultPoint, string target = null) => StandardFaultInjection.InjectDelay(FaultInjectionServiceExtensions.GetFaultsForTargetRaw<DelayFaultSettings>(faultConfiguration, faultPoint, "Delay", target), faultPoint);

    public static void InjectExceptionRaw(
      string faultConfiguration,
      string faultPoint,
      string target = null)
    {
      StandardFaultInjection.InjectException(FaultInjectionServiceExtensions.GetFaultsForTargetRaw<ExceptionFaultSettings>(faultConfiguration, faultPoint, "Exception", target), faultPoint);
    }
  }
}
