// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebPlatformFeatureFlags
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateSpecificConstants(null)]
  public static class WebPlatformFeatureFlags
  {
    [GenerateConstant(null)]
    public const string VisualStudioServicesContributionUnSecureBrowsers = "VisualStudio.Services.Contribution.EnableOnPremUnsecureBrowsers";
    [GenerateConstant(null)]
    public const string ClientSideErrorLogging = "VisualStudio.Service.WebPlatform.ClientErrorReporting";
    [GenerateConstant(null)]
    public const string UseGalleryCdn = "Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri";
    [GenerateConstant(null)]
    public const string MarkdownRendering = "VisualStudio.Services.WebAccess.MarkdownRendering";
    [GenerateConstant(null)]
    public const string SubresourceIntegrity = "VisualStudio.Services.WebAccess.SubresourceIntegrity";
    [GenerateConstant(null)]
    public const string ReactProfileCard = "VisualStudio.Services.IdentityPicker.ReactProfileCard";
    [GenerateConstant(null)]
    public const string UseNewBranding = "VisualStudio.Services.WebPlatform.UseNewBranding";
    public const string UseCDN = "VisualStudio.Services.WebAccess.UseCDN";
    public const string UseRegionalCdnUrls = "VisualStudio.Services.WebAccess.UseRegionalCDNURLs";
    public const string NoTeamContext = "VisualStudio.Services.WebAccess.NoTeamContext";
    public const string UsePublicAccessMappingMoniker = "VisualStudio.Services.WebPlatform.UsePublicAccessMappingMoniker";
  }
}
