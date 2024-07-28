// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemLinks3Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemLinks", ResourceVersion = 2)]
  [ControllerApiVersion(4.1)]
  [ClientGroupByResource("reportingWorkItemLinks")]
  [ClientIgnore]
  public class ReportingWorkItemLinks3Controller : ReportingWorkItemLinksControllerBase
  {
    private const int TraceRange = 5916500;

    internal override int GetTraceRange() => 5916500;

    [TraceFilter(5916500, 5916510)]
    [StreamingObjectResponse]
    [HttpGet]
    [ClientExample("GET__wit_reporting_workItemLinks.json", null, null, null)]
    public ReportingWorkItemLinksBatch<ReportingWorkItemLink> GetReportingLinksByLinkType(
      [FromUri(Name = "linkTypes"), ClientParameterAsIEnumerable(typeof (string), ',')] string linkTypes = null,
      [FromUri(Name = "types"), ClientParameterAsIEnumerable(typeof (string), ',')] string types = null,
      [FromUri(Name = "continuationToken")] string continuationToken = null,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.ReadReportingLinksImpl<ReportingWorkItemLink>(new ReportingWorkItemLinksRequest<ReportingWorkItemLink>()
      {
        ContinuationToken = continuationToken,
        StartDateTime = startDateTime,
        Types = (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(types),
        LinkTypes = (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(linkTypes),
        CreateLink = ReportingWorkItemLinks3Controller.\u003C\u003EO.\u003C0\u003E__Create ?? (ReportingWorkItemLinks3Controller.\u003C\u003EO.\u003C0\u003E__Create = new CreateLink<ReportingWorkItemLink>(ReportingWorkItemLink.Create))
      });
    }
  }
}
