// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeFieldV3Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypesField", ResourceVersion = 3)]
  public class WorkItemTypeFieldV3Controller : WorkItemTypeFieldBaseController
  {
    [HttpGet]
    [PublicProjectRequestRestrictions("5.0")]
    [ClientExample("GET__workItemType_fields.json", "1. Get list of project scoped work item type fields data", null, null)]
    [ClientExample("GET__workItemType_fields_expand.json", "2. Get list of project scoped work item type fields data with expand option", null, null)]
    public IEnumerable<WorkItemTypeFieldWithReferences> GetWorkItemTypeFieldsWithReferences(
      string type,
      [FromUri(Name = "$expand")] WorkItemTypeFieldsExpandLevel expand = WorkItemTypeFieldsExpandLevel.None)
    {
      IEnumerable<WorkItemTypeFieldWithReferences> fieldWithReferences = WorkItemTypeFieldInstanceFactory.CreateWorkItemTypeFieldWithReferences(this.WitRequestContext, this.GetWorkItemTypeFieldInstances(type, expand));
      return fieldWithReferences == null ? (IEnumerable<WorkItemTypeFieldWithReferences>) null : (IEnumerable<WorkItemTypeFieldWithReferences>) fieldWithReferences.ToList<WorkItemTypeFieldWithReferences>();
    }

    [HttpGet]
    [PublicProjectRequestRestrictions("5.0")]
    [ClientExample("GET__workItemType_field.json", "Get project scoped work item type field data", null, null)]
    [ClientExample("GET__workItemType_field_expand.json", "Get project scoped work item type field data with expand option", null, null)]
    public WorkItemTypeFieldWithReferences GetWorkItemTypeFieldWithReferences(
      string type,
      string field,
      [FromUri(Name = "$expand")] WorkItemTypeFieldsExpandLevel expand = WorkItemTypeFieldsExpandLevel.None)
    {
      return WorkItemTypeFieldInstanceFactory.CreateWorkItemTypeFieldWithReferences(this.WitRequestContext, this.GetWorkItemTypeFieldInstance(type, field, expand));
    }
  }
}
