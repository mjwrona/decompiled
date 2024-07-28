// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationServiceFeatureFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Location.Server
{
  public static class LocationServiceFeatureFlags
  {
    public const string DontIncludeHostPublicMapping = "VisualStudio.FrameworkService.LocationService.DontIncludeHostPublicMapping";
    public const string DuplicateHostsCheck = "Microsoft.VisualStudio.RemoteLocationProvider.DuplicateHostsCheck";
    public const string IgnoreExternalClientAccessMapping = "VisualStudio.Services.Location.IgnoreExternalClientAccessMapping";
    public const string RemoveApplicationDefinitionForDev12 = "VisualStudio.FrameworkService.LocationService.RemoveApplicationDefinitionForDev12";
    public const string UseDevOpsDomainForS2S = "VisualStudio.Services.Location.UseDevOpsDomainForS2S";
    public const string UseLegacyIdentity = "VisualStudio.Services.ConnectionData.UseLegacyIdentity";
  }
}
