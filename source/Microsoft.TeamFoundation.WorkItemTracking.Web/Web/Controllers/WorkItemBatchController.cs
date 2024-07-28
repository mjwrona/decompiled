// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemBatchController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ClientGroupByResource("workItems")]
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemsBatch", ResourceVersion = 1)]
  public class WorkItemBatchController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5910000;

    [TraceFilter(5910000, 5910010)]
    [HttpPost]
    [PublicProjectRequestRestrictions(false, false, "5.0")]
    [ClientExample("POST__wit_WorkItemsBatch_ids-_ids__fields-_columns_.json", "Get list of work items for specific fields", null, null)]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItemsBatch(
      WorkItemBatchGetRequest workItemGetRequest)
    {
      if (workItemGetRequest == null)
        throw new VssPropertyValidationException(nameof (workItemGetRequest), ResourceStrings.NullOrEmptyParameter((object) nameof (workItemGetRequest)));
      if (workItemGetRequest.Ids == null || workItemGetRequest.Ids.Count<int>() == 0)
        throw new VssPropertyValidationException("Ids", ResourceStrings.NullOrEmptyParameter((object) "Ids"));
      if (workItemGetRequest.Fields == null)
        workItemGetRequest.Fields = Enumerable.Empty<string>();
      bool includeLinks;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy serverErrorPolicy;
      WorkItemGetRequestProcessor.ProcessWorkItemOptions(workItemGetRequest.Expand, workItemGetRequest.ErrorPolicy, out includeLinks, out serverErrorPolicy);
      WorkItemTrackingRequestContext witRequestContext = this.WitRequestContext;
      ITeamFoundationWorkItemService workItemService = this.WorkItemService;
      int num1 = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext) ? 1 : 0;
      List<int> list1 = workItemGetRequest.Ids.ToList<int>();
      IEnumerable<string> fields = workItemGetRequest.Fields;
      List<string> list2 = fields != null ? fields.ToList<string>() : (List<string>) null;
      DateTime? asOf = workItemGetRequest.AsOf;
      int expand = (int) workItemGetRequest.Expand;
      int num2 = includeLinks ? 1 : 0;
      int errorPolicy = (int) serverErrorPolicy;
      Guid? nullableProjectId = this.GetNullableProjectId();
      int num3 = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext) ? 1 : 0;
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) WorkItemGetRequestProcessor.GetWorkItems(witRequestContext, workItemService, num1 != 0, (ICollection<int>) list1, (ICollection<string>) list2, asOf, (WorkItemExpand) expand, num2 != 0, (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy) errorPolicy, nullableProjectId, num3 != 0).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
    }
  }
}
