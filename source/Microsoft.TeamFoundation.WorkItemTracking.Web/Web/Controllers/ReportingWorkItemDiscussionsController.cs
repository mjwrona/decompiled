// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemDiscussionsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemRevisionsDiscussions", ResourceVersion = 1)]
  [ControllerApiVersion(5.1)]
  public class ReportingWorkItemDiscussionsController : ReportingWorkItemRevisionsControllerBase
  {
    private const int TraceRange = 5915500;

    internal override int GetTraceRange() => 5915500;

    [TraceFilter(15520, 15529)]
    [HttpGet]
    [ValidateModel]
    [ClientResponseType(typeof (ReportingWorkItemRevisionsBatch), null, null)]
    [StreamingObjectResponse]
    public ReportingWorkItemRevisionsBatch ReadReportingDiscussions(
      [FromUri(Name = "continuationToken")] string continuationToken = null,
      [FromUri(Name = "$maxPageSize")] int? maxPageSize = null)
    {
      return this.ReadReportingRevisionsImpl(continuationToken, new DateTime?(), (IEnumerable<string>) new string[2]
      {
        "System.Id",
        "System.Rev"
      }, (IEnumerable<string>) new string[0], new bool?(true), new bool?(true), new bool?(), new bool?(), new bool?(), new bool?(), true, true, maxPageSize, new bool?(true));
    }
  }
}
