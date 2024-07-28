// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamExplorerProjects
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamExplorerProjects
  {
    private string m_fileName;
    private bool m_autoLoad;
    private Uri m_defaultServerUri;
    private Dictionary<Uri, TeamExplorerProjects.ServerSettings> m_servers;
    private Dictionary<Guid, TeamExplorerProjects.CollectionSettings> m_collections;
    private object m_dataLock = new object();
    private static TeamExplorerProjects m_teamExplorerProject;
    private const string cTEFolderName = "Team Explorer";
    private const string cTEConfigFileName = "TeamExplorer.config";
    private const string cServerListElement = "server_list";
    private const string cServerElement = "server";
    private const string cCollectionElement = "collection";
    private const string cQueryElement = "query";
    private const string cProjectElement = "project";
    private const string cActiveAttribute = "active";
    private const string cTeamAttribute = "team";
    private const string cCurrentAttribute = "current";
    private const string cAutoloadAttribute = "autoload";
    private const string cGuidAttribute = "guid";
    private const string cUrlAttribute = "url";
    private const string cProjectUriAttribute = "projectUri";
    private const string cNameAttribute = "name";
    private const string cIsHostedAttribute = "isHosted";
    private const string cCapFlagsSccAttribute = "capFlagsScc";
    private const string cSupportsGitAttribute = "supportsGit";
    private const string cSupportsTFVCAttribute = "supportsTFVC";
    private const string cRepositoryElement = "repository";
    private const string cRepositoryTypeAttribute = "type";
    private const string cRepositoryIsForkAttribute = "isFork";
    private const string cRepositoryActiveCloneAttribute = "activeClone";
    private const string cYes = "yes";
    private const string cTFHostConfigWriteError = "TFHost.ErrorWritingTEConfigFile";

    protected TeamExplorerProjects()
    {
      this.m_defaultServerUri = (Uri) null;
      this.m_autoLoad = false;
      this.m_collections = new Dictionary<Guid, TeamExplorerProjects.CollectionSettings>();
      this.m_servers = new Dictionary<Uri, TeamExplorerProjects.ServerSettings>(UriUtility.AbsoluteUriStringComparer);
      if (TeamExplorerProjects.m_teamExplorerProject != null)
        return;
      TeamExplorerProjects.m_teamExplorerProject = this;
    }

    private TeamExplorerProjects(string fileName)
      : this()
    {
      this.m_fileName = fileName;
      this.LoadFile();
    }

    public static TeamExplorerProjects Instance
    {
      get
      {
        if (TeamExplorerProjects.m_teamExplorerProject == null)
        {
          string str = Path.Combine(UIHost.VsApplicationDataPath, "Team Explorer");
          if (!Directory.Exists(str))
            Directory.CreateDirectory(str);
          TeamExplorerProjects.m_teamExplorerProject = new TeamExplorerProjects(Path.Combine(str, "TeamExplorer.config"));
        }
        return TeamExplorerProjects.m_teamExplorerProject;
      }
    }

    public ITeamProjectPickerDefaultSelectionProvider GetDefaultSelectionProvider(
      TeamProjectPickerMode mode)
    {
      return (ITeamProjectPickerDefaultSelectionProvider) new TeamExplorerProjects.DefaultsProvider(this, mode);
    }

    public string DefaultCollectionUrl
    {
      get
      {
        lock (this.m_dataLock)
        {
          if (this.m_defaultServerUri == (Uri) null)
            return (string) null;
          TeamExplorerProjects.ServerSettings serverSettings;
          if (this.m_servers.TryGetValue(this.m_defaultServerUri, out serverSettings))
          {
            Guid selectedCollectionId = serverSettings.SelectedCollectionId;
            if (selectedCollectionId != Guid.Empty)
              return UriUtility.GetInvariantAbsoluteUri(this.m_collections[selectedCollectionId].Uri);
          }
        }
        return (string) null;
      }
    }

    public Guid DefaultCollectionId
    {
      get
      {
        lock (this.m_dataLock)
        {
          TeamExplorerProjects.ServerSettings serverSettings;
          if (this.m_servers.TryGetValue(this.m_defaultServerUri, out serverSettings))
            return serverSettings.SelectedCollectionId;
        }
        return Guid.Empty;
      }
    }

    public bool AutoLoad
    {
      get
      {
        lock (this.m_dataLock)
          return this.m_autoLoad;
      }
      set
      {
        lock (this.m_dataLock)
          this.m_autoLoad = value;
      }
    }

    public void Save()
    {
      try
      {
        lock (this.m_dataLock)
          this.WriteConfig();
      }
      catch (Exception ex)
      {
        if (ex is IOException)
          return;
        TeamFoundationTrace.Error("Exception while saving TeamExplorer.config", ex);
        int num = (int) UIHost.ShowError(ClientResources.TFHost_ErrorWritingTEConfigFile(), "TFHost.ErrorWritingTEConfigFile", (string) null);
      }
    }

    public bool TryGetCollectionId(string collectionUrl, out Guid collectionId)
    {
      collectionId = Guid.Empty;
      try
      {
        lock (this.m_dataLock)
        {
          Uri uri2 = new Uri(collectionUrl);
          foreach (TeamExplorerProjects.CollectionSettings collectionSettings in this.m_collections.Values)
          {
            if (UriUtility.Equals(collectionSettings.Uri, uri2))
            {
              collectionId = collectionSettings.Id;
              return true;
            }
          }
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    public List<Guid> GetCollectionIds()
    {
      lock (this.m_dataLock)
        return this.m_collections.Keys.ToList<Guid>();
    }

    public Dictionary<string, string> GetCollectionProperties(Guid collectionId)
    {
      Dictionary<string, string> collectionProperties = new Dictionary<string, string>();
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          collectionProperties["CollectionIsHosted"] = collectionSettings.IsHosted.ToString();
          collectionProperties["CollectionName"] = collectionSettings.Name;
          collectionProperties["CollectionUri"] = collectionSettings.Uri.AbsoluteUri;
        }
      }
      return collectionProperties;
    }

    public List<string> GetProjectsForCollection(Guid collectionId)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return new List<string>(collectionSettings.SelectedProjects.Select<TeamExplorerProjects.ProjectSettings, string>((Func<TeamExplorerProjects.ProjectSettings, string>) (ps => ps.Uri.AbsoluteUri)));
      }
      return new List<string>();
    }

    public void SetProjectsForCollection(TfsTeamProjectCollection tpc, IEnumerable<Uri> projectUris)
    {
      IEnumerable<string> projectUris1 = projectUris.Select<Uri, string>((Func<Uri, string>) (t => t.AbsoluteUri));
      this.SetProjectsForCollection(tpc, projectUris1);
    }

    public void SetProjectsForCollection(
      TfsTeamProjectCollection tpc,
      IEnumerable<string> projectUris)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings = this.GetOrCreateCollectionSettings(tpc);
        List<TeamExplorerProjects.ProjectSettings> collection = new List<TeamExplorerProjects.ProjectSettings>();
        foreach (string projectUri in projectUris)
        {
          TeamExplorerProjects.ProjectSettings projectSettings = this.FindProject(collectionSettings, projectUri) ?? new TeamExplorerProjects.ProjectSettings(new Uri(projectUri));
          collection.Add(projectSettings);
        }
        collectionSettings.SelectedProjects.Clear();
        collectionSettings.SelectedProjects.AddRange((IEnumerable<TeamExplorerProjects.ProjectSettings>) collection);
        if (this.FindProject(collectionSettings, collectionSettings.ActiveProject) != null)
          return;
        collectionSettings.ActiveProject = (string) null;
        collectionSettings.ActiveTeam = Guid.Empty;
      }
    }

    public void AddProjectForCollection(TfsTeamProjectCollection tpc, string projectUri)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings = this.GetOrCreateCollectionSettings(tpc);
        if (this.FindProject(collectionSettings, projectUri) != null)
          return;
        collectionSettings.SelectedProjects.Add(new TeamExplorerProjects.ProjectSettings(projectUri));
      }
    }

    public void RemoveProjectForCollection(Guid collectionId, string projectUri)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (!this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return;
        int projectIndex = this.FindProjectIndex(collectionSettings, projectUri);
        if (projectIndex < 0)
          return;
        collectionSettings.SelectedProjects.RemoveAt(projectIndex);
        if (TFStringComparer.ProjectUri.Compare(collectionSettings.ActiveProject, projectUri) != 0)
          return;
        collectionSettings.ActiveProject = (string) null;
        collectionSettings.ActiveTeam = Guid.Empty;
      }
    }

    public bool SetProjectProperties(
      Guid collectionId,
      string projectUri,
      Dictionary<string, string> properties)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
          if (project != null)
          {
            string a;
            if (properties.TryGetValue("ProjectName", out a))
              project.Name = a;
            if (properties.TryGetValue("ProjectCapFlagsScc", out a))
              project.CapFlagsScc = a;
            if (properties.TryGetValue("ProjectSupportsGit", out a))
              project.SupportsGit = string.Equals(a, bool.TrueString, StringComparison.OrdinalIgnoreCase);
            if (properties.TryGetValue("ProjectSupportsTFVC", out a))
              project.SupportsTFVC = string.Equals(a, bool.TrueString, StringComparison.OrdinalIgnoreCase);
            return true;
          }
        }
      }
      return false;
    }

    public Dictionary<string, string> GetProjectProperties(Guid collectionId, string projectUri)
    {
      Dictionary<string, string> projectProperties = new Dictionary<string, string>();
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
          if (project != null)
          {
            projectProperties["ProjectCapFlagsScc"] = project.CapFlagsScc;
            projectProperties["ProjectSupportsGit"] = project.SupportsGit ? bool.TrueString : bool.FalseString;
            projectProperties["ProjectSupportsTFVC"] = project.SupportsTFVC ? bool.TrueString : bool.FalseString;
            projectProperties["ProjectName"] = project.Name;
            projectProperties["ProjectUri"] = project.Uri.AbsoluteUri;
            if (TFStringComparer.ProjectUri.Compare(projectUri, collectionSettings.ActiveProject) == 0)
              projectProperties["TeamId"] = collectionSettings.ActiveTeam.ToString();
          }
        }
      }
      return projectProperties;
    }

    public Guid? GetDefaultQueryGuid(Guid collectionId, string projectUri)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          Guid guid;
          if (collectionSettings.DefaultQueries.TryGetValue(projectUri, out guid))
            return new Guid?(guid);
        }
      }
      return new Guid?();
    }

    public void SetDefaultQueryGuid(
      TfsTeamProjectCollection tpc,
      string projectUri,
      Guid queryGuid)
    {
      lock (this.m_dataLock)
      {
        this.GetOrCreateCollectionSettings(tpc).DefaultQueries[projectUri] = queryGuid;
        this.AddProjectForCollection(tpc, projectUri);
      }
    }

    public string GetActiveProjectForCollection(Guid collectionId)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          if (collectionSettings.ActiveProject == null)
            collectionSettings.ActiveProject = collectionSettings.SelectedProjects.Count > 0 ? collectionSettings.SelectedProjects[0].Uri.AbsoluteUri : (string) null;
          return collectionSettings.ActiveProject;
        }
      }
      return (string) null;
    }

    public string GetActiveProjectForCollection(string collectionUrl)
    {
      lock (this.m_dataLock)
      {
        Guid collectionId;
        if (this.TryGetCollectionId(collectionUrl, out collectionId))
          return this.GetActiveProjectForCollection(collectionId);
      }
      return (string) null;
    }

    public Guid GetActiveTeamForCollection(Guid collectionId)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return collectionSettings.ActiveTeam;
      }
      return Guid.Empty;
    }

    public Guid GetActiveTeamForCollection(string collectionUrl)
    {
      lock (this.m_dataLock)
      {
        Guid collectionId;
        if (this.TryGetCollectionId(collectionUrl, out collectionId))
          return this.GetActiveTeamForCollection(collectionId);
      }
      return Guid.Empty;
    }

    public void SetActiveProjectForCollection(TfsTeamProjectCollection tpc, string projectUri) => this.SetActiveProjectForCollection(tpc, projectUri, Guid.Empty);

    public void SetActiveProjectForCollection(
      TfsTeamProjectCollection tpc,
      string projectUri,
      Guid teamId)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings = this.GetOrCreateCollectionSettings(tpc);
        if (!string.IsNullOrEmpty(projectUri) && this.FindProject(collectionSettings, projectUri) == null)
          collectionSettings.SelectedProjects.Add(new TeamExplorerProjects.ProjectSettings(projectUri));
        collectionSettings.ActiveProject = projectUri;
        collectionSettings.ActiveTeam = teamId;
      }
    }

    public void SetDefaultCollection(TfsTeamProjectCollection tpc)
    {
      lock (this.m_dataLock)
      {
        if (tpc == null)
        {
          this.m_defaultServerUri = (Uri) null;
          this.m_autoLoad = false;
        }
        else
        {
          TeamExplorerProjects.CollectionSettings collectionSettings1 = this.GetOrCreateCollectionSettings(tpc);
          Uri applicationInstanceUri = TFUtil.GetApplicationInstanceUri((TfsConnection) tpc);
          if (this.m_servers.ContainsKey(applicationInstanceUri))
          {
            this.m_servers[applicationInstanceUri].SelectedCollectionId = collectionSettings1.Id;
          }
          else
          {
            TeamFoundationTrace.Info("Corrupt TE.config file");
            this.m_servers.Clear();
            this.m_collections.Clear();
            TeamExplorerProjects.CollectionSettings collectionSettings2 = this.GetOrCreateCollectionSettings(tpc);
            this.m_servers[applicationInstanceUri].SelectedCollectionId = collectionSettings2.Id;
          }
          this.m_defaultServerUri = applicationInstanceUri;
        }
      }
    }

    public List<Guid> GetRepositoriesForProject(
      Guid collectionId,
      string projectUri,
      string repositoryType)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
          if (project != null)
          {
            IEnumerable<TeamExplorerProjects.RepositorySettings> source = (IEnumerable<TeamExplorerProjects.RepositorySettings>) project.SelectedRepositories;
            if (!string.IsNullOrEmpty(repositoryType))
              source = source.Where<TeamExplorerProjects.RepositorySettings>((Func<TeamExplorerProjects.RepositorySettings, bool>) (r => r.SourceControlType == repositoryType));
            return source.Select<TeamExplorerProjects.RepositorySettings, Guid>((Func<TeamExplorerProjects.RepositorySettings, Guid>) (repo => repo.Id)).ToList<Guid>();
          }
        }
      }
      return new List<Guid>();
    }

    public List<Guid> GetRepositoriesForProject(Guid collectionId, string projectUri) => this.GetRepositoriesForProject(collectionId, projectUri, (string) null);

    public List<Guid> GetActiveRepositoriesForProject(Guid collectionId, string projectUri)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
          if (project != null)
            return new List<Guid>((IEnumerable<Guid>) project.ActiveRepositories);
        }
      }
      return new List<Guid>();
    }

    public List<Guid> GetActiveRepositoriesForProject(string collectionUrl, string projectUri)
    {
      lock (this.m_dataLock)
      {
        Guid collectionId;
        if (this.TryGetCollectionId(collectionUrl, out collectionId))
          return this.GetActiveRepositoriesForProject(collectionId, projectUri);
      }
      return new List<Guid>();
    }

    public void SetRepositoriesForProject(
      Guid collectionId,
      string projectUri,
      IEnumerable<TeamExplorerProjects.RepositorySettings> repositories)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (!this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return;
        TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
        if (project == null)
          return;
        foreach (TeamExplorerProjects.RepositorySettings repository in repositories)
        {
          TeamExplorerProjects.RepositorySettings repositorySettings = this.FindRepository(project, repository.Id);
          if (repositorySettings != null)
            repositorySettings.Name = repository.Name;
          else
            repositorySettings = new TeamExplorerProjects.RepositorySettings(repository.Id, repository.SourceControlType, repository.Name, repository.IsFork);
          project.SelectedRepositories.Add(repositorySettings);
        }
        this.CleanActiveRepositories(project);
      }
    }

    public void RemoveRepositoriesForProjectByType(
      Guid collectionId,
      string projectUri,
      string repositoryType)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (!this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return;
        this.FindProject(collectionSettings, projectUri)?.SelectedRepositories.RemoveAll((Predicate<TeamExplorerProjects.RepositorySettings>) (repo => repo.SourceControlType == repositoryType));
      }
    }

    private void CleanActiveRepositories(TeamExplorerProjects.ProjectSettings ps)
    {
      for (int index = ps.ActiveRepositories.Count - 1; index >= 0; --index)
      {
        if (this.FindRepository(ps, ps.ActiveRepositories[index]) == null)
          ps.ActiveRepositories.RemoveAt(index);
      }
    }

    public void AddRepositoryForProject(
      Guid collectionId,
      string projectUri,
      Guid repositoryId,
      string repositoryType,
      string repositoryName,
      bool? isFork = null)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (!this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return;
        TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
        if (project == null)
          return;
        TeamExplorerProjects.RepositorySettings repository = this.FindRepository(project, repositoryId);
        if (repository != null)
        {
          repository.Name = repositoryName;
          repository.IsFork = isFork;
        }
        else
        {
          TeamExplorerProjects.RepositorySettings repositorySettings = new TeamExplorerProjects.RepositorySettings(repositoryId, repositoryType, repositoryName, isFork);
          project.SelectedRepositories.Add(repositorySettings);
        }
      }
    }

    public void RemoveRepositoryForProject(Guid collectionId, string projectUri, Guid repositoryId)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (!this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return;
        TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
        if (project == null)
          return;
        int repositoryIndex = this.FindRepositoryIndex(project, repositoryId);
        if (repositoryIndex < 0)
          return;
        project.SelectedRepositories.RemoveAt(repositoryIndex);
        project.ActiveRepositories.Remove(repositoryId);
      }
    }

    public void SetActiveRepositoriesForProject(
      Guid collectionId,
      string projectUri,
      IEnumerable<Guid> repositoryIds)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (!this.m_collections.TryGetValue(collectionId, out collectionSettings))
          return;
        TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
        if (project == null)
          return;
        project.ActiveRepositories.Clear();
        string b = (string) null;
        foreach (Guid repositoryId in repositoryIds)
        {
          TeamExplorerProjects.RepositorySettings repository = this.FindRepository(project, repositoryId);
          if (repository != null)
          {
            if (b == null)
              b = repository.SourceControlType;
            else if (!string.Equals(repository.SourceControlType, b))
              continue;
            if (!project.ActiveRepositories.Contains(repositoryId))
              project.ActiveRepositories.Add(repositoryId);
          }
        }
      }
    }

    public Dictionary<string, string> GetRepositoryProperties(
      Guid collectionId,
      string projectUri,
      Guid repositoryId)
    {
      Dictionary<string, string> repositoryProperties = new Dictionary<string, string>();
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
          if (project != null)
          {
            TeamExplorerProjects.RepositorySettings repository = this.FindRepository(project, repositoryId);
            if (repository != null)
            {
              repositoryProperties["RepositoryType"] = repository.SourceControlType;
              repositoryProperties["RepositoryId"] = repository.Id.ToString();
              repositoryProperties["RepositoryName"] = repository.Name;
              repositoryProperties["RepositoryActiveClone"] = repository.ActiveClone;
              bool? isFork = repository.IsFork;
              if (isFork.HasValue)
              {
                Dictionary<string, string> dictionary = repositoryProperties;
                isFork = repository.IsFork;
                string str = isFork.ToString();
                dictionary["RepositoryIsFork"] = str;
              }
            }
          }
        }
      }
      return repositoryProperties;
    }

    public bool SetRepositoryProperties(
      Guid collectionId,
      string projectUri,
      Guid repositoryId,
      Dictionary<string, string> properties)
    {
      lock (this.m_dataLock)
      {
        TeamExplorerProjects.CollectionSettings collectionSettings;
        if (this.m_collections.TryGetValue(collectionId, out collectionSettings))
        {
          TeamExplorerProjects.ProjectSettings project = this.FindProject(collectionSettings, projectUri);
          if (project != null)
          {
            TeamExplorerProjects.RepositorySettings repository = this.FindRepository(project, repositoryId);
            if (repository != null)
            {
              string str1;
              if (properties.TryGetValue("RepositoryName", out str1))
                repository.Name = str1;
              if (properties.TryGetValue("RepositoryActiveClone", out str1))
                repository.ActiveClone = str1;
              string str2;
              if (properties.TryGetValue("RepositoryIsFork", out str2))
                repository.IsFork = new bool?(bool.TrueString.Equals(str2, StringComparison.OrdinalIgnoreCase));
              return true;
            }
          }
        }
      }
      return false;
    }

    private TeamExplorerProjects.CollectionSettings GetOrCreateCollectionSettings(
      TfsTeamProjectCollection tpc)
    {
      Guid instanceId = tpc.InstanceId;
      TeamExplorerProjects.CollectionSettings collectionSettings1;
      if (!this.m_collections.TryGetValue(instanceId, out collectionSettings1))
      {
        bool flag = false;
        foreach (TeamExplorerProjects.CollectionSettings collectionSettings2 in this.m_collections.Values)
        {
          if (UriUtility.Equals(collectionSettings2.Uri, tpc.Uri))
          {
            flag = true;
            collectionSettings1 = collectionSettings2;
            this.m_collections.Remove(collectionSettings1.Id);
            foreach (TeamExplorerProjects.ServerSettings serverSettings in this.m_servers.Values)
            {
              if (serverSettings.Collections.Contains(collectionSettings1.Id))
              {
                serverSettings.Collections.Remove(collectionSettings1.Id);
                if (serverSettings.SelectedCollectionId == collectionSettings1.Id)
                  serverSettings.SelectedCollectionId = instanceId;
                serverSettings.Collections.Add(instanceId);
              }
            }
            collectionSettings1.Id = instanceId;
            this.m_collections[instanceId] = collectionSettings1;
            break;
          }
        }
        if (!flag)
        {
          Uri applicationInstanceUri = TFUtil.GetApplicationInstanceUri((TfsConnection) tpc);
          TeamExplorerProjects.ServerSettings serverSettings;
          if (!this.m_servers.TryGetValue(applicationInstanceUri, out serverSettings))
          {
            serverSettings = new TeamExplorerProjects.ServerSettings(applicationInstanceUri);
            this.m_servers[applicationInstanceUri] = serverSettings;
          }
          collectionSettings1 = new TeamExplorerProjects.CollectionSettings(instanceId, tpc.Uri);
          collectionSettings1.Name = tpc.Name;
          collectionSettings1.IsHosted = tpc.IsHostedServer;
          serverSettings.Collections.Add(instanceId);
          this.m_collections[instanceId] = collectionSettings1;
        }
      }
      else if (!UriUtility.Equals(collectionSettings1.Uri, tpc.Uri))
      {
        collectionSettings1.Uri = tpc.Uri;
        Uri applicationInstanceUri = TFUtil.GetApplicationInstanceUri((TfsConnection) tpc);
        TeamExplorerProjects.ServerSettings serverSettings;
        if (!this.m_servers.TryGetValue(applicationInstanceUri, out serverSettings))
        {
          this.RemoveCollectionFromServersCache(collectionSettings1.Id);
          serverSettings = new TeamExplorerProjects.ServerSettings(applicationInstanceUri);
          this.m_servers[applicationInstanceUri] = serverSettings;
          serverSettings.Collections.Add(instanceId);
        }
        else if (!serverSettings.Collections.Contains(collectionSettings1.Id))
        {
          this.RemoveCollectionFromServersCache(collectionSettings1.Id);
          serverSettings.Collections.Add(collectionSettings1.Id);
        }
      }
      collectionSettings1.Name = tpc.Name;
      collectionSettings1.IsHosted = tpc.IsHostedServer;
      return collectionSettings1;
    }

    private void RemoveCollectionFromServersCache(Guid collectionId)
    {
      foreach (TeamExplorerProjects.ServerSettings serverSettings in this.m_servers.Values)
      {
        if (serverSettings.Collections.Contains(collectionId))
        {
          serverSettings.Collections.Remove(collectionId);
          if (serverSettings.SelectedCollectionId == collectionId)
            serverSettings.SelectedCollectionId = Guid.Empty;
        }
      }
    }

    private TeamExplorerProjects.ProjectSettings FindProject(
      TeamExplorerProjects.CollectionSettings collectionSettings,
      string projectUri)
    {
      return collectionSettings.SelectedProjects.Find((Predicate<TeamExplorerProjects.ProjectSettings>) (ps => TFStringComparer.ProjectUri.Equals(ps.Uri.AbsoluteUri, projectUri)));
    }

    private int FindProjectIndex(
      TeamExplorerProjects.CollectionSettings collectionSettings,
      string projectUri)
    {
      return collectionSettings.SelectedProjects.FindIndex((Predicate<TeamExplorerProjects.ProjectSettings>) (ps => TFStringComparer.ProjectUri.Equals(ps.Uri.AbsoluteUri, projectUri)));
    }

    private TeamExplorerProjects.RepositorySettings FindRepository(
      TeamExplorerProjects.ProjectSettings projectSettings,
      Guid repositoryId)
    {
      return projectSettings.SelectedRepositories.Find((Predicate<TeamExplorerProjects.RepositorySettings>) (rs => rs.Id == repositoryId));
    }

    private int FindRepositoryIndex(
      TeamExplorerProjects.ProjectSettings projectSettings,
      Guid repositoryId)
    {
      return projectSettings.SelectedRepositories.FindIndex((Predicate<TeamExplorerProjects.RepositorySettings>) (rs => rs.Id == repositoryId));
    }

    private void LoadFile()
    {
      lock (this.m_dataLock)
      {
        try
        {
          if (!File.Exists(this.m_fileName))
            return;
          using (XmlReader reader = XmlReader.Create(this.m_fileName, new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit
          }))
            this.ReadConfig(reader);
        }
        catch (Exception ex)
        {
          try
          {
            this.DeleteConfigFile();
          }
          catch
          {
          }
          this.Clear();
        }
      }
    }

    private static bool TryParseUri(string s, out Uri result)
    {
      try
      {
        result = new Uri(s);
        return true;
      }
      catch
      {
        result = (Uri) null;
        return false;
      }
    }

    private static bool TryParseGuid(string s, out Guid result)
    {
      try
      {
        result = new Guid(s);
        return true;
      }
      catch
      {
        result = new Guid();
        return false;
      }
    }

    private void Clear()
    {
      this.m_autoLoad = false;
      this.m_defaultServerUri = (Uri) null;
      this.m_servers.Clear();
      this.m_collections.Clear();
    }

    protected void ReadConfig(XmlReader reader)
    {
      this.Clear();
      try
      {
        TeamExplorerProjects.NodeContext nodeContext = TeamExplorerProjects.NodeContext.Unknown;
        Uri uri1 = (Uri) null;
        Guid result1 = Guid.Empty;
        TeamExplorerProjects.ProjectSettings projectSettings = (TeamExplorerProjects.ProjectSettings) null;
        while (reader.Read())
        {
          int content = (int) reader.MoveToContent();
          switch (reader.NodeType)
          {
            case XmlNodeType.Element:
              switch (reader.Name.ToLower(CultureInfo.InvariantCulture))
              {
                case "server":
                  nodeContext = nodeContext == TeamExplorerProjects.NodeContext.Unknown ? TeamExplorerProjects.NodeContext.Server : throw new FormatException(this.m_fileName);
                  string attribute1 = reader.GetAttribute("url");
                  uri1 = !string.IsNullOrEmpty(attribute1) ? new Uri(attribute1) : throw new FormatException(this.m_fileName);
                  if (reader.GetAttribute("current") == "yes")
                    this.m_defaultServerUri = uri1;
                  this.m_servers[uri1] = new TeamExplorerProjects.ServerSettings(uri1);
                  continue;
                case "collection":
                  nodeContext = nodeContext == TeamExplorerProjects.NodeContext.Server ? TeamExplorerProjects.NodeContext.Collection : throw new FormatException(this.m_fileName);
                  string uriString = TeamExplorerProjects.TryParseGuid(reader.GetAttribute("guid"), out result1) ? reader.GetAttribute("url") : throw new FormatException(this.m_fileName);
                  Uri uri2 = !string.IsNullOrEmpty(uriString) ? new Uri(uriString) : throw new FormatException(this.m_fileName);
                  string attribute2 = reader.GetAttribute("name");
                  bool flag1 = false;
                  if (reader.GetAttribute("isHosted") == "yes")
                    flag1 = true;
                  if (reader.GetAttribute("current") == "yes")
                    this.m_servers[uri1].SelectedCollectionId = result1;
                  if (reader.GetAttribute("autoload") == "yes")
                  {
                    if (this.m_autoLoad)
                      throw new FormatException(this.m_fileName);
                    if (this.m_servers[uri1].SelectedCollectionId != result1 || !UriUtility.Equals(this.m_defaultServerUri, uri1))
                      throw new FormatException(this.m_fileName);
                    this.m_autoLoad = true;
                  }
                  TeamExplorerProjects.CollectionSettings collectionSettings = new TeamExplorerProjects.CollectionSettings(result1, uri2);
                  collectionSettings.Name = attribute2;
                  collectionSettings.IsHosted = flag1;
                  this.m_servers[uri1].Collections.Add(result1);
                  this.m_collections[result1] = collectionSettings;
                  continue;
                case "project":
                  if (nodeContext != TeamExplorerProjects.NodeContext.Collection)
                    throw new FormatException(this.m_fileName);
                  if (!reader.IsEmptyElement)
                    nodeContext = TeamExplorerProjects.NodeContext.Project;
                  string attribute3 = reader.GetAttribute("projectUri");
                  if (string.IsNullOrEmpty(attribute3))
                    throw new FormatException(this.m_fileName);
                  if (TeamExplorerProjects.TryParseUri(attribute3, out Uri _))
                  {
                    string attribute4 = reader.GetAttribute("name");
                    string attribute5 = reader.GetAttribute("capFlagsScc");
                    bool flag2 = false;
                    bool flag3 = false;
                    if (reader.GetAttribute("supportsGit") == "yes")
                      flag2 = true;
                    if (reader.GetAttribute("supportsTFVC") == "yes")
                      flag3 = true;
                    if (reader.GetAttribute("active") == "yes")
                      this.m_collections[result1].ActiveProject = attribute3;
                    projectSettings = new TeamExplorerProjects.ProjectSettings(new Uri(attribute3));
                    projectSettings.Name = attribute4;
                    projectSettings.CapFlagsScc = attribute5;
                    projectSettings.SupportsGit = flag2;
                    projectSettings.SupportsTFVC = flag3;
                    this.m_collections[result1].SelectedProjects.Add(projectSettings);
                    string attribute6 = reader.GetAttribute("team");
                    Guid result2;
                    if (!string.IsNullOrEmpty(attribute6) && TeamExplorerProjects.TryParseGuid(attribute6, out result2))
                    {
                      this.m_collections[result1].ActiveTeam = result2;
                      continue;
                    }
                    continue;
                  }
                  continue;
                case "repository":
                  if (nodeContext != TeamExplorerProjects.NodeContext.Project)
                    throw new FormatException(this.m_fileName);
                  string attribute7 = reader.GetAttribute("type");
                  string attribute8 = reader.GetAttribute("name");
                  if (string.IsNullOrEmpty(attribute7) || string.IsNullOrEmpty(attribute8))
                    throw new FormatException(this.m_fileName);
                  Guid result3 = Guid.Empty;
                  if (!TeamExplorerProjects.TryParseGuid(reader.GetAttribute("guid"), out result3))
                    throw new FormatException(this.m_fileName);
                  bool? isFork = new bool?();
                  string attribute9 = reader.GetAttribute("isFork");
                  if (!string.IsNullOrEmpty(attribute9))
                    isFork = new bool?(bool.TrueString.Equals(attribute9, StringComparison.OrdinalIgnoreCase));
                  projectSettings.SelectedRepositories.Add(new TeamExplorerProjects.RepositorySettings(result3, attribute7, attribute8, isFork)
                  {
                    ActiveClone = reader.GetAttribute("activeClone")
                  });
                  if (reader.GetAttribute("active") == "yes")
                  {
                    projectSettings.ActiveRepositories.Add(result3);
                    continue;
                  }
                  continue;
                case "query":
                  if (nodeContext != TeamExplorerProjects.NodeContext.Collection)
                    throw new FormatException(this.m_fileName);
                  string attribute10 = reader.GetAttribute("projectUri");
                  if (string.IsNullOrEmpty(attribute10))
                    throw new FormatException(this.m_fileName);
                  string attribute11 = reader.GetAttribute("guid");
                  Guid result4;
                  if (!string.IsNullOrEmpty(attribute11) && TeamExplorerProjects.TryParseGuid(attribute11, out result4))
                  {
                    this.m_collections[result1].DefaultQueries[attribute10] = result4;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case XmlNodeType.EndElement:
              switch (reader.Name.ToLower(CultureInfo.InvariantCulture))
              {
                case "project":
                  nodeContext = nodeContext == TeamExplorerProjects.NodeContext.Project ? TeamExplorerProjects.NodeContext.Collection : throw new FormatException(this.m_fileName);
                  projectSettings = (TeamExplorerProjects.ProjectSettings) null;
                  continue;
                case "collection":
                  nodeContext = nodeContext == TeamExplorerProjects.NodeContext.Collection ? TeamExplorerProjects.NodeContext.Server : throw new FormatException(this.m_fileName);
                  continue;
                case "server":
                  nodeContext = nodeContext == TeamExplorerProjects.NodeContext.Server ? TeamExplorerProjects.NodeContext.Unknown : throw new FormatException(this.m_fileName);
                  continue;
                default:
                  continue;
              }
            default:
              continue;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    protected virtual void WriteConfig()
    {
      Directory.CreateDirectory(Path.GetDirectoryName(this.m_fileName));
      XmlTextWriter xmlTextWriter1 = new XmlTextWriter(this.m_fileName, (Encoding) null);
      try
      {
        xmlTextWriter1.Formatting = Formatting.Indented;
        xmlTextWriter1.Indentation = 4;
        xmlTextWriter1.WriteComment("This configuration file specifies the previously-configured connection details for Azure DevOps Server.");
        xmlTextWriter1.WriteStartElement("server_list");
        foreach (TeamExplorerProjects.ServerSettings serverSettings in this.m_servers.Values)
        {
          xmlTextWriter1.WriteStartElement("server");
          xmlTextWriter1.WriteAttributeString("url", UriUtility.GetInvariantAbsoluteUri(serverSettings.Uri));
          if (this.m_defaultServerUri != (Uri) null && UriUtility.Equals(this.m_defaultServerUri, serverSettings.Uri))
            xmlTextWriter1.WriteAttributeString("current", "yes");
          foreach (Guid collection1 in serverSettings.Collections)
          {
            TeamExplorerProjects.CollectionSettings collection2 = this.m_collections[collection1];
            xmlTextWriter1.WriteStartElement("collection");
            xmlTextWriter1.WriteAttributeString("guid", collection1.ToString());
            xmlTextWriter1.WriteAttributeString("url", UriUtility.GetInvariantAbsoluteUri(collection2.Uri));
            xmlTextWriter1.WriteAttributeString("name", collection2.Name);
            if (collection2.IsHosted)
              xmlTextWriter1.WriteAttributeString("isHosted", "yes");
            if (serverSettings.SelectedCollectionId == collection1)
            {
              xmlTextWriter1.WriteAttributeString("current", "yes");
              if (this.m_autoLoad && this.m_defaultServerUri != (Uri) null && UriUtility.Equals(this.m_defaultServerUri, serverSettings.Uri))
                xmlTextWriter1.WriteAttributeString("autoload", "yes");
            }
            foreach (TeamExplorerProjects.ProjectSettings selectedProject in collection2.SelectedProjects)
            {
              xmlTextWriter1.WriteStartElement("project");
              string absoluteUri = selectedProject.Uri.AbsoluteUri;
              xmlTextWriter1.WriteAttributeString("projectUri", absoluteUri);
              xmlTextWriter1.WriteAttributeString("name", selectedProject.Name);
              xmlTextWriter1.WriteAttributeString("capFlagsScc", selectedProject.CapFlagsScc);
              if (selectedProject.SupportsGit && selectedProject.CapFlagsScc != "2")
                xmlTextWriter1.WriteAttributeString("supportsGit", "yes");
              if (selectedProject.SupportsTFVC && selectedProject.CapFlagsScc != "1")
                xmlTextWriter1.WriteAttributeString("supportsTFVC", "yes");
              if (TFStringComparer.ProjectUri.Compare(absoluteUri, collection2.ActiveProject) == 0)
              {
                xmlTextWriter1.WriteAttributeString("active", "yes");
                if (collection2.ActiveTeam != Guid.Empty)
                  xmlTextWriter1.WriteAttributeString("team", collection2.ActiveTeam.ToString());
              }
              foreach (TeamExplorerProjects.RepositorySettings repositorySettings in (IEnumerable<TeamExplorerProjects.RepositorySettings>) selectedProject.SelectedRepositories.OrderBy<TeamExplorerProjects.RepositorySettings, string>((Func<TeamExplorerProjects.RepositorySettings, string>) (r => r.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase))
              {
                xmlTextWriter1.WriteStartElement("repository");
                xmlTextWriter1.WriteAttributeString("type", repositorySettings.SourceControlType);
                xmlTextWriter1.WriteAttributeString("name", repositorySettings.Name);
                xmlTextWriter1.WriteAttributeString("guid", repositorySettings.Id.ToString());
                if (selectedProject.ActiveRepositories.Contains(repositorySettings.Id))
                  xmlTextWriter1.WriteAttributeString("active", "yes");
                if (!string.IsNullOrEmpty(repositorySettings.ActiveClone))
                  xmlTextWriter1.WriteAttributeString("activeClone", repositorySettings.ActiveClone);
                if (repositorySettings.IsFork.HasValue)
                {
                  XmlTextWriter xmlTextWriter2 = xmlTextWriter1;
                  bool? isFork = repositorySettings.IsFork;
                  bool flag = true;
                  string str = isFork.GetValueOrDefault() == flag & isFork.HasValue ? bool.TrueString : bool.FalseString;
                  xmlTextWriter2.WriteAttributeString("isFork", str);
                }
                xmlTextWriter1.WriteEndElement();
              }
              xmlTextWriter1.WriteEndElement();
            }
            foreach (KeyValuePair<string, Guid> defaultQuery in collection2.DefaultQueries)
            {
              xmlTextWriter1.WriteStartElement("query");
              xmlTextWriter1.WriteAttributeString("projectUri", defaultQuery.Key);
              xmlTextWriter1.WriteAttributeString("guid", defaultQuery.Value.ToString());
              xmlTextWriter1.WriteEndElement();
            }
            xmlTextWriter1.WriteFullEndElement();
          }
          xmlTextWriter1.WriteFullEndElement();
        }
        xmlTextWriter1.WriteFullEndElement();
      }
      finally
      {
        xmlTextWriter1.Close();
      }
    }

    private void DeleteConfigFile() => File.Delete(this.m_fileName);

    private enum NodeContext
    {
      Unknown,
      Server,
      Collection,
      Project,
    }

    private class ProjectSettings
    {
      public ProjectSettings(string uri)
        : this(new Uri(uri))
      {
      }

      public ProjectSettings(Uri uri)
      {
        this.Uri = uri;
        this.SelectedRepositories = new List<TeamExplorerProjects.RepositorySettings>();
        this.ActiveRepositories = new List<Guid>();
      }

      public Uri Uri { get; private set; }

      public string Name { get; set; }

      public string CapFlagsScc { get; set; }

      public bool SupportsGit { get; set; }

      public bool SupportsTFVC { get; set; }

      public List<TeamExplorerProjects.RepositorySettings> SelectedRepositories { get; private set; }

      public List<Guid> ActiveRepositories { get; private set; }
    }

    private class CollectionSettings
    {
      public CollectionSettings(Guid id, Uri uri)
      {
        this.Id = id;
        this.Uri = uri;
        this.DefaultQueries = new Dictionary<string, Guid>((IEqualityComparer<string>) TFStringComparer.TeamProjectName);
        this.SelectedProjects = new List<TeamExplorerProjects.ProjectSettings>();
        this.ActiveTeam = Guid.Empty;
      }

      public Guid Id { get; set; }

      public Uri Uri { get; set; }

      public string Name { get; set; }

      public bool IsHosted { get; set; }

      public string ActiveProject { get; set; }

      public Guid ActiveTeam { get; set; }

      public List<TeamExplorerProjects.ProjectSettings> SelectedProjects { get; private set; }

      public Dictionary<string, Guid> DefaultQueries { get; private set; }
    }

    private class ServerSettings
    {
      public ServerSettings(Uri uri)
      {
        this.Uri = uri;
        this.Collections = new List<Guid>();
      }

      public Uri Uri { get; private set; }

      public List<Guid> Collections { get; private set; }

      public Guid SelectedCollectionId { get; set; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RepositorySettings
    {
      public RepositorySettings(Guid id, string sourceControlType, string name)
        : this(id, sourceControlType, name, new bool?())
      {
      }

      public RepositorySettings(Guid id, string sourceControlType, string name, bool? isFork)
      {
        this.Id = id;
        this.SourceControlType = sourceControlType;
        this.Name = name;
        this.IsFork = isFork;
      }

      public Guid Id { get; private set; }

      public string Name { get; set; }

      public string SourceControlType { get; private set; }

      public string ActiveClone { get; set; }

      public bool? IsFork { get; set; }
    }

    private class DefaultsProvider : ITeamProjectPickerDefaultSelectionProvider
    {
      private TeamExplorerProjects m_projects;
      private TeamProjectPickerMode m_mode;

      public DefaultsProvider(TeamExplorerProjects projects, TeamProjectPickerMode mode)
      {
        this.m_projects = projects;
        this.m_mode = mode;
      }

      Uri ITeamProjectPickerDefaultSelectionProvider.GetDefaultServerUri() => this.m_projects.m_defaultServerUri;

      Guid? ITeamProjectPickerDefaultSelectionProvider.GetDefaultCollectionId(Uri serverUri)
      {
        TeamExplorerProjects.ServerSettings serverSettings;
        return this.m_projects.m_servers.TryGetValue(serverUri, out serverSettings) ? new Guid?(serverSettings.SelectedCollectionId) : new Guid?();
      }

      IEnumerable<string> ITeamProjectPickerDefaultSelectionProvider.GetDefaultProjects(
        Guid collectionId)
      {
        if (this.m_mode == TeamProjectPickerMode.SingleProject)
        {
          List<string> defaultProjects = new List<string>();
          string projectForCollection = this.m_projects.GetActiveProjectForCollection(collectionId);
          if (!string.IsNullOrEmpty(projectForCollection))
            defaultProjects.Add(projectForCollection);
          return (IEnumerable<string>) defaultProjects;
        }
        return this.m_mode == TeamProjectPickerMode.MultiProject ? (IEnumerable<string>) this.m_projects.GetProjectsForCollection(collectionId) : (IEnumerable<string>) new List<string>();
      }
    }
  }
}
