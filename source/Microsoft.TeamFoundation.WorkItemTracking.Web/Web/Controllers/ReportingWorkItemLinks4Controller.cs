// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemLinks4Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemLinks", ResourceVersion = 3)]
  [ControllerApiVersion(5.0)]
  [ClientGroupByResource("reportingWorkItemLinks")]
  public class ReportingWorkItemLinks4Controller : ReportingWorkItemLinksControllerBase
  {
    private const int TraceRange = 5916500;

    internal override int GetTraceRange() => 5916500;

    [TraceFilter(5916500, 5916510)]
    [StreamingObjectResponse]
    [HttpGet]
    [ClientExample("GET__wit_reporting_workItemLinks_50.json", null, null, null)]
    [ClientResponseType(typeof (ReportingWorkItemLinksBatch), null, null)]
    public ReportingWorkItemLinksBatch<WorkItemRelation> GetReportingLinksByLinkType(
      [FromUri(Name = "linkTypes"), ClientParameterAsIEnumerable(typeof (string), ',')] string linkTypes = null,
      [FromUri(Name = "types"), ClientParameterAsIEnumerable(typeof (string), ',')] string types = null,
      [FromUri(Name = "continuationToken")] string continuationToken = null,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.ReadReportingLinksImpl<WorkItemRelation>(new ReportingWorkItemLinksRequest<WorkItemRelation>()
      {
        ContinuationToken = continuationToken,
        StartDateTime = startDateTime,
        Types = (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(types),
        LinkTypes = (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(linkTypes),
        CreateLink = ReportingWorkItemLinks4Controller.\u003C\u003EO.\u003C0\u003E__CreateReportingLink ?? (ReportingWorkItemLinks4Controller.\u003C\u003EO.\u003C0\u003E__CreateReportingLink = new CreateLink<WorkItemRelation>(ReportingLinkFactory.CreateReportingLink))
      });
    }
  }
}
