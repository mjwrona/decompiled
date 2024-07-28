// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemHistoryController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "history", ResourceVersion = 2)]
  public class WorkItemHistoryController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5913000;

    public override string TraceArea => "history";

    [TraceFilter(5913000, 5913010)]
    [HttpGet]
    [ClientExample("GET__wit_workitems__taskId_history.json", "List work item history", null, null)]
    [ClientExample("GET__wit_workitems__taskId_history_project_scope.json", "List work item history", null, null)]
    public IEnumerable<WorkItemHistory> GetHistory(int id, [FromUri(Name = "$top")] int top = 200, [FromUri(Name = "$skip")] int skip = 0)
    {
      if (top <= 0 || top > 200)
        throw new VssPropertyValidationException("$top", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      if (skip < 0)
        throw new VssPropertyValidationException("$skip", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRange((object) "$skip"));
      IEnumerable<WorkItemHistory> source = this.GetAllHistory(id).Skip<WorkItemHistory>(skip).Take<WorkItemHistory>(top);
      return source == null ? (IEnumerable<WorkItemHistory>) null : (IEnumerable<WorkItemHistory>) source.ToList<WorkItemHistory>();
    }

    [TraceFilter(5913011, 5913020)]
    [HttpGet]
    public WorkItemHistory GetHistoryById(int id, int revisionNumber)
    {
      IEnumerable<WorkItemFieldData> workItemFieldValues = this.WorkItemService.GetWorkItemFieldValues(this.TfsRequestContext, Enumerable.Repeat<WorkItemIdRevisionPair>(new WorkItemIdRevisionPair()
      {
        Id = id,
        Revision = revisionNumber
      }, 1), (IEnumerable<string>) new string[2]
      {
        "System.History",
        "System.ChangedBy"
      });
      return (workItemFieldValues.Count<WorkItemFieldData>() != 0 ? WorkItemHistory.Create(this.WitRequestContext, workItemFieldValues.First<WorkItemFieldData>()) : throw new WorkItemIdRevisionNotFoundException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemRevisionNotFound((object) id, (object) revisionNumber))) ?? throw new HistoryNotUpdatedForRevisionException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.HistoryNotUpdatedForRevision((object) revisionNumber, (object) id));
    }

    private IEnumerable<WorkItemHistory> GetAllHistory(int id) => WorkItemHistory.Create(this.WitRequestContext, this.WorkItemService.GetWorkItems(this.TfsRequestContext, (IEnumerable<int>) new int[1]
    {
      id
    }, false, false, includeTags: false).FirstOrDefault<WorkItem>());
  }
}
