// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.WorkItemHttpClientWrapper2
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class WorkItemHttpClientWrapper2 : WorkItemHttpClientWrapper
  {
    public WorkItemHttpClientWrapper2(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      TraceMetaData traceMetadata)
      : base(executionContext, traceMetadata)
    {
    }

    public override ReportingWorkItemRevisionsBatch GetWorkItemDiscussionsBatch(
      Guid projectId,
      string continuationToken,
      int maxPageSize)
    {
      ReportingWorkItemRevisionsBatch workItemRevisionBatch = this.ExpRetryInvoker.InvokeWithFaultCheck<ReportingWorkItemRevisionsBatch>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<ReportingWorkItemRevisionsBatch>((Func<CancellationTokenSource, Task<ReportingWorkItemRevisionsBatch>>) (tokenSource =>
      {
        WorkItemTrackingHttpClient workItemHttpClient = this.WorkItemHttpClient;
        Guid project = projectId;
        int? nullable = new int?(maxPageSize);
        string continuationToken1 = continuationToken;
        int? maxPageSize1 = nullable;
        CancellationToken token = tokenSource.Token;
        return workItemHttpClient.ReadReportingDiscussionsAsync(project, continuationToken1, maxPageSize1, cancellationToken: token);
      }), this.WaitTimeInMs, this.TraceMetaData)), this.FaultService, this.RetryLimit, this.WaitTimeInMs, true, this.TraceMetaData);
      WorkItemHttpClientWrapper.ValidateResponse(workItemRevisionBatch, continuationToken, projectId, true);
      return workItemRevisionBatch;
    }
  }
}
