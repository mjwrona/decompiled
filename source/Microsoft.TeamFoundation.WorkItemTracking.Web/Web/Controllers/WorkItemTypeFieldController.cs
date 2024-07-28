// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeFieldController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypesField", ResourceVersion = 1)]
  public class WorkItemTypeFieldController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5914000;

    public override string TraceArea => "workItemTypesField";

    [TraceFilter(5914010, 5914020)]
    [HttpGet]
    [ClientExample("GET__dependent_fields.json", "Get the work item relation types", null, null)]
    public FieldDependentRule GetDependentFields(string type, string field) => FieldDependentRuleFactory.GetDependentFields(this.WitRequestContext, this.ProjectId, type, field);
  }
}
