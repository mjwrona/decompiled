// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class TfsHelpers
  {
    public static Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext TfsWebContext(
      this RequestContext requestContext)
    {
      return WebContextFactory.GetWebContext(requestContext) as Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext;
    }

    public static bool TryGetTfsWebContext(out Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext)
    {
      tfsWebContext = WebContextFactory.GetCurrentRequestWebContext<Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext>(false);
      return tfsWebContext != null;
    }

    public static Microsoft.TeamFoundation.Server.WebAccess.FeatureContext FeatureContext(
      this IVssRequestContext tfsRequestContext)
    {
      return TfsHelpers.Implementation.Instance.FeatureContext(tfsRequestContext);
    }

    public static Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext TfsWebContext(
      this ViewContext viewContext)
    {
      return viewContext.RequestContext.TfsWebContext();
    }

    public static IEnumerable<ProjectInfo> GetCollectionProjects(
      IVssRequestContext tfsRequestContext,
      Guid collectionGuid)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      ArgumentUtility.CheckForEmptyGuid(collectionGuid, nameof (collectionGuid));
      using (IVssRequestContext tfsRequestContext1 = tfsRequestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(tfsRequestContext, collectionGuid, RequestContextType.UserContext, true, true))
        return TfsHelpers.GetCollectionProjects(tfsRequestContext1, true);
    }

    public static IEnumerable<ProjectInfo> GetCollectionProjects(
      IVssRequestContext tfsRequestContext)
    {
      return TfsHelpers.GetCollectionProjects(tfsRequestContext, true);
    }

    public static IEnumerable<ProjectInfo> GetCollectionProjects(
      IVssRequestContext tfsRequestContext,
      bool wellFormedProjectsOnly)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      IProjectService service = tfsRequestContext.GetService<IProjectService>();
      IEnumerable<ProjectInfo> source = !wellFormedProjectsOnly ? service.GetProjects(tfsRequestContext.Elevate()) : service.GetProjects(tfsRequestContext.Elevate(), ProjectState.WellFormed);
      IVssSecurityNamespace securityNamespace = tfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(tfsRequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      return (IEnumerable<ProjectInfo>) new SortedSet<ProjectInfo>(source.Where<ProjectInfo>((Func<ProjectInfo, bool>) (pi => securityNamespace.HasPermission(tfsRequestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(tfsRequestContext, securityNamespace, pi.Uri), securityNamespace.Description.ReadPermission))), (IComparer<ProjectInfo>) ProjectComparer.Instance);
    }

    public static ProjectInfo ExtractProjectInfo(CatalogNode projectCatalogNode)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(projectCatalogNode, nameof (projectCatalogNode));
      return TfsHelpers.ExtractProjectInfo(projectCatalogNode.Resource);
    }

    public static ProjectInfo ExtractProjectInfo(CatalogResource projectCatalogResource)
    {
      ArgumentUtility.CheckForNull<CatalogResource>(projectCatalogResource, nameof (projectCatalogResource));
      return new ProjectInfo(ProjectInfo.GetProjectId(projectCatalogResource.Properties["ProjectUri"]), projectCatalogResource.Properties["ProjectName"], projectCatalogResource.Properties["ProjectState"].ParseEnum<ProjectState>(), description: projectCatalogResource.Description);
    }

    internal static Guid GetCollectionGuid(IVssRequestContext tfsRequestContext, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      IVssRequestContext vssRequestContext = tfsRequestContext.To(TeamFoundationHostType.Application);
      List<CatalogResource> catalogResourceList = vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryResources(vssRequestContext, (IEnumerable<Guid>) new Guid[1]
      {
        CatalogResourceTypes.TeamProject
      }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("ProjectUri", projectUri.ToString())
      }, CatalogQueryOptions.IncludeParents);
      return catalogResourceList.Count == 0 ? Guid.Empty : new Guid(catalogResourceList[0].NodeReferences[0].ParentNode.Resource.Properties["InstanceId"]);
    }

    internal static Guid GetCollectionGuid(IVssRequestContext tfsRequestContext, string projectName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      IVssRequestContext vssRequestContext = tfsRequestContext.To(TeamFoundationHostType.Application);
      List<CatalogResource> catalogResourceList = vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryResources(vssRequestContext, (IEnumerable<Guid>) new Guid[1]
      {
        CatalogResourceTypes.TeamProject
      }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("ProjectName", projectName)
      }, CatalogQueryOptions.IncludeParents);
      return catalogResourceList.Count == 0 ? Guid.Empty : new Guid(catalogResourceList[0].NodeReferences[0].ParentNode.Resource.Properties["InstanceId"]);
    }

    public static WebApiTeam GetTeam(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string teamName)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "TfsHelpers.GetTeamByName"))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectGuid, nameof (projectGuid));
        ArgumentUtility.CheckStringForNullOrEmpty(teamName, nameof (teamName));
        WebApiTeam teamInProject = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectGuid, teamName);
        if (teamInProject == null)
        {
          requestContext.Trace(520035, TraceLevel.Error, "WebAccess", "Controller", "Team (" + teamName + ") does not exist. ProjectGuid provided was " + projectGuid.ToString());
          throw new TeamDoesNotExistWithNameException(teamName);
        }
        return teamInProject;
      }
    }

    public static bool IsHosted(this IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    public static bool IsDeploymentAdmin(this IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationIdentityService service = vssRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(vssRequestContext, new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      })[0];
      return requestContext.UserContext != (IdentityDescriptor) null && service.IsMember(vssRequestContext, readIdentity.Descriptor, requestContext.UserContext);
    }

    public static bool IsAccountAdmin(this IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application).Elevate();
      TeamFoundationIdentityService service = vssRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(vssRequestContext, new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      })[0];
      return requestContext.UserContext != (IdentityDescriptor) null && service.IsMember(vssRequestContext, readIdentity.Descriptor, requestContext.UserContext);
    }

    public static Guid GetProjectGuid(CommonStructureProjectInfo project) => new Guid(LinkingUtilities.DecodeUri(project.Uri.Trim()).ToolSpecificId);

    public static IdentityFavorites TeamFavoriteStore(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext,
      string scope)
    {
      IWebTeamContext webTeamContext = tfsWebContext.TfsRequestContext.GetWebTeamContext();
      if (webTeamContext.Team == null)
        throw new InvalidOperationException(WACommonResources.TeamContextNotFound);
      return tfsWebContext.FavoriteStore(new Guid?(webTeamContext.Team.Id), scope);
    }

    public static IdentityFavorites MyFavoriteStore(this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext, string scope) => tfsWebContext.FavoriteStore(tfsWebContext.TfsRequestContext.GetUserId(), tfsWebContext.Project, (WebApiTeam) null, scope);

    public static IdentityFavorites FavoritesStore(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext,
      WebApiTeam team,
      string scope)
    {
      ProjectInfo projectFromId = TfsProjectHelpers.GetProjectFromId(tfsWebContext.TfsRequestContext, team.ProjectId);
      return tfsWebContext.FavoriteStore(team.Id, projectFromId, team, scope);
    }

    public static IdentityFavorites FavoriteStore(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext,
      Guid? identityId,
      string scope)
    {
      IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
      if (!identityId.HasValue)
        identityId = new Guid?(tfsRequestContext.GetUserId());
      return tfsWebContext.FavoriteStore(identityId.Value, tfsWebContext.Project, tfsWebContext.Team, scope);
    }

    public static IdentityFavorites FavoriteStore(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext,
      Guid identityId,
      ProjectInfo project,
      WebApiTeam team,
      string scope)
    {
      bool flag = true;
      IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
      StringBuilder stringBuilder = new StringBuilder();
      if (project != null)
      {
        stringBuilder.Append('.');
        stringBuilder.Append(project.Id.ToString());
        bool result;
        if (!bool.TryParse(tfsWebContext.RequestContext.HttpContext.Request["ignoreDefaultTeam"], out result))
          result = false;
        if (team != null && !result)
        {
          stringBuilder.Append('.');
          stringBuilder.Append(team.Id.ToString());
          flag = false;
        }
      }
      if (!string.IsNullOrWhiteSpace(scope))
      {
        stringBuilder.Append('.');
        stringBuilder.Append(scope);
      }
      Guid identityId1 = identityId;
      string namespaceSuffix = stringBuilder.ToString();
      IdentityFavorites view = IdentityPropertiesView.CreateView<IdentityFavorites>(tfsRequestContext, identityId1, namespaceSuffix);
      FavoriteViewScopeInformation scopeInformation = new FavoriteViewScopeInformation();
      scopeInformation.IsPersonal = flag;
      scopeInformation.FeatureScope = scope;
      if (project != null)
        scopeInformation.ProjectGuid = project.Id;
      view.ScopeInformation = scopeInformation;
      return view;
    }

    public static IEnumerable<HostProperties> GetAccessibleCollections(
      IVssRequestContext tfsRequestContext)
    {
      IVssRequestContext vssRequestContext1 = tfsRequestContext.To(TeamFoundationHostType.Application);
      List<CatalogNode> catalogNodeList = vssRequestContext1.GetService<ITeamFoundationCatalogService>().QueryNodes(vssRequestContext1, CatalogPath.MakeRecursive(CatalogPath.OrganizationalPath, true), CatalogResourceTypes.ProjectCollection, (IEnumerable<KeyValuePair<string, string>>) null, CatalogQueryOptions.None);
      IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationHostManagementService service = vssRequestContext2.GetService<TeamFoundationHostManagementService>();
      List<HostProperties> accessibleCollections = new List<HostProperties>();
      foreach (CatalogNode catalogNode in catalogNodeList)
      {
        Guid hostId = new Guid(catalogNode.Resource.Properties["InstanceId"]);
        HostProperties hostProperties = service.QueryServiceHostPropertiesCached(vssRequestContext2, hostId);
        if (hostProperties != null && hostProperties.Status == TeamFoundationServiceHostStatus.Started)
          accessibleCollections.Add(hostProperties);
      }
      return (IEnumerable<HostProperties>) accessibleCollections;
    }

    public static bool IsFeatureAvailable(this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext, string featureId) => TfsHelpers.Implementation.Instance.IsFeatureAvailable(tfsWebContext, featureId);

    public class Implementation
    {
      private static TfsHelpers.Implementation s_instance;

      protected Implementation()
      {
      }

      public static TfsHelpers.Implementation Instance
      {
        get
        {
          if (TfsHelpers.Implementation.s_instance == null)
            TfsHelpers.Implementation.s_instance = new TfsHelpers.Implementation();
          return TfsHelpers.Implementation.s_instance;
        }
        internal set => TfsHelpers.Implementation.s_instance = value;
      }

      public virtual bool IsFeatureAvailable(Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext, string featureId) => tfsWebContext.TfsRequestContext.IsFeatureEnabled(featureId);

      public virtual IVssRequestContext TfsRequestContext(
        HttpContextBase httpContext,
        bool required)
      {
        return WebPlatformHelpers.TfsRequestContext(httpContext, required);
      }

      public virtual Microsoft.TeamFoundation.Server.WebAccess.FeatureContext FeatureContext(
        IVssRequestContext tfsRequestContext)
      {
        object obj;
        if (!tfsRequestContext.Items.TryGetValue("TfsFeatureContext", out obj))
        {
          obj = (object) new Microsoft.TeamFoundation.Server.WebAccess.FeatureContext(tfsRequestContext);
          tfsRequestContext.Items["TfsFeatureContext"] = obj;
        }
        return obj as Microsoft.TeamFoundation.Server.WebAccess.FeatureContext;
      }
    }
  }
}
