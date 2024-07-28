// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.InfoRefsPublicProjectRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Routing;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class InfoRefsPublicProjectRequestRestrictionsAttribute : 
    GitPublicProjectRequestRestrictionsAttribute
  {
    public InfoRefsPublicProjectRequestRestrictionsAttribute()
    {
    }

    public InfoRefsPublicProjectRequestRestrictionsAttribute(
      AuthenticationMechanisms mechanisms,
      UserAgentFilterType agentFilterType,
      string agentFilter)
      : base(mechanisms, agentFilterType, agentFilter)
    {
    }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      IWebRequestContextInternal requestContextInternal = requestContext.WebRequestContextInternal();
      HttpRequestBase request = requestContextInternal.HttpContext.Request;
      string str = request.QueryString["service"];
      if (str != null && str.Equals("git-upload-pack", StringComparison.Ordinal))
      {
        string routeValue1 = request.RequestContext.RouteData.GetRouteValue<string>("project");
        string routeValue2 = request.RequestContext.RouteData.GetRouteValue<string>("GitRepositoryName");
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repository = (ITfsGitRepository) null;
        try
        {
          if (service.TryFindRepositoryByName(requestContext, routeValue1, routeValue2, out repository))
          {
            if (!GitServerUtils.IsRequestOptimized(requestContextInternal.HttpContext, repository.Settings))
              return base.Allow(requestContext, routeValues);
          }
        }
        finally
        {
          repository?.Dispose();
        }
      }
      return new AllowPublicAccessResult(false, false, Guid.Empty);
    }
  }
}
