// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.GalleryHelper
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Dashboards
{
  public class GalleryHelper
  {
    private static readonly Guid GalleryServiceId = new Guid("00000029-0000-8888-8000-000000000000");

    private static string TryGetWrapper(
      IVssRequestContext requestContext,
      Func<string> function,
      int tracepoint)
    {
      try
      {
        return function();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracepoint, "Dashboards", nameof (GalleryHelper), ex);
        return (string) null;
      }
    }

    private static string GetMarketPlaceRootUrl(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      string locationServiceUrl = vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, GalleryHelper.GalleryServiceId, AccessMappingConstants.ClientAccessMappingMoniker);
      if (string.IsNullOrEmpty(locationServiceUrl))
        return (string) null;
      if (!locationServiceUrl.EndsWith("/"))
        locationServiceUrl += "/";
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        locationServiceUrl += "_gallery";
      if (locationServiceUrl.Contains("marketplace.visualstudio.com"))
        locationServiceUrl += "azuredevops/";
      return locationServiceUrl;
    }

    public static string TryGetMarketPlaceRootUrl(IVssRequestContext requestContext) => GalleryHelper.TryGetWrapper(requestContext, (Func<string>) (() => GalleryHelper.GetMarketPlaceRootUrl(requestContext)), 10017601);

    private static string GetMarketPlaceExtensionUrl(
      IVssRequestContext requestContext,
      ContributionIdentifier contribution)
    {
      string str = GalleryHelper.GetMarketPlaceRootUrl(requestContext);
      if (str == null)
        return (string) null;
      if (str.Contains("marketplace.visualstudio.com"))
        str = str.Replace("azuredevops/", "");
      return str.TrimEnd('/') + "/items/" + contribution.PublisherName + "." + contribution.ExtensionName;
    }

    public static string TryGetMarketPlaceExtensionUrl(
      IVssRequestContext requestContext,
      ContributionIdentifier contribution)
    {
      return GalleryHelper.TryGetWrapper(requestContext, (Func<string>) (() => GalleryHelper.GetMarketPlaceExtensionUrl(requestContext, contribution)), 10017602);
    }
  }
}
