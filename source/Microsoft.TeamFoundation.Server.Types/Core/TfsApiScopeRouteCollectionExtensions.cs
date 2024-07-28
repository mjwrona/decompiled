// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TfsApiScopeRouteCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TfsApiScopeRouteCollectionExtensions
  {
    public static IHttpRoute MapResourceRoute(
      this HttpRouteCollection routes,
      TfsApiResourceScope tfsApiScope,
      Guid locationId,
      string area,
      string resource,
      string routeTemplate,
      VssRestApiVersion initialVersion,
      VssRestApiReleaseState releaseState = VssRestApiReleaseState.Preview,
      int maxResourceVersion = 1,
      object defaults = null,
      object constraints = null,
      string routeName = null,
      HttpMessageHandler handler = null,
      VssRestApiVersion? deprecatedAtVersion = null)
    {
      TeamFoundationHostType hostType1 = TfsApiScopeRouteCollectionExtensions.ConvertToHostType(tfsApiScope);
      List<string> stringList = new List<string>();
      bool flag = false;
      if (tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Project))
      {
        string str = "{project}";
        stringList.Add(str);
      }
      if (tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Team))
      {
        string str = "{project}/{team}";
        stringList.Add(str);
      }
      if (stringList.Count > 0)
        flag = tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Application) || tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Collection) || tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Deployment);
      HttpRouteCollection routes1 = routes;
      int hostType2 = (int) hostType1;
      Guid locationId1 = locationId;
      string area1 = area;
      string resource1 = resource;
      string routeTemplate1 = routeTemplate;
      int initialVersion1 = (int) initialVersion;
      int releaseState1 = (int) releaseState;
      int maxResourceVersion1 = maxResourceVersion;
      object defaults1 = defaults;
      object constraints1 = constraints;
      string routeName1 = routeName;
      HttpMessageHandler handler1 = handler;
      VssRestApiVersion? nullable = deprecatedAtVersion;
      List<string> hostScopeTemplates = stringList;
      int num = flag ? 1 : 0;
      VssRestApiVersion? deprecatedAtVersion1 = nullable;
      return routes1.MapResourceRoute((TeamFoundationHostType) hostType2, locationId1, area1, resource1, routeTemplate1, (VssRestApiVersion) initialVersion1, (VssRestApiReleaseState) releaseState1, maxResourceVersion1, defaults1, constraints1, routeName1, handler1, (IEnumerable<string>) hostScopeTemplates, num != 0, deprecatedAtVersion1);
    }

    public static TeamFoundationHostType ConvertToHostType(TfsApiResourceScope scope)
    {
      TeamFoundationHostType hostType = TeamFoundationHostType.Unknown;
      if (scope.HasFlag((Enum) TfsApiResourceScope.Deployment))
        hostType |= TeamFoundationHostType.Deployment;
      if (scope.HasFlag((Enum) TfsApiResourceScope.Application))
        hostType |= TeamFoundationHostType.Application;
      if (scope.HasFlag((Enum) TfsApiResourceScope.Collection) || scope.HasFlag((Enum) TfsApiResourceScope.Project) || scope.HasFlag((Enum) TfsApiResourceScope.Team))
        hostType |= TeamFoundationHostType.ProjectCollection;
      return hostType;
    }
  }
}
