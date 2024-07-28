// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemRevisionsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemRevisions", ResourceVersion = 1)]
  [ControllerApiVersion(2.0)]
  public class ReportingWorkItemRevisionsController : ReportingWorkItemRevisionsControllerBase
  {
    private const int TraceRange = 5915000;

    internal override int GetTraceRange() => 5915000;

    [TraceFilter(5915000, 5915009)]
    [HttpGet]
    [ValidateModel]
    [ClientResponseType(typeof (ReportingWorkItemRevisionsBatch), null, null)]
    [StreamingObjectResponse]
    public ReportingWorkItemRevisionsBatch ReadReportingRevisionsGet(
      [FromUri(Name = "fields"), ClientParameterAsIEnumerable(typeof (string), ',')] string fields = null,
      [FromUri(Name = "types"), ClientParameterAsIEnumerable(typeof (string), ',')] string types = null,
      [FromUri(Name = "watermark")] int watermark = 0,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null,
      [FromUri(Name = "includeIdentityRef")] bool? includeIdentityRef = null)
    {
      return this.ReadReportingRevisionsImpl(watermark.ToString(), startDateTime, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(types), includeIdentityRef, new bool?(false), new bool?(), new bool?(), new bool?(), new bool?(), true, false, new int?());
    }

    [TraceFilter(5915000, 5915009)]
    [HttpGet]
    [ValidateModel]
    [ClientResponseType(typeof (ReportingWorkItemRevisionsBatch), null, null)]
    [StreamingObjectResponse]
    public ReportingWorkItemRevisionsBatch ReadReportingRevisionsGet(
      [FromUri(Name = "includeDeleted")] bool includeDeleted,
      [FromUri(Name = "fields"), ClientParameterAsIEnumerable(typeof (string), ',')] string fields = null,
      [FromUri(Name = "types"), ClientParameterAsIEnumerable(typeof (string), ',')] string types = null,
      [FromUri(Name = "watermark")] int watermark = 0,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null,
      [FromUri(Name = "includeIdentityRef")] bool? includeIdentityRef = null)
    {
      return this.ReadReportingRevisionsImpl(watermark.ToString(), startDateTime, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(types), includeIdentityRef, new bool?(includeDeleted), new bool?(), new bool?(), new bool?(), new bool?(), true, false, new int?());
    }

    [TraceFilter(5915010, 5915019)]
    [HttpPost]
    [ValidateModel]
    [ClientResponseType(typeof (ReportingWorkItemRevisionsBatch), null, null)]
    [ClientLocationId("F828FE59-DD87-495D-A17C-7A8D6211CA6C")]
    [StreamingObjectResponse]
    public ReportingWorkItemRevisionsBatch ReadReportingRevisionsPost(
      ReportingWorkItemRevisionsFilter filter,
      [FromUri(Name = "watermark")] int watermark = 0,
      [FromUri(Name = "startDateTime")] DateTime? startDateTime = null)
    {
      filter = filter ?? new ReportingWorkItemRevisionsFilter();
      return this.ReadReportingRevisionsImpl(watermark.ToString(), startDateTime, filter.Fields, filter.Types, filter.IncludeIdentityRef, filter.IncludeDeleted, new bool?(), new bool?(), new bool?(), new bool?(), false, false, new int?());
    }
  }
}
