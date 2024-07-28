// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.NavigationContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class NavigationContext : WebSdkMetadata
  {
    private const string c_legacyRouteId = "LegacyWebAccessRoute";
    private NavigationContextLevels m_levels;
    private NavigationContextLevels m_topMostLevel;
    private RequestContext m_requestContext;
    private IVssRequestContext m_tfsRequestContext;
    private TeamFoundationServiceHostModel m_serviceHost;

    public NavigationContext()
    {
    }

    public NavigationContext(IVssRequestContext tfsRequestContext, RequestContext requestContext)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      this.m_requestContext = requestContext;
      this.m_tfsRequestContext = tfsRequestContext;
      RouteData routeData = this.m_requestContext.RouteData;
      if (routeData != null)
      {
        this.Area = routeData.GetRouteArea();
        this.CurrentController = this.GetRouteValue(tfsRequestContext, routeData, "originalController");
        if (this.CurrentController == null)
          this.CurrentController = this.GetRouteValue(tfsRequestContext, routeData, "controller");
        this.CurrentAction = this.GetRouteValue(tfsRequestContext, routeData, "action");
        this.CurrentParameters = this.GetRouteValue(tfsRequestContext, routeData, "parameters");
        str1 = this.GetRouteValue(tfsRequestContext, routeData, "project");
        str2 = this.GetRouteValue(tfsRequestContext, routeData, "team");
      }
      switch (this.m_tfsRequestContext.IntendedHostType())
      {
        case TeamFoundationHostType.Deployment:
          this.m_levels = NavigationContextLevels.Deployment;
          this.m_topMostLevel = NavigationContextLevels.Deployment;
          break;
        case TeamFoundationHostType.Application:
          this.m_levels = NavigationContextLevels.Deployment | NavigationContextLevels.Application;
          this.m_topMostLevel = NavigationContextLevels.Application;
          break;
        case TeamFoundationHostType.ProjectCollection:
          this.m_levels = NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection;
          this.m_topMostLevel = NavigationContextLevels.Collection;
          this.Project = str1;
          if (string.IsNullOrEmpty(this.Project))
            break;
          this.m_levels |= NavigationContextLevels.Project;
          this.m_topMostLevel = NavigationContextLevels.Project;
          if (CommonUtility.ShouldIgnoreTeamContext(tfsRequestContext) || string.IsNullOrEmpty(str2))
            break;
          this.Team = str2;
          this.m_levels |= NavigationContextLevels.Team;
          this.m_topMostLevel = NavigationContextLevels.Team;
          break;
      }
    }

    public void UpdateResolvedRoute()
    {
      IContributionRoutingService service = this.m_tfsRequestContext.GetService<IContributionRoutingService>();
      IContributedRoute route = service.GetRoute(this.m_tfsRequestContext);
      if (route != null)
      {
        this.RouteTemplates = route.Templates;
        this.RouteId = route.ContributionId;
      }
      else
        this.RouteId = "LegacyWebAccessRoute";
      this.RouteValues = new Dictionary<string, string>();
      IDictionary<string, object> routeValues = service.GetRouteValues(this.m_tfsRequestContext);
      if (routeValues == null)
        return;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) routeValues)
      {
        if (keyValuePair.Value != null)
          this.RouteValues[keyValuePair.Key] = keyValuePair.Value.ToString();
      }
    }

    private string GetRouteValue(
      IVssRequestContext requestContext,
      RouteData routeData,
      string key)
    {
      return routeData.GetRouteValue<string>(key) ?? requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, key);
    }

    public NavigationContextLevels Levels => this.m_levels;

    [DataMember]
    public NavigationContextLevels TopMostLevel
    {
      get => this.m_topMostLevel;
      set
      {
        this.m_topMostLevel = value;
        switch (value)
        {
          case NavigationContextLevels.Deployment:
            this.m_levels = NavigationContextLevels.Deployment;
            break;
          case NavigationContextLevels.Application:
            this.m_levels = NavigationContextLevels.Deployment | NavigationContextLevels.Application;
            break;
          case NavigationContextLevels.Collection:
            this.m_levels = NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection;
            break;
          case NavigationContextLevels.Project:
            this.m_levels = NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection | NavigationContextLevels.Project;
            break;
          case NavigationContextLevels.Team:
            this.m_levels = NavigationContextLevels.All;
            break;
        }
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Area { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CurrentController { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CurrentAction { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CurrentParameters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CommandName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RouteId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string[] RouteTemplates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> RouteValues { get; set; }

    public Guid InstanceId => this.m_tfsRequestContext.ServiceHost.InstanceId;

    public TeamFoundationServiceHostModel ServiceHost
    {
      get
      {
        if (this.m_serviceHost == null)
          this.m_serviceHost = new TeamFoundationServiceHostModel(this.m_tfsRequestContext);
        return this.m_serviceHost;
      }
    }

    public string Project { get; private set; }

    public string Team { get; private set; }

    public void SetProjectName(string newProjectName)
    {
      this.Project = newProjectName;
      this.m_requestContext.RouteData.Values["project"] = (object) newProjectName;
    }

    public string GetLevelId(bool includeArea = true)
    {
      string str = string.Empty;
      if (includeArea && !string.IsNullOrEmpty(this.Area))
        str = str + this.Area + "-";
      return str + this.TopMostLevel.ToString();
    }
  }
}
