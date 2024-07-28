// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemRevisions2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemRevisions", ResourceVersion = 2)]
  [ControllerApiVersion(2.2)]
  [ClientGroupByResource("reportingWorkItemRevisions")]
  public class ReportingWorkItemRevisions2Controller : ReportingWorkItemRevisionsControllerBase
  {
    private const int TraceRange = 5915500;

    internal override int GetTraceRange() => 5915500;

    [TraceFilter(5915500, 5915509)]
    [HttpGet]
    [ValidateModel]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsBatch), null, null)]
    [StreamingObjectResponse]
    [ClientExample("GET__wit_reporting_workItemRevisions.json", "Get the first batch of work item revisions", null, null)]
    [ClientExample("GET__wit_reporting_workItemRevisions_includeIdentityRef-true_watermark-794.json", "Get a batch of work item revisions with identity references", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsBatch ReadReportingRevisionsGet(
      [FromUri(Name = "fields"), ClientParameterAsIEnumerable(typeof (string), ',')] string fields = null,
      [FromUri(Name = "types"), ClientParameterAsIEnumerable(typeof (string), ',')] string types = null,
      [FromUri(Name = "continuationToken")] string continuationToken = null,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null,
      [FromUri(Name = "includeIdentityRef")] bool? includeIdentityRef = null,
      [FromUri(Name = "includeDeleted")] bool? includeDeleted = null,
      [FromUri(Name = "includeTagRef")] bool? includeTagRef = null,
      [FromUri(Name = "includeLatestOnly")] bool? includeLatestOnly = null,
      [FromUri(Name = "$expand")] ReportingRevisionsExpand expand = ReportingRevisionsExpand.None,
      [FromUri(Name = "includeDiscussionChangesOnly")] bool? includeDiscussionChangesOnly = null,
      [FromUri(Name = "$maxPageSize")] int? maxPageSize = null)
    {
      bool flag = expand == ReportingRevisionsExpand.Fields;
      if (fields != null && fields.Any<char>() && expand != ReportingRevisionsExpand.None)
        throw new ConflictingParametersException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ExpandParameterConflict());
      return this.ReadReportingRevisionsImpl(continuationToken, startDateTime, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(types), includeIdentityRef, includeDeleted, includeTagRef, includeLatestOnly, new bool?(flag), includeDiscussionChangesOnly, true, true, maxPageSize);
    }

    [TraceFilter(5915510, 5915519)]
    [HttpPost]
    [ValidateModel]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsBatch), null, null)]
    [StreamingObjectResponse]
    [ClientExample("POST__wit_reporting_workItemRevisions_watermark-794.json", null, null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsBatch ReadReportingRevisionsPost(
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsFilter filter,
      [FromUri(Name = "continuationToken")] string continuationToken = null,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null,
      [FromUri(Name = "$expand")] ReportingRevisionsExpand expand = ReportingRevisionsExpand.None)
    {
      bool flag = expand == ReportingRevisionsExpand.Fields;
      filter = filter ?? new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsFilter();
      if (filter.Fields != null && filter.Fields.Any<string>() && expand != ReportingRevisionsExpand.None)
        throw new ConflictingParametersException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ExpandParameterConflict());
      return this.ReadReportingRevisionsImpl(continuationToken, startDateTime, filter.Fields, filter.Types, filter.IncludeIdentityRef, filter.IncludeDeleted, filter.IncludeTagRef, filter.IncludeLatestOnly, new bool?(flag), filter.IncludeDiscussionChangesOnly, false, true, new int?());
    }
  }
}
