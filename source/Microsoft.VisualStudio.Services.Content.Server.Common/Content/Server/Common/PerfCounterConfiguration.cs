// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.PerfCounterConfiguration
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class PerfCounterConfiguration
  {
    internal static readonly int HeartbeatIntervalMs = 5000;
    private const string PerfCountersEnabledKey = "/Configuration/PerfCountersEnabled";

    public static bool AreWindowsCountersEnabledInThisScaleUnit(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/PerfCountersEnabled", true, false);
  }
}
