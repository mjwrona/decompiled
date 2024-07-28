// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Traceability.Server.WebAccessUrlBuilder
// Assembly: Microsoft.TeamFoundation.Traceability.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C62AF110-A283-470F-B32B-FE03F2A1E0D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Traceability.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Traceability.Server
{
  public static class WebAccessUrlBuilder
  {
    private const string WorkItemUrlRouteId = "ms.vss-work-web.work-items-form-route-with-id";

    public static string GetWorkItemWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      int? id)
    {
      if ((requestContext != null ? requestContext.RequestUri() : (Uri) null) == (Uri) null || !id.HasValue)
        return string.Empty;
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "project",
          (object) projectName
        },
        {
          nameof (id),
          (object) id.Value
        }
      };
      return new UriBuilder(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker))
      {
        Path = service.RouteUrl(requestContext, "ms.vss-work-web.work-items-form-route-with-id", new RouteValueDictionary((IDictionary<string, object>) dictionary))
      }.Uri.AbsoluteUri;
    }
  }
}
