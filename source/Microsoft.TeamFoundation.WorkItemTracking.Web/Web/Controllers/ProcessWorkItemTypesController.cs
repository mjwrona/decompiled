// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessWorkItemTypesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "workItemTypes", ResourceVersion = 1)]
  public class ProcessWorkItemTypesController : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5921000;

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemTypeModel>), null, null)]
    [TraceFilter(5921000, 5921010)]
    [ClientLocationId("E2E9D1A6-432D-4062-8870-BFCB8C324AD7")]
    public HttpResponseMessage GetWorkItemTypes(Guid processId, [FromUri(Name = "$expand")] GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      return this.Request.CreateResponse<IEnumerable<WorkItemTypeModel>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(this.TfsRequestContext, processId, true, true).Select<ComposedWorkItemType, WorkItemTypeModel>((Func<ComposedWorkItemType, WorkItemTypeModel>) (wit =>
      {
        GetWorkItemTypeExpandResult typeExpandResult = new GetWorkItemTypeExpandResult(expand, this.TfsRequestContext, wit);
        return WorkItemTypeModelFactory.Create(this.TfsRequestContext, processId, wit, typeExpandResult.States, typeExpandResult.Behaviors, typeExpandResult.Layout);
      })));
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeModel), null, null)]
    [TraceFilter(5921010, 5921020)]
    [ClientLocationId("E2E9D1A6-432D-4062-8870-BFCB8C324AD7")]
    public HttpResponseMessage GetWorkItemType(
      Guid processId,
      string witRefName,
      [FromUri(Name = "$expand")] GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      ComposedWorkItemType workItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(this.TfsRequestContext, processId, witRefName, true, true);
      GetWorkItemTypeExpandResult typeExpandResult = new GetWorkItemTypeExpandResult(expand, this.TfsRequestContext, workItemType);
      return this.Request.CreateResponse<WorkItemTypeModel>(HttpStatusCode.OK, WorkItemTypeModelFactory.Create(this.TfsRequestContext, processId, workItemType, typeExpandResult.States, typeExpandResult.Behaviors, typeExpandResult.Layout));
    }
  }
}
