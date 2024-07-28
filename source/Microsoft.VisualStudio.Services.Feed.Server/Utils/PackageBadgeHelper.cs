// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.PackageBadgeHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public static class PackageBadgeHelper
  {
    public static Package GetLatestReleaseVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId)
    {
      IFeedIndexService service = requestContext.GetService<IFeedIndexService>();
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      string packageId1 = packageId;
      ResultOptions resultOptions = new ResultOptions();
      resultOptions.IncludeAllVersions = false;
      resultOptions.IncludeDescriptions = false;
      bool? isListed = new bool?(true);
      bool? isRelease = new bool?(true);
      bool? isDeleted = new bool?(false);
      return service.GetPackage(requestContext1, feed1, packageId1, resultOptions, isListed, isRelease, isDeleted);
    }

    public static XDocument GetPackageBadgeSvg(
      IVssRequestContext requestContext,
      string protocol,
      string version)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      BadgeOptions options = new BadgeOptions(BadgeLogo.Artifacts, protocol, version, leftBackground: PackageBadgeHelper.GetColorValue(service, requestContext, "/Configuration/Feed/Badges/Packages/ProtocolBackgroundColor", "#555555"), rightBackground: PackageBadgeHelper.GetColorValue(service, requestContext, "/Configuration/Feed/Badges/Packages/VersionBackgroundColor", "#0078d4"));
      return BadgeSvgGenerator.CreateImage(requestContext, ref options);
    }

    public static string GetColorValue(
      IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string registryPath,
      string defaultValue)
    {
      return registryService.GetValue<string>(requestContext.Elevate(), (RegistryQuery) registryPath, true, defaultValue);
    }

    public static class DefaultColors
    {
      public const string VersionBackground = "#0078d4";
    }
  }
}
