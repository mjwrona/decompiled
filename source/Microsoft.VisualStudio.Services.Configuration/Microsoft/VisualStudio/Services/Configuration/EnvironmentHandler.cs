// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.EnvironmentHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class EnvironmentHandler
  {
    public static bool IsDomainController() => OSDetails.IsDomainController;

    public static bool IsMachineInWorkgroup() => OSDetails.IsMachineInWorkgroup();

    public static bool IsServer()
    {
      EnvironmentHandler.PlatformOverrideType platformOverrideType = EnvironmentHandler.GetOverride();
      return platformOverrideType != EnvironmentHandler.PlatformOverrideType.None ? platformOverrideType == EnvironmentHandler.PlatformOverrideType.Server : OSDetails.IsServer;
    }

    public static bool IsClient()
    {
      EnvironmentHandler.PlatformOverrideType platformOverrideType = EnvironmentHandler.GetOverride();
      return platformOverrideType != EnvironmentHandler.PlatformOverrideType.None ? platformOverrideType == EnvironmentHandler.PlatformOverrideType.Client : OSDetails.IsClient;
    }

    private static EnvironmentHandler.PlatformOverrideType GetOverride() => EnvironmentHandler.PlatformOverrideType.None;

    private enum PlatformOverrideType
    {
      Client,
      Server,
      None,
    }
  }
}
