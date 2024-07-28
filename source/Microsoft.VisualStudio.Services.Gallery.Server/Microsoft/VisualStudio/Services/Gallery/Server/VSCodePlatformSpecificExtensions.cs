// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.VSCodePlatformSpecificExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class VSCodePlatformSpecificExtensions
  {
    internal const string RegistryPathForVSCodeSupportedTargetPlatforms = "/Configuration/Service/Gallery/PlatformSpecificExtensions/SupportedVSCodeTargetPlatforms";
    internal const string RegistryPathForVSCodeLegacySupportedTargetPlatforms = "/Configuration/Service/Gallery/PlatformSpecificExtensions/LegacySupportedVSCodeTargetPlatforms";
    internal const string RegistryPathForRecordsCountPerPageOnManageTabInReports = "/Configuration/Service/Gallery/PlatformSpecificExtensions/RecordsCountPerPageOnManageTabInReports";
    internal const string UniversalTargetPlatform = "universal";
    internal const string UniversalTargetPlatformDisplayName = "Universal";
    internal const int MaxTargetPlatformsCount = 30;
    internal const int DefaultRecordsCountPerPageOnManageTabInReports = 8;
    internal const string WebTargetPlatform = "web";
  }
}
