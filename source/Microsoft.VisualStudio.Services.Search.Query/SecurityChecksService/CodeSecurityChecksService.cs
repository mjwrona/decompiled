// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.CodeSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  public class CodeSecurityChecksService : 
    GitRepoSecurityChecksService,
    ICodeSecurityChecksService,
    ISecurityChecksService,
    IVssFrameworkService,
    IDisposable
  {
    private const string ProjectSecurityTokenFormat = "{0}vstfs:///Classification/TeamProject/{1}";
    private const double SecurityChecksCacheHitKpi = 1.0;
    private const double SecurityChecksCacheMissKpi = 0.0;
    private readonly object m_projectSecurityNamespaceLock;
    private readonly ReaderWriterLockSlim m_projectsSetsLock;
    private readonly object m_tfsSecurityNamespaceLock;
    private Guid m_collectionHostId;
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private int m_numberOfReposToCheckInASecuritySet;
    private TeamFoundationTask m_projectListRebuildTask;
    private CountdownEvent m_projectListRebuildTaskEvent;
    private IVssSecurityNamespace m_projectSecurityNamespace;
    private CodeSecurityChecksCache m_securityCache;
    private TeamFoundationTask m_securitySetsRebuildTask;
    private CountdownEvent m_securitySetsRebuildTaskEvent;
    private IVssSecurityNamespace m_tfsSecurityNamespace;
    private IDictionary<string, Guid> m_tfvcProjectGuidMap;
    private int m_totalRepos;
    private bool m_isRepoSecuritySetBuilt;
    private bool m_disposeSecurityCache;
    private bool m_disposedValue;

    protected internal IList<TeamProjectReference> CustomRepoProjectsList { get; set; }

    protected internal IList<TeamProjectReference> ProjectsList { get; set; }

    public CodeSecurityChecksService()
      : this((IIndexingUnitDataAccess) null, (CodeSecurityChecksCache) null)
    {
    }

    internal CodeSecurityChecksService(
      IIndexingUnitDataAccess indexingUnitDataAccess,
      CodeSecurityChecksCache securityCache)
    {
      this.m_tfvcProjectGuidMap = (IDictionary<string, Guid>) new Dictionary<string, Guid>();
      this.RepositorySecuritySets = new Dictionary<byte[], List<GitRepositoryData>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
      this.m_projectsSetsLock = new ReaderWriterLockSlim();
      this.ProjectsList = (IList<TeamProjectReference>) new List<TeamProjectReference>();
      this.m_tfsSecurityNamespaceLock = new object();
      this.m_projectSecurityNamespaceLock = new object();
      this.m_indexingUnitDataAccess = indexingUnitDataAccess;
      this.DataAccessFactoryInstance = DataAccessFactory.GetInstance();
      this.m_securityCache = securityCache;
    }

    internal IDataAccessFactory DataAccessFactoryInstance { get; set; }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext context = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.To(TeamFoundationHostType.Deployment) : throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      IVssRequestContext vssRequestContext = systemRequestContext;
      this.m_numberOfReposToCheckInASecuritySet = systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/NumberOfRepositoriesToCheckInASecuritySet");
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SecuritySetsRebuildIntervalInSec"));
      this.m_securitySetsRebuildTaskEvent = new CountdownEvent(1);
      if (this.m_securityCache == null)
      {
        this.m_securityCache = new CodeSecurityChecksCache(systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SecurityChecksCacheMaxSize"), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/SecurityChecksCacheExpirationInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/SecurityChecksCacheCleanupTaskIntervalInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/SecurityChecksCacheInactivityExpirationInMin")));
        this.m_disposeSecurityCache = true;
      }
      else
        this.m_disposeSecurityCache = false;
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
      this.m_projectListRebuildTaskEvent = new CountdownEvent(1);
      this.m_collectionHostId = vssRequestContext.ServiceHost.CollectionServiceHost.InstanceId;
      this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.LoadSearchSecurityNamespace(vssRequestContext);
      this.LoadGitSecurityNamespace(vssRequestContext);
      this.LoadTfvcSecurityNamespace(vssRequestContext);
      this.LoadProjectSecurityNamespace(vssRequestContext);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityNamespaceLoadTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      if (systemRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/ServiceStartSkipInlineSqlQueries"))
      {
        DateTime startTime = DateTime.UtcNow.AddSeconds((double) systemRequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/ServiceStartDelayInSqlQueryInSec"));
        this.m_projectListRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildProjectsListCallback), (object) null, startTime, (int) timeSpan.TotalMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081396, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("Adding project build task at time {0}. StartTime for task was {1}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) startTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)))));
      }
      else
      {
        this.BuildProjectsList(vssRequestContext);
        this.m_projectListRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildProjectsListCallback), (object) null, (int) timeSpan.TotalMilliseconds);
      }
      if (this.m_projectListRebuildTask != null)
        context.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, this.m_projectListRebuildTask);
      if (!systemRequestContext.IsFeatureEnabled("Search.Server.SecurityChecksCache"))
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
        if (this.m_projectListRebuildTask != null)
          context.GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, this.m_projectListRebuildTask);
        this.m_securitySetsRebuildTaskEvent.Signal();
        this.m_securitySetsRebuildTaskEvent.Wait();
        this.m_securitySetsRebuildTaskEvent.Dispose();
        this.RepoSecuritySetsLock.Dispose();
        this.m_projectListRebuildTaskEvent.Signal();
        this.m_projectListRebuildTaskEvent.Wait();
        this.m_projectListRebuildTaskEvent.Dispose();
        this.m_projectsSetsLock.Dispose();
        if (!systemRequestContext.IsFeatureEnabled("Search.Server.SecurityChecksCache"))
          return;
        this.m_securityCache.TearDown(systemRequestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public IEnumerable<GitRepositoryData> GetUserAccessibleRepositoriesScopedToSearchQuery(
      IVssRequestContext userRequestContext,
      EntitySearchQuery searchQuery,
      out bool allReposAreAccessible)
    {
      if (searchQuery == null)
        throw new ArgumentNullException(nameof (searchQuery));
      if (!this.m_isRepoSecuritySetBuilt)
        throw new SearchException("Service initializing. Please retry in a few minutes.");
      if (userRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/SingleRepoSecurityCheckEnabled", true))
      {
        List<GitRepositoryData> scopedAccessibleRepos = new List<GitRepositoryData>();
        if (this.GetUserAccessibleScopedRepositories(userRequestContext, searchQuery, scopedAccessibleRepos))
        {
          allReposAreAccessible = false;
          return (IEnumerable<GitRepositoryData>) scopedAccessibleRepos;
        }
      }
      return this.GetUserAccessibleRepositories(userRequestContext, (string) null, out allReposAreAccessible);
    }

    public IEnumerable<string> GetUserAccessibleCustomProjects(IVssRequestContext requestContext) => this.GetUserAccessibleProjectsFromProjectList(requestContext, this.CustomRepoProjectsList ?? this.ProjectsList);

    public IEnumerable<GitRepositoryData> GetUserAccessibleRepositories(
      IVssRequestContext requestContext,
      out bool allReposAreAccessible)
    {
      return this.GetUserAccessibleRepositories(requestContext, (string) null, out allReposAreAccessible);
    }

    public IEnumerable<GitRepositoryData> GetUserAccessibleRepositories(
      IVssRequestContext requestContext,
      string projectIdentifier)
    {
      return this.GetUserAccessibleRepositories(requestContext, projectIdentifier, out bool _);
    }

    public IEnumerable<string> GetUserAccessibleProjects(IVssRequestContext requestContext) => this.GetUserAccessibleProjectsFromProjectList(requestContext, this.ProjectsList);

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

    public IEnumerable<CodeResult> GetUserAccessibleTfvcFiles(
      IVssRequestContext userRequestContext,
      IEnumerable<CodeResult> results)
    {
      if (userRequestContext == null)
        throw new ArgumentNullException(nameof (userRequestContext));
      if (results == null)
        throw new ArgumentNullException(nameof (results));
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<CodeResult> accessibleTfvcFiles = new List<CodeResult>();
      foreach (CodeResult result in results)
      {
        if (result.VcType == Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType.Git || result.VcType == Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType.Custom || result.VcType == Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType.Tfvc && this.TfvcFileHasReadPermission(userRequestContext, result.Path))
          accessibleTfvcFiles.Add(result);
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ReadAccessChecksTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      return (IEnumerable<CodeResult>) accessibleTfvcFiles;
    }

    public IEnumerable<FilterCategory> GetUserAccessibleFacets(
      IVssRequestContext userRequestContext,
      IEnumerable<FilterCategory> allFacets)
    {
      if (userRequestContext == null)
        throw new ArgumentNullException(nameof (userRequestContext));
      if (allFacets == null)
        throw new ArgumentNullException(nameof (allFacets));
      if (this.m_projectSecurityNamespace == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ProjectSecurityNamespaceNotLoaded", "Query Pipeline", 1.0);
        return allFacets;
      }
      List<FilterCategory> accessibleFacets = new List<FilterCategory>();
      foreach (FilterCategory allFacet in allFacets)
      {
        if (allFacet.Name.Equals("ProjectFilters", StringComparison.OrdinalIgnoreCase))
        {
          List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> filterList = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>();
          foreach (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter filter in allFacet.Filters)
          {
            bool flag1 = true;
            string name = filter.Name;
            bool flag2 = false;
            this.RepoSecuritySetsLock.EnterReadLock();
            Guid guid;
            try
            {
              if (this.m_tfvcProjectGuidMap.TryGetValue(name, out guid))
                flag2 = true;
            }
            finally
            {
              this.RepoSecuritySetsLock.ExitReadLock();
            }
            if (flag2)
            {
              string token = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}vstfs:///Classification/TeamProject/{1}", (object) PermissionNamespaces.Project, (object) guid.ToString());
              flag1 = this.m_projectSecurityNamespace.HasPermission(userRequestContext, token, 1, false);
            }
            if (flag1)
              filterList.Add(filter);
          }
          allFacet.Filters = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) filterList;
        }
        accessibleFacets.Add(allFacet);
      }
      return (IEnumerable<FilterCategory>) accessibleFacets;
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
          throw new InvalidAccessException(FormattableString.Invariant(FormattableStringFactory.Create("Expecting user to be authenticated or anonymous. User name is {0}", (object) requestContext.UserContext.Identifier)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081400, "Query Pipeline", "SecurityChecks", nameof (ValidateAndSetUserPermissionsForSearchService));
      }
    }

    private IEnumerable<GitRepositoryData> GetUserAccessibleRepositories(
      IVssRequestContext userRequestContext,
      string projectIdentifier,
      out bool allReposAreAccessible)
    {
      if (!this.m_isRepoSecuritySetBuilt)
        throw new SearchException("Service initializing. Please retry in a few minutes.");
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081341, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleRepositories));
      int numRssWithMismatchedAccess = 0;
      int numExceptions = 0;
      bool isCacheHit = false;
      Stopwatch timer = new Stopwatch();
      timer.Start();
      List<GitRepositoryData> accessibleRepos = (List<GitRepositoryData>) null;
      List<byte[]> allAccessibleRepoSetHashList = (List<byte[]>) null;
      try
      {
        this.RepoSecuritySetsLock.EnterReadLock();
        int numRepoSecuritySets;
        try
        {
          numRepoSecuritySets = this.RepositorySecuritySets.Count;
        }
        finally
        {
          this.RepoSecuritySetsLock.ExitReadLock();
        }
        bool flag = userRequestContext.IsFeatureEnabled("Search.Server.SecurityChecksCache");
        if (flag)
          this.TryGettingUserAccessibleReposFromCache(userRequestContext, (ICodeSecurityChecksCache) this.m_securityCache, projectIdentifier, 1.0, 0.0, out allAccessibleRepoSetHashList, out accessibleRepos, out isCacheHit);
        if (!isCacheHit)
        {
          this.GetAllUserAccessibleRepositories(userRequestContext, projectIdentifier, this.m_numberOfReposToCheckInASecuritySet, out allAccessibleRepoSetHashList, out accessibleRepos, out numRssWithMismatchedAccess, out numExceptions);
        }
        else
        {
          this.PopulateUserSecurityChecksDataInRequestContext(userRequestContext);
          if (userRequestContext.IsTracing(1081366, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
          {
            int num = 800;
            int count = accessibleRepos.Count;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 1; index <= count; ++index)
            {
              stringBuilder.Append((object) accessibleRepos[index - 1].Id);
              stringBuilder.Append(",");
              if (index % num == 0 || index == count)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081366, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("User ID = {0}. allReposAreAccessible = {1}. userAccessibleRepos = {2}", (object) userRequestContext.GetUserId(), (object) (accessibleRepos.Count == this.m_totalRepos), (object) stringBuilder)));
                stringBuilder.Clear();
              }
            }
          }
        }
        allReposAreAccessible = accessibleRepos.Count == this.m_totalRepos && this.m_totalRepos > 0;
        if (!isCacheHit & flag)
          this.m_securityCache.UpdateRepositorySetsCache(userRequestContext, allAccessibleRepoSetHashList);
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1081347, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionHostId: {0}, NumberOfUserAccessibleRepos: {1}, UserAccessibleRepoTime: {2}, NumberOfRepoSecuritySets: {3}", (object) this.m_collectionHostId, (object) accessibleRepos.Count, (object) timer.ElapsedMilliseconds, (object) numRepoSecuritySets)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081348, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionHostId: {0}, RepoSecuritySetsWithMismatchedAccess: {1}, NumberOfExceptions: {2}", (object) this.m_collectionHostId, (object) numRssWithMismatchedAccess, (object) numExceptions)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetUserAccessibleRepositoriesTime", "Query Pipeline", (double) timer.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfUserAccessibleRepositories", "Query Pipeline", (double) accessibleRepos.Count);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfFailuresDuringAccessChecks", "Query Pipeline", (double) (numRssWithMismatchedAccess + numExceptions));
        if (userRequestContext.IsTracing(1081397, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
        {
          foreach (GitRepositoryData gitRepositoryData in accessibleRepos)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081397, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("UserAccessibleRepo: ID: {0}, ProjectID: {1}", (object) gitRepositoryData.Id, (object) gitRepositoryData.ProjectId)));
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081342, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleRepositories));
      }
      return (IEnumerable<GitRepositoryData>) accessibleRepos;
    }

    private IEnumerable<string> GetUserAccessibleProjectsFromProjectList(
      IVssRequestContext userRequestContext,
      IList<TeamProjectReference> projectReferences)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081426, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleProjectsFromProjectList));
      Stopwatch timer = new Stopwatch();
      timer.Start();
      List<string> accessibleProjects = new List<string>();
      this.m_projectsSetsLock.EnterReadLock();
      try
      {
        foreach (TeamProjectReference projectReference in (IEnumerable<TeamProjectReference>) projectReferences)
        {
          if (this.HasProjectReadPermission(userRequestContext, projectReference))
            accessibleProjects.Add(projectReference.Id.ToString());
        }
        return (IEnumerable<string>) accessibleProjects;
      }
      finally
      {
        this.m_projectsSetsLock.ExitReadLock();
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1081430, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionHostId: {0}, NumberOfUserAccessibleProjects: {1}, UserAccessibleProjectsTime: {2}, TotalNoOfProjects: {3}", (object) this.m_collectionHostId, (object) accessibleProjects.Count, (object) timer.ElapsedMilliseconds, (object) projectReferences.Count)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetUserAccessibleProjectsTime", "Query Pipeline", (double) timer.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfUserAccessibleProjects", "Query Pipeline", (double) accessibleProjects.Count);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081429, "Query Pipeline", "Query Pipeline", nameof (GetUserAccessibleProjectsFromProjectList));
      }
    }

    internal virtual bool HasProjectReadPermission(
      IVssRequestContext userRequestContext,
      TeamProjectReference project)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081427, "Query Pipeline", "SecurityChecks", nameof (HasProjectReadPermission));
      bool flag = true;
      try
      {
        string token = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}vstfs:///Classification/TeamProject/{1}", (object) PermissionNamespaces.Project, (object) project.Id.ToString());
        flag = this.m_projectSecurityNamespace.HasPermission(userRequestContext, token, 1, false);
        if (flag)
          this.SetSecurityData(userRequestContext, FrameworkSecurity.TeamProjectNamespaceId, 1, token);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081350, "Query Pipeline", "SecurityChecks", "ExceptionAsWarning : " + ex?.ToString());
        flag = false;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081428, "Query Pipeline", "SecurityChecks", nameof (HasProjectReadPermission));
      }
      return flag;
    }

    private void BuildProjectsListCallback(
      IVssRequestContext collectionRequestContext,
      object taskArgs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(collectionRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081423, "Query Pipeline", "SecurityChecks", nameof (BuildProjectsListCallback));
      try
      {
        this.m_projectListRebuildTaskEvent.AddCount();
        this.BuildProjectsList(collectionRequestContext);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081354, "Query Pipeline", "SecurityChecks", ex);
      }
      finally
      {
        this.m_projectListRebuildTaskEvent.Signal();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081424, "Query Pipeline", "SecurityChecks", nameof (BuildProjectsListCallback));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
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
        if (this.m_securitySetsRebuildTaskEvent != null)
        {
          this.m_securitySetsRebuildTaskEvent.AddCount();
          this.BuildRepositorySecuritySets(collectionRequestContext);
          string message = "m_securitySetsRebuildTaskEvent is not null";
          collectionRequestContext.Trace(1083159, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks", message);
        }
        else
        {
          string message = "m_securitySetsRebuildTaskEvent is null";
          collectionRequestContext.Trace(1083159, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks", message);
        }
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

    private IDictionary<string, Guid> GetTfvcProjectGuidMap(
      IVssRequestContext requestContext,
      Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnitDictionary)
    {
      IDictionary<string, Guid> tfvcProjectGuidMap = (IDictionary<string, Guid>) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      try
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(requestContext, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
        if (indexingUnits != null)
        {
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits.OrderByDescending<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.IndexingUnitId)))
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnit1;
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
            projectIndexingUnitDictionary.TryGetValue(indexingUnit.ParentUnitId, out indexingUnit2);
            if (indexingUnit2?.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes)
            {
              string projectName = entityAttributes.ProjectName;
              if (!string.IsNullOrWhiteSpace(projectIndexingUnitDictionary[indexingUnit.ParentUnitId].Properties?.Name))
                projectName = projectIndexingUnitDictionary[indexingUnit.ParentUnitId].Properties?.Name;
              Guid projectId = projectIndexingUnitDictionary[indexingUnit.ParentUnitId].TFSEntityId;
              if (!string.IsNullOrWhiteSpace(projectName))
              {
                if (tfvcProjectGuidMap.ContainsKey(projectName))
                {
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1081390, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetTfvcProjectGuidMap: Duplicate Entry found: ProjectName: '{0}', ProjectGuid: '{1}', IndexingUnitId: '{2}'", (object) projectName, (object) projectId, (object) indexingUnit.IndexingUnitId)));
                }
                else
                {
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081394, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("GetTfvcProjectGuidMap: Adding new Entry ProjectName: '{0}', ProjectGuid: '{1}', IndexingUnitId: '{2}'", (object) projectName, (object) projectId, (object) indexingUnit.IndexingUnitId))));
                  tfvcProjectGuidMap.Add(projectName, projectIndexingUnitDictionary[indexingUnit.ParentUnitId].TFSEntityId);
                }
              }
              else
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081359, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project indexing unit.ProjectName or project indexing unit.ProjectId is null for tfvc repo with id:{0}", (object) indexingUnit.IndexingUnitId));
            }
            else
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081359, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "indexingUnit parent.TFSEntityAttributes as ProjectCodeTFSAttributes returned null for tfvc repo with id:{0}", (object) indexingUnit.IndexingUnitId));
          }
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081391, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetTfvcProjectGuidMap: ignoring excpetion: {0}", (object) ex));
      }
      return tfvcProjectGuidMap;
    }

    internal virtual ProjectHttpClientWrapper GetProjectHttpClientWrapper(
      IVssRequestContext requestContext)
    {
      return new ProjectHttpClientWrapper(requestContext, new TraceMetaData(1081422, "Query Pipeline", "SecurityChecks"));
    }

    internal virtual void BuildRepositorySecuritySets(IVssRequestContext collectionRequestContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081396, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("Started BuildRepositorySecuritySets task at time {0}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)))));
        this.m_indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(collectionRequestContext, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
        Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.GetIndexingUnits(collectionRequestContext, "Project", (IEntityType) CodeEntityType.GetInstance());
        IDictionary<string, Guid> tfvcProjectGuidMap = this.GetTfvcProjectGuidMap(collectionRequestContext, indexingUnits2);
        this.m_totalRepos = indexingUnits1.Count;
        Dictionary<byte[], List<GitRepositoryData>> dictionary = new Dictionary<byte[], List<GitRepositoryData>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
        bool flag = collectionRequestContext.IsTracing(1081396, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks");
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits1.OrderByDescending<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.IndexingUnitId)))
        {
          GitCodeRepoTFSAttributes entityAttributes1 = indexingUnit1.TFSEntityAttributes as GitCodeRepoTFSAttributes;
          byte[] hash = (indexingUnit1.Properties as GitCodeRepoIndexingProperties).SecurityHashcode;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
          indexingUnits2.TryGetValue(indexingUnit1.ParentUnitId, out indexingUnit2);
          if (indexingUnit2 != null)
          {
            string str = indexingUnit2.Properties?.Name ?? (indexingUnit2.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null);
            if (!string.IsNullOrWhiteSpace(str))
            {
              Guid tfsEntityId = indexingUnit2.TFSEntityId;
              GitRepositoryData repo = new GitRepositoryData()
              {
                Name = entityAttributes1.RepositoryName,
                Id = indexingUnit1.TFSEntityId,
                ProjectName = str,
                ProjectId = indexingUnit2.TFSEntityId,
                isdisabled = indexingUnit1.Properties.IsDisabled
              };
              if (this.IsInvalidSecurityHash(hash))
              {
                if (hash != null && hash.Length != Microsoft.VisualStudio.Services.Search.Common.RepositoryConstants.SecurityHashLength && hash.Length > 1)
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081398, "Query Pipeline", "Query", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Found an invalid hash: RepoId: '{0}', ProjectId: '{1}', hashValue: '{2}'", (object) repo.Id, (object) repo.ProjectId, (object) BitConverter.ToString(hash)))));
                else
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081398, "Query Pipeline", "Query", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Found null hash: RepoId: '{0}', ProjectId: '{1}'", (object) repo.Id, (object) repo.ProjectId))));
                hash = this.GenerateUniqueHash();
              }
              List<GitRepositoryData> gitRepositoryDataList;
              if (!dictionary.TryGetValue(hash, out gitRepositoryDataList))
              {
                gitRepositoryDataList = new List<GitRepositoryData>()
                {
                  repo
                };
                dictionary.Add(hash, gitRepositoryDataList);
              }
              else
                gitRepositoryDataList.Add(repo);
              if (flag)
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081396, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Adding new entry with hash: '{0}', Id: '{1}', ProjectId: '{2}'", (object) BitConverter.ToString(hash), (object) repo.Id, (object) repo.ProjectId)));
            }
            else
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081396, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project indexing unit.ProjectName or project indexing unit.ProjectId is null for git repo with id:{0}", (object) indexingUnit1.IndexingUnitId));
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081359, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "project indexing unit is null for git repo with indexing unit id:{0}", (object) indexingUnit1.IndexingUnitId));
        }
        this.RepoSecuritySetsLock.EnterWriteLock();
        Dictionary<byte[], List<GitRepositoryData>> repositorySecuritySets;
        try
        {
          repositorySecuritySets = this.RepositorySecuritySets;
          this.RepositorySecuritySets = dictionary;
          this.m_tfvcProjectGuidMap = tfvcProjectGuidMap;
          this.m_isRepoSecuritySetBuilt = true;
        }
        finally
        {
          this.RepoSecuritySetsLock.ExitWriteLock();
        }
        this.ClearSecurityChecksCacheIfNeeded(repositorySecuritySets.Keys.ToList<byte[]>());
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("RepositorySecuritySetCreationTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfRepositorySecuritySets", "Query Pipeline", (double) dictionary.Count);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081340, "Query Pipeline", "SecurityChecks", ex);
      }
    }

    internal virtual void BuildProjectsList(IVssRequestContext collectionRequestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      ProjectHttpClientWrapper httpClientWrapper = this.GetProjectHttpClientWrapper(collectionRequestContext);
      this.m_projectsSetsLock.EnterWriteLock();
      try
      {
        this.ProjectsList = (IList<TeamProjectReference>) httpClientWrapper.GetProjects().ToList<TeamProjectReference>();
        this.CustomRepoProjectsList = (IList<TeamProjectReference>) null;
        if (!collectionRequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", true))
          return;
        this.CustomRepoProjectsList = (IList<TeamProjectReference>) this.CreateCustomProjectList(collectionRequestContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfCustomProjects", "Query Pipeline", (double) this.CustomRepoProjectsList.Count);
      }
      finally
      {
        this.m_projectsSetsLock.ExitWriteLock();
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ProjectsListCreationTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfProjects", "Query Pipeline", (double) this.ProjectsList.Count);
      }
    }

    internal List<TeamProjectReference> CreateCustomProjectList(
      IVssRequestContext collectionRequestContext)
    {
      List<TeamProjectReference> customProjectList = new List<TeamProjectReference>();
      try
      {
        IDictionary<string, TeamProjectReference> dictionary = (IDictionary<string, TeamProjectReference>) new Dictionary<string, TeamProjectReference>();
        foreach (TeamProjectReference projects in (IEnumerable<TeamProjectReference>) this.ProjectsList)
          dictionary.Add(projects.Name, projects);
        IEnumerable<string> projects1 = this.DataAccessFactoryInstance.GetCustomRepositoryDataAccess().GetProjects(collectionRequestContext, collectionRequestContext.GetCollectionID());
        foreach (string key in projects1)
        {
          TeamProjectReference projectReference;
          if (dictionary.TryGetValue(key, out projectReference))
            customProjectList.Add(projectReference);
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083045, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("{0} is not present in TFS", (object) key)));
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfCustomProjectsInSql", "Query Pipeline", (double) projects1.Count<string>());
      }
      catch (Exception ex)
      {
        customProjectList = (List<TeamProjectReference>) null;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083045, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("{0} occured while creating Custom Project List", (object) ex)));
      }
      return customProjectList;
    }

    internal bool ClearSecurityChecksCacheIfNeeded(List<byte[]> oldSecuritySets)
    {
      this.RepoSecuritySetsLock.EnterReadLock();
      try
      {
        bool flag = false;
        if (oldSecuritySets.Count != this.RepositorySecuritySets.Count)
        {
          flag = true;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081398, "Query Pipeline", "Query", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Count of sets Not equal: OldSecuritySets: '{0}', New SecuritySets Count: '{1}'", (object) oldSecuritySets.Count, (object) this.RepositorySecuritySets.Count))));
        }
        else
        {
          foreach (byte[] oldSecuritySet in oldSecuritySets)
          {
            byte[] bytes = oldSecuritySet;
            if (!this.RepositorySecuritySets.ContainsKey(bytes))
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081398, "Query Pipeline", "Query", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Hash of old SecuritySets:'{0}', not Present in New SecuritySets'", (object) BitConverter.ToString(bytes)))));
              flag = true;
              break;
            }
          }
        }
        if (!flag || this.m_securityCache == null)
          return false;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081398, "Query Pipeline", "Query", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("repoSecuritySets: Hashes in old SecuritySets:'{0}', Hashes in New SecuritySets: '{1}'", (object) this.GetSecuritySetsString(oldSecuritySets), (object) this.GetSecuritySetsString(this.RepositorySecuritySets.Keys.ToList<byte[]>())))));
        this.m_securityCache.ClearCache();
        return true;
      }
      finally
      {
        this.RepoSecuritySetsLock.ExitReadLock();
      }
    }

    private string GetSecuritySetsString(List<byte[]> securitySetHashes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte[] securitySetHash in securitySetHashes)
        stringBuilder.Append(BitConverter.ToString(securitySetHash) + " ");
      return stringBuilder.ToString();
    }

    private void LoadTfvcSecurityNamespace(IVssRequestContext collectionRequestContext)
    {
      if (this.m_tfsSecurityNamespace != null)
        return;
      lock (this.m_tfsSecurityNamespaceLock)
      {
        if (this.m_tfsSecurityNamespace != null)
          return;
        SecurityChecksUtils.LoadRemoteSecurityNamespace(collectionRequestContext, SecurityConstants.RepositorySecurity2NamespaceGuid);
        this.m_tfsSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(collectionRequestContext, SecurityConstants.RepositorySecurity2NamespaceGuid);
        if (this.m_tfsSecurityNamespace != null)
          return;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081395, "Query Pipeline", "SecurityChecks", "Loading RepositorySecurity2NamespaceGuid failed");
      }
    }

    private void LoadProjectSecurityNamespace(IVssRequestContext collectionRequestContext)
    {
      if (this.m_projectSecurityNamespace != null)
        return;
      lock (this.m_projectSecurityNamespaceLock)
      {
        if (this.m_projectSecurityNamespace != null)
          return;
        SecurityChecksUtils.LoadRemoteSecurityNamespace(collectionRequestContext, FrameworkSecurity.TeamProjectNamespaceId);
        this.m_projectSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(collectionRequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      }
    }

    private bool TfvcFileHasReadPermission(IVssRequestContext userRequestContext, string filePath)
    {
      if (this.m_tfsSecurityNamespace == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityNamespaceNotLoaded", "Query Pipeline", 1.0);
        return true;
      }
      string token = filePath;
      if (filePath.HasProjectName())
      {
        string projectName = filePath.GetRootElement();
        bool flag = false;
        this.RepoSecuritySetsLock.EnterReadLock();
        Guid guid;
        try
        {
          flag = this.m_tfvcProjectGuidMap.TryGetValue(projectName, out guid);
        }
        finally
        {
          this.RepoSecuritySetsLock.ExitReadLock();
        }
        if (flag)
        {
          token = filePath.GetRelativePath().GetCompletePath(guid.ToString());
        }
        else
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081392, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("TfvcFileHasReadPermission, filePath:{0}, projectName: {1}", (object) filePath, (object) projectName))));
          return false;
        }
      }
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081393, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("TfvcFileHasReadPermission, filePath:{0}", (object) filePath))));
      Stopwatch readAccessChecksTimer = Stopwatch.StartNew();
      bool flag1 = false;
      try
      {
        flag1 = this.m_tfsSecurityNamespace.HasPermission(userRequestContext, token, 1, false);
        if (flag1)
          this.SetSecurityData(userRequestContext, SecurityConstants.RepositorySecurity2NamespaceGuid, 1, token);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081350, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("m_tfsSecurityNamespace.HasPermission failed for tfvcToken:{0} with exception: {1}", (object) token, (object) ex)));
      }
      readAccessChecksTimer.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081349, "Query Pipeline", "SecurityChecks", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionHostId: {0}, FilePath: {1}, TimeTakenPerFileInMilliSeconds: {2}", (object) this.m_collectionHostId, (object) filePath, (object) readAccessChecksTimer.ElapsedMilliseconds)));
      return flag1;
    }

    private bool GetUserAccessibleScopedRepositories(
      IVssRequestContext userRequestContext,
      EntitySearchQuery searchQuery,
      List<GitRepositoryData> scopedAccessibleRepos)
    {
      bool scopedRepositories = false;
      if (!searchQuery.SummarizedHitCountsNeeded)
      {
        IEnumerable<SearchFilter> filters = searchQuery.Filters;
        if ((filters != null ? (filters.Count<SearchFilter>() >= 2 ? 1 : 0) : 0) != 0)
        {
          string singleValuedElseNull1 = searchQuery.GetFilterIfPresentAndSingleValuedElseNull("ProjectFilters");
          string singleValuedElseNull2 = searchQuery.GetFilterIfPresentAndSingleValuedElseNull("RepositoryFilters");
          if (!string.IsNullOrWhiteSpace(singleValuedElseNull1) && !string.IsNullOrWhiteSpace(singleValuedElseNull2))
          {
            QueryingUnit chilQueryingUnit1 = userRequestContext.GetService<CodeQueryScopingCacheService>().GetCacheRoot()?.GetChilQueryingUnit(singleValuedElseNull1);
            QueryingUnit chilQueryingUnit2 = chilQueryingUnit1?.GetChilQueryingUnit(singleValuedElseNull2);
            if (chilQueryingUnit1 != null && chilQueryingUnit2 != null && chilQueryingUnit1.TFSEntityId != Guid.Empty && chilQueryingUnit2.TFSEntityId != Guid.Empty)
            {
              if (chilQueryingUnit2.IndexingUnitType == "Git_Repository")
              {
                GitRepositoryData repo = new GitRepositoryData()
                {
                  Id = chilQueryingUnit2.TFSEntityId,
                  Name = chilQueryingUnit2.EntityName,
                  ProjectId = chilQueryingUnit1.TFSEntityId,
                  ProjectName = chilQueryingUnit1.EntityName
                };
                if (this.GitRepoHasReadPermission(userRequestContext, repo))
                  scopedAccessibleRepos.Add(repo);
              }
              scopedRepositories = true;
            }
          }
        }
      }
      return scopedRepositories;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing)
      {
        if (this.m_projectListRebuildTaskEvent != null)
        {
          this.m_projectListRebuildTaskEvent.Dispose();
          this.m_projectListRebuildTaskEvent = (CountdownEvent) null;
        }
        if (this.m_projectsSetsLock != null)
          this.m_projectsSetsLock.Dispose();
        if (this.m_disposeSecurityCache && this.m_securityCache != null)
        {
          this.m_securityCache.Dispose();
          this.m_securityCache = (CodeSecurityChecksCache) null;
        }
        if (this.m_securitySetsRebuildTaskEvent != null)
        {
          this.m_securitySetsRebuildTaskEvent.Dispose();
          this.m_securitySetsRebuildTaskEvent = (CountdownEvent) null;
        }
        this.m_disposedValue = true;
      }
      base.Dispose(disposing);
    }
  }
}
