// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebPlatformBootstrapFeatureFlags
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class WebPlatformBootstrapFeatureFlags
  {
    public static readonly string[] FeatureFlagNames = new string[5]
    {
      "VisualStudio.Services.Contribution.EnableOnPremUnsecureBrowsers",
      "VisualStudio.Service.WebPlatform.ClientErrorReporting",
      "Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri",
      "VisualStudio.Services.WebAccess.SubresourceIntegrity",
      "VisualStudio.Services.IdentityPicker.ReactProfileCard"
    };
  }
}
