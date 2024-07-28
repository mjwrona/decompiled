// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemLinksController
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
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemLinks", ResourceVersion = 1)]
  [ControllerApiVersion(2.0)]
  public class ReportingWorkItemLinksController : ReportingWorkItemLinksControllerBase
  {
    private const int TraceRange = 5916000;

    internal override int GetTraceRange() => 5916000;

    [TraceFilter(5916000, 5916010)]
    [StreamingObjectResponse]
    [HttpGet]
    [ClientIgnore]
    public ReportingWorkItemLinksBatch<ReportingWorkItemLink> GetReportingLinks(
      [FromUri(Name = "types"), ClientParameterAsIEnumerable(typeof (string), ',')] string types = null,
      [FromUri(Name = "watermark")] long baseWatermark = 0,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null)
    {
      return this.ReadReportingLinksImpl<ReportingWorkItemLink>(new ReportingWorkItemLinksRequest<ReportingWorkItemLink>()
      {
        Watermark = baseWatermark.ToString(),
        StartDateTime = startDateTime,
        Types = (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(types),
        FromContinuationToken = false,
        CreateLink = (CreateLink<ReportingWorkItemLink>) ((requestContext, linkChange, dictionary, includeRemoteUrl) => ReportingWorkItemLink.Create(requestContext, linkChange))
      });
    }
  }
}
