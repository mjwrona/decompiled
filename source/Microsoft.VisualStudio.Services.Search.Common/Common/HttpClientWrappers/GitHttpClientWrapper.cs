// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GitHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class GitHttpClientWrapper
  {
    private readonly GitHttpClient m_gitHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly Guid m_repositoryId;
    private readonly Guid m_projectId;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;
    private readonly bool m_shouldRetry;
    private readonly bool m_isGetLatestRefEnabled;

    protected internal GitHttpClientWrapper()
    {
    }

    public GitHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
      : this(executionContext, Guid.Empty, Guid.Empty, traceMetadata)
    {
    }

    public GitHttpClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      string projectId,
      string repositoryId,
      TraceMetaData traceMetadata)
      : this(executionContext, new Guid(projectId), new Guid(repositoryId), traceMetadata)
    {
    }

    public GitHttpClientWrapper(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      Guid projectId,
      Guid repositoryId,
      TraceMetaData traceMetadata)
    {
      this.m_gitHttpClient = executionContext != null ? executionContext.RequestContext.GetRedirectedClientIfNeeded<GitHttpClient>(executionContext.RequestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("Search.Server.Git.EnableReadReplica")) : throw new ArgumentNullException(nameof (executionContext));
      this.m_faultService = executionContext.FaultService;
      this.m_projectId = projectId;
      this.m_repositoryId = repositoryId;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
      this.m_shouldRetry = !executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      this.m_isGetLatestRefEnabled = executionContext.RequestContext.IsGetLatestRefEnabled();
    }

    public virtual List<GitRef> GetRefs(string branchName) => this.GetRefs(this.m_repositoryId, branchName);

    public virtual List<GitRef> GetRefs(Guid repositoryId, string branchName)
    {
      if (branchName == null)
        throw new ArgumentNullException(nameof (branchName));
      string str = "refs/";
      string refName = branchName.Substring(str.Length);
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<GitRef>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<GitRef>>((Func<CancellationTokenSource, Task<List<GitRef>>>) (tokenSource =>
      {
        GitHttpClient gitHttpClient = this.m_gitHttpClient;
        Guid repositoryId1 = repositoryId;
        string filter = refName;
        int? nullable = this.m_isGetLatestRefEnabled ? new int?(1) : new int?();
        CancellationToken token = tokenSource.Token;
        bool? includeLinks = new bool?();
        bool? includeStatuses = new bool?();
        bool? includeMyBranches = new bool?();
        bool? latestStatusesOnly = new bool?();
        bool? peelTags = new bool?();
        int? top = nullable;
        CancellationToken cancellationToken = token;
        return gitHttpClient.GetRefsAsync(repositoryId1, filter, includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, (string) null, top, (string) null, (object) null, cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual List<GitRef> GetAllRefs() => this.GetAllRefs(this.m_repositoryId);

    public virtual List<GitRef> GetAllRefs(Guid repositoryId) => AsyncInvoker.InvokeAsyncWait<List<GitRef>>((Func<CancellationTokenSource, Task<List<GitRef>>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid repositoryId1 = repositoryId;
      CancellationToken token = tokenSource.Token;
      bool? includeLinks = new bool?();
      bool? includeStatuses = new bool?();
      bool? includeMyBranches = new bool?();
      bool? latestStatusesOnly = new bool?();
      bool? peelTags = new bool?();
      int? top = new int?();
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetRefsAsync(repositoryId1, (string) null, includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, (string) null, top, (string) null, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData);

    public virtual GitCommit GetLatestCommit(Guid repositoryId, string branchName)
    {
      List<GitRef> refs = this.GetRefs(repositoryId, branchName);
      return refs != null && !refs.All<GitRef>((Func<GitRef, bool>) (item => item == null)) ? this.GetCommit(repositoryId, refs[0].ObjectId) : (GitCommit) null;
    }

    public virtual GitCommit GetLatestCommit(string branchName) => this.GetLatestCommit(this.m_repositoryId, branchName);

    public virtual GitCommit GetCommit(string commitId) => this.GetCommit(this.m_repositoryId, commitId);

    public virtual List<GitPush> GetGitPushes(
      Guid repositoryId,
      string refName,
      DateTime commitDate,
      int? top = null)
    {
      GitPushSearchCriteria gitPushSearchCriteria = new GitPushSearchCriteria()
      {
        FromDate = new DateTime?(commitDate),
        RefName = refName,
        IncludeRefUpdates = true,
        IncludeLinks = true
      };
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<GitPush>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<GitPush>>((Func<CancellationTokenSource, Task<List<GitPush>>>) (tokenSource =>
      {
        GitHttpClient gitHttpClient = this.m_gitHttpClient;
        Guid repositoryId1 = repositoryId;
        GitPushSearchCriteria pushSearchCriteria = gitPushSearchCriteria;
        int? nullable = top;
        CancellationToken token = tokenSource.Token;
        int? skip = new int?();
        int? top1 = nullable;
        GitPushSearchCriteria searchCriteria = pushSearchCriteria;
        CancellationToken cancellationToken = token;
        return gitHttpClient.GetPushesAsync(repositoryId1, skip, top1, searchCriteria, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual GitPush GetGitPush(Guid repositoryId, int pushId) => this.m_expRetryInvoker.InvokeWithFaultCheck<GitPush>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitPush>((Func<CancellationTokenSource, Task<GitPush>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid repositoryId1 = repositoryId;
      int pushId1 = pushId;
      CancellationToken token = tokenSource.Token;
      int? includeCommits = new int?();
      bool? includeRefUpdates = new bool?();
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetPushAsync(repositoryId1, pushId1, includeCommits, includeRefUpdates, cancellationToken: cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual GitCommit GetCommit(Guid repositoryId, string commitId) => this.m_expRetryInvoker.InvokeWithFaultCheck<GitCommit>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitCommit>((Func<CancellationTokenSource, Task<GitCommit>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid guid = repositoryId;
      string commitId1 = commitId;
      Guid repositoryId1 = guid;
      int? changeCount = new int?();
      CancellationToken token = tokenSource.Token;
      return gitHttpClient.GetCommitAsync(commitId1, repositoryId1, changeCount, cancellationToken: token);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual GitBranchStats GetBranchStat(string project, string branch)
    {
      using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
        return this.m_gitHttpClient.GetBranchAsync(project, this.m_repositoryId, branch, cancellationToken: cancellationTokenSource.Token).Result;
    }

    public virtual GitTreeRef GetTreeRef(string objectId) => this.m_expRetryInvoker.InvokeWithFaultCheck<GitTreeRef>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitTreeRef>((Func<CancellationTokenSource, Task<GitTreeRef>>) (tokenSource => this.m_gitHttpClient.GetTreeAsync(this.m_repositoryId, objectId, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Stream GetBlobStream(string objectId) => this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid repositoryId = this.m_repositoryId;
      string sha1 = objectId;
      bool? download = new bool?(true);
      CancellationToken token = tokenSource.Token;
      bool? resolveLfs = new bool?();
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetBlobContentAsync(repositoryId, sha1, download, (string) null, resolveLfs, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual List<GitRepository> GetRepositoriesInProject(
      string projectId,
      bool excludeForkedRepositories,
      bool includeLinks = false)
    {
      List<GitRepository> repositoriesInProject = this.GetRepositoriesInProject(projectId, includeLinks) ?? new List<GitRepository>();
      if (excludeForkedRepositories && repositoriesInProject != null)
        repositoriesInProject.RemoveAll(new Predicate<GitRepository>(this.IsForkedRepository));
      return repositoriesInProject;
    }

    public virtual List<GitRepository> GetRepositoriesInProject(string projectId, bool includeLinks = false) => this.m_expRetryInvoker.InvokeWithFaultCheck<List<GitRepository>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<GitRepository>>((Func<CancellationTokenSource, Task<List<GitRepository>>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      string project = projectId;
      bool? includeLinks1 = new bool?(includeLinks);
      CancellationToken token = tokenSource.Token;
      bool? includeAllUrls = new bool?();
      bool? includeHidden = new bool?();
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetRepositoriesAsync(project, includeLinks1, includeAllUrls, includeHidden, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Stream GetFileInRepository(Guid repositoryId, string path) => this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource => this.m_gitHttpClient.GetItemContentAsync(repositoryId, path)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual string GetLatestProcessedChange(Guid repositoryId, string path) => this.m_expRetryInvoker.InvokeWithFaultCheck<string>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitItem>((Func<CancellationTokenSource, Task<GitItem>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid repositoryId1 = repositoryId;
      string path1 = path;
      bool? nullable = new bool?(true);
      VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
      bool? includeContentMetadata = new bool?();
      bool? latestProcessedChange = nullable;
      bool? download = new bool?();
      bool? includeContent = new bool?();
      bool? resolveLfs = new bool?();
      bool? sanitize = new bool?();
      CancellationToken cancellationToken = new CancellationToken();
      return gitHttpClient.GetItemAsync(repositoryId1, path1, recursionLevel: recursionLevel, includeContentMetadata: includeContentMetadata, latestProcessedChange: latestProcessedChange, download: download, includeContent: includeContent, resolveLfs: resolveLfs, sanitize: sanitize, cancellationToken: cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData).LatestProcessedChange.CommitId), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Stream GetBlobContentAsync(string blobId) => this.m_shouldRetry ? this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid projectId = this.m_projectId;
      Guid repositoryId = this.m_repositoryId;
      string sha1 = blobId;
      CancellationToken token = tokenSource.Token;
      bool? download = new bool?();
      bool? resolveLfs = new bool?();
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetBlobContentAsync(projectId, repositoryId, sha1, download, (string) null, resolveLfs, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData) : AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid projectId = this.m_projectId;
      Guid repositoryId = this.m_repositoryId;
      string sha1 = blobId;
      CancellationToken token = tokenSource.Token;
      bool? download = new bool?();
      bool? resolveLfs = new bool?();
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetBlobContentAsync(projectId, repositoryId, sha1, download, (string) null, resolveLfs, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData);

    public virtual List<GitPush> GetPushesAsync(
      int? skip,
      int? top,
      GitPushSearchCriteria searchCriteria)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<GitPush>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<GitPush>>((Func<CancellationTokenSource, Task<List<GitPush>>>) (tokenSource => this.m_gitHttpClient.GetPushesAsync(this.m_projectId, this.m_repositoryId, skip, top, searchCriteria, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual GitCommitDiffs GetCommitDiffsAsync(
      string baseVersion,
      string targetVersion,
      int? top,
      int? skip)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<GitCommitDiffs>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitCommitDiffs>((Func<CancellationTokenSource, Task<GitCommitDiffs>>) (tokenSource =>
      {
        GitHttpClient gitHttpClient = this.m_gitHttpClient;
        Guid projectId = this.m_projectId;
        Guid repositoryId = this.m_repositoryId;
        GitBaseVersionDescriptor versionDescriptor1 = new GitBaseVersionDescriptor()
        {
          VersionType = GitVersionType.Commit,
          Version = baseVersion,
          VersionOptions = GitVersionOptions.None
        };
        GitTargetVersionDescriptor versionDescriptor2 = new GitTargetVersionDescriptor()
        {
          VersionType = GitVersionType.Commit,
          Version = targetVersion,
          VersionOptions = GitVersionOptions.None
        };
        int? nullable1 = top;
        int? nullable2 = skip;
        bool? diffCommonCommit = new bool?();
        int? top1 = nullable1;
        int? skip1 = nullable2;
        GitBaseVersionDescriptor baseVersionDescriptor = versionDescriptor1;
        GitTargetVersionDescriptor targetVersionDescriptor = versionDescriptor2;
        CancellationToken cancellationToken = new CancellationToken();
        return gitHttpClient.GetCommitDiffsAsync(projectId, repositoryId, diffCommonCommit, top1, skip1, baseVersionDescriptor, targetVersionDescriptor, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual GitItem GetItemAsync(string itemPath, GitVersionDescriptor versionDescriptor) => this.m_expRetryInvoker.InvokeWithFaultCheck<GitItem>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitItem>((Func<CancellationTokenSource, Task<GitItem>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid projectId = this.m_projectId;
      Guid repositoryId = this.m_repositoryId;
      string path = itemPath;
      VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?(VersionControlRecursionType.None);
      GitVersionDescriptor versionDescriptor1 = versionDescriptor;
      bool? includeContentMetadata = new bool?();
      bool? latestProcessedChange = new bool?();
      bool? download = new bool?();
      GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
      bool? includeContent = new bool?();
      bool? resolveLfs = new bool?();
      bool? sanitize = new bool?();
      CancellationToken cancellationToken = new CancellationToken();
      return gitHttpClient.GetItemAsync(projectId, repositoryId, path, recursionLevel: recursionLevel, includeContentMetadata: includeContentMetadata, latestProcessedChange: latestProcessedChange, download: download, versionDescriptor: versionDescriptor2, includeContent: includeContent, resolveLfs: resolveLfs, sanitize: sanitize, cancellationToken: cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual GitItem GetItem(
      string itemPath,
      GitVersionDescriptor versionDescriptor,
      out bool isDeleted)
    {
      isDeleted = false;
      GitItem gitItem = (GitItem) null;
      try
      {
        gitItem = this.GetItemAsync(itemPath, versionDescriptor);
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (GitItemDoesNotExistFaultMapper)).IsMatch(ex))
        {
          isDeleted = true;
          return gitItem;
        }
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      return gitItem;
    }

    public virtual GitItem GetItemAsync(string itemPath) => this.m_expRetryInvoker.InvokeWithFaultCheck<GitItem>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitItem>((Func<CancellationTokenSource, Task<GitItem>>) (tokenSource => this.m_gitHttpClient.GetItemAsync(this.m_projectId, this.m_repositoryId, itemPath)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Stream GetItemContentAsync(
      Guid projectId,
      Guid repositoryId,
      string itemPath,
      GitVersionDescriptor versionDescriptor)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
      {
        GitHttpClient gitHttpClient = this.m_gitHttpClient;
        Guid project = projectId;
        Guid repositoryId1 = repositoryId;
        string path = itemPath;
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?(VersionControlRecursionType.None);
        GitVersionDescriptor versionDescriptor1 = versionDescriptor;
        bool? includeContentMetadata = new bool?();
        bool? latestProcessedChange = new bool?();
        bool? download = new bool?();
        GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
        bool? includeContent = new bool?();
        bool? resolveLfs = new bool?();
        bool? sanitize = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        return gitHttpClient.GetItemContentAsync(project, repositoryId1, path, recursionLevel: recursionLevel, includeContentMetadata: includeContentMetadata, latestProcessedChange: latestProcessedChange, download: download, versionDescriptor: versionDescriptor2, includeContent: includeContent, resolveLfs: resolveLfs, sanitize: sanitize, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual List<GitItem> GetItemsAsync(
      GitVersionDescriptor versionDescriptor,
      Guid projectId,
      Guid repositoryId,
      string scopePath = "/",
      VersionControlRecursionType versionControlRecursionType = VersionControlRecursionType.Full)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<GitItem>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<GitItem>>((Func<CancellationTokenSource, Task<List<GitItem>>>) (tokenSource =>
      {
        GitHttpClient gitHttpClient = this.m_gitHttpClient;
        Guid project = projectId;
        Guid repositoryId1 = repositoryId;
        string scopePath1 = scopePath;
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?(versionControlRecursionType);
        GitVersionDescriptor versionDescriptor1 = versionDescriptor;
        bool? includeContentMetadata = new bool?();
        bool? latestProcessedChange = new bool?();
        bool? download = new bool?();
        bool? includeLinks = new bool?();
        GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
        bool? zipForUnix = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        return gitHttpClient.GetItemsAsync(project, repositoryId1, scopePath1, recursionLevel, includeContentMetadata, latestProcessedChange, download, includeLinks, versionDescriptor2, zipForUnix, (object) null, cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual List<GitItem> GetItemsAsync(
      GitVersionDescriptor versionDescriptor,
      string scopePath = "/",
      VersionControlRecursionType versionControlRecursionType = VersionControlRecursionType.Full)
    {
      return this.GetItemsAsync(versionDescriptor, this.m_projectId, this.m_repositoryId, scopePath, versionControlRecursionType);
    }

    public virtual IPagedList<GitItem> GetItemsPagedAsync(
      GitVersionDescriptor versionDescriptor,
      Guid projectId,
      Guid repositoryId,
      string scopePath = "/",
      string continuationToken = null,
      int? top = null)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<IPagedList<GitItem>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<IPagedList<GitItem>>((Func<CancellationTokenSource, Task<IPagedList<GitItem>>>) (tokenSource =>
      {
        GitHttpClient gitHttpClient = this.m_gitHttpClient;
        Guid project = projectId;
        Guid repositoryId1 = repositoryId;
        string scopePath1 = scopePath;
        string str = continuationToken;
        int? top1 = top;
        string continuationToken1 = str;
        GitVersionDescriptor versionDescriptor1 = versionDescriptor;
        CancellationToken cancellationToken = new CancellationToken();
        return gitHttpClient.GetItemsPagedAsync(project, repositoryId1, scopePath1, top1, continuationToken1, versionDescriptor1, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual IPagedList<GitItem> GetItemsPagedAsync(
      GitVersionDescriptor versionDescriptor,
      string scopePath = "/",
      string continuationToken = null,
      int? top = null)
    {
      return this.GetItemsPagedAsync(versionDescriptor, this.m_projectId, this.m_repositoryId, scopePath, continuationToken, top);
    }

    public virtual GitTreeDiffResponse GetTreeDiffsAsync(
      string baseId,
      string targetId,
      string continuationToken = null,
      int? top = null)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<GitTreeDiffResponse>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitTreeDiffResponse>((Func<CancellationTokenSource, Task<GitTreeDiffResponse>>) (tokenSource => this.m_gitHttpClient.GetTreeDiffsAsync(this.m_projectId, this.m_repositoryId, baseId, targetId, top, continuationToken)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual IEnumerable<GitTreeDiffEntry> GetTreeDiffsForRootTreeAsync(
      string baseCommitId,
      string targetCommitId,
      string scopePath = "/")
    {
      GitVersionDescriptor versionDescriptor1 = new GitVersionDescriptor()
      {
        Version = baseCommitId,
        VersionType = GitVersionType.Commit
      };
      GitVersionDescriptor versionDescriptor2 = new GitVersionDescriptor()
      {
        Version = targetCommitId,
        VersionType = GitVersionType.Commit
      };
      List<GitItem> itemsAsync1 = this.GetItemsAsync(versionDescriptor1, scopePath, VersionControlRecursionType.OneLevel);
      List<GitItem> itemsAsync2 = this.GetItemsAsync(versionDescriptor2, scopePath, VersionControlRecursionType.OneLevel);
      Dictionary<string, GitItem> baseGitItemDictionary = new Dictionary<string, GitItem>();
      itemsAsync1?.ForEach((Action<GitItem>) (x => baseGitItemDictionary.Add(x.Path, x)));
      Dictionary<string, GitItem> targetGitItemDictionary = new Dictionary<string, GitItem>();
      itemsAsync2?.ForEach((Action<GitItem>) (x => targetGitItemDictionary.Add(x.Path, x)));
      List<GitTreeDiffEntry> forRootTreeAsync = new List<GitTreeDiffEntry>();
      foreach (KeyValuePair<string, GitItem> keyValuePair in targetGitItemDictionary)
      {
        string key = keyValuePair.Key;
        if (baseGitItemDictionary.Keys.Contains<string>(key))
        {
          if (baseGitItemDictionary[key].ObjectId != targetGitItemDictionary[key].ObjectId)
            forRootTreeAsync.Add(new GitTreeDiffEntry()
            {
              BaseObjectId = baseGitItemDictionary[key].ObjectId,
              ChangeType = VersionControlChangeType.Edit,
              ObjectType = targetGitItemDictionary[key].GitObjectType,
              Path = key,
              TargetObjectId = targetGitItemDictionary[key].ObjectId
            });
          baseGitItemDictionary.Remove(key);
        }
        else
          forRootTreeAsync.Add(new GitTreeDiffEntry()
          {
            BaseObjectId = RepositoryConstants.BranchCreationOrDeletionCommitId,
            ChangeType = VersionControlChangeType.Add,
            ObjectType = targetGitItemDictionary[key].GitObjectType,
            Path = key,
            TargetObjectId = targetGitItemDictionary[key].ObjectId
          });
      }
      foreach (KeyValuePair<string, GitItem> keyValuePair in baseGitItemDictionary)
      {
        string key = keyValuePair.Key;
        forRootTreeAsync.Add(new GitTreeDiffEntry()
        {
          BaseObjectId = baseGitItemDictionary[key].ObjectId,
          ChangeType = VersionControlChangeType.Delete,
          ObjectType = baseGitItemDictionary[key].GitObjectType,
          Path = key,
          TargetObjectId = RepositoryConstants.BranchCreationOrDeletionCommitId
        });
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      forRootTreeAsync.Sort(GitHttpClientWrapper.\u003C\u003EO.\u003C0\u003E__CompareGitTreeDiffEntry ?? (GitHttpClientWrapper.\u003C\u003EO.\u003C0\u003E__CompareGitTreeDiffEntry = new Comparison<GitTreeDiffEntry>(GitHttpClientWrapper.CompareGitTreeDiffEntry)));
      return (IEnumerable<GitTreeDiffEntry>) forRootTreeAsync;
    }

    private static int CompareGitTreeDiffEntry(
      GitTreeDiffEntry firstGitTreeDiffEntry,
      GitTreeDiffEntry secondGitTreeDiffEntry)
    {
      return CrawlerHelpers.GitPathStringCompare(firstGitTreeDiffEntry.Path, secondGitTreeDiffEntry.Path);
    }

    public virtual Stream GetBlobsZipAsync(List<string> blobIds) => this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource => this.m_gitHttpClient.GetBlobsZipAsync((IEnumerable<string>) blobIds, this.m_projectId, this.m_repositoryId)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);

    public virtual Stream GetBlobContentResolvingLFSAsync(string blobId) => this.m_shouldRetry ? this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid projectId = this.m_projectId;
      Guid repositoryId = this.m_repositoryId;
      string sha1 = blobId;
      bool? nullable = new bool?(true);
      CancellationToken token = tokenSource.Token;
      bool? download = new bool?();
      bool? resolveLfs = nullable;
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetBlobContentAsync(projectId, repositoryId, sha1, download, (string) null, resolveLfs, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData) : AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
    {
      GitHttpClient gitHttpClient = this.m_gitHttpClient;
      Guid projectId = this.m_projectId;
      Guid repositoryId = this.m_repositoryId;
      string sha1 = blobId;
      bool? nullable = new bool?(true);
      CancellationToken token = tokenSource.Token;
      bool? download = new bool?();
      bool? resolveLfs = nullable;
      CancellationToken cancellationToken = token;
      return gitHttpClient.GetBlobContentAsync(projectId, repositoryId, sha1, download, (string) null, resolveLfs, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData);

    public virtual GitRepository GetRepositoryAsync(Guid repositoryId, bool excludeForkedRepository)
    {
      GitRepository repositoryAsync = this.GetRepositoryAsync(repositoryId);
      return excludeForkedRepository && this.IsForkedRepository(repositoryAsync) ? (GitRepository) null : repositoryAsync;
    }

    public virtual GitRepository GetRepositoryAsync(Guid repositoryId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        return this.m_expRetryInvoker.InvokeWithFaultCheck<GitRepository>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<GitRepository>((Func<CancellationTokenSource, Task<GitRepository>>) (tokenSource => this.m_gitHttpClient.GetRepositoryAsync(repositoryId, (object) null, tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      }
      finally
      {
        stopwatch.Stop();
      }
    }

    public virtual IEnumerable<GitRepository> GetRepositoriesAsync(bool includeHidden = false)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        return this.m_expRetryInvoker.InvokeWithFaultCheck<IEnumerable<GitRepository>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<GitRepository>>((Func<CancellationTokenSource, Task<List<GitRepository>>>) (tokenSource =>
        {
          GitHttpClient gitHttpClient = this.m_gitHttpClient;
          bool? includeLinks = new bool?();
          bool? nullable = new bool?(includeHidden);
          CancellationToken token = tokenSource.Token;
          bool? includeAllUrls = new bool?();
          bool? includeHidden1 = nullable;
          CancellationToken cancellationToken = token;
          return gitHttpClient.GetRepositoriesAsync((string) null, includeLinks, includeAllUrls, includeHidden1, (object) null, cancellationToken);
        }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("TFSGetRepositoriesCall", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }

    public virtual List<List<GitItem>> GetItemsBatch(GitItemRequestData gitItemRequestData)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        return AsyncInvoker.InvokeAsyncWait<List<List<GitItem>>>((Func<CancellationTokenSource, Task<List<List<GitItem>>>>) (tokenSource =>
        {
          GitHttpClient gitHttpClient = this.m_gitHttpClient;
          Guid projectId = this.m_projectId;
          Guid repositoryId1 = this.m_repositoryId;
          GitItemRequestData requestData = gitItemRequestData;
          Guid project = projectId;
          Guid repositoryId2 = repositoryId1;
          CancellationToken token = tokenSource.Token;
          return gitHttpClient.GetItemsBatchAsync(requestData, project, repositoryId2, (object) null, token);
        }), this.m_waitTimeInMs, this.m_traceMetaData);
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("GitGitItemsBatchCall", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }

    public virtual int GetDocumentCount(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid repositoryId,
      string scopePath,
      string commitId,
      VersionControlRecursionType versionControlRecursionType = VersionControlRecursionType.OneLevel)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new ArgumentException("Scope path is null or whitespace", nameof (scopePath));
      if (string.IsNullOrWhiteSpace(commitId) || commitId == RepositoryConstants.DefaultLastIndexCommitId)
        return 0;
      GitVersionDescriptor versionDescriptor = new GitVersionDescriptor()
      {
        Version = commitId,
        VersionType = GitVersionType.Commit
      };
      if ((!scopePath.StartsWith("/", StringComparison.Ordinal) || scopePath.Length <= "/".Length) && versionControlRecursionType != VersionControlRecursionType.Full)
        return this.GetItemsAsync(versionDescriptor, projectId, repositoryId, scopePath, VersionControlRecursionType.OneLevel).Count<GitItem>((Func<GitItem, bool>) (x => x.GitObjectType == GitObjectType.Blob));
      string continuationToken = string.Empty;
      int documentCount = 0;
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PatchInMemoryThresholdForMaxDocs");
      try
      {
        do
        {
          IPagedList<GitItem> itemsPagedAsync = this.GetItemsPagedAsync(versionDescriptor, projectId, repositoryId, scopePath, continuationToken, new int?(configValue));
          continuationToken = itemsPagedAsync.ContinuationToken;
          documentCount += itemsPagedAsync.Count<GitItem>((Func<GitItem, bool>) (item => item.GitObjectType == GitObjectType.Blob));
        }
        while (!string.IsNullOrWhiteSpace(continuationToken));
      }
      catch (Exception ex) when (IndexFaultMapManager.GetFaultMapper(typeof (GitItemDoesNotExistFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Failed to get document count for scope path {0}, commit {1}, Exception: {2}. Possibly the folder doesn't exist at the specified commit.", (object) scopePath, (object) commitId, (object) ex)));
        documentCount = 0;
      }
      return documentCount;
    }

    public virtual int GetDocumentCount(
      IVssRequestContext requestContext,
      string scopePath,
      string commitId,
      VersionControlRecursionType versionControlRecursionType = VersionControlRecursionType.OneLevel)
    {
      return this.GetDocumentCount(requestContext, this.m_projectId, this.m_repositoryId, scopePath, commitId, versionControlRecursionType);
    }

    public virtual bool GetGitRepoSizeEstimates(
      IVssRequestContext requestContext,
      int shardDensity,
      int numberOfBranches,
      double fileCountFactorForMultibranch,
      out int estimatedDocCount,
      out int estimatedDocCountGrowth,
      out long estimatedSize,
      out long estimatedSizeGrowth,
      out long actualInitialSize)
    {
      GitRepository repositoryAsync = this.GetRepositoryAsync(this.m_repositoryId);
      return this.GetGitRepoSizeEstimates(requestContext, repositoryAsync, shardDensity, numberOfBranches, fileCountFactorForMultibranch, out estimatedDocCount, out estimatedDocCountGrowth, out estimatedSize, out estimatedSizeGrowth, out actualInitialSize);
    }

    public virtual bool GetGitRepoSizeEstimates(
      IVssRequestContext requestContext,
      GitRepository gitRepository,
      int shardDensity,
      int numberOfBranches,
      double fileCountFactorForMultibranch,
      out int estimatedDocCount,
      out int estimatedDocCountGrowth,
      out long estimatedSize,
      out long estimatedSizeGrowth,
      out long actualInitialSize)
    {
      if (gitRepository == null)
        throw new ArgumentNullException(nameof (gitRepository));
      float currentHostConfigValue = requestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityGitRepositoryGrowthFactor", true, 0.3f);
      long? size = gitRepository.Size;
      if (size.HasValue)
      {
        actualInitialSize = size.Value;
        if (numberOfBranches <= 0)
          numberOfBranches = 1;
        estimatedDocCount = (int) ((double) checked (actualInitialSize * (long) numberOfBranches) * fileCountFactorForMultibranch * (double) shardDensity / 1073741824.0);
        GitHttpClientWrapper.GetGitRepoSizeEstimates(shardDensity, currentHostConfigValue, estimatedDocCount, out estimatedDocCountGrowth, out estimatedSize, out estimatedSizeGrowth);
        return true;
      }
      estimatedDocCount = 0;
      estimatedDocCountGrowth = 0;
      estimatedSize = 0L;
      estimatedSizeGrowth = 0L;
      actualInitialSize = 0L;
      return false;
    }

    public static void GetGitRepoSizeEstimates(
      int shardDensity,
      float gitRepoGrowthFactor,
      int estimatedDocCount,
      out int estimatedDocCountGrowth,
      out long estimatedSize,
      out long estimatedSizeGrowth)
    {
      estimatedDocCountGrowth = (int) ((double) estimatedDocCount * (double) gitRepoGrowthFactor);
      estimatedSize = 1073741824L / (long) shardDensity * (long) estimatedDocCount;
      estimatedSizeGrowth = (long) ((double) estimatedSize * (double) gitRepoGrowthFactor);
    }

    public IDictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem> GetFilePathToGitItems(
      GitCommit gitCommit,
      GitVersionDescriptor gitVersionDescriptor,
      List<string> filePaths,
      string indexingUnitType,
      DocumentContractType contractType,
      int maxBatchSize)
    {
      if (gitCommit == null)
        throw new ArgumentNullException(nameof (gitCommit));
      if (filePaths == null)
        throw new ArgumentNullException(nameof (filePaths));
      IDictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem> filePathToGitItems = (IDictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem>) new Dictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem>();
      int count1 = filePaths.Count;
      int val1 = count1;
      for (int index1 = 0; index1 < count1; index1 += maxBatchSize)
      {
        int count2 = Math.Min(val1, maxBatchSize);
        List<string> range = filePaths.GetRange(index1, count2);
        val1 -= count2;
        if (range.Count > 0)
        {
          List<GitItemDescriptor> gitItemDescriptorList = new List<GitItemDescriptor>(range.Count);
          foreach (string str in range)
          {
            GitItemDescriptor gitItemDescriptor = new GitItemDescriptor()
            {
              Path = str,
              RecursionLevel = VersionControlRecursionType.None,
              VersionType = GitVersionType.Commit,
              Version = gitCommit.CommitId
            };
            gitItemDescriptorList.Add(gitItemDescriptor);
          }
          GitItemRequestData gitItemRequestData = new GitItemRequestData()
          {
            ItemDescriptors = gitItemDescriptorList.ToArray(),
            IncludeContentMetadata = false,
            IncludeLinks = false,
            LatestProcessedChange = true
          };
          try
          {
            List<List<GitItem>> itemsBatch = this.GetItemsBatch(gitItemRequestData);
            if (itemsBatch.Count != range.Count)
              throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Expected number of records from GetItemsBatch : {0}, Found {1} records.", (object) range.Count, (object) itemsBatch.Count)));
            for (int index2 = 0; index2 < range.Count; ++index2)
            {
              string path = gitItemDescriptorList[index2].Path;
              Microsoft.VisualStudio.Services.Search.Common.FileAttributes key = new Microsoft.VisualStudio.Services.Search.Common.FileAttributes(gitItemDescriptorList[index2].Path, indexingUnitType, contractType);
              if (itemsBatch[index2] != null && itemsBatch[index2].Count > 0)
              {
                foreach (GitItem gitItem in itemsBatch[index2])
                  filePathToGitItems[key] = gitItem;
              }
              else
                filePathToGitItems[key] = (GitItem) null;
            }
          }
          catch (Exception ex)
          {
            this.ThrowIfShouldNotContinue(ex);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Exception caught in GetItemsBatch invocation. {0}. Falling back to file by file crawling.", (object) ex)));
            foreach (string filePath in range)
            {
              Microsoft.VisualStudio.Services.Search.Common.FileAttributes key = new Microsoft.VisualStudio.Services.Search.Common.FileAttributes(filePath, indexingUnitType, contractType);
              if (!filePathToGitItems.ContainsKey(key))
                filePathToGitItems[key] = this.GetGitItem(gitCommit, gitVersionDescriptor, filePath);
            }
          }
        }
        else
          break;
      }
      return filePathToGitItems;
    }

    public IDictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem> GetFilePathToGitItemsThroughContinuationTokenAPI(
      GitCommit gitCommit,
      GitVersionDescriptor gitVersionDescriptor,
      HashSet<string> filePaths,
      string indexingUnitType,
      DocumentContractType contractType,
      int maxBatchSizeForContinuationTokenAPI,
      int maxBatchSizeForGetItemsBatchAPI,
      string scopePath)
    {
      if (gitCommit == null)
        throw new ArgumentNullException(nameof (gitCommit));
      if (filePaths == null)
        throw new ArgumentNullException(nameof (filePaths));
      if (scopePath == null)
        throw new ArgumentNullException(nameof (scopePath));
      IDictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem> continuationTokenApi = (IDictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem>) new Dictionary<Microsoft.VisualStudio.Services.Search.Common.FileAttributes, GitItem>();
      string continuationToken = string.Empty;
      int count = filePaths.Count;
      if (!filePaths.Any<string>())
        return continuationTokenApi;
      foreach (string filePath in filePaths)
      {
        Microsoft.VisualStudio.Services.Search.Common.FileAttributes key = new Microsoft.VisualStudio.Services.Search.Common.FileAttributes(filePath, indexingUnitType, contractType);
        continuationTokenApi[key] = (GitItem) null;
      }
      try
      {
        if (scopePath.StartsWith("/", StringComparison.Ordinal) && scopePath.Length > "/".Length)
        {
          do
          {
            IPagedList<GitItem> itemsPagedAsync = this.GetItemsPagedAsync(gitVersionDescriptor, scopePath, continuationToken, new int?(maxBatchSizeForContinuationTokenAPI));
            continuationToken = itemsPagedAsync.ContinuationToken;
            foreach (GitItem gitItem in (IEnumerable<GitItem>) itemsPagedAsync)
            {
              if (filePaths.Contains(gitItem.Path))
              {
                Microsoft.VisualStudio.Services.Search.Common.FileAttributes key = new Microsoft.VisualStudio.Services.Search.Common.FileAttributes(gitItem.Path, indexingUnitType, contractType);
                if (gitItem.CommitId == null)
                  gitItem.CommitId = gitCommit.CommitId;
                gitItem.LatestProcessedChange = new GitCommitRef()
                {
                  Committer = gitCommit.Committer
                };
                continuationTokenApi[key] = gitItem;
                --count;
              }
            }
            if (string.IsNullOrWhiteSpace(continuationToken))
              break;
          }
          while (count > 0);
        }
        else
        {
          foreach (GitItem gitItem in this.GetItemsAsync(gitVersionDescriptor, scopePath, VersionControlRecursionType.OneLevel))
          {
            if (filePaths.Contains(gitItem.Path))
            {
              Microsoft.VisualStudio.Services.Search.Common.FileAttributes key = new Microsoft.VisualStudio.Services.Search.Common.FileAttributes(gitItem.Path, indexingUnitType, contractType);
              gitItem.LatestProcessedChange = new GitCommitRef()
              {
                Committer = gitCommit.Committer
              };
              continuationTokenApi[key] = gitItem;
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.ThrowIfShouldNotContinue(ex);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Exception caught in GetItemsPagedAsync invocation. {0}. Falling back to file by file crawling.", (object) ex)));
        return this.GetFilePathToGitItems(gitCommit, gitVersionDescriptor, filePaths.ToList<string>(), indexingUnitType, contractType, maxBatchSizeForGetItemsBatchAPI);
      }
      return continuationTokenApi;
    }

    public virtual GitItem GetGitItem(
      GitCommit commit,
      GitVersionDescriptor versionDescriptor,
      string filePath)
    {
      if (commit == null)
        throw new ArgumentNullException(nameof (commit));
      GitItem gitItem;
      try
      {
        gitItem = this.GetItemAsync(filePath, versionDescriptor);
        gitItem.CommitId = commit.CommitId;
        gitItem.LatestProcessedChange = new GitCommitRef()
        {
          Committer = commit.Committer
        };
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (GitItemDoesNotExistFaultMapper)).IsMatch(ex))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("GitHttpClientWrapper: Exception caught while traversing Git file '{0}'. Exception message : '{1}'. Marking this file for deletion.", (object) filePath, (object) ex)));
          gitItem = (GitItem) null;
        }
        else
          throw;
      }
      return gitItem;
    }

    public virtual bool IsEmpty(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      out int documentCount)
    {
      GitCodeRepoTFSAttributes repoTfsAttributes = repoIndexingUnit != null ? repoIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes : throw new ArgumentNullException(nameof (repoIndexingUnit));
      documentCount = 0;
      bool flag = false;
      foreach (string branchName in repoTfsAttributes.BranchesToIndex)
      {
        GitCommit latestCommitFromTfs = this.GetLatestCommitFromTFS(branchName, repoIndexingUnit.TFSEntityId);
        if (!string.IsNullOrWhiteSpace(latestCommitFromTfs?.CommitId) && latestCommitFromTfs.CommitId != RepositoryConstants.BranchCreationOrDeletionCommitId)
        {
          documentCount = this.GetDocumentCount(requestContext, projectIndexingUnit.TFSEntityId, repoIndexingUnit.TFSEntityId, "/", latestCommitFromTfs.CommitId, VersionControlRecursionType.Full);
          if (documentCount > 0)
          {
            flag = true;
            break;
          }
        }
      }
      return !flag;
    }

    private bool IsForkedRepository(GitRepository repository) => repository.IsFork;

    internal virtual void ThrowIfShouldNotContinue(Exception ex)
    {
      if (!IndexFaultMapManager.GetFaultMapper(typeof (VssThrottlingFaultMapper)).IsMatch(ex) && !IndexFaultMapManager.GetFaultMapper(typeof (ProjectNotFoundFaultMapper)).IsMatch(ex) && !IndexFaultMapManager.GetFaultMapper(typeof (RepoDoesNotExistFaultMapper)).IsMatch(ex) && !IndexFaultMapManager.GetFaultMapper(typeof (TfsBranchNotFoundFaultMapper)).IsMatch(ex))
        return;
      ExceptionDispatchInfo.Capture(ex).Throw();
    }

    internal static bool ValidateRefList(List<GitRef> refs, string branchName, Guid repositoryId)
    {
      if (refs == null || !refs.Any<GitRef>((Func<GitRef, bool>) (item => item != null)))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080108, "Indexing Pipeline", "Crawl", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No ref found with Name '{0}' for Repository Id : '{1}'", (object) branchName, (object) repositoryId));
        return false;
      }
      if (refs.Exists((Predicate<GitRef>) (x => x.Name.Equals(branchName))))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080108, "Indexing Pipeline", "Crawl", "Branch ref is found");
        return true;
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080108, "Indexing Pipeline", "Crawl", "Branch is not found as refs returned were not an exact match");
      return false;
    }

    internal virtual GitPush GetNextGitPush(string branchName, Guid repositoryId, int pushId)
    {
      try
      {
        GitPush gitPush1 = this.GetGitPush(repositoryId, pushId);
        List<GitPush> gitPushes = this.GetGitPushes(repositoryId, branchName, gitPush1.Date);
        SortedDictionary<int, GitPush> pushes = new SortedDictionary<int, GitPush>();
        Func<GitPush, bool> predicate = (Func<GitPush, bool>) (x => x.PushId > pushId);
        gitPushes.Where<GitPush>(predicate).ForEach<GitPush>((Action<GitPush>) (x => pushes.Add(x.PushId, x)));
        if (pushes.Count != 0)
        {
          foreach (KeyValuePair<int, GitPush> keyValuePair in pushes)
          {
            GitPush gitPush2 = this.GetGitPush(repositoryId, keyValuePair.Value.PushId);
            if (gitPush2 != null && gitPush2.Commits != null && gitPush2.Commits.Any<GitCommitRef>())
              return gitPush2;
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080108, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("None of the {0} gitPushes had any commits in it.", (object) pushes.Count)));
          return (GitPush) null;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080108, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("No pushes found post {0} after pushId = {1}. {2} gitPushes found.", (object) gitPush1.Date, (object) gitPush1.PushId, (object) pushes.Count)));
        return (GitPush) null;
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (GitCommitNotFoundFaultMapper)).IsMatch(ex))
          return (GitPush) null;
        ExceptionDispatchInfo.Capture(ex).Throw();
        return (GitPush) null;
      }
    }

    internal virtual GitCommit GetLatestCommitFromTFS(string branchName, Guid repositoryId)
    {
      List<GitRef> refs = this.GetRefs(repositoryId, branchName);
      if (!GitHttpClientWrapper.ValidateRefList(refs, branchName, repositoryId))
        return (GitCommit) null;
      string commitId = refs.Where<GitRef>((Func<GitRef, bool>) (x => x.Name.Equals(branchName))).Select<GitRef, string>((Func<GitRef, string>) (x => x.ObjectId)).FirstOrDefault<string>();
      try
      {
        return this.GetCommit(repositoryId, commitId);
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (GitCommitNotFoundFaultMapper)).IsMatch(ex))
          return (GitCommit) null;
        ExceptionDispatchInfo.Capture(ex).Throw();
        return (GitCommit) null;
      }
    }
  }
}
