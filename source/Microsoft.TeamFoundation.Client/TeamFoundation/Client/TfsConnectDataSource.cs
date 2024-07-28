// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConnectDataSource
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  internal class TfsConnectDataSource : BaseDataSource
  {
    private TfsConnectDataSource.ServerNode m_selectedServer;
    private Dictionary<TfsConnectDataSource.TeamProjectCollectionNode, ObservableCollection<TfsConnectDataSource.TeamProjectNode>> m_selectedProjectsCache;
    private Dictionary<TfsConnectDataSource.ServerNode, TfsConnectDataSource.ContainerNode> m_selectedDirectoryNodeCache;
    private ObservableCollection<TfsConnectDataSource.ServerNode> m_registedServers;
    private TfsBackgroundWorkerManager m_workerManager;
    private bool m_canSelectServer;
    private bool m_canSelectProject;
    private bool m_canSelectMultipleProjects;
    private List<string> m_defaultSelectedProjects;
    private ITeamProjectPickerDefaultSelectionProvider m_defaultsProvider;
    private Guid? m_defaultCollectionId;
    private SortedList m_defaultSelectedProjectsList;

    public TfsConnectDataSource(TeamProjectPickerMode mode, bool disableCollectionSelect)
    {
      this.m_canSelectMultipleProjects = mode == TeamProjectPickerMode.MultiProject;
      this.m_canSelectProject = mode != 0;
      this.m_canSelectServer = !disableCollectionSelect;
      this.m_workerManager = new TfsBackgroundWorkerManager();
      this.m_selectedProjectsCache = new Dictionary<TfsConnectDataSource.TeamProjectCollectionNode, ObservableCollection<TfsConnectDataSource.TeamProjectNode>>();
      this.m_selectedDirectoryNodeCache = new Dictionary<TfsConnectDataSource.ServerNode, TfsConnectDataSource.ContainerNode>();
      this.FillServerList();
    }

    public TfsConnectDataSource.ServerNode AddServer(TfsConnection server)
    {
      TfsConnectDataSource.ServerNode serverNode = new TfsConnectDataSource.ServerNode(server, this.m_workerManager, this.CanSelectProject);
      this.RegisteredServers.Add(serverNode);
      return serverNode;
    }

    public void SetSelectedCollection(TfsTeamProjectCollection server)
    {
      this.ClearUserSelections();
      if (server == null || !server.HasAuthenticated)
      {
        this.SelectedServer = (TfsConnectDataSource.ServerNode) null;
        this.m_defaultCollectionId = new Guid?();
      }
      else
      {
        Uri applicationInstanceUri = TFUtil.GetApplicationInstanceUri((TfsConnection) server);
        foreach (TfsConnectDataSource.ServerNode registeredServer in (Collection<TfsConnectDataSource.ServerNode>) this.RegisteredServers)
        {
          if (UriUtility.Equals(registeredServer.Server.Uri, applicationInstanceUri))
          {
            this.SelectedServer = registeredServer;
            break;
          }
        }
        this.m_defaultCollectionId = new Guid?(server.InstanceId);
      }
    }

    public void SetDefaultSelectionProvider(
      ITeamProjectPickerDefaultSelectionProvider provider)
    {
      this.ClearUserSelections();
      this.m_defaultsProvider = provider;
      this.m_defaultCollectionId = new Guid?();
      this.SelectedServer = (TfsConnectDataSource.ServerNode) null;
      if (provider == null)
        return;
      Uri defaultServerUri = provider.GetDefaultServerUri();
      if (!(defaultServerUri != (Uri) null))
        return;
      foreach (TfsConnectDataSource.ServerNode registeredServer in (Collection<TfsConnectDataSource.ServerNode>) this.RegisteredServers)
      {
        if (UriUtility.Equals(registeredServer.Server.Uri, defaultServerUri))
        {
          this.SelectedServer = registeredServer;
          break;
        }
      }
    }

    public void SetDefaultSelectedProjects(Microsoft.TeamFoundation.Server.ProjectInfo[] projects)
    {
      this.ClearUserSelections();
      this.m_defaultSelectedProjects = new List<string>();
      foreach (Microsoft.TeamFoundation.Server.ProjectInfo project in projects)
        this.m_defaultSelectedProjects.Add(project.Uri);
    }

    public void Dispose() => this.m_workerManager.Shutdown();

    public SortedList DefaultSelectedProjectsList
    {
      get => this.m_defaultSelectedProjectsList;
      set
      {
        this.ClearUserSelections();
        this.m_defaultSelectedProjectsList = value;
      }
    }

    public TfsTeamProjectCollection ActiveServer { get; set; }

    public bool CanSelectServer => this.m_canSelectServer;

    public bool CanSelectProject => this.m_canSelectProject;

    public bool CanSelectMultipleProjects => this.m_canSelectMultipleProjects;

    public ObservableCollection<TfsConnectDataSource.TeamProjectNode> SelectedProjects
    {
      get
      {
        if (this.SelectedCollection == null || this.SelectedCollection.State != RetrievablePropertyState.Retrieved)
          return (ObservableCollection<TfsConnectDataSource.TeamProjectNode>) null;
        ObservableCollection<TfsConnectDataSource.TeamProjectNode> projectNodes;
        if (!this.m_selectedProjectsCache.TryGetValue(this.SelectedCollection, out projectNodes))
        {
          projectNodes = new ObservableCollection<TfsConnectDataSource.TeamProjectNode>();
          this.m_selectedProjectsCache[this.SelectedCollection] = projectNodes;
          if (this.m_defaultCollectionId.HasValue && this.m_defaultCollectionId.Value == this.SelectedCollection.Id && this.m_defaultSelectedProjects != null)
            this.AddListedProjectsByUris(projectNodes, (IEnumerable<string>) this.m_defaultSelectedProjects);
          else if (this.DefaultSelectedProjectsList != null)
          {
            if (this.DefaultSelectedProjectsList.ContainsKey((object) this.SelectedCollection.Server.Uri.AbsoluteUri.ToLowerInvariant()))
              this.AddListedProjectsByUris(projectNodes, (IEnumerable<string>) (this.DefaultSelectedProjectsList[(object) this.SelectedCollection.Server.Uri.AbsoluteUri.ToLowerInvariant()] as IList<string>));
          }
          else if (this.m_defaultsProvider != null)
          {
            IEnumerable<string> defaultProjects = this.m_defaultsProvider.GetDefaultProjects(this.SelectedCollection.Id);
            if (defaultProjects != null)
              this.AddListedProjectsByUris(projectNodes, defaultProjects);
          }
        }
        return projectNodes;
      }
    }

    public ObservableCollection<TfsConnectDataSource.ServerNode> RegisteredServers => this.m_registedServers;

    public TfsConnectDataSource.ServerNode SelectedServer
    {
      get => this.m_selectedServer;
      set
      {
        this.m_selectedServer = value;
        this.OnPropertyChanged(nameof (SelectedServer));
      }
    }

    public TfsConnectDataSource.TeamProjectCollectionNode SelectedCollection
    {
      get
      {
        if (this.SelectedServer == null || this.SelectedServer.State != RetrievablePropertyState.Retrieved)
          return (TfsConnectDataSource.TeamProjectCollectionNode) null;
        return this.SelectedServer.Count == 1 ? this.SelectedServer[0] as TfsConnectDataSource.TeamProjectCollectionNode : this.SelectedDirectoryNode as TfsConnectDataSource.TeamProjectCollectionNode;
      }
    }

    public TfsConnectDataSource.ContainerNode SelectedDirectoryNode
    {
      get
      {
        if (this.SelectedServer == null || this.SelectedServer.State != RetrievablePropertyState.Retrieved)
          return (TfsConnectDataSource.ContainerNode) null;
        TfsConnectDataSource.ContainerNode selectedDirectoryNode = (TfsConnectDataSource.ContainerNode) null;
        if (!this.m_selectedDirectoryNodeCache.TryGetValue(this.SelectedServer, out selectedDirectoryNode))
        {
          if (this.m_defaultCollectionId.HasValue)
            selectedDirectoryNode = (TfsConnectDataSource.ContainerNode) this.GetTPCbyGuid((TfsConnectDataSource.ContainerNode) this.SelectedServer, this.m_defaultCollectionId.Value);
          if (this.m_defaultsProvider != null && selectedDirectoryNode == null)
          {
            Guid? defaultCollectionId = this.m_defaultsProvider.GetDefaultCollectionId(TFUtil.GetApplicationInstanceUri(this.SelectedServer.Server));
            if (defaultCollectionId.HasValue)
              selectedDirectoryNode = (TfsConnectDataSource.ContainerNode) this.GetTPCbyGuid((TfsConnectDataSource.ContainerNode) this.SelectedServer, defaultCollectionId.Value);
          }
          this.m_selectedDirectoryNodeCache[this.SelectedServer] = selectedDirectoryNode;
        }
        return selectedDirectoryNode;
      }
      set
      {
        this.m_selectedDirectoryNodeCache[this.SelectedServer] = value;
        this.OnPropertyChanged("SelectedCollection");
        this.OnPropertyChanged("SelectedCollectionUrl");
        this.OnPropertyChanged(nameof (SelectedDirectoryNode));
      }
    }

    private void ClearUserSelections()
    {
      this.m_selectedDirectoryNodeCache.Clear();
      this.m_selectedProjectsCache.Clear();
    }

    private void AddListedProjectsByUris(
      ObservableCollection<TfsConnectDataSource.TeamProjectNode> projectNodes,
      IEnumerable<string> projectsUris)
    {
      foreach (TfsConnectDataSource.TeamProjectNode selected in (Collection<TfsConnectDataSource.INode>) this.SelectedCollection)
      {
        string uri = selected.Info.Uri;
        foreach (string projectsUri in projectsUris)
        {
          if (TFStringComparer.ProjectUri.Equals(projectsUri, uri))
          {
            projectNodes.Add(selected);
            break;
          }
        }
      }
    }

    private TfsConnectDataSource.TeamProjectCollectionNode GetTPCbyGuid(
      TfsConnectDataSource.ContainerNode rootNode,
      Guid guid)
    {
      foreach (TfsConnectDataSource.ContainerNode containerNode in (Collection<TfsConnectDataSource.INode>) rootNode)
      {
        if (containerNode is TfsConnectDataSource.TeamProjectCollectionNode tpCbyGuid && tpCbyGuid.Id == guid)
          return tpCbyGuid;
      }
      return (TfsConnectDataSource.TeamProjectCollectionNode) null;
    }

    private void FillServerList()
    {
      this.m_registedServers = new ObservableCollection<TfsConnectDataSource.ServerNode>();
      List<TfsConnection> tfsConnectionList = new List<TfsConnection>();
      foreach (RegisteredConfigurationServer configurationServer in RegisteredTfsConnections.GetConfigurationServers())
        tfsConnectionList.Add((TfsConnection) TfsConfigurationServerFactory.GetConfigurationServer(configurationServer));
      foreach (RegisteredProjectCollection projectCollection in RegisteredTfsConnections.GetLegacyProjectCollections())
        tfsConnectionList.Add((TfsConnection) TfsTeamProjectCollectionFactory.GetTeamProjectCollection(projectCollection));
      foreach (TfsConnection server in tfsConnectionList)
        this.m_registedServers.Add(new TfsConnectDataSource.ServerNode(server, this.m_workerManager, this.CanSelectProject));
    }

    private interface IStateChanger
    {
      void SetState(RetrievablePropertyState newState);
    }

    public interface INode
    {
      string Name { get; }
    }

    public abstract class ContainerNode : 
      RetrievableCollection<TfsConnectDataSource.INode>,
      TfsConnectDataSource.INode,
      TfsConnectDataSource.IStateChanger
    {
      private string m_name;
      private TfsBackgroundWorkerManager m_workerManager;

      public ContainerNode(string displayName, TfsBackgroundWorkerManager manager)
      {
        this.m_workerManager = manager;
        this.m_name = displayName;
      }

      public void Refresh()
      {
        if (!(this.GetWorker() is TfsConnectDataSource.ContainerNodeWorker) || this.State == RetrievablePropertyState.Working)
          return;
        this.Clear();
        this.m_workerManager.RegisterWorker(this.GetWorker());
        this.State = RetrievablePropertyState.Working;
        this.m_workerManager.CancelAllQueued();
        this.m_workerManager.RunWorker((object) this, (object) this, TfsBackgroundWorkerPriority.Highest);
      }

      public string Name => this.m_name;

      public TfsBackgroundWorkerManager WorkerManager => this.m_workerManager;

      public override string ToString() => this.Name;

      protected abstract TfsBaseWorker GetWorker();

      void TfsConnectDataSource.IStateChanger.SetState(RetrievablePropertyState newState) => this.State = newState;
    }

    public class ServerNode : TfsConnectDataSource.ContainerNode
    {
      public ServerNode(
        TfsConnection server,
        TfsBackgroundWorkerManager manager,
        bool retrieveProjects)
        : base(server.Name, manager)
      {
        this.RetrieveProjects = retrieveProjects;
        this.Server = server;
        this.IsLegacyServer = server is TfsTeamProjectCollection;
      }

      public bool IsLegacyServer { get; private set; }

      public TfsConnection Server { get; set; }

      public bool RetrieveProjects { get; private set; }

      public bool HasNewCredentials { get; set; }

      public TfsConnection CredentialsSwitchingServer { get; set; }

      public override RetrievablePropertyState State
      {
        get => base.State;
        protected set => base.State = value;
      }

      internal void RefreshWithNewCredentials()
      {
        this.CredentialsSwitchingServer = (TfsConnection) TfsConfigurationServerManager.GetServerForSwitchUser(this.Server.Uri, this.Server.IdentityToImpersonate);
        this.Refresh();
      }

      protected override TfsBaseWorker GetWorker() => (TfsBaseWorker) new TfsConnectDataSource.GetServerDetailsWorker(this);
    }

    public class TeamProjectCollectionNode : TfsConnectDataSource.ContainerNode
    {
      public TeamProjectCollectionNode(string name, TfsBackgroundWorkerManager manager)
        : base(name, manager)
      {
      }

      public TfsConnectDataSource.ServerNode ParentServerNode { get; set; }

      public CatalogNode CatalogNode { get; set; }

      public TfsTeamProjectCollection Server { get; set; }

      public Guid Id => this.ParentServerNode.IsLegacyServer ? this.ParentServerNode.Server.InstanceId : new Guid(this.CatalogNode.Resource.Properties["InstanceId"]);

      internal void GetTeamProjectCollection() => this.Server = ((TfsConfigurationServer) this.ParentServerNode.Server).GetTeamProjectCollection(this.Id);

      protected override TfsBaseWorker GetWorker() => this.ParentServerNode.IsLegacyServer ? (TfsBaseWorker) new TfsConnectDataSource.LegacyProjectsRetriever(this) : (TfsBaseWorker) new TfsConnectDataSource.DefaultProjectsRetriever(this);
    }

    public class TeamProjectNode : TfsConnectDataSource.INode
    {
      private Microsoft.TeamFoundation.Server.ProjectInfo m_info;

      public TeamProjectNode(Microsoft.TeamFoundation.Server.ProjectInfo proj) => this.m_info = proj;

      public string Name => this.m_info.Name;

      public Microsoft.TeamFoundation.Server.ProjectInfo Info => this.m_info;

      public override string ToString() => this.Name;
    }

    private abstract class ContainerNodeWorker : TfsBaseWorker
    {
      public ContainerNodeWorker(TfsConnectDataSource.ContainerNode node)
        : base((object) node)
      {
      }

      public override sealed object DoWork(object argument, CancelEventArgs e) => this.RetrieveChildren();

      public override sealed void WorkCompleted(
        object argument,
        object result,
        AsyncCompletedEventArgs e)
      {
        TfsConnectDataSource.ContainerNode containerNode = argument as TfsConnectDataSource.ContainerNode;
        if (e.Error != null)
          containerNode.Error = e.Error;
        else if (e.Cancelled)
        {
          ((TfsConnectDataSource.IStateChanger) containerNode).SetState(RetrievablePropertyState.Uninitialized);
        }
        else
        {
          this.FillChildren(result);
          ((TfsConnectDataSource.IStateChanger) containerNode).SetState(RetrievablePropertyState.Retrieved);
        }
      }

      protected abstract object RetrieveChildren();

      protected abstract void FillChildren(object result);
    }

    private class LegacyProjectsRetriever : TfsConnectDataSource.ContainerNodeWorker
    {
      public LegacyProjectsRetriever(
        TfsConnectDataSource.TeamProjectCollectionNode node)
        : base((TfsConnectDataSource.ContainerNode) node)
      {
      }

      protected override object RetrieveChildren()
      {
        TfsConnectDataSource.TeamProjectCollectionNode identifier = this.Identifier as TfsConnectDataSource.TeamProjectCollectionNode;
        if (!identifier.ParentServerNode.RetrieveProjects)
          return (object) Array.Empty<Microsoft.TeamFoundation.Server.ProjectInfo>();
        Microsoft.TeamFoundation.Server.ProjectInfo[] projectInfoArray = ((ICommonStructureService) identifier.ParentServerNode.Server.GetService(typeof (ICommonStructureService))).ListProjects();
        List<Uri> uriList = new List<Uri>();
        foreach (Microsoft.TeamFoundation.Server.ProjectInfo projectInfo in projectInfoArray)
          uriList.Add(new Uri(projectInfo.Uri));
        return (object) projectInfoArray;
      }

      protected override void FillChildren(object result)
      {
        TfsConnectDataSource.TeamProjectCollectionNode identifier = this.Identifier as TfsConnectDataSource.TeamProjectCollectionNode;
        foreach (Microsoft.TeamFoundation.Server.ProjectInfo proj in result as Microsoft.TeamFoundation.Server.ProjectInfo[])
        {
          TfsConnectDataSource.TeamProjectNode teamProjectNode = new TfsConnectDataSource.TeamProjectNode(proj);
          identifier.Add((TfsConnectDataSource.INode) teamProjectNode);
        }
      }
    }

    private class DefaultProjectsRetriever : TfsConnectDataSource.ContainerNodeWorker
    {
      public DefaultProjectsRetriever(
        TfsConnectDataSource.TeamProjectCollectionNode node)
        : base((TfsConnectDataSource.ContainerNode) node)
      {
      }

      protected override object RetrieveChildren()
      {
        TfsConnectDataSource.TeamProjectCollectionNode identifier = this.Identifier as TfsConnectDataSource.TeamProjectCollectionNode;
        CatalogNode catalogNode1 = identifier.CatalogNode;
        identifier.GetTeamProjectCollection();
        identifier.Server.ClientCredentials.PromptType = CredentialPromptType.PromptIfNeeded;
        identifier.Server.Authenticate();
        CatalogNode catalogNode2 = identifier.Server.CatalogNode;
        if (!identifier.ParentServerNode.RetrieveProjects)
          return (object) new List<TfsConnectDataSource.TeamProjectNode>(0);
        Guid[] resourceTypeFilters = new Guid[1]
        {
          TeamProjectCatalogConstants.ResourceType
        };
        ReadOnlyCollection<CatalogNode> readOnlyCollection = catalogNode1.QueryChildren((IEnumerable<Guid>) resourceTypeFilters, true, CatalogQueryOptions.None);
        List<TfsConnectDataSource.TeamProjectNode> teamProjectNodeList = new List<TfsConnectDataSource.TeamProjectNode>(readOnlyCollection.Count);
        List<Uri> uriList = new List<Uri>();
        foreach (CatalogNode catalogNode3 in readOnlyCollection)
        {
          if (string.Equals(catalogNode3.Resource.Properties["ProjectState"], "WellFormed", StringComparison.OrdinalIgnoreCase))
          {
            Microsoft.TeamFoundation.Server.ProjectInfo proj = new Microsoft.TeamFoundation.Server.ProjectInfo();
            proj.Name = catalogNode3.Resource.DisplayName;
            proj.Uri = catalogNode3.Resource.Properties["ProjectUri"];
            TfsConnectDataSource.TeamProjectNode teamProjectNode = new TfsConnectDataSource.TeamProjectNode(proj);
            teamProjectNodeList.Add(teamProjectNode);
            uriList.Add(new Uri(proj.Uri));
          }
        }
        return (object) teamProjectNodeList;
      }

      protected override void FillChildren(object result)
      {
        TfsConnectDataSource.TeamProjectCollectionNode identifier = this.Identifier as TfsConnectDataSource.TeamProjectCollectionNode;
        foreach (TfsConnectDataSource.TeamProjectNode teamProjectNode in result as List<TfsConnectDataSource.TeamProjectNode>)
          identifier.Add((TfsConnectDataSource.INode) teamProjectNode);
      }
    }

    private class GetServerDetailsWorker : TfsConnectDataSource.ContainerNodeWorker
    {
      public GetServerDetailsWorker(TfsConnectDataSource.ServerNode serverNode)
        : base((TfsConnectDataSource.ContainerNode) serverNode)
      {
      }

      protected override object RetrieveChildren()
      {
        TfsConnectDataSource.ServerNode identifier = this.Identifier as TfsConnectDataSource.ServerNode;
        TfsConnection tfsConnection = identifier.CredentialsSwitchingServer ?? identifier.Server;
        tfsConnection.ClientCredentials.PromptType = CredentialPromptType.PromptIfNeeded;
        tfsConnection.Authenticate();
        if (identifier.IsLegacyServer)
          return (object) new List<TfsConnectDataSource.ContainerNode>()
          {
            (TfsConnectDataSource.ContainerNode) new TfsConnectDataSource.TeamProjectCollectionNode(ClientResources.TfsConnectDialogLegacyServer((object) tfsConnection.Name), identifier.WorkerManager)
            {
              ParentServerNode = identifier,
              Server = (tfsConnection as TfsTeamProjectCollection)
            }
          };
        tfsConnection.GetService(typeof (ICatalogService));
        Guid[] resourceTypeFilters = new Guid[1]
        {
          CatalogResourceTypes.ProjectCollection
        };
        CatalogNode catalogNode = tfsConnection.CatalogNode;
        return catalogNode != null ? (object) this.PopulateServerHierarchy(catalogNode.QueryChildren((IEnumerable<Guid>) resourceTypeFilters, true, CatalogQueryOptions.None)) : (object) new List<TfsConnectDataSource.ContainerNode>();
      }

      private List<TfsConnectDataSource.ContainerNode> PopulateServerHierarchy(
        ReadOnlyCollection<CatalogNode> children)
      {
        List<TfsConnectDataSource.ContainerNode> containerNodeList = new List<TfsConnectDataSource.ContainerNode>();
        TfsConnectDataSource.ServerNode identifier = this.Identifier as TfsConnectDataSource.ServerNode;
        Dictionary<string, TfsConnectDataSource.ContainerNode> dictionary = new Dictionary<string, TfsConnectDataSource.ContainerNode>();
        foreach (CatalogNode child in children)
          containerNodeList.Add((TfsConnectDataSource.ContainerNode) new TfsConnectDataSource.TeamProjectCollectionNode(child.Resource.DisplayName, identifier.WorkerManager)
          {
            ParentServerNode = identifier,
            CatalogNode = child
          });
        return containerNodeList;
      }

      protected override void FillChildren(object result)
      {
        TfsConnectDataSource.ServerNode identifier = this.Identifier as TfsConnectDataSource.ServerNode;
        foreach (TfsConnectDataSource.ContainerNode containerNode in result as List<TfsConnectDataSource.ContainerNode>)
          identifier.Add((TfsConnectDataSource.INode) containerNode);
      }
    }
  }
}
