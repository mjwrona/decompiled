// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.TfvcHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class TfvcHttpClientWrapper
  {
    private readonly TfvcHttpClient m_tfvcHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly IIndexerFaultService m_faultService;
    private readonly TraceMetaData m_traceMetaData;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;
    private readonly int m_intialRetryDelayInMs;
    private const int ChangeBatchSize = 10000;
    private const int DefaultTfsCallWaitTimeInSeconds = 300;

    public TfvcHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      this.m_traceMetaData = traceMetadata;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_intialRetryDelayInMs = TfvcHttpClientWrapper.GetIntialRetryDelayInMs(executionContext);
      this.m_waitTimeInMs = TfvcHttpClientWrapper.GetTfsApiCallWaitTimeInMillisec(executionContext);
      this.m_faultService = executionContext.FaultService;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
      this.m_tfvcHttpClient = executionContext.RequestContext.GetClient<TfvcHttpClient>();
    }

    protected internal TfvcHttpClientWrapper()
    {
    }

    private static int GetTfsApiCallWaitTimeInMillisec(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext) => executionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/TfsCallWaitTimeInSecondsForTfvc", true, 300) * 1000;

    private static int GetIntialRetryDelayInMs(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext) => executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;

    public virtual List<TeamProjectReference> GetTfvcProjects(
      List<TeamProjectReference> teamProjRefs)
    {
      List<TfvcItem> childItems = this.GetChildItems("$/", (TfvcVersionDescriptor) null, VersionControlRecursionType.OneLevel);
      return this.GetListOfAllTfvcProjects(teamProjRefs.ToList<TeamProjectReference>(), childItems);
    }

    public virtual List<TfvcItem> GetChildItems(
      string scopePath,
      TfvcVersionDescriptor versionInfo,
      VersionControlRecursionType recursionType)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new ArgumentException("GetChildItems called with Invalid scopePath argument.");
      if (!Enum.IsDefined(typeof (VersionControlRecursionType), (object) recursionType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "recursionType {0} is not supported", (object) recursionType.ToString()));
      return TfvcHttpClientWrapper.GetFilteredChildItems(this.GetItems(scopePath, versionInfo, recursionType), scopePath);
    }

    public virtual List<TfvcItem> GetChildItems(
      string project,
      string scopePath,
      TfvcVersionDescriptor versionInfo,
      VersionControlRecursionType recursionType)
    {
      if (string.IsNullOrWhiteSpace(scopePath) || string.IsNullOrWhiteSpace(project))
        throw new ArgumentException("GetChildItems called with Invalid arguments.");
      if (!Enum.IsDefined(typeof (VersionControlRecursionType), (object) recursionType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "recursionType {0} is not supported", (object) recursionType.ToString()));
      return TfvcHttpClientWrapper.GetFilteredChildItems(this.GetItems(project, scopePath, versionInfo, recursionType), scopePath.TrimEnd('/'));
    }

    internal virtual List<TfvcItem> GetItems(
      string scopePath,
      TfvcVersionDescriptor versionInfo,
      VersionControlRecursionType recursionType)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<TfvcItem>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<TfvcItem>>((Func<CancellationTokenSource, Task<List<TfvcItem>>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        string scopePath1 = scopePath;
        TfvcVersionDescriptor versionDescriptor1 = versionInfo;
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?(recursionType);
        CancellationToken token = tokenSource.Token;
        bool? includeLinks = new bool?();
        TfvcVersionDescriptor versionDescriptor2 = versionDescriptor1;
        CancellationToken cancellationToken = token;
        return tfvcHttpClient.GetItemsAsync(scopePath1, recursionLevel, includeLinks, versionDescriptor2, (object) null, cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    public virtual IPagedList<TfvcItem> GetTfvcItemsPaged(
      string project,
      string scopePath,
      int baseChangesetId,
      int targetChangesetId,
      int top,
      string continuationToken = null)
    {
      if (string.IsNullOrWhiteSpace(scopePath) || string.IsNullOrWhiteSpace(project) || targetChangesetId <= 0 || top <= 0 || targetChangesetId <= baseChangesetId)
        throw new ArgumentException("GetTfvcItemsPaged called with Invalid arguments.");
      IPagedList<TfvcItem> items = (IPagedList<TfvcItem>) null;
      if (baseChangesetId <= 0)
      {
        items = this.GetItemsPagedAsync(project, scopePath, new int?(targetChangesetId), top, continuationToken);
      }
      else
      {
        IPagedList<TfvcItemPreviousHash> changesetPagedAsync = this.GetItemsByChangesetPagedAsync(project, scopePath, baseChangesetId, targetChangesetId, top, continuationToken);
        if (!changesetPagedAsync.IsNullOrEmpty<TfvcItemPreviousHash>())
        {
          items = (IPagedList<TfvcItem>) new PagedList<TfvcItem>((IEnumerable<TfvcItem>) new List<TfvcItem>(), changesetPagedAsync.ContinuationToken);
          foreach (TfvcItemPreviousHash itemPreviousHash in (IEnumerable<TfvcItemPreviousHash>) changesetPagedAsync)
            items.Add((TfvcItem) itemPreviousHash);
        }
      }
      return !TfvcHttpClientWrapper.ValidateMetadataResponse((IList<TfvcItem>) items, scopePath) ? (IPagedList<TfvcItem>) new PagedList<TfvcItem>((IEnumerable<TfvcItem>) new List<TfvcItem>(), items?.ContinuationToken) : items;
    }

    internal virtual IPagedList<TfvcItemPreviousHash> GetItemsByChangesetPagedAsync(
      string project,
      string scopePath,
      int baseChangesetId,
      int targetChangesetId,
      int top,
      string continuationToken = null)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<IPagedList<TfvcItemPreviousHash>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<IPagedList<TfvcItemPreviousHash>>((Func<CancellationTokenSource, Task<IPagedList<TfvcItemPreviousHash>>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        string project1 = project;
        int top1 = top;
        string str = scopePath;
        int baseChangeset = baseChangesetId;
        string scopePath1 = str;
        int? targetChangeset = new int?(targetChangesetId);
        string continuationToken1 = continuationToken;
        CancellationToken token = tokenSource.Token;
        return tfvcHttpClient.GetItemsByChangesetPagedAsync(project1, top1, baseChangeset, scopePath1, targetChangeset, continuationToken1, cancellationToken: token);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    internal virtual IPagedList<TfvcItem> GetItemsPagedAsync(
      string project,
      string scopePath,
      int? changesetId,
      int top,
      string continuationToken = null)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<IPagedList<TfvcItem>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<IPagedList<TfvcItem>>((Func<CancellationTokenSource, Task<IPagedList<TfvcItem>>>) (tokenSource => this.m_tfvcHttpClient.GetItemsPagedAsync(project, top, scopePath, changesetId, continuationToken, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    internal virtual List<TfvcItem> GetItems(
      string project,
      string scopePath,
      TfvcVersionDescriptor versionInfo,
      VersionControlRecursionType recursionType)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<TfvcItem>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<TfvcItem>>((Func<CancellationTokenSource, Task<List<TfvcItem>>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        string project1 = project;
        string scopePath1 = scopePath;
        TfvcVersionDescriptor versionDescriptor1 = versionInfo;
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?(recursionType);
        CancellationToken token = tokenSource.Token;
        bool? includeLinks = new bool?();
        TfvcVersionDescriptor versionDescriptor2 = versionDescriptor1;
        CancellationToken cancellationToken = token;
        return tfvcHttpClient.GetItemsAsync(project1, scopePath1, recursionLevel, includeLinks, versionDescriptor2, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    public virtual TfvcItem GetItem(string path, TfvcVersionDescriptor versionInfo = null) => AsyncInvoker.InvokeAsyncWait<TfvcItem>((Func<CancellationTokenSource, Task<TfvcItem>>) (tokenSource =>
    {
      TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
      string path1 = path;
      TfvcVersionDescriptor versionDescriptor1 = versionInfo;
      CancellationToken token = tokenSource.Token;
      bool? download = new bool?();
      VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
      TfvcVersionDescriptor versionDescriptor2 = versionDescriptor1;
      bool? includeContent = new bool?();
      CancellationToken cancellationToken = token;
      return tfvcHttpClient.GetItemAsync(path1, (string) null, download, (string) null, recursionLevel, versionDescriptor2, includeContent, (object) null, cancellationToken);
    }), this.m_waitTimeInMs, this.m_traceMetaData);

    public virtual TfvcItem GetItem(
      string path,
      TfvcVersionDescriptor versionInfo,
      out bool isDeleted)
    {
      isDeleted = false;
      TfvcItem tfvcItem = (TfvcItem) null;
      try
      {
        tfvcItem = this.GetItem(path, versionInfo);
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (TfvcItemNotFoundFaultMapper)).IsMatch(ex))
        {
          isDeleted = true;
          return (TfvcItem) null;
        }
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      return tfvcItem;
    }

    public virtual List<TfvcItem> GetAllFiles(
      string project,
      string scopePath,
      TfvcVersionDescriptor versionInfo)
    {
      Queue<string> crawlQueue = new Queue<string>();
      crawlQueue.Enqueue(scopePath);
      List<TfvcItem> allFiles = new List<TfvcItem>();
      while (crawlQueue.Count > 0)
      {
        string scopePath1 = crawlQueue.Dequeue();
        List<TfvcItem> childItems = this.GetChildItems(project, scopePath1, versionInfo, VersionControlRecursionType.None);
        childItems.Where<TfvcItem>((Func<TfvcItem, bool>) (item => item.IsFolder)).ToList<TfvcItem>().ForEach((Action<TfvcItem>) (folder => crawlQueue.Enqueue(folder.Path)));
        allFiles.AddRange(childItems.Where<TfvcItem>((Func<TfvcItem, bool>) (item => !item.IsFolder && item.DeletionId == 0)));
      }
      return allFiles;
    }

    public virtual Stream GetItemsBatchZipAsync(
      string projectId,
      IEnumerable<string> filePaths,
      TfvcVersionDescriptor versionDescriptor)
    {
      if (string.IsNullOrWhiteSpace(projectId) || filePaths == null)
        throw new ArgumentException("GetItemsBatchZipAsync called with Invalid arguments.");
      IEnumerable<TfvcItemDescriptor> source = filePaths.Select<string, TfvcItemDescriptor>((Func<string, TfvcItemDescriptor>) (path => new TfvcItemDescriptor()
      {
        Path = path,
        Version = versionDescriptor.Version,
        VersionType = versionDescriptor.VersionType
      }));
      TfvcItemRequestData requestData = new TfvcItemRequestData()
      {
        ItemDescriptors = source.ToArray<TfvcItemDescriptor>()
      };
      return this.m_expRetryInvoker.InvokeWithFaultCheck<Stream>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        string str = projectId;
        TfvcItemRequestData itemRequestData = requestData;
        string project = str;
        CancellationToken token = tokenSource.Token;
        return tfvcHttpClient.GetItemsBatchZipAsync(itemRequestData, project, (object) null, token);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    public virtual Stream GetItemContent(string scopePath, TfvcVersionDescriptor versionInfo)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new ArgumentException("GetItemContent called with Invalid arguments.");
      return AsyncInvoker.InvokeAsyncWait<Stream>((Func<CancellationTokenSource, Task<Stream>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        string path = scopePath;
        TfvcVersionDescriptor versionDescriptor1 = versionInfo;
        CancellationToken token = tokenSource.Token;
        bool? download = new bool?();
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
        TfvcVersionDescriptor versionDescriptor2 = versionDescriptor1;
        bool? includeContent = new bool?();
        CancellationToken cancellationToken = token;
        return tfvcHttpClient.GetItemContentAsync(path, (string) null, download, (string) null, recursionLevel, versionDescriptor2, includeContent, (object) null, cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData);
    }

    public virtual TfvcChangesetRef GetLatestChangeset(
      string projectId,
      TfvcChangesetSearchCriteria searchCriteriaObject)
    {
      return this.GetChangesets(projectId, searchCriteriaObject, new int?(1)).FirstOrDefault<TfvcChangesetRef>();
    }

    public virtual TfvcChangesetRef GetLatestChangeset(
      string projectId,
      TfvcChangesetSearchCriteria searchCriteriaObject,
      out bool isDeleted)
    {
      TfvcChangesetRef latestChangeset = (TfvcChangesetRef) null;
      isDeleted = false;
      try
      {
        latestChangeset = this.GetChangesets(projectId, searchCriteriaObject, new int?(1)).FirstOrDefault<TfvcChangesetRef>();
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (TfvcItemNotFoundFaultMapper)).IsMatch(ex))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082631, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("Files got deleted in TFS. Got this exception: {0}", (object) ex.Message)));
          isDeleted = true;
          return (TfvcChangesetRef) null;
        }
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      return latestChangeset;
    }

    public virtual TfvcChangeset GetChangeset(string projectId, int changeSetId)
    {
      if (string.IsNullOrWhiteSpace(projectId) || changeSetId < 0)
        throw new ArgumentException("GetChangeset called with Invalid arguments.");
      return this.m_expRetryInvoker.InvokeWithFaultCheck<TfvcChangeset>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<TfvcChangeset>((Func<CancellationTokenSource, Task<TfvcChangeset>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        string project = projectId;
        int id = changeSetId;
        CancellationToken token = tokenSource.Token;
        int? maxChangeCount = new int?();
        bool? includeDetails = new bool?();
        bool? includeWorkItems = new bool?();
        int? maxCommentLength = new int?();
        bool? includeSourceRename = new bool?();
        int? skip = new int?();
        int? top = new int?();
        CancellationToken cancellationToken = token;
        return tfvcHttpClient.GetChangesetAsync(project, id, maxChangeCount, includeDetails, includeWorkItems, maxCommentLength, includeSourceRename, skip, top, cancellationToken: cancellationToken);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    public virtual List<TfvcChangesetRef> GetChangesets(
      string projectId,
      TfvcChangesetSearchCriteria searchCriteriaObject,
      int? top = null,
      int? skip = 0)
    {
      if (!string.IsNullOrWhiteSpace(projectId))
      {
        int? nullable1 = top;
        int num = 0;
        if (!(nullable1.GetValueOrDefault() <= num & nullable1.HasValue))
          return this.m_expRetryInvoker.InvokeWithFaultCheck<List<TfvcChangesetRef>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<TfvcChangesetRef>>((Func<CancellationTokenSource, Task<List<TfvcChangesetRef>>>) (tokenSource =>
          {
            TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
            string project = projectId;
            int? nullable2 = top;
            TfvcChangesetSearchCriteria changesetSearchCriteria = searchCriteriaObject;
            CancellationToken token = tokenSource.Token;
            int? maxCommentLength = new int?();
            int? skip1 = new int?();
            int? top1 = nullable2;
            TfvcChangesetSearchCriteria searchCriteria = changesetSearchCriteria;
            CancellationToken cancellationToken = token;
            return tfvcHttpClient.GetChangesetsAsync(project, maxCommentLength, skip1, top1, searchCriteria: searchCriteria, cancellationToken: cancellationToken);
          }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
      }
      throw new ArgumentException("GetChangesets called with Invalid arguments.");
    }

    public virtual List<TfvcChangesetRef> GetUnIndexedChangesets(
      string projectId,
      string scopePath,
      int lastIndexedChangesetId)
    {
      TfvcChangesetSearchCriteria searchCriteriaObject = new TfvcChangesetSearchCriteria()
      {
        FromId = lastIndexedChangesetId,
        ItemPath = scopePath
      };
      List<TfvcChangesetRef> changesets = this.GetChangesets(projectId, searchCriteriaObject);
      changesets.RemoveAll((Predicate<TfvcChangesetRef>) (c => c.ChangesetId == lastIndexedChangesetId));
      return changesets;
    }

    public virtual List<TfvcChange> GetAllChangesetChanges(int changeSetId)
    {
      if (changeSetId < 0)
        throw new ArgumentException("GetAllChangesetChanges called with Invalid arguments.");
      int skip = 0;
      List<TfvcChange> changesetChanges = new List<TfvcChange>();
      List<TfvcChange> tfvcChangeList;
      do
      {
        tfvcChangeList = this.m_expRetryInvoker.InvokeWithFaultCheck<List<TfvcChange>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<TfvcChange>>((Func<CancellationTokenSource, Task<List<TfvcChange>>>) (tokenSource =>
        {
          TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
          int? id = new int?(changeSetId);
          int? nullable = new int?(10000);
          int? skip1 = new int?(skip);
          int? top = nullable;
          CancellationToken token = tokenSource.Token;
          return tfvcHttpClient.GetChangesetChangesAsync(id, skip1, top, cancellationToken: token);
        }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
        if (tfvcChangeList != null)
        {
          changesetChanges.AddRange((IEnumerable<TfvcChange>) tfvcChangeList);
          skip += tfvcChangeList.Count;
        }
      }
      while (tfvcChangeList != null && tfvcChangeList.Any<TfvcChange>());
      return changesetChanges;
    }

    public virtual List<TfvcChange> GetChangesetChanges(int changeSetId, int skip)
    {
      if (changeSetId < 0)
        throw new ArgumentException("GetChangesetChanges called with Invalid arguments.");
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<TfvcChange>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<TfvcChange>>((Func<CancellationTokenSource, Task<List<TfvcChange>>>) (tokenSource =>
      {
        TfvcHttpClient tfvcHttpClient = this.m_tfvcHttpClient;
        int? id = new int?(changeSetId);
        int? nullable = new int?(10000);
        int? skip1 = new int?(skip);
        int? top = nullable;
        CancellationToken token = tokenSource.Token;
        return tfvcHttpClient.GetChangesetChangesAsync(id, skip1, top, cancellationToken: token);
      }), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData) ?? new List<TfvcChange>();
    }

    public virtual List<TfvcBranch> GetBranchesInCollection(
      bool includeParent = false,
      bool includeChildren = false,
      bool includeDeleted = false)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<TfvcBranch>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<TfvcBranch>>((Func<CancellationTokenSource, Task<List<TfvcBranch>>>) (tokenSource => this.m_tfvcHttpClient.GetBranchesAsync((string) null, new bool?(includeParent), new bool?(includeChildren), new bool?(includeDeleted), cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_intialRetryDelayInMs, true, this.m_traceMetaData);
    }

    public virtual TfvcStatistics GetTfvcStatistics(
      IVssRequestContext requestContext,
      Guid projectId,
      string scopePath)
    {
      return AsyncInvoker.InvokeAsyncWait<TfvcStatistics>((Func<CancellationTokenSource, Task<TfvcStatistics>>) (tokenSource => this.m_tfvcHttpClient.GetTfvcStatisticsAsync(projectId, scopePath, cancellationToken: tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData);
    }

    public virtual void GetDocumentCountEstimates(
      IVssRequestContext requestContext,
      DocumentContractType documentContractType,
      Guid projectId,
      string scopePath,
      out int estimatedDocCount,
      out int estimatedDocCountGrowth)
    {
      long num1;
      try
      {
        num1 = this.GetTfvcStatistics(requestContext, projectId, scopePath).FileCountTotal;
      }
      catch (Exception ex) when (IndexFaultMapManager.GetFaultMapper(typeof (TfvcItemNotFoundFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082631, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("Files got deleted in TFS. Got this exception: {0}", (object) ex.Message)));
        num1 = 0L;
      }
      catch (Exception ex) when (IndexFaultMapManager.GetFaultMapper(typeof (VssTimeOutFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082631, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("Failed to get TfvcStatistics, Exception: {0}", (object) ex)));
        int currentHostConfigValue = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/Code/MaxAllowedShardsToACollection", true, 50);
        int documentsInAshard = documentContractType.GetMaxNumberOfDocumentsInAShard(requestContext);
        estimatedDocCount = currentHostConfigValue * documentsInAshard;
        estimatedDocCountGrowth = 0;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Crawl", "TfvcGetStatisticsCallTimedOut", (object) true.ToString());
        return;
      }
      float currentHostConfigValue1 = requestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityTfvcRepositoryGrowthFactor", true, 1f);
      if (num1 <= (long) int.MaxValue)
      {
        estimatedDocCount = (int) num1;
        float num2 = (float) estimatedDocCount * currentHostConfigValue1;
        if ((double) num2 < 2147483648.0)
          estimatedDocCountGrowth = (int) num2;
        else
          estimatedDocCountGrowth = int.MaxValue;
      }
      else
      {
        estimatedDocCount = int.MaxValue;
        estimatedDocCountGrowth = int.MaxValue;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082631, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("ProjectId {0} has {1} files which is more than what we support.", (object) projectId, (object) num1)));
      }
    }

    public virtual bool IsEmpty(
      IVssRequestContext requestContext,
      DocumentContractType documentContractType,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepoIndexingUnit)
    {
      if (tfvcRepoIndexingUnit == null)
        throw new ArgumentNullException(nameof (tfvcRepoIndexingUnit));
      int estimatedDocCount;
      this.GetDocumentCountEstimates(requestContext, documentContractType, tfvcRepoIndexingUnit.TFSEntityId, tfvcRepoIndexingUnit.GetTFSEntityName(), out estimatedDocCount, out int _);
      return estimatedDocCount == 0;
    }

    internal static bool ValidateMetadataResponse(IList<TfvcItem> items, string path)
    {
      if (items != null && items.Count != 0)
        return true;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080117, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("Unexpected Tfvc metadata or Files may have been destroyed at path : '{0}'. NULL value is returned.", (object) path)));
      return false;
    }

    internal List<TeamProjectReference> GetListOfAllTfvcProjects(
      List<TeamProjectReference> teamProjRefs,
      List<TfvcItem> tfvcCodebaseInProjects)
    {
      return teamProjRefs.Where<TeamProjectReference>((Func<TeamProjectReference, bool>) (projRef => tfvcCodebaseInProjects.Any<TfvcItem>((Func<TfvcItem, bool>) (tfvcItem => tfvcItem.IsProjectWithName(projRef.Name))))).ToList<TeamProjectReference>();
    }

    private static List<TfvcItem> GetFilteredChildItems(
      List<TfvcItem> childItems,
      string parentScopePath)
    {
      List<TfvcItem> filteredChildItems = new List<TfvcItem>();
      if (TfvcHttpClientWrapper.ValidateMetadataResponse((IList<TfvcItem>) childItems, parentScopePath))
      {
        foreach (TfvcItem childItem in childItems)
        {
          if (childItem.Path != parentScopePath)
            filteredChildItems.Add(childItem);
        }
      }
      return filteredChildItems;
    }
  }
}
