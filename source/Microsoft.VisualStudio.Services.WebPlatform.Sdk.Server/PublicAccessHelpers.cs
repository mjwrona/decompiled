// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.PublicAccessHelpers
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public static class PublicAccessHelpers
  {
    public static bool RouteSupportsPublicAccess(IVssRequestContext requestContext)
    {
      bool flag = false;
      Contribution routedContribution = requestContext.GetService<IContributionRoutingService>().GetRoutedContribution(requestContext);
      if (routedContribution != null && routedContribution.RestrictedTo != null)
        flag = ContributionRestriction.HasAnyClaim(routedContribution, "anonymous", "public");
      return flag;
    }

    public static IDataProviderScope GetDataProviderScopeForRoute(IVssRequestContext requestContext)
    {
      IDataProviderScope providerScopeForRoute = (IDataProviderScope) null;
      if (PublicAccessHelpers.RouteSupportsPublicAccess(requestContext))
        providerScopeForRoute = ExtensionDataProviderScopeExtensions.GetRequestScope(requestContext);
      return providerScopeForRoute;
    }
  }
}
