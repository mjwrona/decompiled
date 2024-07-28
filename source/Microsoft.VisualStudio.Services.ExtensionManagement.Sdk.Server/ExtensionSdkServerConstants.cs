// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionSdkServerConstants
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class ExtensionSdkServerConstants
  {
    public const string ExtensionServiceBusName = "Microsoft.VisualStudio.Services.Extension";
    public const string VersionRegistryPath = "/Configuration/Extensions";
    public static readonly char[] RegistrySeparators = new char[1]
    {
      '/'
    };
    internal const string ExtensionRightsKey = "extension-rights";
    public const string InExtensionFallbackMode = "InExtensionFallbackMode";
    public const string CdnPrefix = "ext";
    public const string DisableDemandExtensionCheck = "VisualStudio.Services.ExtensionsDisableDemandCheck";
    public const string EnableExtensionsGetHttpClientOptionsForEventualReadConsistencyLevel = "VisualStudio.Services.EnableExtensionsGetHttpClientOptionsForEventualReadConsistencyLevel";
  }
}
