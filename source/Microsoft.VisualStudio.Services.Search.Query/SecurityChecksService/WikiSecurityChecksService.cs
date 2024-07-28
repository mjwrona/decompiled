// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.WikiSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  public class WikiSecurityChecksService : 
    GitRepoSecurityChecksService,
    IWikiSecurityChecksService,
    ISecurityChecksService,
    IVssFrameworkService
  {
    private const double SecurityChecksCacheHitKpi = 1.0;
    private const double SecurityChecksCacheMissKpi = 0.0;
    private Guid m_collectionHostId;
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private int m_numberOfReposToCheckInASecuritySet;
    private CodeSecurityChecksCache m_securityCache;
    private TeamFoundationTask m_securitySetsRebuildTask;
    private CountdownEvent m_securitySetsRebuildTaskEvent;
    private int m_totalRepos;
    private bool m_disposeSecurityCache;
    private bool m_disposedValue;

    public WikiSecurityChecksService()
      : this((IIndexingUnitDataAccess) null, (CodeSecurityChecksCache) null)
    {
    }

    internal WikiSecurityChecksService(
      IIndexingUnitDataAccess indexingUnitDataAccess,
      CodeSecurityChecksCache securityCache)
    {
      this.m_indexingUnitDataAccess = indexingUnitDataAccess;
      this.DataAccessFactoryInstance = DataAccessFactory.GetInstance();
      this.m_securityCache = securityCache;
    }

    internal IDataAccessFactory DataAccessFactoryInstance { get; set; }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_numberOfReposToCheckInASecuritySet = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/NumberOfRepositoriesToCheckInASecuritySet") : throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SecuritySetsRebuildIntervalInSec"));
      this.m_securitySetsRebuildTaskEvent = new CountdownEvent(1);
      this.m_securitySetsRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildSecuritySetsCallback), (object) null, (int) timeSpan.TotalMilliseconds);
      if (this.m_securityCache == null)
      {
        this.m_securityCache = new CodeSecurityChecksCache(systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SecurityChecksCacheMaxSize"), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/SecurityChecksCacheExpirationInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/SecurityChecksCacheCleanupTaskIntervalInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/SecurityChecksCacheInactivityExpirationInMin")));
        this.m_disposeSecurityCache = true;
      }
      else
        this.m_disposeSecurityCache = false;
      IVssRequestContext vssRequestContext = systemRequestContext;
      this.m_collectionHostId = vssRequestContext.ServiceHost.CollectionServiceHost.InstanceId;
      this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
      IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.LoadGitSecurityNamespace(vssRequestContext);
      this.LoadSearchSecurityNamespace(vssRequestContext);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityNamespaceLoadTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      if (systemRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/ServiceStartSkipInlineSqlQueries"))
      {
        DateTime startTime = DateTime.UtcNow.AddSeconds((double) systemRequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/ServiceStartDelayInSqlQueryInSec"));
        this.m_securitySetsRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildSecuritySetsCallback), (object) null, startTime, (int) timeSpan.TotalMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081396, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("Adding repo build task at time {0}. StartTime for task was {1}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) startTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)))));
      }
      else
      {
        this.BuildRepositorySecuritySets(vssRequestContext);
        this.m_securitySetsRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildSecuritySetsCallback), (object) null, (int) timeSpan.TotalMilliseconds);
      }
      if (this.m_securitySetsRebuildTask != null)
        context.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, this.m_securitySetsRebuildTask);
      if (!systemRequestContext.IsFeatureEnabled("Search.Server.WikiSecurityChecksCache"))
        return;
      this.m_securityCache.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(systemRequestContext));
      try
      {
        IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
        IVssRequestContext requestContext = systemRequestContext;
        if (this.m_securitySetsRebuildTask != null)
          context.GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, this.m_securitySetsRebuildTask);
        this.m_securitySetsRebuildTaskEvent.Signal();
        this.m_securitySetsRebuildTaskEvent.Wait();
        this.m_securitySetsRebuildTaskEvent.Dispose();
        this.RepoSecuritySetsLock.Dispose();
        if (!systemRequestContext.IsFeatureEnabled("Search.Server.WikiSecurityChecksCache"))
          return;
        this.m_securityCache.TearDown(systemRequestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public IEnumerable<GitRepositoryData> GetUserAccessibleRepositories(
      IVssRequestContext requestContext,
      out bool allReposAreAccessible)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081461, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleRepositories));
      int numRssWithMismatchedAccess = 0;
      int numExceptions = 0;
      bool isCacheHit = false;
      Stopwatch timer = new Stopwatch();
      timer.Start();
      List<GitRepositoryData> accessibleRepos = new List<GitRepositoryData>();
      try
      {
        List<byte[]> allAccessibleRepoSetHashList = new List<byte[]>();
        int numRepoSecuritySets = this.RepositorySecuritySets.Count;
        bool flag = requestContext.IsFeatureEnabled("Search.Server.WikiSecurityChecksCache");
        if (flag)
          this.TryGettingUserAccessibleReposFromCache(requestContext, (ICodeSecurityChecksCache) this.m_securityCache, (string) null, 1.0, 0.0, out allAccessibleRepoSetHashList, out accessibleRepos, out isCacheHit);
        if (!isCacheHit)
          this.GetAllUserAccessibleRepositories(requestContext, (string) null, this.m_numberOfReposToCheckInASecuritySet, out allAccessibleRepoSetHashList, out accessibleRepos, out numRssWithMismatchedAccess, out numExceptions);
        else
          this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
        allReposAreAccessible = accessibleRepos.Count == this.m_totalRepos;
        if (accessibleRepos.Count == 0)
          this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
        if (!isCacheHit & flag)
          this.m_securityCache.UpdateRepositorySetsCache(requestContext, allAccessibleRepoSetHashList);
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1081347, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionHostId: {0}, NumberOfUserAccessibleRepos: {1}, UserAccessibleRepoTime: {2}, NumberOfRepoSecuritySets: {3}", (object) this.m_collectionHostId, (object) accessibleRepos.Count, (object) timer.ElapsedMilliseconds, (object) numRepoSecuritySets)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081348, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionHostId: {0}, RepoSecuritySetsWithMismatchedAccess: {1}, NumberOfExceptions: {2}", (object) this.m_collectionHostId, (object) numRssWithMismatchedAccess, (object) numExceptions)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetUserAccessibleWikiRepositoriesTime", "Query Pipeline", (double) timer.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfUserAccessibleWikiRepositories", "Query Pipeline", (double) accessibleRepos.Count);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfFailuresDuringWikiRepoAccessChecks", "Query Pipeline", (double) (numRssWithMismatchedAccess + numExceptions));
        if (requestContext.IsTracing(1081397, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
        {
          foreach (GitRepositoryData gitRepositoryData in accessibleRepos)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081397, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("UserAccessibleRepo: ID: {0}, ProjectID: {1}", (object) gitRepositoryData.Id, (object) gitRepositoryData.ProjectId)));
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081462, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleRepositories));
      }
      return (IEnumerable<GitRepositoryData>) accessibleRepos;
    }

    public void PopulateUserSecurityChecksDataInRequestContext(IVssRequestContext requestContext)
    {
      string token = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}", (object) "", (object) requestContext.DataspaceIdentifier));
      int requiredPermissions = CommonConstants.SearchReadPermissionBitForMembers;
      if (this.IsUserAnonymous(requestContext))
        requiredPermissions = CommonConstants.SearchReadPermissionBitForPublicUsers;
      this.SetSecurityData(requestContext, CommonConstants.SearchSecurityNamespaceGuid, requiredPermissions, token);
    }

    public bool IsUserAnonymous(IVssRequestContext requestContext)
    {
      if (!requestContext.Items.ContainsKey("isUserAnonymousKey"))
        this.ValidateAndSetUserPermissionsForSearchService(requestContext);
      return (bool) requestContext.Items["isUserAnonymousKey"];
    }

    public void ValidateAndSetUserPermissionsForSearchService(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081399, "Query Pipeline", "SecurityChecks", nameof (ValidateAndSetUserPermissionsForSearchService));
      try
      {
        string token = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}", (object) "", (object) requestContext.DataspaceIdentifier));
        if (this.SearchSecurityNamespace.HasPermission(requestContext, token, CommonConstants.SearchReadPermissionBitForMembers))
          requestContext.Items["isUserAnonymousKey"] = (object) false;
        else if (this.SearchSecurityNamespace.HasPermission(requestContext, token, CommonConstants.SearchReadPermissionBitForPublicUsers))
          requestContext.Items["isUserAnonymousKey"] = (object) true;
        else
          throw new InvalidAccessException(FormattableString.Invariant(FormattableStringFactory.Create("Expecting user to be authenticated or anonymous. User name is {0}", (object) requestContext.AuthenticatedUserName)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081400, "Query Pipeline", "SecurityChecks", nameof (ValidateAndSetUserPermissionsForSearchService));
      }
    }

    internal virtual void BuildRepositorySecuritySets(IVssRequestContext collectionRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(collectionRequestContext, "Git_Repository", (IEntityType) WikiEntityType.GetInstance(), -1);
        IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = (IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.GetIndexingUnits(collectionRequestContext, "Project", (IEntityType) WikiEntityType.GetInstance());
        this.m_totalRepos = indexingUnits1.Count;
        Dictionary<byte[], List<GitRepositoryData>> dictionary = new Dictionary<byte[], List<GitRepositoryData>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
        bool flag = collectionRequestContext.IsTracing(1081396, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks");
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in indexingUnits1)
        {
          GitCodeRepoTFSAttributes entityAttributes1 = indexingUnit1.TFSEntityAttributes as GitCodeRepoTFSAttributes;
          byte[] numArray = (indexingUnit1.Properties as GitCodeRepoIndexingProperties).SecurityHashcode;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
          indexingUnits2.TryGetValue(indexingUnit1.ParentUnitId, out indexingUnit2);
          if (indexingUnit2 != null)
          {
            if (!string.IsNullOrWhiteSpace(indexingUnit2.Properties?.Name ?? (indexingUnit2.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes3 ? entityAttributes3.ProjectName : (string) null)))
            {
              Guid tfsEntityId = indexingUnit2.TFSEntityId;
              GitRepositoryData gitRepositoryData = new GitRepositoryData()
              {
                Name = entityAttributes1.RepositoryName,
                Id = indexingUnit1.TFSEntityId,
                ProjectName = indexingUnit2.Properties?.Name ?? (indexingUnit2.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null),
                ProjectId = indexingUnit2.TFSEntityId
              };
              if (this.IsInvalidSecurityHash(numArray))
              {
                if (numArray != null && numArray.Length != RepositoryConstants.SecurityHashLength && numArray.Length > 1)
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081459, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Found an invalid hash: RepoId: '{0}', ProjectId: '{1}', hashValue: '{2}'", (object) gitRepositoryData.Id, (object) gitRepositoryData.ProjectId, (object) BitConverter.ToString(numArray))));
                numArray = this.GenerateUniqueHash();
              }
              List<GitRepositoryData> gitRepositoryDataList;
              if (!dictionary.TryGetValue(numArray, out gitRepositoryDataList))
              {
                gitRepositoryDataList = new List<GitRepositoryData>()
                {
                  gitRepositoryData
                };
                dictionary.Add(numArray, gitRepositoryDataList);
              }
              else
                gitRepositoryDataList.Add(gitRepositoryData);
              if (flag)
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081459, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Adding new entry with hash: '{0}', Id: '{1}', ProjectId: '{2}'", (object) BitConverter.ToString(numArray), (object) gitRepositoryData.Id, (object) gitRepositoryData.ProjectId)));
            }
            else
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081459, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project indexing unit.ProjectName or project indexing unit.ProjectId is null for git repo with id:{0}", (object) indexingUnit1.IndexingUnitId));
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081460, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "project indexing unit is null for git repo with indexing unit id:{0}", (object) indexingUnit1.IndexingUnitId));
        }
        this.RepoSecuritySetsLock.EnterWriteLock();
        try
        {
          this.RepositorySecuritySets = dictionary;
        }
        finally
        {
          this.RepoSecuritySetsLock.ExitWriteLock();
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("WikiRepositorySecuritySetCreationTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfWikiRepositorySecuritySets", "Query Pipeline", (double) dictionary.Count);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081458, "Query Pipeline", "SecurityChecks", ex);
      }
    }

    private void BuildSecuritySetsCallback(
      IVssRequestContext collectionRequestContext,
      object taskArgs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(collectionRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081343, "Query Pipeline", "SecurityChecks", nameof (BuildSecuritySetsCallback));
      try
      {
        this.m_securitySetsRebuildTaskEvent.AddCount();
        this.BuildRepositorySecuritySets(collectionRequestContext);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081358, "Query Pipeline", "SecurityChecks", ex);
      }
      finally
      {
        this.m_securitySetsRebuildTaskEvent.Signal();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081344, "Query Pipeline", "SecurityChecks", nameof (BuildSecuritySetsCallback));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      string indexingUnitType,
      IEntityType entityType)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, indexingUnitType, entityType, -1);
      Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits1)
        indexingUnits2.Add(indexingUnit.IndexingUnitId, indexingUnit);
      return indexingUnits2;
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.m_disposedValue)
      {
        if (disposing)
        {
          if (this.m_securitySetsRebuildTaskEvent != null)
          {
            this.m_securitySetsRebuildTaskEvent.Dispose();
            this.m_securitySetsRebuildTaskEvent = (CountdownEvent) null;
          }
          if (this.m_disposeSecurityCache && this.m_securityCache != null)
          {
            this.m_securityCache.Dispose();
            this.m_securityCache = (CodeSecurityChecksCache) null;
          }
        }
        this.m_disposedValue = true;
      }
      base.Dispose(disposing);
    }
  }
}
