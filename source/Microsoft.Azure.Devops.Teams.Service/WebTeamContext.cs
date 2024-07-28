// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.WebTeamContext
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;

namespace Microsoft.Azure.Devops.Teams.Service
{
  public class WebTeamContext : IWebTeamContext
  {
    public const string c_TeamParameterName = "teamId";
    public const string c_DefaultTeamRouteParameter = "teamName";

    internal WebTeamContext(IVssRequestContext requestContext, Guid? fallbackTeamId)
    {
      this.Project = this.GetProject(requestContext);
      this.Team = this.ResolveTeamFromUrlWithFallback(requestContext, fallbackTeamId);
    }

    internal WebTeamContext(IVssRequestContext requestContext, WebTeamContextResolveMode mode = WebTeamContextResolveMode.Default)
    {
      this.Project = this.GetProject(requestContext);
      this.Team = this.ResolveTeamFromUrl(requestContext, mode);
    }

    public ProjectInfo Project { get; private set; }

    public WebApiTeam Team { get; private set; }

    private ProjectInfo GetProject(IVssRequestContext requestContext) => requestContext.GetService<IRequestProjectService>().GetProject(requestContext) ?? throw new ProjectDoesNotExistException();

    private WebApiTeam ResolveTeamFromUrl(
      IVssRequestContext requestContext,
      WebTeamContextResolveMode resolveMode)
    {
      if (!resolveMode.HasFlag((Enum) WebTeamContextResolveMode.IgnoreGlobalTeamContext))
      {
        WebApiTeam teamContextTeam = this.GetTeamContextTeam(requestContext);
        if (teamContextTeam != null)
          return teamContextTeam;
      }
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      string str = service.GetRouteValue<string>(requestContext, "teamName");
      if (string.IsNullOrEmpty(str))
        str = service.GetQueryParameter(requestContext, "teamId");
      if (string.IsNullOrEmpty(str))
        str = service.GetRouteValue<string>(requestContext, "team");
      if (string.IsNullOrEmpty(str))
      {
        if (!resolveMode.HasFlag((Enum) WebTeamContextResolveMode.AllowEmptyTeam))
          throw new TeamNotFoundInUrlException();
        return (WebApiTeam) null;
      }
      WebApiTeam teamInProject = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, this.Project.Id, str);
      return teamInProject != null || resolveMode.HasFlag((Enum) WebTeamContextResolveMode.AllowEmptyTeam) ? teamInProject : throw new TeamNotFoundException(str);
    }

    private WebApiTeam ResolveTeamFromUrlWithFallback(
      IVssRequestContext requestContext,
      Guid? fallbackTeamId)
    {
      WebApiTeam webApiTeam = this.ResolveTeamFromUrl(requestContext, WebTeamContextResolveMode.AllowEmptyTeam);
      if (webApiTeam != null)
        return webApiTeam;
      if (!fallbackTeamId.HasValue || !(fallbackTeamId.Value != Guid.Empty))
        return (WebApiTeam) null;
      string teamIdOrName = fallbackTeamId.Value.ToString();
      return requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, this.Project.Id, teamIdOrName);
    }

    private WebApiTeam GetTeamContextTeam(IVssRequestContext requestContext)
    {
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext, false);
      return pageSource != null && this.Project != null && pageSource.Team != null ? requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, this.Project.Id, pageSource.Team.Id.ToString()) : (WebApiTeam) null;
    }
  }
}
