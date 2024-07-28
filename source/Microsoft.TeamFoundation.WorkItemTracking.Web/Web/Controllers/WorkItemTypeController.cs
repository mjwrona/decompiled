// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypes", ResourceVersion = 2)]
  public class WorkItemTypeController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5911000;

    public override string TraceArea => "workItemTypes";

    [TraceFilter(5911000, 5911010)]
    [HttpGet]
    [ClientExample("GET__wit_workItemTypes.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType> GetWorkItemTypes()
    {
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType> itemTypesInternal = WorkItemTypeServiceHelper.GetWorkItemTypesInternal(this.TfsRequestContext, this.ProjectId);
      return itemTypesInternal == null ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>) null : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>) itemTypesInternal.ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>();
    }

    [TraceFilter(5911000, 5911010)]
    [HttpGet]
    [ClientExample("GET__wit_workItemTypes_Bug.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType GetWorkItemType(
      string type)
    {
      return WorkItemTypeServiceHelper.GetWorkItemTypeInternal(this.TfsRequestContext, this.ProjectId, type);
    }
  }
}
