// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.WorkItemHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Exceptions;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class WorkItemHttpClientWrapper
  {
    protected WorkItemTrackingHttpClient WorkItemHttpClient { get; }

    protected ExponentialBackoffRetryInvoker ExpRetryInvoker { get; }

    protected IIndexerFaultService FaultService { get; }

    protected TraceMetaData TraceMetaData { get; }

    protected int WaitTimeInMs { get; }

    protected int RetryLimit { get; }

    protected int MaxWorkItemIds { get; }

    public WorkItemHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      this.TraceMetaData = traceMetadata;
      this.ExpRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.FaultService = executionContext.FaultService;
      this.WaitTimeInMs = WorkItemHttpClientWrapper.GetTfsApiCallWaitTimeInMs(executionContext);
      this.RetryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
      this.WorkItemHttpClient = executionContext.RequestContext.GetClient<WorkItemTrackingHttpClient>(executionContext.RequestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("Search.Server.WorkItem.EnableReadReplica"));
      this.MaxWorkItemIds = executionContext.ServiceSettings.PipelineSettings.WorkItemFetchBatchSize;
    }

    public virtual WorkItemClassificationNode GetRootAreaNode(Guid projectId) => this.GetClassificationNode(projectId, TreeStructureGroup.Areas, (string) null, int.MaxValue);

    public virtual WorkItemClassificationNode GetRootIterationNode(Guid projectId) => this.GetClassificationNode(projectId, TreeStructureGroup.Iterations, (string) null, int.MaxValue);

    public virtual WorkItemQueryResult GetWorkItemBatchByQuery(
      Guid projectId,
      int lastWorkItemId,
      int maxResultCount)
    {
      Wiql wiql = new Wiql()
      {
        Query = FormattableString.Invariant(FormattableStringFactory.Create("SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.Id] > {0} ORDER BY [System.Id] ASC", (object) lastWorkItemId))
      };
      return this.QueryByWiql(projectId, wiql, maxResultCount);
    }

    public virtual IEnumerable<WorkItem> GetWorkItemDiscussions(
      int workItemId,
      int fromRevision,
      int take,
      bool retryOnFailure)
    {
      return this.ExpRetryInvoker.InvokeWithFaultCheck<WorkItemComments>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<WorkItemComments>((Func<CancellationTokenSource, Task<WorkItemComments>>) (tokenSource => this.WorkItemHttpClient.GetCommentsAsync(workItemId, new int?(fromRevision), new int?(take), new CommentSortOrder?(CommentSortOrder.Asc), cancellationToken: tokenSource.Token)), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, retryOnFailure ? this.RetryLimit : 0, this.WaitTimeInMs, true, this.TraceMetaData).Comments.Select<WorkItemComment, WorkItem>((Func<WorkItemComment, WorkItem>) (c => new WorkItem()
      {
        Id = new int?(workItemId),
        Rev = new int?(c.Revision),
        Fields = (IDictionary<string, object>) new FriendlyDictionary<string, object>()
        {
          ["System.Id"] = (object) workItemId,
          ["System.Rev"] = (object) c.Revision,
          ["System.History"] = (object) c.Text
        }
      }));
    }

    public virtual bool IsPermanentlyDeleted(int workItemId, bool retryOnFailure = true)
    {
      try
      {
        this.ExpRetryInvoker.InvokeWithFaultCheck<WorkItemComments>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<WorkItemComments>((Func<CancellationTokenSource, Task<WorkItemComments>>) (tokenSource =>
        {
          WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
          int id = workItemId;
          int? nullable = new int?(1);
          CancellationToken token = tokenSource.Token;
          int? fromRevision = new int?();
          int? top = nullable;
          CommentSortOrder? order = new CommentSortOrder?();
          CancellationToken cancellationToken = token;
          return workItemHttpClient.GetCommentsAsync(id, fromRevision, top, order, cancellationToken: cancellationToken);
        }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, retryOnFailure ? this.RetryLimit : 0, this.WaitTimeInMs, true, this.TraceMetaData);
      }
      catch (AggregateException ex) when (new WorkItemPermanentlyDeletedFaultMapper().IsMatch((Exception) ex))
      {
        return true;
      }
      return false;
    }

    public virtual WorkItem GetWorkItemFromRecycleBin(
      int workItemId,
      Guid? projectId = null,
      bool retryOnFailure = true)
    {
      WorkItem resource = this.ExpRetryInvoker.InvokeWithFaultCheck<WorkItemDelete>((Func<object>) (() => projectId.HasValue ? (object) AsyncInvoker.InvokeAsyncWait<WorkItemDelete>((Func<CancellationTokenSource, Task<WorkItemDelete>>) (tokenSource => this.WorkItemHttpClient.GetDeletedWorkItemAsync(projectId.Value, workItemId, cancellationToken: tokenSource.Token)), this.WaitTimeInMs, this.TraceMetaData) : (object) AsyncInvoker.InvokeAsyncWait<WorkItemDelete>((Func<CancellationTokenSource, Task<WorkItemDelete>>) (tokenSource => this.WorkItemHttpClient.GetDeletedWorkItemAsync(workItemId, cancellationToken: tokenSource.Token)), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, retryOnFailure ? this.RetryLimit : 0, this.WaitTimeInMs, true, this.TraceMetaData).Resource;
      resource.Fields["System.Id"] = (object) resource.Id;
      resource.Fields["System.Rev"] = (object) resource.Rev;
      resource.Fields["System.IsDeleted"] = (object) true;
      WorkItemHttpClientWrapper.SerializeIdentityRefValues(resource);
      return resource;
    }

    public virtual IEnumerable<WorkItem> GetWorkItemBatch(
      IList<int> workItemIdBatch,
      IEnumerable<string> fields = null,
      bool throwForDeletedWorkItems = true)
    {
      if (workItemIdBatch == null)
        throw new ArgumentNullException(nameof (workItemIdBatch));
      if (workItemIdBatch.Count > this.MaxWorkItemIds)
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("The workItemIdBatch contains {0} items which is more than allowed threshold of {1}", (object) workItemIdBatch.Count, (object) this.MaxWorkItemIds)));
      List<WorkItem> workItemBatch = this.ExpRetryInvoker.InvokeWithFaultCheck<List<WorkItem>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<WorkItem>>((Func<CancellationTokenSource, Task<List<WorkItem>>>) (tokenSource =>
      {
        WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
        IList<int> ids = workItemIdBatch;
        IEnumerable<string> fields1 = fields;
        WorkItemExpand? nullable1 = new WorkItemExpand?(fields == null ? WorkItemExpand.All : WorkItemExpand.None);
        WorkItemErrorPolicy? nullable2 = throwForDeletedWorkItems ? new WorkItemErrorPolicy?() : new WorkItemErrorPolicy?(WorkItemErrorPolicy.Omit);
        CancellationToken token = tokenSource.Token;
        DateTime? asOf = new DateTime?();
        WorkItemExpand? expand = nullable1;
        WorkItemErrorPolicy? errorPolicy = nullable2;
        CancellationToken cancellationToken = token;
        return workItemHttpClient.GetWorkItemsAsync((IEnumerable<int>) ids, fields1, asOf, expand, errorPolicy, cancellationToken: cancellationToken);
      }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      workItemBatch.ForEach(WorkItemHttpClientWrapper.\u003C\u003EO.\u003C0\u003E__SerializeIdentityRefValues ?? (WorkItemHttpClientWrapper.\u003C\u003EO.\u003C0\u003E__SerializeIdentityRefValues = new Action<WorkItem>(WorkItemHttpClientWrapper.SerializeIdentityRefValues)));
      return (IEnumerable<WorkItem>) workItemBatch;
    }

    public virtual List<WorkItemField> GetFields() => this.ExpRetryInvoker.InvokeWithFaultCheck<List<WorkItemField>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<WorkItemField2>>((Func<CancellationTokenSource, Task<List<WorkItemField2>>>) (tokenSource =>
    {
      WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
      CancellationToken token = tokenSource.Token;
      GetFieldsExpand? expand = new GetFieldsExpand?();
      CancellationToken cancellationToken = token;
      return workItemHttpClient.GetWorkItemFieldsAsync(expand, cancellationToken: cancellationToken);
    }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);

    public virtual WorkItemQueryResult QueryByWiql(Guid projectId, Wiql wiql, int maxResultCount)
    {
      if (maxResultCount <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxResultCount), (object) maxResultCount, "Value has to be a positive integer.");
      return this.ExpRetryInvoker.InvokeWithFaultCheck<WorkItemQueryResult>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<WorkItemQueryResult>((Func<CancellationTokenSource, Task<WorkItemQueryResult>>) (tokenSource =>
      {
        WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
        Guid guid = projectId;
        Wiql wiql1 = wiql;
        Guid project = guid;
        int? nullable = new int?(maxResultCount);
        CancellationToken token = tokenSource.Token;
        bool? timePrecision = new bool?();
        int? top = nullable;
        CancellationToken cancellationToken = token;
        return workItemHttpClient.QueryByWiqlAsync(wiql1, project, timePrecision, top, cancellationToken: cancellationToken);
      }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);
    }

    public virtual ReportingWorkItemRevisionsBatch GetWorkItemRevisionsBatch(
      Guid projectId,
      string continuationToken,
      int maxPageSize)
    {
      ReportingWorkItemRevisionsBatch workItemRevisionBatch = this.ExpRetryInvoker.InvokeWithFaultCheck<ReportingWorkItemRevisionsBatch>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<ReportingWorkItemRevisionsBatch>((Func<CancellationTokenSource, Task<ReportingWorkItemRevisionsBatch>>) (tokenSource =>
      {
        WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
        Guid project = projectId;
        string continuationToken1 = continuationToken;
        ReportingRevisionsExpand? nullable1 = new ReportingRevisionsExpand?(ReportingRevisionsExpand.Fields);
        int? nullable2 = new int?(maxPageSize);
        bool? nullable3 = new bool?(true);
        bool? nullable4 = new bool?(true);
        bool? nullable5 = new bool?(true);
        CancellationToken token = tokenSource.Token;
        DateTime? startDateTime = new DateTime?();
        bool? includeIdentityRef = nullable4;
        bool? includeDeleted = nullable3;
        bool? includeTagRef = new bool?();
        bool? includeLatestOnly = nullable5;
        ReportingRevisionsExpand? expand = nullable1;
        bool? includeDiscussionChangesOnly = new bool?();
        int? maxPageSize1 = nullable2;
        CancellationToken cancellationToken = token;
        return workItemHttpClient.ReadReportingRevisionsGetAsync(project, (IEnumerable<string>) null, (IEnumerable<string>) null, continuationToken1, startDateTime, includeIdentityRef, includeDeleted, includeTagRef, includeLatestOnly, expand, includeDiscussionChangesOnly, maxPageSize1, (object) null, cancellationToken);
      }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);
      WorkItemHttpClientWrapper.ValidateResponse(workItemRevisionBatch, continuationToken, projectId);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      workItemRevisionBatch.Values.ForEach<WorkItem>(WorkItemHttpClientWrapper.\u003C\u003EO.\u003C0\u003E__SerializeIdentityRefValues ?? (WorkItemHttpClientWrapper.\u003C\u003EO.\u003C0\u003E__SerializeIdentityRefValues = new Action<WorkItem>(WorkItemHttpClientWrapper.SerializeIdentityRefValues)));
      return workItemRevisionBatch;
    }

    public virtual ReportingWorkItemRevisionsBatch GetWorkItemDiscussionsBatch(
      Guid projectId,
      string continuationToken,
      int maxPageSize)
    {
      ReportingWorkItemRevisionsBatch workItemRevisionBatch = this.ExpRetryInvoker.InvokeWithFaultCheck<ReportingWorkItemRevisionsBatch>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<ReportingWorkItemRevisionsBatch>((Func<CancellationTokenSource, Task<ReportingWorkItemRevisionsBatch>>) (tokenSource =>
      {
        WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
        Guid project = projectId;
        bool? nullable1 = new bool?(true);
        string str = continuationToken;
        bool? nullable2 = new bool?(true);
        int? nullable3 = new int?(maxPageSize);
        bool? nullable4 = new bool?(true);
        string[] fields = new string[3]
        {
          "System.History",
          "System.Id",
          "System.Rev"
        };
        string continuationToken1 = str;
        CancellationToken token = tokenSource.Token;
        DateTime? startDateTime = new DateTime?();
        bool? includeIdentityRef = nullable1;
        bool? includeDeleted = nullable2;
        bool? includeTagRef = new bool?();
        bool? includeLatestOnly = new bool?();
        ReportingRevisionsExpand? expand = new ReportingRevisionsExpand?();
        bool? includeDiscussionChangesOnly = nullable4;
        int? maxPageSize1 = nullable3;
        CancellationToken cancellationToken = token;
        return workItemHttpClient.ReadReportingRevisionsGetAsync(project, (IEnumerable<string>) fields, (IEnumerable<string>) null, continuationToken1, startDateTime, includeIdentityRef, includeDeleted, includeTagRef, includeLatestOnly, expand, includeDiscussionChangesOnly, maxPageSize1, (object) null, cancellationToken);
      }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);
      WorkItemHttpClientWrapper.ValidateResponse(workItemRevisionBatch, continuationToken, projectId, true);
      return workItemRevisionBatch;
    }

    public virtual string GetCurrentContinuationToken(Guid projectId) => (this.ExpRetryInvoker.InvokeWithFaultCheck<ReportingWorkItemRevisionsBatch>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<ReportingWorkItemRevisionsBatch>((Func<CancellationTokenSource, Task<ReportingWorkItemRevisionsBatch>>) (tokenSource =>
    {
      WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
      Guid project = projectId;
      DateTime? startDateTime = new DateTime?(DateTime.UtcNow);
      bool? nullable = new bool?(true);
      CancellationToken token = tokenSource.Token;
      bool? includeIdentityRef = new bool?();
      bool? includeDeleted = new bool?();
      bool? includeTagRef = new bool?();
      bool? includeLatestOnly = nullable;
      ReportingRevisionsExpand? expand = new ReportingRevisionsExpand?();
      bool? includeDiscussionChangesOnly = new bool?();
      int? maxPageSize = new int?();
      CancellationToken cancellationToken = token;
      return workItemHttpClient.ReadReportingRevisionsGetAsync(project, (IEnumerable<string>) null, (IEnumerable<string>) null, (string) null, startDateTime, includeIdentityRef, includeDeleted, includeTagRef, includeLatestOnly, expand, includeDiscussionChangesOnly, maxPageSize, (object) null, cancellationToken);
    }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData) ?? throw new NullRevisionBatchException(FormattableString.Invariant(FormattableStringFactory.Create("Null response returned obtained while getting the current continuation token for Project ID : {0}.", (object) projectId)))).ContinuationToken;

    public virtual WorkItem GetWorkItem(
      int workItemId,
      IEnumerable<string> fields = null,
      bool retryOnFailure = true)
    {
      WorkItem workItem = this.ExpRetryInvoker.InvokeWithFaultCheck<WorkItem>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<WorkItem>((Func<CancellationTokenSource, Task<WorkItem>>) (tokenSource =>
      {
        WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
        int id = workItemId;
        IEnumerable<string> fields1 = fields;
        WorkItemExpand? nullable = new WorkItemExpand?(fields == null ? WorkItemExpand.All : WorkItemExpand.None);
        CancellationToken token = tokenSource.Token;
        DateTime? asOf = new DateTime?();
        WorkItemExpand? expand = nullable;
        CancellationToken cancellationToken = token;
        return workItemHttpClient.GetWorkItemAsync(id, fields1, asOf, expand, cancellationToken: cancellationToken);
      }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, retryOnFailure ? this.RetryLimit : 0, this.WaitTimeInMs, true, this.TraceMetaData);
      WorkItemHttpClientWrapper.SerializeIdentityRefValues(workItem);
      return workItem;
    }

    public virtual List<WorkItemClassificationNode> GetRootClassificationNodes(Guid projectId) => this.ExpRetryInvoker.InvokeWithFaultCheck<List<WorkItemClassificationNode>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<WorkItemClassificationNode>>((Func<CancellationTokenSource, Task<List<WorkItemClassificationNode>>>) (tokenSource => this.WorkItemHttpClient.GetRootNodesAsync(projectId, new int?(int.MaxValue), cancellationToken: tokenSource.Token)), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);

    public virtual WorkItemClassificationNode GetClassificationNode(
      Guid projectId,
      TreeStructureGroup type,
      string path,
      int depth)
    {
      return this.ExpRetryInvoker.InvokeWithFaultCheck<WorkItemClassificationNode>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<WorkItemClassificationNode>((Func<CancellationTokenSource, Task<WorkItemClassificationNode>>) (tokenSource => this.WorkItemHttpClient.GetClassificationNodeAsync(projectId, type, path, new int?(depth), cancellationToken: tokenSource.Token)), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);
    }

    private static void SerializeIdentityRefValues(WorkItem workItem)
    {
      IDictionary<string, object> fields = workItem.Fields;
      foreach (string key in fields.Keys.ToList<string>())
      {
        if (fields[key] is IdentityRef identityRef)
        {
          bool flag1 = !string.IsNullOrWhiteSpace(identityRef.DisplayName);
          bool flag2 = !string.IsNullOrWhiteSpace(identityRef.UniqueName);
          if (flag1 & flag2)
            fields[key] = (object) FormattableString.Invariant(FormattableStringFactory.Create("{0} <{1}>", (object) identityRef.DisplayName, (object) identityRef.UniqueName));
          else
            fields[key] = !flag1 ? (!flag2 ? (object) string.Empty : (object) identityRef.UniqueName) : (object) identityRef.DisplayName;
        }
      }
    }

    protected static void ValidateResponse(
      ReportingWorkItemRevisionsBatch workItemRevisionBatch,
      string continuationTokenSent,
      Guid projectId,
      bool includeDiscussionsOnly = false)
    {
      string b = workItemRevisionBatch != null ? workItemRevisionBatch.ContinuationToken : throw new NullRevisionBatchException(FormattableString.Invariant(FormattableStringFactory.Create("Null response obtained while crawling for {0}. Project ID : {1}, continuationToken {2}.", includeDiscussionsOnly ? (object) "discussions" : (object) "revisions", (object) projectId, (object) continuationTokenSent)));
      if (!string.IsNullOrEmpty(continuationTokenSent) && string.IsNullOrEmpty(b))
        throw new InvalidContinuationTokenException(FormattableString.Invariant(FormattableStringFactory.Create("Received empty/null continuationToken in response for project id:{0}. continuationTokenSent:{1}, continuationTokenReceived:{2}", (object) projectId, (object) continuationTokenSent, (object) b)));
      if (string.IsNullOrEmpty(continuationTokenSent) || string.Equals(continuationTokenSent, b) || workItemRevisionBatch.Values != null && workItemRevisionBatch.Values.Any<WorkItem>())
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080125, "Indexing Pipeline", "Crawl", FormattableString.Invariant(FormattableStringFactory.Create("{0} values obtained in response while crawling for {1}. Project ID : {2}, continuationToken sent:{3}, continuationToken received:{4}.", workItemRevisionBatch.Values == null ? (object) "Null" : (object) "Empty", includeDiscussionsOnly ? (object) "discussions" : (object) "revisions", (object) projectId, (object) continuationTokenSent, (object) b)));
    }

    private static int GetTfsApiCallWaitTimeInMs(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext) => executionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/WorkItemRevisionsApiCallWaitTimeInMs", true, 30000);
  }
}
