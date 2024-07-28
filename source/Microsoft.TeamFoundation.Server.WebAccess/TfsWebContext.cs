// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TfsWebContext : ProjectWebContext
  {
    private bool m_teamInitialized;
    private WebApiTeam m_team;
    private bool m_databaseInitialized;
    private ITeamFoundationDatabaseProperties m_database;
    private IEnumerable<TeamData> m_myTeams;
    private IEnumerable<WebApiTeam> m_projectTeams;
    private IEnumerable<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>> m_allProjects;
    private IEnumerable<TfsServiceHostDescriptor> m_allCollections;
    private Dictionary<Guid, CatalogNode> m_collectionCatalogNodes;
    private Dictionary<Guid, TfsServiceHostDescriptor> m_collectionHostsProperties;
    private Dictionary<Guid, List<CatalogNode>> m_projectCatalogNodesSorted;
    private Dictionary<string, CatalogNode> m_collectionByPath;
    private Uri m_currentProjectUri;
    private Guid m_currentProjectGuid;
    private bool? m_currentUserHasGlobalReadAccess;
    private TeamFoundationIdentity m_currentIdentity;
    private FeatureContext m_featureContext;
    private MRUNavigationContextEntry[] m_mruNavigationContexts;
    private bool? m_inDefaultTeamContext;
    private bool? m_userIsTeamAdmin;
    private bool? m_userIsTeamMember;
    public const string CLIENTHOST_QUERYPARAM_NAME = "clienthost";
    public const string CLIENTHOST_TEE = "tee";

    public TfsWebContext(RequestContext requestContext)
      : base(requestContext)
    {
    }

    public TfsWebContext()
    {
    }

    protected override void InitializeContext()
    {
      base.InitializeContext();
      if (string.IsNullOrEmpty(this.NavigationContext.Team))
        return;
      this.EnsureTeam();
    }

    public virtual FeatureContext FeatureContext
    {
      get
      {
        if (this.m_featureContext == null)
          this.m_featureContext = this.TfsRequestContext.FeatureContext();
        return this.m_featureContext;
      }
    }

    public void EnterMethod(MethodInformation methodInformation)
    {
      if (string.IsNullOrEmpty(this.TfsRequestContext.ServiceName))
        this.TfsRequestContext.ServiceName = "Web Access";
      this.TfsRequestContext.EnterMethod(methodInformation);
    }

    public TeamFoundationIdentity CurrentUserIdentity
    {
      get
      {
        if (this.m_currentIdentity == null)
          this.m_currentIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(this.TfsRequestContext);
        return this.m_currentIdentity;
      }
    }

    public virtual Uri CurrentProjectUri
    {
      get
      {
        if (this.m_currentProjectUri == (Uri) null && this.Project != null)
          this.m_currentProjectUri = new Uri(this.Project.Uri);
        return this.m_currentProjectUri;
      }
    }

    public Guid CurrentProjectGuid
    {
      get
      {
        if (this.m_currentProjectGuid == Guid.Empty && this.Project != null)
          this.m_currentProjectGuid = this.Project.Id;
        return this.m_currentProjectGuid;
      }
    }

    public virtual WebApiTeam Team
    {
      get
      {
        this.EnsureTeam();
        return this.m_team;
      }
    }

    public ITeamFoundationDatabaseProperties Database
    {
      get
      {
        if (!this.m_databaseInitialized)
        {
          this.m_database = (ITeamFoundationDatabaseProperties) null;
          if (((IEnumerable<string>) this.RequestContext.HttpContext.Request.QueryString.AllKeys).Contains<string>("databaseId"))
          {
            string s = this.RequestContext.HttpContext.Request.QueryString["databaseId"];
            int result;
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out result))
              this.m_database = this.TfsRequestContext.Elevate().GetService<TeamFoundationDatabaseManagementService>().GetDatabase(this.TfsRequestContext.Elevate(), result, true);
          }
          this.m_databaseInitialized = true;
        }
        return this.m_database;
      }
    }

    public bool CurrentUserHasTeamAdminPermission
    {
      get
      {
        bool teamAdminPermission = false;
        if (this.TeamContext != null && this.Team != null)
        {
          if (!this.m_userIsTeamAdmin.HasValue)
            this.m_userIsTeamAdmin = new bool?(this.TfsRequestContext.GetService<ITeamService>().UserIsTeamAdmin(this.TfsRequestContext, this.Team.Identity));
          teamAdminPermission = this.m_userIsTeamAdmin.Value;
        }
        return teamAdminPermission;
      }
    }

    public bool CurrentUserHasTeamPermission
    {
      get
      {
        bool hasTeamPermission = false;
        if (this.TeamContext != null && this.Team != null)
        {
          if (!this.m_userIsTeamMember.HasValue)
            this.m_userIsTeamMember = new bool?(this.TfsRequestContext.GetService<ITeamService>().UserHasPermission(this.TfsRequestContext, this.Team.Identity, 2));
          hasTeamPermission = this.m_userIsTeamMember.Value;
        }
        return hasTeamPermission;
      }
    }

    public bool InDefaultTeamContext
    {
      get
      {
        if (!this.m_inDefaultTeamContext.HasValue)
          this.m_inDefaultTeamContext = this.Team == null ? new bool?(false) : new bool?(object.Equals((object) this.Team.Id, (object) this.TfsRequestContext.GetService<ITeamService>().GetDefaultTeamId(this.TfsRequestContext, this.Project.Id)));
        return this.m_inDefaultTeamContext.Value;
      }
    }

    public bool CurrentUserHasGlobalReadAccess
    {
      get
      {
        if (!this.m_currentUserHasGlobalReadAccess.HasValue)
          this.m_currentUserHasGlobalReadAccess = new bool?(this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 1, false));
        return this.m_currentUserHasGlobalReadAccess.Value;
      }
    }

    public virtual MRUNavigationContextEntry[] MruNavigationContexts
    {
      get
      {
        if (this.m_mruNavigationContexts == null)
          this.m_mruNavigationContexts = MRUNavigationContextEntryManager.ReadMRUNavigationContexts(this);
        return this.m_mruNavigationContexts;
      }
    }

    private void EnsureTeam()
    {
      if (this.m_teamInitialized)
        return;
      if (CommonUtility.ShouldIgnoreTeamContext(this.TfsRequestContext))
      {
        this.m_teamInitialized = true;
      }
      else
      {
        if (this.Project != null)
        {
          if (!string.IsNullOrEmpty(this.NavigationContext.Team))
          {
            try
            {
              Guid result;
              if (Guid.TryParseExact(this.NavigationContext.Team, "D", out result))
              {
                try
                {
                  this.m_team = TfsHelpers.GetTeam(this.TfsRequestContext, this.Project.Id, result.ToString());
                }
                catch (TeamDoesNotExistWithNameException ex)
                {
                  this.m_team = TfsHelpers.GetTeam(this.TfsRequestContext, this.Project.Id, this.NavigationContext.Team);
                }
              }
              else
              {
                try
                {
                  this.m_team = TfsHelpers.GetTeam(this.TfsRequestContext, this.Project.Id, this.NavigationContext.Team);
                }
                catch (TeamDoesNotExistWithNameException ex)
                {
                  if (!this.Project.Name.Equals(this.NavigationContext.Project, StringComparison.OrdinalIgnoreCase) && this.TeamNameMatchesDefaultTeamName(this.NavigationContext.Team, this.NavigationContext.Project))
                    this.m_team = this.TfsRequestContext.GetService<ITeamService>().GetDefaultTeam(this.TfsRequestContext, this.Project.Id);
                  else
                    throw;
                }
              }
            }
            catch (TeamDoesNotExistWithNameException ex)
            {
              throw new HttpException(404, ex.Message, (Exception) ex);
            }
          }
          else
          {
            ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
            WebApiTeam defaultTeam = service.GetDefaultTeam(this.TfsRequestContext, this.Project.Id);
            if (!this.TfsRequestContext.IsFeatureEnabled("WebAccess.Core.Safeguard.TfsWebContextDefaultTeamNoFallback") && defaultTeam == null)
            {
              this.TfsRequestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Framework, (Exception) new Microsoft.Azure.Devops.Teams.Service.DefaultTeamNotFoundException(this.Project.Id.ToString()));
              service.QueryTeamsInProject(this.TfsRequestContext, this.Project.Id).OrderBy<WebApiTeam, string>((Func<WebApiTeam, string>) (t => t.Name)).FirstOrDefault<WebApiTeam>();
            }
          }
        }
        this.m_teamInitialized = true;
        if (this.m_team == null)
          return;
        this.TfsRequestContext.RootContext.Items["RequestTeam"] = (object) this.m_team;
      }
    }

    private bool TeamNameMatchesDefaultTeamName(string teamNameToCompare, string oldProjectName) => TFStringComparer.TeamProjectName.Equals(FrameworkResources.ProjectDefaultTeam((object) oldProjectName), teamNameToCompare);

    protected override TeamContext CreateTeamContext() => this.Team == null ? (TeamContext) null : (TeamContext) new TfsTeamContext(this.TfsRequestContext, this.Team);

    protected override UserContext CreateUserContext(Microsoft.VisualStudio.Services.Identity.Identity userIdentity) => (UserContext) new TfsUserContext(this.TfsRequestContext, userIdentity);

    public IEnumerable<TeamData> GetMyTeams()
    {
      if (this.m_myTeams == null)
      {
        if (!this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          this.m_myTeams = Enumerable.Empty<TeamData>();
        }
        else
        {
          IReadOnlyCollection<WebApiTeam> webApiTeams = this.TfsRequestContext.GetService<ITeamService>().QueryMyTeamsInCollection(this.TfsRequestContext, this.TfsRequestContext.UserContext);
          Guid instanceId = this.TfsRequestContext.ServiceHost.InstanceId;
          Dictionary<string, CatalogNode> dictionary = this.GetProjectCatalogNodes(instanceId).ToDictionary<CatalogNode, string>((Func<CatalogNode, string>) (pn => pn.Resource.Properties["ProjectUri"]));
          Dictionary<Guid, TfsServiceHostDescriptor> collectionProperties = this.GetCollectionProperties();
          List<TeamData> source = new List<TeamData>();
          foreach (WebApiTeam webApiTeam in (IEnumerable<WebApiTeam>) webApiTeams)
          {
            string projectUri = ProjectInfo.GetProjectUri(webApiTeam.ProjectId);
            CatalogNode projectCatalogNode;
            if (dictionary.TryGetValue(projectUri, out projectCatalogNode))
            {
              TeamData teamData = new TeamData()
              {
                Team = webApiTeam,
                ProjectInfo = TfsHelpers.ExtractProjectInfo(projectCatalogNode),
                ProjectCollection = collectionProperties[instanceId]
              };
              source.Add(teamData);
            }
          }
          this.m_myTeams = (IEnumerable<TeamData>) source.OrderBy<TeamData, string>((Func<TeamData, string>) (team => team.Team.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<TeamData>();
        }
      }
      return this.m_myTeams;
    }

    public virtual IEnumerable<WebApiTeam> GetProjectTeams()
    {
      if (!this.NavigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project))
        throw new InvalidOperationException("Invalid navigation level.");
      if (this.m_projectTeams == null)
        this.m_projectTeams = (IEnumerable<WebApiTeam>) this.TfsRequestContext.GetService<ITeamService>().QueryTeamsInProject(this.TfsRequestContext, this.CurrentProjectGuid).OrderBy<WebApiTeam, string>((Func<WebApiTeam, string>) (team => team.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<WebApiTeam>();
      return this.m_projectTeams;
    }

    private Dictionary<Guid, CatalogNode> GetCollectionCatalogNodes()
    {
      if (this.m_collectionCatalogNodes == null)
        this.InitCatalogInfo();
      return this.m_collectionCatalogNodes;
    }

    internal CatalogNode GetCollectionCatalogNode(Guid collectionId)
    {
      CatalogNode catalogNode;
      return this.GetCollectionCatalogNodes().TryGetValue(collectionId, out catalogNode) ? catalogNode : (CatalogNode) null;
    }

    internal CatalogNode GetProjectCatalogNode(Guid collectionId, string uri) => this.GetProjectCatalogNodes(collectionId).FirstOrDefault<CatalogNode>((Func<CatalogNode, bool>) (pn => TFStringComparer.ProjectUri.Equals(pn.Resource.Properties["ProjectUri"], uri)));

    internal IEnumerable<CatalogNode> GetProjectCatalogNodes(Guid collectionId)
    {
      if (this.m_projectCatalogNodesSorted == null)
        this.InitCatalogInfo();
      List<CatalogNode> catalogNodeList;
      return this.m_projectCatalogNodesSorted.TryGetValue(collectionId, out catalogNodeList) ? (IEnumerable<CatalogNode>) catalogNodeList : Enumerable.Empty<CatalogNode>();
    }

    internal Dictionary<Guid, TfsServiceHostDescriptor> GetCollectionProperties()
    {
      if (this.m_collectionHostsProperties == null)
        this.InitCatalogInfo();
      return this.m_collectionHostsProperties;
    }

    internal TfsServiceHostDescriptor GetCollectionProperties(Guid collectionId)
    {
      TfsServiceHostDescriptor serviceHostDescriptor;
      return this.GetCollectionProperties().TryGetValue(collectionId, out serviceHostDescriptor) ? serviceHostDescriptor : (TfsServiceHostDescriptor) null;
    }

    private void InitCatalogInfo()
    {
      IVssRequestContext vssRequestContext1 = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      List<CatalogNode> catalogNodeList1 = vssRequestContext1.GetService<ITeamFoundationCatalogService>().QueryNodes(vssRequestContext1, (IEnumerable<string>) new string[1]
      {
        "3eYRYkJOok6GHrKam0AcAA==**"
      }, (IEnumerable<Guid>) new Guid[2]
      {
        CatalogResourceTypes.ProjectCollection,
        CatalogResourceTypes.TeamProject
      }, CatalogQueryOptions.None);
      List<CatalogNode> catalogNodeList2 = new List<CatalogNode>();
      this.m_collectionCatalogNodes = new Dictionary<Guid, CatalogNode>();
      this.m_collectionByPath = new Dictionary<string, CatalogNode>();
      this.m_projectCatalogNodesSorted = new Dictionary<Guid, List<CatalogNode>>();
      foreach (CatalogNode catalogNode in catalogNodeList1)
      {
        if (catalogNode.Resource.ResourceTypeIdentifier.Equals(CatalogResourceTypes.ProjectCollection))
        {
          Guid key = new Guid(catalogNode.Resource.Properties["InstanceId"]);
          this.m_collectionByPath.Add(catalogNode.FullPath, catalogNode);
          this.m_collectionCatalogNodes.Add(key, catalogNode);
          this.m_projectCatalogNodesSorted.Add(key, new List<CatalogNode>());
        }
        else
        {
          CommonStructureProjectState result;
          if (catalogNode.Resource.Properties["ProjectState"].TryParseEnum<CommonStructureProjectState>(out result) && result == CommonStructureProjectState.WellFormed)
            catalogNodeList2.Add(catalogNode);
        }
      }
      foreach (CatalogNode catalogNode1 in catalogNodeList2)
      {
        CatalogNode catalogNode2;
        if (this.m_collectionByPath.TryGetValue(catalogNode1.ParentPath, out catalogNode2))
          this.m_projectCatalogNodesSorted[new Guid(catalogNode2.Resource.Properties["InstanceId"])].Add(catalogNode1);
      }
      this.m_collectionHostsProperties = new Dictionary<Guid, TfsServiceHostDescriptor>();
      IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment).Elevate();
      foreach (HostProperties hostProperties in vssRequestContext2.GetService<TeamFoundationHostManagementService>().QueryChildrenServiceHostPropertiesCached(vssRequestContext2, vssRequestContext1.ServiceHost.InstanceId))
      {
        if (this.m_collectionCatalogNodes.Keys.Contains<Guid>(hostProperties.Id))
        {
          TfsServiceHostDescriptor serviceHostDescriptor = new TfsServiceHostDescriptor(hostProperties, hostProperties.VirtualPath(this.TfsRequestContext));
          this.m_collectionHostsProperties.Add(hostProperties.Id, serviceHostDescriptor);
        }
      }
    }

    public IEnumerable<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>> GetMyProjects() => (IEnumerable<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>) this.GetMyTeams().GroupBy<TeamData, ProjectInfo>((Func<TeamData, ProjectInfo>) (ti => ti.ProjectInfo)).OrderBy<IGrouping<ProjectInfo, TeamData>, string>((Func<IGrouping<ProjectInfo, TeamData>, string>) (g => g.Key.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).Select<IGrouping<ProjectInfo, TeamData>, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>((Func<IGrouping<ProjectInfo, TeamData>, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>) (group => new KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>(group.Key, group.First<TeamData>().ProjectCollection))).OrderBy<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>, string>((Func<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>, string>) (pair => pair.Key.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase);

    public IEnumerable<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>> GetAllProjects()
    {
      if (this.m_allProjects == null)
        this.m_allProjects = (IEnumerable<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>) this.GetCollectionProperties().Values.Where<TfsServiceHostDescriptor>((Func<TfsServiceHostDescriptor, bool>) (tpcp => tpcp.Status == TeamFoundationServiceHostStatus.Started)).SelectMany<TfsServiceHostDescriptor, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>((Func<TfsServiceHostDescriptor, IEnumerable<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>>) (tpc => this.GetProjectCatalogNodes(tpc.Id).Select<CatalogNode, ProjectInfo>((Func<CatalogNode, ProjectInfo>) (pn => TfsHelpers.ExtractProjectInfo(pn))).Where<ProjectInfo>((Func<ProjectInfo, bool>) (pinfo => pinfo.State == ProjectState.WellFormed)).Select<ProjectInfo, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>((Func<ProjectInfo, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>) (pinfo => new KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>(pinfo, tpc))))).OrderBy<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>, string>((Func<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>, string>) (pair => pair.Key.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>();
      return this.m_allProjects;
    }

    public IEnumerable<TfsServiceHostDescriptor> GetMyCollections() => (IEnumerable<TfsServiceHostDescriptor>) this.GetMyProjects().GroupBy<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>, TfsServiceHostDescriptor>((Func<KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>, TfsServiceHostDescriptor>) (pair => pair.Value)).Select<IGrouping<TfsServiceHostDescriptor, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>, TfsServiceHostDescriptor>((Func<IGrouping<TfsServiceHostDescriptor, KeyValuePair<ProjectInfo, TfsServiceHostDescriptor>>, TfsServiceHostDescriptor>) (g => g.Key)).OrderBy<TfsServiceHostDescriptor, string>((Func<TfsServiceHostDescriptor, string>) (tpc => tpc.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase);

    public virtual IEnumerable<TfsServiceHostDescriptor> GetAllCollections()
    {
      if (this.m_allCollections == null)
        this.m_allCollections = (IEnumerable<TfsServiceHostDescriptor>) this.GetCollectionProperties().Values.OrderBy<TfsServiceHostDescriptor, string>((Func<TfsServiceHostDescriptor, string>) (tpc => tpc.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<TfsServiceHostDescriptor>();
      return this.m_allCollections;
    }

    public int WebApiVersionClient
    {
      get
      {
        string s = this.RequestContext.HttpContext.Request["__v"];
        int result;
        return !string.IsNullOrEmpty(s) && int.TryParse(s, out result) ? result : 1;
      }
    }

    internal void RemoveMRUNavigationEntry(int entryToRemoveHashCode)
    {
      MRUNavigationContextEntry[] array = ((IEnumerable<MRUNavigationContextEntry>) this.MruNavigationContexts).Where<MRUNavigationContextEntry>((Func<MRUNavigationContextEntry, bool>) (entry => entry.GetHashCode() != entryToRemoveHashCode)).ToArray<MRUNavigationContextEntry>();
      if (array.Length == this.MruNavigationContexts.Length)
        return;
      MRUNavigationContextEntryManager.WriteMRUNavigationContexts(this.TfsRequestContext, (IEnumerable<MRUNavigationContextEntry>) array);
    }

    internal void RefreshCurrentUserIdentity() => this.m_currentIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(this.TfsRequestContext);

    protected override void HandleProjectRename(string requestProjectName, ProjectInfo project) => this.RequestProjectName = requestProjectName;

    public string RequestProjectName { get; private set; }
  }
}
