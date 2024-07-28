// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.BranchService
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class BranchService : IBranchService, IVssFrameworkService, IDisposable
  {
    private Guid m_collectionHostId;
    private TeamFoundationTask m_branchCacheRebuildTask;
    private CountdownEvent m_branchCacheRebuildTaskEvent;
    private const string GitBranchRefPrefix = "refs/heads/";
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private Dictionary<string, Dictionary<string, GitCodeRepoTFSAttributes>> m_gitCodeRepoAttributes;
    private Dictionary<string, Dictionary<string, CustomRepoCodeTFSAttributes>> m_customCodeRepoAttributes;
    private Dictionary<Guid, Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>> m_gitBranchIndexInfoForEachRepo;
    private Dictionary<BranchService.RepositoryInfo, Dictionary<string, DepotInfo>> m_depotIndexInfoForEachRepo;
    private Dictionary<Guid, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> m_tfvcProjectIndexInfo;
    private bool disposedValue;

    internal IDataAccessFactory DataAccessFactoryInstance { get; set; }

    public BranchService()
      : this((IIndexingUnitDataAccess) null)
    {
    }

    internal BranchService(IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      this.m_indexingUnitDataAccess = indexingUnitDataAccess;
      this.m_gitCodeRepoAttributes = new Dictionary<string, Dictionary<string, GitCodeRepoTFSAttributes>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_customCodeRepoAttributes = new Dictionary<string, Dictionary<string, CustomRepoCodeTFSAttributes>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_gitBranchIndexInfoForEachRepo = new Dictionary<Guid, Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>>();
      this.m_depotIndexInfoForEachRepo = new Dictionary<BranchService.RepositoryInfo, Dictionary<string, DepotInfo>>();
      this.m_tfvcProjectIndexInfo = new Dictionary<Guid, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
      this.DataAccessFactoryInstance = DataAccessFactory.GetInstance();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_collectionHostId = systemRequestContext.ServiceHost.CollectionServiceHost.InstanceId;
      this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) systemRequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/BranchCacheUpdateIntervalInSeconds", true, 1800));
      this.m_branchCacheRebuildTaskEvent = new CountdownEvent(1);
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      if (systemRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/ServiceStartSkipInlineSqlQueries"))
      {
        this.m_branchCacheRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.RebuildRepositoryBranchCache), (object) null, DateTime.UtcNow.AddSeconds((double) systemRequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/ServiceStartDelayInSqlQueryInSec")), (int) timeSpan.TotalMilliseconds);
      }
      else
      {
        this.BuildGitRepositoryBranchCache(vssRequestContext);
        this.BuildCustomRepositoryBranchCache(vssRequestContext);
        if (vssRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/CacheBranchIndexInfo", defaultValue: true))
        {
          this.BuildGitRepositoryBranchIndexInfoCache(vssRequestContext);
          this.BuildTfvcRepositoryIndexInfoCache(vssRequestContext);
        }
        if (vssRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/CacheDepotIndexInfo"))
          this.BuildCustomRepositoryDepotIndexInfoCache(vssRequestContext);
        this.m_branchCacheRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.RebuildRepositoryBranchCache), (object) null, (int) timeSpan.TotalMilliseconds);
      }
      vssRequestContext.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, this.m_branchCacheRebuildTask);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_branchCacheRebuildTask != null)
        vssRequestContext.GetService<ITeamFoundationTaskService>().RemoveTask(vssRequestContext, this.m_branchCacheRebuildTask);
      this.m_branchCacheRebuildTaskEvent.Signal();
      this.m_branchCacheRebuildTaskEvent.Wait();
      this.m_branchCacheRebuildTaskEvent.Dispose();
    }

    public IList<string> GetBranches(
      IVssRequestContext userRequestContext,
      string projectName,
      string repositoryName)
    {
      List<string> list = this.GetGitBranches(projectName, repositoryName).ToList<string>();
      list.AddRange((IEnumerable<string>) this.GetCustomBranches(projectName, repositoryName).ToList<string>());
      return (IList<string>) list;
    }

    public string GetDefaultBranch(
      IVssRequestContext userRequestContext,
      string projectName,
      string repositoryName)
    {
      string gitDefaultBranch = this.GetGitDefaultBranch(projectName, repositoryName);
      string customDefaultBranch = this.GetCustomDefaultBranch(projectName, repositoryName);
      return !string.IsNullOrEmpty(customDefaultBranch) ? customDefaultBranch : gitDefaultBranch;
    }

    public List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> GetRepositoryIndexInfo(
      IVssRequestContext userRequestContext,
      Guid repositoryId,
      List<string> configuredBranches)
    {
      if (repositoryId == Guid.Empty)
        throw new ArgumentException("The parameter cannot be null or empty", nameof (repositoryId));
      if (configuredBranches == null)
        throw new ArgumentNullException(nameof (configuredBranches));
      if (configuredBranches.Count == 0)
        throw new ArgumentException("The parameter cannot be empty", nameof (configuredBranches));
      List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> repositoryIndexInfo = new List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
      if (this.m_gitBranchIndexInfoForEachRepo.ContainsKey(repositoryId))
      {
        Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> dictionary = this.m_gitBranchIndexInfoForEachRepo[repositoryId];
        foreach (string configuredBranch in configuredBranches)
        {
          Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo branchInfo = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(configuredBranch);
          if (dictionary.ContainsKey(configuredBranch))
            branchInfo = dictionary[configuredBranch];
          repositoryIndexInfo.Add(branchInfo);
        }
      }
      else
      {
        foreach (string configuredBranch in configuredBranches)
        {
          Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo branchInfo = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(configuredBranch);
          repositoryIndexInfo.Add(branchInfo);
        }
      }
      return repositoryIndexInfo;
    }

    public List<DepotInfo> GetCustomRepositoryIndexInfo(
      string projectName,
      string repositoryName,
      List<string> configuredBranches)
    {
      if (projectName == null)
        throw new ArgumentNullException(nameof (projectName));
      if (repositoryName == null)
        throw new ArgumentNullException(nameof (repositoryName));
      if (configuredBranches == null)
        throw new ArgumentNullException(nameof (configuredBranches));
      if (configuredBranches.Count == 0)
        throw new ArgumentException("The parameter cannot be empty", nameof (configuredBranches));
      BranchService.RepositoryInfo key = new BranchService.RepositoryInfo(projectName, repositoryName);
      List<DepotInfo> repositoryIndexInfo = new List<DepotInfo>();
      if (this.m_depotIndexInfoForEachRepo.ContainsKey(key))
      {
        foreach (KeyValuePair<string, DepotInfo> keyValuePair in this.m_depotIndexInfoForEachRepo[key])
        {
          keyValuePair.Value.IndexedBranches = (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>) keyValuePair.Value.IndexedBranches.Where<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>((Func<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo, bool>) (b => configuredBranches.Contains(b.Name))).ToList<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
          repositoryIndexInfo.Add(keyValuePair.Value);
        }
      }
      else
      {
        DepotInfo depotInfo = new DepotInfo(string.Empty);
        repositoryIndexInfo.Add(depotInfo);
      }
      return repositoryIndexInfo;
    }

    public List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> GetTfvcRepositoryIndexInfo(
      Guid projectId,
      string projectName)
    {
      if (projectId == Guid.Empty)
        throw new ArgumentException("The parameter cannot be null or empty", nameof (projectId));
      if (projectName == null)
        throw new ArgumentNullException(nameof (projectName));
      List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> repositoryIndexInfo = new List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
      if (this.m_tfvcProjectIndexInfo.ContainsKey(projectId))
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo branchInfo = this.m_tfvcProjectIndexInfo[projectId];
        repositoryIndexInfo.Add(branchInfo);
      }
      else
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo branchInfo = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(projectName);
        repositoryIndexInfo.Add(branchInfo);
      }
      return repositoryIndexInfo;
    }

    internal void RebuildRepositoryBranchCache(
      IVssRequestContext deploymentRequestContext,
      object taskArgs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(deploymentRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081411, "Query Pipeline", nameof (BranchService), nameof (RebuildRepositoryBranchCache));
      try
      {
        this.m_branchCacheRebuildTaskEvent.AddCount();
        try
        {
          this.BuildGitRepositoryBranchCache(deploymentRequestContext);
          this.BuildCustomRepositoryBranchCache(deploymentRequestContext);
          if (deploymentRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/CacheBranchIndexInfo", defaultValue: true))
          {
            this.BuildGitRepositoryBranchIndexInfoCache(deploymentRequestContext);
            this.BuildTfvcRepositoryIndexInfoCache(deploymentRequestContext);
          }
          if (!deploymentRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/CacheDepotIndexInfo"))
            return;
          this.BuildCustomRepositoryDepotIndexInfoCache(deploymentRequestContext);
        }
        finally
        {
          this.m_branchCacheRebuildTaskEvent.Signal();
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081412, "Query Pipeline", nameof (BranchService), nameof (RebuildRepositoryBranchCache));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal virtual void BuildGitRepositoryBranchCache(IVssRequestContext deploymentRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (IVssRequestContext requestContext = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, this.m_collectionHostId, RequestContextType.SystemContext))
        {
          if (this.m_indexingUnitDataAccess == null)
            this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1);
          Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnitDictionary = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits2)
            projectIndexingUnitDictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
          Dictionary<string, Dictionary<string, GitCodeRepoTFSAttributes>> gitCodeRepoAttributes = new Dictionary<string, Dictionary<string, GitCodeRepoTFSAttributes>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          indexingUnits1.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo =>
          {
            if (!(repo.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes3))
              throw new FormatException("Expected GitCodeRepoTFSAttributes for entityId " + repo.TFSEntityId.ToString() + " but found " + repo.TFSEntityAttributes.GetType()?.ToString());
            Guid tfsEntityId;
            if (entityAttributes3.Branches != null)
            {
              entityAttributes3.Branches = entityAttributes3.Branches.Select<string, string>((Func<string, string>) (branch => BranchService.GetBranchNameWithoutPrefix("refs/heads/", branch))).ToList<string>();
            }
            else
            {
              entityAttributes3.Branches = new List<string>();
              tfsEntityId = repo.TFSEntityId;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081410, "Query Pipeline", nameof (BranchService), "TFSEntityAttributes.Branches is not expected to be null for EntityId: " + tfsEntityId.ToString());
            }
            entityAttributes3.DefaultBranch = BranchService.GetBranchNameWithoutPrefix("refs/heads/", entityAttributes3.DefaultBranch);
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
            projectIndexingUnitDictionary.TryGetValue(repo.ParentUnitId, out indexingUnit);
            string key = indexingUnit?.Properties?.Name ?? (indexingUnit?.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes4 ? entityAttributes4.ProjectName : (string) null);
            if (string.IsNullOrWhiteSpace(key))
            {
              tfsEntityId = repo.TFSEntityId;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081410, "Query Pipeline", nameof (BranchService), "ProjectName is not expected to be null or empty for EntityId: " + tfsEntityId.ToString());
            }
            else
            {
              Dictionary<string, GitCodeRepoTFSAttributes> dictionary;
              if (!gitCodeRepoAttributes.TryGetValue(key, out dictionary))
              {
                dictionary = new Dictionary<string, GitCodeRepoTFSAttributes>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                gitCodeRepoAttributes[key] = dictionary;
              }
              dictionary[entityAttributes3.RepositoryName] = entityAttributes3;
            }
          }));
          this.m_gitCodeRepoAttributes = gitCodeRepoAttributes;
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("RepositoryBranchCacheBuildTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081410, "Query Pipeline", nameof (BranchService), ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
    }

    internal virtual void BuildCustomRepositoryBranchCache(
      IVssRequestContext deploymentRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (IVssRequestContext requestContext = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, this.m_collectionHostId, RequestContextType.SystemContext))
        {
          if (this.m_indexingUnitDataAccess == null)
            this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), -1);
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1);
          Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary1 = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits2)
            dictionary1.Add(indexingUnit.IndexingUnitId, indexingUnit);
          Dictionary<string, Dictionary<string, CustomRepoCodeTFSAttributes>> dictionary2 = new Dictionary<string, Dictionary<string, CustomRepoCodeTFSAttributes>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in indexingUnits1)
          {
            if (!(indexingUnit1.TFSEntityAttributes is CustomRepoCodeTFSAttributes entityAttributes1))
              throw new FormatException("Expected CustomRepoCodeTFSAttributes for entityId " + indexingUnit1.TFSEntityId.ToString() + " but found " + indexingUnit1.TFSEntityAttributes.GetType()?.ToString());
            if (entityAttributes1.Branches == null)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081410, "Query Pipeline", nameof (BranchService), "TFSEntityAttributes.Branches is null for EntityId: " + indexingUnit1.TFSEntityId.ToString());
              break;
            }
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
            dictionary1.TryGetValue(indexingUnit1.ParentUnitId, out indexingUnit2);
            string key = indexingUnit2?.Properties?.Name ?? (indexingUnit2?.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null);
            if (string.IsNullOrWhiteSpace(key))
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081410, "Query Pipeline", nameof (BranchService), "ProjectName is not expected to be null or empty for EntityId: " + indexingUnit1.TFSEntityId.ToString());
              break;
            }
            Dictionary<string, CustomRepoCodeTFSAttributes> dictionary3;
            if (!dictionary2.TryGetValue(key, out dictionary3))
            {
              dictionary3 = new Dictionary<string, CustomRepoCodeTFSAttributes>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              dictionary2[key] = dictionary3;
            }
            dictionary3[entityAttributes1.RepositoryName] = entityAttributes1;
          }
          this.m_customCodeRepoAttributes = dictionary2;
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("RepositoryBranchCacheBuildTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081410, "Query Pipeline", nameof (BranchService), ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
    }

    internal virtual void BuildGitRepositoryBranchIndexInfoCache(
      IVssRequestContext deploymentRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (IVssRequestContext requestContext = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, this.m_collectionHostId, RequestContextType.SystemContext))
        {
          if (this.m_indexingUnitDataAccess == null)
            this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
          bool isShadow = false;
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Git_Repository", isShadow, (IEntityType) CodeEntityType.GetInstance(), -1);
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1);
          Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnitDictionary = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits2)
          {
            if (!projectIndexingUnitDictionary.ContainsKey(indexingUnit.IndexingUnitId))
              projectIndexingUnitDictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
          }
          Dictionary<Guid, Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>> gitBranchIndexInfoForEachRepo = new Dictionary<Guid, Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>>();
          indexingUnits1.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo =>
          {
            Guid tfsEntityId = repo.TFSEntityId;
            string fromTfsAttributes = repo.GetRepositoryNameFromTFSAttributes();
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
            projectIndexingUnitDictionary.TryGetValue(repo.ParentUnitId, out indexingUnit);
            string str = indexingUnit?.Properties?.Name ?? (indexingUnit?.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null);
            Guid guid;
            if (string.IsNullOrWhiteSpace(str))
            {
              guid = tfsEntityId;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081415, "Query Pipeline", nameof (BranchService), "ProjectName is not expected to be null or empty for EntityId: " + guid.ToString());
            }
            GitCodeRepoIndexingProperties properties = repo.Properties as GitCodeRepoIndexingProperties;
            Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> dictionary = new Dictionary<string, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
            if (properties == null || properties.IsDisabled)
              return;
            Dictionary<string, GitBranchIndexInfo> branchIndexInfo = properties.BranchIndexInfo;
            if (branchIndexInfo != null)
            {
              foreach (string key in branchIndexInfo.Keys)
              {
                string nameWithoutPrefix = BranchService.GetBranchNameWithoutPrefix("refs/heads/", key);
                Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo branchInfo = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(nameWithoutPrefix, branchIndexInfo[key].LastIndexedCommitId, BranchService.MaxDateTime(branchIndexInfo[key].BranchLastProcessedTime, branchIndexInfo[key].LastIndexedCommitUtcTime));
                if (!dictionary.ContainsKey(nameWithoutPrefix))
                {
                  dictionary.Add(nameWithoutPrefix, branchInfo);
                }
                else
                {
                  dictionary[nameWithoutPrefix] = branchInfo;
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081415, "Query Pipeline", nameof (BranchService), "Found duplicate key for Branch Name: " + key + "BranchNameWithoutPrefix: " + nameWithoutPrefix);
                }
              }
            }
            if (gitBranchIndexInfoForEachRepo.ContainsKey(tfsEntityId))
            {
              string[] strArray = new string[6];
              strArray[0] = "Found duplicate key Repository TFS Entity Id: ";
              guid = tfsEntityId;
              strArray[1] = guid.ToString();
              strArray[2] = "for Project Name: ";
              strArray[3] = str;
              strArray[4] = " and Repo Name: ";
              strArray[5] = fromTfsAttributes;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081415, "Query Pipeline", nameof (BranchService), string.Concat(strArray));
            }
            this.m_gitBranchIndexInfoForEachRepo[tfsEntityId] = dictionary;
            gitBranchIndexInfoForEachRepo[tfsEntityId] = dictionary;
          }));
          this.m_gitBranchIndexInfoForEachRepo = gitBranchIndexInfoForEachRepo;
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GitRepositoryBranchIndexInfoCacheBuildTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081415, "Query Pipeline", nameof (BranchService), ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
    }

    internal virtual void BuildCustomRepositoryDepotIndexInfoCache(
      IVssRequestContext deploymentRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (IVssRequestContext requestContext = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, this.m_collectionHostId, RequestContextType.SystemContext))
        {
          if (this.m_indexingUnitDataAccess == null)
            this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
          bool isShadow = false;
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "CustomRepository", isShadow, (IEntityType) CodeEntityType.GetInstance(), -1);
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1);
          Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnitDictionary = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits2)
            projectIndexingUnitDictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
          Dictionary<BranchService.RepositoryInfo, Dictionary<string, DepotInfo>> localDepotIndexInfoForEachRepo = new Dictionary<BranchService.RepositoryInfo, Dictionary<string, DepotInfo>>();
          indexingUnits1.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo =>
          {
            Guid tfsEntityId = repo.TFSEntityId;
            string fromTfsAttributes = repo.GetRepositoryNameFromTFSAttributes();
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
            projectIndexingUnitDictionary.TryGetValue(repo.ParentUnitId, out indexingUnit);
            string projectName = indexingUnit?.Properties?.Name ?? (indexingUnit?.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null);
            if (string.IsNullOrWhiteSpace(projectName))
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081413, "Query Pipeline", nameof (BranchService), "ProjectName is not expected to be null or empty for EntityId: " + tfsEntityId.ToString());
            CustomRepoCodeIndexingProperties properties = repo.Properties as CustomRepoCodeIndexingProperties;
            Dictionary<string, DepotInfo> dictionary = new Dictionary<string, DepotInfo>();
            if (properties != null && !properties.IsDisabled)
            {
              Dictionary<string, Dictionary<string, DepotIndexInfo>> depotIndexInfo = properties.DepotIndexInfo;
              if (depotIndexInfo != null)
              {
                foreach (string key in depotIndexInfo.Keys)
                {
                  IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> branchInfos = (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>) new List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
                  IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> list = (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>) depotIndexInfo[key].Select<KeyValuePair<string, DepotIndexInfo>, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>((Func<KeyValuePair<string, DepotIndexInfo>, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>) (b => new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(b.Key, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) b.Value.LastIndexedChangeId), b.Value.LastIndexedChangeUtcTime))).ToList<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
                  dictionary.Add(key, new DepotInfo(key, list));
                }
              }
            }
            BranchService.RepositoryInfo key1 = new BranchService.RepositoryInfo(projectName, fromTfsAttributes);
            if (localDepotIndexInfoForEachRepo.ContainsKey(key1))
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081413, "Query Pipeline", nameof (BranchService), "Combination of Project Name: " + projectName + " and Repo Name: " + fromTfsAttributes + " must be unique");
            localDepotIndexInfoForEachRepo[key1] = dictionary;
            this.m_depotIndexInfoForEachRepo[key1] = dictionary;
          }));
          this.m_depotIndexInfoForEachRepo = localDepotIndexInfoForEachRepo;
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("CustomRepositoryBranchIndexInfoCacheBuildTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081413, "Query Pipeline", nameof (BranchService), ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
    }

    internal virtual void BuildTfvcRepositoryIndexInfoCache(
      IVssRequestContext deploymentRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (IVssRequestContext requestContext = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, this.m_collectionHostId, RequestContextType.SystemContext))
        {
          if (this.m_indexingUnitDataAccess == null)
            this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
          bool isShadow = false;
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "TFVC_Repository", isShadow, (IEntityType) CodeEntityType.GetInstance(), -1);
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1);
          Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnitDictionary = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits2)
            projectIndexingUnitDictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
          Dictionary<Guid, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo> tfvcProjectIndexInfo = new Dictionary<Guid, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo>();
          indexingUnits1.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo =>
          {
            Guid tfsEntityId = repo.TFSEntityId;
            string fromTfsAttributes = repo.GetRepositoryNameFromTFSAttributes();
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
            projectIndexingUnitDictionary.TryGetValue(repo.ParentUnitId, out indexingUnit);
            string name = indexingUnit?.Properties?.Name ?? (indexingUnit?.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null);
            if (string.IsNullOrWhiteSpace(name))
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081410, "Query Pipeline", nameof (BranchService), "ProjectName is not expected to be null or empty for EntityId: " + tfsEntityId.ToString());
            TfvcCodeRepoIndexingProperties properties = repo.Properties as TfvcCodeRepoIndexingProperties;
            Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo branchInfo = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(name);
            if (this.m_tfvcProjectIndexInfo.ContainsKey(tfsEntityId) && !properties.IsDisabled)
              branchInfo = this.m_tfvcProjectIndexInfo[tfsEntityId];
            if (properties == null || properties.IsDisabled)
              return;
            int indexedChangeSetId = properties.LastIndexedChangeSetId;
            DateTime lastProcessedTime = properties.RepositoryLastProcessedTime;
            if (!branchInfo.LastIndexedChangeId.IsNullOrEmpty<char>() || indexedChangeSetId != -1)
              branchInfo = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo(fromTfsAttributes, indexedChangeSetId.ToString((IFormatProvider) CultureInfo.InvariantCulture), lastProcessedTime);
            this.m_tfvcProjectIndexInfo[tfsEntityId] = branchInfo;
            tfvcProjectIndexInfo[tfsEntityId] = branchInfo;
          }));
          this.m_tfvcProjectIndexInfo = tfvcProjectIndexInfo;
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("TfvcRepositoryIndexInfoCacheBuildTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081414, "Query Pipeline", nameof (BranchService), ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
    }

    public static DateTime MaxDateTime(DateTime a, DateTime b) => !(a > b) ? b : a;

    private IList<string> GetGitBranches(string projectName, string repositoryName)
    {
      GitCodeRepoTFSAttributes repoTfsAttributes = this.CheckGitBranches(projectName, repositoryName);
      return repoTfsAttributes != null ? (IList<string>) repoTfsAttributes.BranchesToIndex : (IList<string>) new List<string>();
    }

    private IList<string> GetCustomBranches(string projectName, string repositoryName)
    {
      CustomRepoCodeTFSAttributes codeTfsAttributes = this.CheckCustomBranches(projectName, repositoryName);
      return codeTfsAttributes != null ? (IList<string>) codeTfsAttributes.BranchesToIndex : (IList<string>) new List<string>();
    }

    private string GetGitDefaultBranch(string projectName, string repositoryName)
    {
      GitCodeRepoTFSAttributes repoTfsAttributes = this.CheckGitBranches(projectName, repositoryName);
      return repoTfsAttributes != null ? repoTfsAttributes.DefaultBranch : string.Empty;
    }

    private string GetCustomDefaultBranch(string projectName, string repositoryName)
    {
      CustomRepoCodeTFSAttributes codeTfsAttributes = this.CheckCustomBranches(projectName, repositoryName);
      return codeTfsAttributes != null ? codeTfsAttributes.DefaultBranch : string.Empty;
    }

    private CustomRepoCodeTFSAttributes CheckCustomBranches(
      string projectName,
      string repositoryName)
    {
      if (projectName == null)
        throw new ArgumentNullException(nameof (projectName));
      if (repositoryName == null)
        throw new ArgumentNullException(nameof (repositoryName));
      Dictionary<string, CustomRepoCodeTFSAttributes> dictionary;
      if (!this.m_customCodeRepoAttributes.TryGetValue(projectName, out dictionary))
        return (CustomRepoCodeTFSAttributes) null;
      CustomRepoCodeTFSAttributes codeTfsAttributes;
      return !dictionary.TryGetValue(repositoryName, out codeTfsAttributes) ? (CustomRepoCodeTFSAttributes) null : codeTfsAttributes;
    }

    private GitCodeRepoTFSAttributes CheckGitBranches(string projectName, string repositoryName)
    {
      if (projectName == null)
        throw new ArgumentNullException(nameof (projectName));
      if (repositoryName == null)
        throw new ArgumentNullException(nameof (repositoryName));
      Dictionary<string, GitCodeRepoTFSAttributes> dictionary;
      if (!this.m_gitCodeRepoAttributes.TryGetValue(projectName, out dictionary))
        return (GitCodeRepoTFSAttributes) null;
      GitCodeRepoTFSAttributes repoTfsAttributes;
      return !dictionary.TryGetValue(repositoryName, out repoTfsAttributes) ? (GitCodeRepoTFSAttributes) null : repoTfsAttributes;
    }

    private static string GetBranchNameWithoutPrefix(string branchRefPrefix, string branchName)
    {
      string nameWithoutPrefix = branchName ?? string.Empty;
      if (nameWithoutPrefix.StartsWith(branchRefPrefix, StringComparison.OrdinalIgnoreCase))
        nameWithoutPrefix = nameWithoutPrefix.Substring(branchRefPrefix.Length);
      return nameWithoutPrefix;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing && this.m_branchCacheRebuildTaskEvent != null)
      {
        this.m_branchCacheRebuildTaskEvent.Dispose();
        this.m_branchCacheRebuildTaskEvent = (CountdownEvent) null;
      }
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public class RepositoryInfo : IEquatable<BranchService.RepositoryInfo>
    {
      public string ProjectName { get; set; }

      public string RepositoryName { get; set; }

      public RepositoryInfo(string projectName, string repositoryName)
      {
        this.ProjectName = projectName;
        this.RepositoryName = repositoryName;
      }

      public override bool Equals(object obj) => this.Equals(obj as BranchService.RepositoryInfo);

      public bool Equals(BranchService.RepositoryInfo other) => other != null && this.ProjectName == other.ProjectName && string.Compare(this.RepositoryName, other.RepositoryName, StringComparison.OrdinalIgnoreCase) == 0;

      public override int GetHashCode() => (-1756218597 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ProjectName)) * -1521134295 + StringComparer.OrdinalIgnoreCase.GetHashCode(this.RepositoryName);
    }
  }
}
