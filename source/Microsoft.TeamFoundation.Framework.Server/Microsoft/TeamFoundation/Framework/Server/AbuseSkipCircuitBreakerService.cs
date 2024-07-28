// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AbuseSkipCircuitBreakerService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AbuseSkipCircuitBreakerService : IVssFrameworkService
  {
    internal static bool SkipCircuitBreakers;

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/FeatureAvailability/Entries/Microsoft.VisualStudio.Services.HostManagement.AbusiveHostSkipCircuitBreakers/...");

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    internal void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      foreach (RegistryEntry changedEntry in changedEntries)
      {
        if (!(changedEntry.Name != "AvailabilityState") && changedEntry.Path.Contains("AbusiveHostSkipCircuitBreakers"))
          AbuseSkipCircuitBreakerService.SkipCircuitBreakers = FeatureState.On == AbuseSkipCircuitBreakerService.ToFeatureState(changedEntry.GetValue((string) null));
      }
    }

    private static FeatureState ToFeatureState(string state)
    {
      switch (state)
      {
        case "0":
          return FeatureState.Off;
        case "1":
          return FeatureState.On;
        default:
          return FeatureState.Undefined;
      }
    }
  }
}
