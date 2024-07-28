// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubTimelineRecordFeedsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "feed")]
  public class TaskHubTimelineRecordFeedsController : TaskHubApiController
  {
    private const string ContinuationTokenHeaderName = "x-continuationtoken";

    [HttpPost]
    [ClientSuppressWarning(ClientWarnings.NamingGuidelines)]
    [ClientResponseType(typeof (void), null, null, MethodName = "AppendTimelineRecordFeed")]
    [ClientExample("POST__distributedtask_AppendTimelineRecordFeed_.json", "Append timeline record feed", null, null)]
    public async Task PostLines(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      TimelineRecordFeedLinesWrapper lines)
    {
      TaskHubTimelineRecordFeedsController recordFeedsController = this;
      ArgumentUtility.CheckForNull<TimelineRecordFeedLinesWrapper>(lines, nameof (lines), "DistributedTask");
      if (lines.Count <= 0)
        return;
      await recordFeedsController.Hub.FeedReceivedAsync(recordFeedsController.TfsRequestContext, recordFeedsController.ScopeIdentifier, planId, timelineId, recordId, lines);
    }

    [HttpGet]
    [ClientResponseType(typeof (TimelineRecordFeedLinesWrapper), null, null)]
    public async Task<HttpResponseMessage> GetLines(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      Guid stepId,
      long? endLine = null,
      int? takeCount = null,
      string continuationToken = null)
    {
      TaskHubTimelineRecordFeedsController recordFeedsController = this;
      TimelineRecordLogLineResult logLinesAsync = await recordFeedsController.Hub.GetLogLinesAsync(recordFeedsController.TfsRequestContext, recordFeedsController.ScopeIdentifier, planId, timelineId, recordId, stepId, continuationToken, endLine, takeCount);
      HttpResponseMessage response;
      if (logLinesAsync != null)
      {
        int? count = logLinesAsync.Lines?.Count;
        int num = 0;
        if (count.GetValueOrDefault() > num & count.HasValue)
        {
          response = recordFeedsController.Request.CreateResponse<TimelineRecordFeedLinesWrapper>(HttpStatusCode.OK, new TimelineRecordFeedLinesWrapper(stepId, (IList<string>) logLinesAsync.Lines.Select<TimelineRecordLogLine, string>((Func<TimelineRecordLogLine, string>) (x => x.Line)).ToList<string>(), logLinesAsync.Lines.First<TimelineRecordLogLine>().LineNumber, logLinesAsync.Lines.Last<TimelineRecordLogLine>().LineNumber));
          goto label_5;
        }
      }
      response = recordFeedsController.Request.CreateResponse<TimelineRecordFeedLinesWrapper>(HttpStatusCode.OK, new TimelineRecordFeedLinesWrapper(stepId));
label_5:
      if (!string.IsNullOrWhiteSpace(logLinesAsync?.ContinuationToken))
        response.Headers.Add("x-continuationtoken", logLinesAsync.ContinuationToken);
      return response;
    }
  }
}
